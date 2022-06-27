using W = Microsoft.Office.Interop.Word;

namespace aasted
{
    public class StyleTextAndPos
    {
        public string Text { get; private set; }
        public int Pos { get; private set; }

        private StyleTextAndPos(int pos, string text) 
        {
            Pos = pos;
            Text = text;
        }

        public static StyleTextAndPos New(int pos, string text) => new StyleTextAndPos(pos, text);

        public static StyleTextAndPos FromSelection(W.Selection selection) => New(selection.Range.Start, selection.Text);

        public override string ToString() => $"Pos:{Pos} Text:{Text}";
    }
}
