using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace aasted
{
    public class MainFormManager
    {
        private static Regex DATEFORMAT_POSTFIX_REGEX = new Regex(string.Format(@"-\d{{{0}}}\.", AastedHelper.DATEFORMAT_POSTFIX.Length));
        public bool IsProcessed(string docFileName) => PostFixStart(docFileName) > 0;

        private const string APPSTATE_FILE = "appstate.txt";
        private string AppStateFileName => Path.Combine(AppDirectory, APPSTATE_FILE);

        public string AppDirectory { get; private set; }
        public string CurrentSourceFileName { get; private set; }
        public string CurrentWorkFileName { get; private set; }
        public string ValidationErrors => _macro?.ValidationErrors ?? "";

        private string[] _docDirectories;
        private AastedPriceMacro _macro;

        public MainFormManager()
        {
            AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public void Refresh()
        {
            _docDirectories = Properties.Settings.Default.DocDirectories
                .Cast<string>()
                .ToArray();

            //if (File.Exists(AppStateFileName))
            //    _docDirectories = File.ReadAllLines(AppStateFileName)
            //        .Where(docDir => Directory.Exists(docDir))
            //        .ToArray();

            //string docFileName = @"c:\a\b\c\d\e.doc";
            //var isProcessed = IsProcessed(docFileName);
            //string processedDocFileName = AastedHelper.ProcessedDocFileName(docFileName, TempDir);
            //isProcessed = IsProcessed(processedDocFileName);
            CurrentSourceFileName = "";
            CurrentWorkFileName = "";
        }

        public void FileSelected(string docFileName)
        {
            ProcessFile(docFileName);
        }

        private void ProcessFile(string docFileName)
        {
            AddToDocDirectories(docFileName);
            CurrentSourceFileName = docFileName;
            CurrentWorkFileName = AastedHelper.ProcessedDocFileName(docFileName, TempDir(docFileName));

            _macro = new AastedPriceMacro(CurrentSourceFileName, CurrentWorkFileName);
        }

        private void AddToDocDirectories(string docFileName)
        {
            string directoryPart = Path.GetDirectoryName(docFileName);
            if (!_docDirectories.Any(docDir => docDir.Equals(directoryPart, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    // create temp and output directories
                    TempDir(directoryPart);
                    OutputDir(directoryPart);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Could not create temp and output directories under {0}", 
                }

                _docDirectories = _docDirectories
                    .ToList()
                    .Append(directoryPart)
                    .ToArray();

                Properties.Settings.Default.DocDirectories = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.DocDirectories.AddRange(_docDirectories);
            }
        }

        private int PostFixStart(string docFileName) => Helper.RegExMatchStart(DATEFORMAT_POSTFIX_REGEX, docFileName);
        private string BatchDir => Helper.CreateDirectoryIfNotExists(AppDirectory, "batch");
        private string TempDir(string docFileName) => Helper.CreateDirectoryIfNotExists(Path.GetDirectoryName(docFileName), Properties.Settings.Default.TempDirName);
        private string OutputDir(string docFileName) => Helper.CreateDirectoryIfNotExists(Path.GetDirectoryName(docFileName), Properties.Settings.Default.OutputDirName);
    }
}
