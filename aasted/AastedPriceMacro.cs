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
        private W.Application _app;
        private Tuple<string, string>[] _prices;

        private const string DOT = ".";

        private static object wdStory = 6;
        private object MISSING = System.Reflection.Missing.Value;

        private const string AastedItemPrice = "AastedItemPrice";
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

                Validate();

                CollectPrices();

                Validate();
            }
            finally
            {
                _doc.Close(ref o, ref o, ref o);
                _app.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_app);
            }
        }

        private void Validate()
        {
        }


        //        Private Sub MiOrgSetSelectionToStyleText(StyleTextToSearchFor As String, Rewind As Boolean)
        //    MiOrgSetSelectionToStyle ActiveDocument.Styles(StyleTextToSearchFor), Rewind
        //End Sub

        //Private Sub MiOrgSetSelectionToStyle(StyleToSearchFor As Style, Rewind As Boolean)
        //    If Rewind Then
        //        Selection.HomeKey Unit:=wdStory
        //    End If
        //    With Selection.Find
        //        .ClearFormatting
        //        .Style = StyleToSearchFor
        //        .Text = ""
        //        .Replacement.Text = ""
        //        .Forward = True
        //        .Wrap = wdFindContinue
        //        .Format = True
        //        .MatchCase = False
        //        .MatchWholeWord = False
        //        .MatchWildcards = False
        //        .MatchSoundsLike = False
        //        .MatchAllWordForms = False
        //    End With
        //End Sub

        //        private string[] GetStyleTexts(string style, bool skipEmpty)
        //        {
        //            W.Range range = _app.ActiveDocument.Content
        //            Word.Find find = range.Find;
        //            find.Text = "xxx";
        //            MiOrgSetSelectionToStyleText(style, true);
        //            //  Do While Selection.Find.Execute
        //            //    Dim TheText As String
        //            //    TheText = MiOrgTrimText(Selection.Text)
        //            //    If Not SkipEmpty Or Len(TheText) > 0 Then
        //            //      result.Add TheText
        //            //    End If
        //            //  Loop
        //            //  Set MiOrgGetStyleTexts = result
        //            //End Function
        //        }

        //        private void MiOrgSetSelectionToStyle(W.Style styleToSearchFor, bool rewind)
        //        {
        //            _doc.se
        //Private Sub MiOrgSetSelectionToStyle(StyleToSearchFor As Style, Rewind As Boolean)
        //    If Rewind Then
        //        Selection.HomeKey Unit:= wdStory
        //    End If
        //    With Selection.Find
        //        .ClearFormatting
        //        .Style = StyleToSearchFor
        //        .Text = ""
        //        .Replacement.Text = ""
        //        .Forward = True
        //        .Wrap = wdFindContinue
        //        .Format = True
        //        .MatchCase = False
        //        .MatchWholeWord = False
        //        .MatchWildcards = False
        //        .MatchSoundsLike = False
        //        .MatchAllWordForms = False
        //    End With
        //End Sub


        //            try
        //            {
        //                return _doc.st

        //    Dim Heading As String
        //    Dim TheStyle As Style

        //    ' try with english heading
        //    Heading = "Heading " & CStr(headingIndex)
        //    On Error Resume Next
        //    Set TheStyle = ActiveDocument.Styles(Heading)
        //    If Err.Number<> 0 Then
        //        On Error GoTo 0
        //        Heading = "Heading " + CStr(headingIndex)
        //        Set TheStyle = ActiveDocument.Styles(Heading)
        //    End If
        //    On Error GoTo 0
        //    Set MiOrgGetHeadingStyle = TheStyle
        //End Function

        private void CollectPrices()
        {
            object headingsObject = _doc.GetCrossReferenceItems(W.WdReferenceType.wdRefTypeHeading);
            _prices = DynamicToStringArray(headingsObject)
                .Select(h => new Tuple<string, string>(h, GetPriceFromHeading(h)))
                .Where(t => !string.IsNullOrEmpty(t.Item2))
                .ToArray();

            var heading1Texts = StyleFindGetText(_heading1Style, true)
                .ToArray();
            var heading1Pos = StyleFindGetPos(_heading1Style, true)
                .ToArray();

            var aastedItemPriceStyle = GetStyle(AastedItemPrice);
            var aastedPriceTexts = StyleFindGetText(aastedItemPriceStyle, true)
                .Select(p => p.Split('\t').Last())
                .ToArray();
            var aastedPricePos = StyleFindGetPos(aastedItemPriceStyle, true)
                .ToArray();

            //        Dim ErrMsg As String
            //For h = 1 To PriceCrossRefTextColl.Count
            //  If MiOrgCollGetIndex(PriceHeading1TextColl, PriceCrossRefTextColl(h)) <= 0 Then
            //    ErrMsg = ErrMsg & "Price crossreference " & PriceCrossRefTextColl(h) & " does not have a corresponding Heading 1 text" & vbCrLf
            //  End If
            //Next

            //Dim DuplicateHeadingTexts As String
            //DuplicateHeadingTexts = MiOrgGetDuplicates(PriceHeading1TextColl, vbNewLine)
            //If DuplicateHeadingTexts<> "" Then
            //   ErrMsg = ErrMsg & "Document contains the following duplicate heading texts: " & DuplicateHeadingTexts & vbCrLf
            //End If


            //If PriceHeading1TextColl.Count<> PriceStyleTexts.Count Then
            //  ErrMsg = ErrMsg & "Heading 1 texts does not match AastedItemPrice count" & vbCrLf
            //End If


            //Dim MismatchSectionIndex As Integer
            //MismatchSectionIndex = MiOrgCollFirstNonConsecutiveIndex(Heading1StylePositions, PriceStylePositions)
            //If MismatchSectionIndex > 0 Then
            //  ErrMsg = ErrMsg & "Order of Heading 1 styles and AastedItemPrice styles is not consecutive. First mismatch is in section " & MismatchSectionIndex & vbCrLf
            //End If


            //If ErrMsg<> "" Then
            // MsgBox ErrMsg, Buttons:= vbOKOnly, Title:= "Error"
            //  End
            //End If

        }

        private string GetPriceFromHeading(string heading)
        {
            string[] splt = heading.Split('.');
            return splt.Length == 2 && splt.First().All(char.IsNumber) ? splt.Last() : null;
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

        private W.Style GetStyle(IEnumerable<string> styleNamesToSearchFor) => LinqHelper.FirstNotNull(LinqHelper.MakeGenetator(styleNamesToSearchFor)
            .Select(styleNameToSearchFor => GetStyle(styleNameToSearchFor)));

        private string GetSelectionText()
        {
            return _app.Selection.Text.Trim();
        }

        private IEnumerable<T> StyleFindGet<T>(W.Style style, bool skipEmpty, Func<T> getItem)
        {
            InitSelectionFind(style, true);
            return ToEnumerable<string, T>(
                () => _app.Selection.Find.Execute(),
                GetSelectionText,
                getItem,
                (t) => !skipEmpty || !string.IsNullOrEmpty(t));
        }


        private IEnumerable<string> StyleFindGetText(W.Style style, bool skipEmpty) => StyleFindGet<string>(style, skipEmpty, GetSelectionText);
        private IEnumerable<int> StyleFindGetPos(W.Style style, bool skipEmpty) => StyleFindGet<int>(style, skipEmpty,
            () => _app.Selection.Range.Start);


        //private IEnumerable<string> GetStyleTexts(W.Style style, bool skipEmpty)
        //{
        //    InitSelectionFind(style, true);
        //    return ToEnumerable(
        //        () => _app.Selection.Find.Execute(),
        //        () => _app.Selection.Text.Trim(),
        //        (t) => skipEmpty || !string.IsNullOrEmpty(t));
        //}

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

        private static string[] HEADING_STYLE_NAMES = new string[] { "Heading", "Overskrift" };
        private W.Style GetHeadingStyle(int headingIndex) => GetStyle(HEADING_STYLE_NAMES.Select(sn => $"{sn} {headingIndex}"));

        private IEnumerable<T> ToEnumerable<T>(Func<int> getCount, Func<int, T> getItem) => Enumerable.Range(0, getCount())
            .Select(i => getItem(i));

        private IEnumerable<T2> ToEnumerable<T1, T2>(Func<bool> continueFunc, Func<T1> getT1, Func<T2> getT2, Func<T1, bool> addCond = null)
        {
            while (continueFunc())
            {
                T1 t1 = getT1();
                if (null == addCond || addCond(t1))
                    yield return getT2();
            }
        }

        //private T[] ToArray<T>(Func<int> getCount, Func<int, T> getItem) => Enumerable.Range(0, getCount())
        //    .Select(i => getItem(i))
        //    .ToArray();

        private string[] DynamicToStringArray(object o) => ((Array)o).Cast<string>().ToArray();
    }
}