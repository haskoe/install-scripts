using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Office.Interop.Word;
using W = Microsoft.Office.Interop.Word;

namespace aasted
{
    public class AastedPriceMacro
    {
        private W._Document _doc;
        private W.Style _heading1Style;
        private W.Style _aastedItemHeadingStyle;
        private W.Style _aastedPriceHeadingStyle;
        private W.Style _aastedItemPriceStyle;
        private W.Style _aastedItemStyle;
        private W.Style _aastedPriceStyle;
        private W.Application _app;

        // 
        //private Tuple<int, string>[] _prices;
        private Tuple<int, string>[] _heading1PriceXrefs;
        private StyleTextAndPos[] _aastedPrices;

        private Dictionary<string, StyleTextAndPos[]> _heading1Dict = new Dictionary<string, StyleTextAndPos[]>();

        private const string DOT = ".";
        private const char TAB = '\t';

        private const string PriceUnit = "EUR"; // todo: extract from aasted item price
        private const string AastedItemHeadingStyleName = "AastedItemHeading";
        private const string AastedPriceHeadingStyleName = "AastedPriceHeading";

        private const string AastedItemStyleName = "AastedItem";
        private const string AastedPriceStyleName = "AastedPrice";
        private const string AastedItemPriceStyleName = "AastedItemPrice";

        private Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        public AastedPriceMacro(string wordFile)
        {
            object read = "ReadWrite";
            object readOnly = false;
            object o = MISSING;
            object filePath = wordFile;

            _app = new Microsoft.Office.Interop.Word.Application();
            try
            {
                _doc = _app.Documents.Open(ref filePath, ref o, ref readOnly, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o, ref o);

                //_styles = ToArray<W.Style>(() => _doc.Styles.Count, (i) => _doc.Styles[i + 1]);

                _heading1Style = GetHeadingStyle(1);
                _aastedItemHeadingStyle = GetStyle(AastedItemHeadingStyleName);
                _aastedPriceHeadingStyle = GetStyle(AastedPriceHeadingStyleName);
                _aastedItemStyle = GetStyle(AastedItemStyleName);
                _aastedPriceStyle = GetStyle(AastedPriceStyleName);
                _aastedItemPriceStyle = GetStyle(AastedItemPriceStyleName);

                CollectPrices();

                ClearTocPriceTable();

                InsertPriceInToc();
            }
            finally
            {
                _doc.Close(ref o, ref o, ref o);
                _app.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_app);
            }
        }

        private void CollectPrices()
        {
            object headingsObject = _doc.GetCrossReferenceItems(W.WdReferenceType.wdRefTypeHeading);

            // Loop igennem alle heading xrefs og gem dem som er en Price heading xref sammen med indeks i headings listen
            // Foerst dannes temp enumerale med alle heading xref tekster
            var temp = Helper.DynamicToStringArray(headingsObject)
                .Select(h => new Tuple<string, string>(h, GetPriceFromHeading(h)))
                .ToArray();

            // og så kan heading indeks pristekst dannes
            _heading1PriceXrefs = Enumerable.Range(0, temp.Length)
                .Where(i => !string.IsNullOrEmpty(temp[i].Item2))
                .Select(i => new Tuple<int, string>(i, temp[i].Item2))
                .ToArray();

            // hent heading 1 text and positioner 
            var heading1TextAndPos = StyleFindGetTextAndPos(_heading1Style, true)
                .ToArray();

            // hent AastedItemPrice style text and positioner 
            _aastedPrices = StyleFindGetTextAndPos(_aastedItemPriceStyle, true)
               .ToArray();

            // validation 1: All price xref texts must have an associated Heading 1 text
            _heading1Dict = heading1TextAndPos
                .GroupBy(x => Helper.RemoveWhiteSpace(x.Text))
                .ToDictionary(g => g.Key, g => g.ToArray());

            AddError("Alle pris krydsreferencer skal have en tilhørende heading 1 tekst", _heading1PriceXrefs
                .Where(p => !_heading1Dict.ContainsKey(Helper.RemoveWhiteSpace(p.Item2)))
                .Select(p => p.Item2)
                .ToArray());

            // validation 2: All heading 1 price texts must be unique
            AddError("Alle Heading 1 pris tekster skal være unikke", _heading1Dict
                .Where(x => x.Value.Length > 1)
                .Select(x => x.Key)
                .ToArray());

            // validation 3: Heading 1 price text and AastedItemPrice price text count must match
            if (_heading1Dict.Count != _aastedPrices.Length)
            {
                AddError("Antal Heading 1 priser og AastedItemPrice priser skal være det samme", new string[] { $"Heading 1 antal:{_heading1Dict.Count} AastedItemPrice antal:{_aastedPrices.Length}" });
            }
            else
            {
                AddError("Antal Heading 1 pris positioner og AastedItemPrice pris positioner i dokumentet følger ikke hinanden", Enumerable.Range(0, heading1TextAndPos.Length)
                    .Where(i => (heading1TextAndPos[i].Pos >= _aastedPrices[i].Pos)
                                ||
                                ((i < heading1TextAndPos.Length - 1) && (_aastedPrices[i].Pos >= heading1TextAndPos[i + 1].Pos)))
                    .Select(i => $"{heading1TextAndPos[i].Text} {_aastedPrices[i].Text}")
                    .ToArray());
            }
        }


