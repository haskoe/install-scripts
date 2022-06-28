using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualBasic;
using Microsoft.Office.Interop.Word;
using W = Microsoft.Office.Interop.Word;

namespace aasted
{
    /// <summary>
    /// AastedPriceMacro indlæser et Word dokument dannet i AZ og efterflg. manuelt redigeret og danner en tabel 
    /// med en række for hver heading 1 markeret tekst som ineholder 2 kolonner: 1) xref til heading 1 nr og tekst og 2) AastedItemPrice
    /// Sekvens af "Heading 1" og AastedItemPrice skal være som flg.: "Heading 1", AastedItemPrice, "Heading 1", AastedItemPrice, .....
    /// Af nedenstående valideringer foretages 1-3. 4 må brugeren selv foretage
    /// 1) Heading 1 uden en afsluttende AastedItemPrice
    /// 2) AastedItemPrice uden en foregående Heading 1
    /// 3) Advarsel hvis Heading 1 tekster ikke er unkke
    /// 4) Heading 1 numre skal være fortløbende 
    /// 
    /// Koden gør flg.:
    /// 1) Dokument kopieres og kopi åbnes
    /// 2) Alle Heading 1 xref'er indlæses i _heading1PriceXrefs da det er xref'en som indsættes i pristabellen
    /// 3) Alle Heading 1 positioner og tekster indlæses i _heading1PriceTextAndPos
    /// 4) Alle aastedItemPrice positioner og tekster indlæses i _aastedPrices
    /// 5) Validering 1-3 foretages og valideringsfejl gemmes i _validationErrors
    /// 6) Alle rækker med undtagelse af den første cleares i pristabel (er markeret med AastedItemHeading)
    /// 7) Der indsættes en række for hver heading 1 i pristabel (InsertPriceInToc)
    /// 8) TOC opdateres
    /// </summary>
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
        private Dictionary<string, Tuple<int, string>[]> _heading1PriceXrefs;
        private StyleTextAndPos[] _aastedPrices;
        private StyleTextAndPos[] _heading1PriceTextAndPos;

        private Dictionary<string, StyleTextAndPos[]> _heading1Dict = new Dictionary<string, StyleTextAndPos[]>();
        private Dictionary<string, string> _heading1ToAastedPrice = new Dictionary<string, string>();

        private const string DOT = ".";
        private const char TAB = '\t';
        private const string PriceXrefSep = ".\t";

        private const string PriceUnit = "EUR"; // todo: extract from aasted item price
        private const string AastedItemHeadingStyleName = "AastedItemHeading";
        private const string AastedPriceHeadingStyleName = "AastedPriceHeading";

        private const string AastedItemStyleName = "AastedItem";
        private const string AastedPriceStyleName = "AastedPrice";
        private const string AastedItemPriceStyleName = "AastedItemPrice";

        private Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        public AastedPriceMacro(string wordFile)
        {
            // make a copy 
            string workFile = Path.Combine(Path.GetDirectoryName(wordFile), $"{Path.GetFileNameWithoutExtension(wordFile)}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(wordFile)}");
            File.Copy(wordFile, workFile);
            object read = "ReadWrite";
            object readOnly = false;
            object o = MISSING;
            object filePath = workFile;

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

                // update toc
                _app.Selection.WholeStory();
                _app.Selection.Fields.Update();
            }
            finally
            {
                _doc.Close(WdSaveOptions.wdSaveChanges, WdOriginalFormat.wdOriginalDocumentFormat, ref MISSING);
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
                .GroupBy(t => Helper.RemoveWhiteSpace(t.Item2))
                .ToDictionary(g => g.Key, g => g.ToArray());

            // hent heading 1 text and positioner 
            _heading1PriceTextAndPos = StyleFindGetTextAndPos(_heading1Style, true)
                .ToArray();

            // hent AastedItemPrice style text and positioner 
            _aastedPrices = StyleFindGetTextAndPos(_aastedItemPriceStyle, true)
               .ToArray();

            foreach (int i in Enumerable.Range(0, _heading1PriceTextAndPos.Length))
            {
                string key = Helper.RemoveWhiteSpace(_heading1PriceTextAndPos[i].Text);
                if (_heading1ToAastedPrice.ContainsKey(key))
                    continue;

                var aastedPrice = _aastedPrices
                    .FirstOrDefault(aap => (aap.Pos > _heading1PriceTextAndPos[i].Pos) && (i == _heading1PriceTextAndPos.Length - 1 || aap.Pos < _heading1PriceTextAndPos[i + 1].Pos));
                _heading1ToAastedPrice.Add(key, aastedPrice?.Text.Split(TAB).Last().Trim() ?? "");
            }

            // validation 1: Duplicate heading 1 texts
            _heading1Dict = _heading1PriceTextAndPos
                .GroupBy(x => Helper.RemoveWhiteSpace(x.Text))
                .ToDictionary(g => g.Key, g => g.ToArray());

            AddError("Dublerede Heading 1 tekster", _heading1Dict
                .Where(x => x.Value.Length > 1)
                .Select(x => x.Value.First().Text)
                .ToArray());


            // Regel: der skal være en efterflg. AastedItemPrice OG en efterflg Heading 1 skal ligge efter efterflg. AastedItemPrice
            AddError("Heading 1 som mangler en AastedItemPrice", _heading1PriceTextAndPos
                .Select(enu => new Tuple<StyleTextAndPos, StyleTextAndPos, StyleTextAndPos>(
                    _heading1PriceTextAndPos.FirstOrDefault(hpp => hpp.Pos > enu.Pos),
                    _aastedPrices.FirstOrDefault(app => app.Pos > enu.Pos),
                    enu
                ))
                .Where(t => !(null != t.Item2 && (null == t.Item1 || t.Item1.Pos> t.Item2.Pos)))
                .Select(t => t.Item3.Text)
                .ToArray());

            // Regel: der skal være en efterflg. AastedItemPrice OG en efterflg Heading 1 skal ligge efter efterflg. AastedItemPrice
            AddError("AastedItemPrice som mangler foregående Heading 1", _aastedPrices
                .Select(enu => new Tuple<StyleTextAndPos, StyleTextAndPos, StyleTextAndPos>(
                    _aastedPrices.LastOrDefault(app => app.Pos < enu.Pos),
                    _heading1PriceTextAndPos.LastOrDefault(hpp => hpp.Pos < enu.Pos),
                    enu
                ))
                .Where(t => !(null != t.Item2 && (null == t.Item1 || t.Item1.Pos < t.Item2.Pos)))
                .Select(t => t.Item3.Text)
                .ToArray());

            // insert validation errors
            if (_validationErrors.Any())
            {
                string txt = string.Join(Environment.NewLine, _validationErrors
                    .Select(kv => $"{kv.Key}:{Environment.NewLine}{string.Join(Environment.NewLine, kv.Value)}"));

                //MoveToPriceTable();
                //_app.Selection.MoveUp(WdUnits.wdLine, ref objMoveCount, WdMovementType.wdMove);
                //_app.Selection.MoveUp(WdUnits.wdLine, ref objMoveCount, WdMovementType.wdMove);
                //_app.Selection.Text = txt;
            }
        }


