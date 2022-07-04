using System;
using System.Windows.Forms;

namespace aasted
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            //var mgr = new MainFormManager();
            //mgr.Refresh();

            //string wordFile = Environment.ExpandEnvironmentVariables(Path.Combine("%USERPROFILE%", "Downloads", "t.doc"));

            //var doc = new AastedPriceMacro(wordFile, null);
        }
    }
}