        private static object moveCount = 1;
        private void ClearTocPriceTable()
        {
            if (MoveToPriceToc())
            {
                W.Table tbl = _app.Selection.Tables[1]; // Selection.Tables has start offset one
                while (tbl.Rows.Count > 2)
                {
                    tbl.Rows[tbl.Rows.Count].Delete();
                }
                SetCell(tbl, 0, 0, "ITEM", _aastedItemHeadingStyle); // Cell array has start offset zero ...???
                SetCell(tbl, 0, 1, PriceUnit, _aastedItemPriceStyle);
                SetCell(tbl, 1, 0, "", null);
                SetCell(tbl, 1, 1, "", null);
            }
            else
            {
                AddError("No price TOC style", "");
            }
        }

        private void InsertPriceInToc()
        {
            if (MoveToPriceToc())
            {
                foreach (var price in _aastedPrices)
                {
                    _app.Selection.set_Style(_aastedItemStyle);

                    // if there is a xref mathing the heading 1 price insert that
                    // if not validation 1 is violated ....
                    if (_heading1Dict.ContainsKey(Helper.RemoveWhiteSpace(price.Text)))
                    {
                        object xrefIdx = 1;
                        var heading1Item = _heading1Dict[Helper.RemoveWhiteSpace(price.Text)];
                        _app.Selection.InsertCrossReference(ref wdRefTypeHeading,
                            WdReferenceKind.wdNumberFullContext, ref xrefIdx, ref objTrue, ref objFalse);
                        _app.Selection.Collapse(ref wdCollapseStart);
                        _app.Selection.InsertBefore("." + TAB);
                        _app.Selection.Collapse(ref wdCollapseEnd);
                        _app.Selection.InsertCrossReference(ref wdRefTypeHeading,
                            WdReferenceKind.wdContentText, ref xrefIdx, ref objTrue, ref objFalse);
                    }
                    else
                    {
                        _app.Selection.Text = price.Text;
                    }
                    _app.Selection.MoveRight(ref wdCell);
                    _app.Selection.set_Style(_aastedPriceStyle);
                    _app.Selection.Text = price.Text;

                    _app.Selection.MoveRight(ref wdCell);
                }
            }
        }

        private bool MoveToPriceToc()
        {
            InitSelectionFind(_aastedItemHeadingStyle, true);
            bool result = _app.Selection.Find.Execute();
            if (result)
            {
                _app.Selection.MoveDown(ref wdLine, ref moveCount, ref MISSING);
            }
            return result;
        }

        private string GetPriceFromHeading(string heading)
        {
            string[] splt = heading.Split('.');
            return splt.Length == 2 && splt.First().All(char.IsNumber) ? splt.Last() : null;
        }

        private void AddError(string errorText, string[] errorItems)
        {
            if (errorItems.Any())
            {
                _validationErrors.Add(errorText, string.Join(Environment.NewLine, errorItems));
            }
        }
        private void AddError(string errorText, string errorItem)
        {
            _validationErrors.Add(errorText, errorItem);
        }


        private static object wdCell = 12;
        private static object wdLine = 5;
        private static object wdStory = 6;
        private static object MISSING = System.Reflection.Missing.Value;
        private static string[] HEADING_STYLE_NAMES = new string[] { "Heading", "Overskrift" };
        private static object wdRefTypeHeading = 1;
        private static object wdNumberFullContext = -4;
        private static object wdContentText = -1;
        private static object wdCollapseStart = 1;
        private static object wdCollapseEnd = 0;
        private static object objTrue = true;
        private static object objFalse = false;

        private void SetCell(W.Table tbl, int row, int col, string txt, W.Style style)
        {
            W.Cell cell = tbl.Cell(row, col);
            cell.Select();
            cell.Range.Text = txt;
            if (null != style)
            {
                _app.Selection.set_Style(style);
            }
        }

        private void InitSelectionFind(W.Style style, bool rewind)
        {
            if (rewind)
            {
                _app.Selection.HomeKey(ref wdStory, ref MISSING);
            }
            W.Find f = _app.Selection.Find;
            f.ClearFormatting();
            f.set_Style(style);
            f.Text = "";
            f.Replacement.Text = "";
            f.Forward = true;
            f.Wrap = W.WdFindWrap.wdFindContinue;
            f.Format = true;
            f.MatchCase = false;
            f.MatchWholeWord = false;
            f.MatchWildcards = false;
            f.MatchSoundsLike = false;
            f.MatchAllWordForms = false;
        }

        private W.Style GetStyle(string styleNameToSearchFor)
        {
            try
            {
                return _doc.Styles[styleNameToSearchFor];
            }
            catch
            {
                return null;
            }
        }

        private W.Style GetStyle(IEnumerable<string> styleNamesToSearchFor) => LinqHelper.FirstNotNull(LinqHelper.MakeGenerator(styleNamesToSearchFor)
            .Select(styleNameToSearchFor => GetStyle(styleNameToSearchFor)));

        private string GetSelectionText() => _app.Selection.Text.Trim();

        private IEnumerable<T> StyleFindGet<T>(W.Style style, bool skipEmpty, Func<T> getItem)
        {
            InitSelectionFind(style, true);
            return LinqHelper.ToEnumerable<string, T>(
                () => _app.Selection.Find.Execute(),
                GetSelectionText,
                getItem,
                (t) => !skipEmpty || !string.IsNullOrEmpty(t));
        }

        public StyleTextAndPos SelectionToStyleTextAndPos() => StyleTextAndPos.FromSelection(_app.Selection);
        private IEnumerable<StyleTextAndPos> StyleFindGetTextAndPos(W.Style style, bool skipEmpty) => StyleFindGet<StyleTextAndPos>(style, skipEmpty, SelectionToStyleTextAndPos);
        private W.Style GetHeadingStyle(int headingIndex) => GetStyle(HEADING_STYLE_NAMES.Select(sn => $"{sn} {headingIndex}"));

    }
}