        private void ClearTocPriceTable()
        {
            if (MoveToPriceTable())
            {
                W.Table tbl = _app.Selection.Tables[1]; // Selection.Tables has start offset one
                while (tbl.Rows.Count > 1)
                {
                    tbl.Rows[tbl.Rows.Count].Delete();
                }
                SetCell(tbl, 1, 1, "ITEM", _aastedItemHeadingStyle); // Cell array has start offset zero ...???
                SetCell(tbl, 1, 2, PriceUnit, _aastedItemPriceStyle);
            }
            else
            {
                AddError("No price TOC style", "");
            }

        }

        private void InsertPriceInToc()
        {
            // Alle Aasted priser indsættes hvis der er en xref til denne
            if (MoveToPriceTable())
            {
                int row = 2;
                var tbl = _app.Selection.Tables[1];
                foreach (var price in _heading1PriceTextAndPos)
                {
                    Selection rng = _app.Selection;
                    rng.set_Style(_aastedItemStyle);

                    // if there is a xref mathing the heading 1 price insert that
                    // if not validation 1 is violated ....
                    if (_heading1PriceXrefs.ContainsKey(Helper.RemoveWhiteSpace(price.Text)))
                    {
                        var headingXref = _heading1PriceXrefs[Helper.RemoveWhiteSpace(price.Text)]
                            .First();
                        object xrefIdx = (headingXref.Item1 + 1);
                        rng.InsertCrossReference(WdReferenceType.wdRefTypeHeading,
                            WdReferenceKind.wdNumberFullContext, xrefIdx, ref objTrue, ref objFalse);
                        rng.Collapse(WdCollapseDirection.wdCollapseStart);
                        rng.InsertBefore(PriceXrefSep);
                        rng.Collapse(WdCollapseDirection.wdCollapseEnd);
                        rng.InsertCrossReference(WdReferenceType.wdRefTypeHeading,
                            WdReferenceKind.wdContentText, xrefIdx, ref objTrue, ref objFalse);
                    }
                    else
                    {
                        rng.Text = price.Text;
                    }
                    rng.MoveRight(WdUnits.wdCell);
                    rng.set_Style(_aastedPriceStyle);
                    rng.Text = _heading1ToAastedPrice[Helper.RemoveWhiteSpace(price.Text)];

                    tbl.Rows.Add();
                    rng.MoveRight(WdUnits.wdCell);
                    //_app.Selection.MoveDown(WdUnits.wdLine, ref objMoveCount, WdMovementType.wdExtend);
                    row += 1;
                }
            }
        }

        private bool MoveToPriceTable()
        {
            InitSelectionFind(_aastedItemHeadingStyle, true);
            bool result = _app.Selection.Find.Execute();
            if (result)
            {
                var tbl = _app.Selection.Tables[1];
                if (tbl.Rows.Count == 1)
                    tbl.Rows.Add();
                _app.Selection.MoveDown(WdUnits.wdLine, ref objMoveCount, WdMovementType.wdMove);
            }
            return result;
        }

        private string GetPriceFromHeading(string heading)
        {
            string[] splt = heading.Split('.');
            return splt.Length == 2 && splt.First().All(char.IsNumber) ? splt.Last().Trim() : null;
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
        private static object objTrue = true;
        private static object objFalse = false;
        private static object objMoveCount = 1;

        private W.Range SelectCell(W.Table tbl, int row, int col)
        {
            W.Cell cell = tbl.Cell(row, col);
            cell.Select();
            return cell.Range;
        }

        private void SetCell(W.Table tbl, int row, int col, string txt, W.Style style)
        {
            var rng = SelectCell(tbl, row, col);
            rng.Text = txt;
            if (null != style)
            {
                rng.set_Style(style);
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