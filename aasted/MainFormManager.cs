using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

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
            //_docDirectories = new string[] { };

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
            CurrentSourceFileName = docFileName;
            CurrentWorkFileName = AastedHelper.ProcessedDocFileName(docFileName, TempDir);

            _macro = new AastedPriceMacro(CurrentSourceFileName, CurrentWorkFileName);
        }

        private void AddToDocDirectories(string docFileName)
        {
            string directoryPart = Path.GetDirectoryName(docFileName);
            if (!_docDirectories.Any(docDir => docDir.Equals(directoryPart, StringComparison.CurrentCultureIgnoreCase)))
            {
                _docDirectories = _docDirectories
                    .ToList()
                    .Append(directoryPart)
                    .ToArray();

                File.WriteAllLines(AppStateFileName, _docDirectories);
                Refresh();
            }
        }

        private int PostFixStart(string docFileName) => Helper.RegExMatchStart(DATEFORMAT_POSTFIX_REGEX, docFileName);
        private string BatchDir => Helper.CreateDirectoryIfNotExists(AppDirectory, "batch");
        private string TempDir => Helper.CreateDirectoryIfNotExists(AppDirectory, "temp");
    }
}
