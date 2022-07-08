using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

using Res = aasted.Properties.Resources;

namespace aasted
{
    public class MainFormManager
    {
        public class ShowMessageEventArgs : EventArgs
        {
            public string Message { get; private set; }

            private ShowMessageEventArgs(string message)
            {
                Message = message;
            }

            public static ShowMessageEventArgs New(string message) => new ShowMessageEventArgs(message);
        }

        public EventHandler<ShowMessageEventArgs> ShowMessage;

        private void InvokeShowMessage(string message) => ShowMessage?.Invoke(this, ShowMessageEventArgs.New(message.Replace(Environment.NewLine,"<br/>")));

        private static Regex DATEFORMAT_POSTFIX_REGEX = new Regex(string.Format(@"-\d{{{0}}}\.", AastedHelper.DATEFORMAT_POSTFIX.Length));
        public bool IsProcessed(string docFileName) => PostFixStart(docFileName) > 0;

        public string AppDirectory { get; private set; }

        public string ValidationErrors => _macro?.ValidationErrors ?? "";
        public bool ValidDocument => string.IsNullOrEmpty(ValidationErrors);

        public bool CanOverwrite { get; private set; }

        private AastedPriceMacro _macro;
        private string _currentWorkFileName;
        private string _docFileName;

        public MainFormManager()
        {
            AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public string TryProcessLatestFile()
        {
            CanOverwrite = false;
            string result = null;

            string docDir = Properties.Settings.Default.DocDirName;
            InvokeShowMessage(string.Format(Res.FindingMostRecentEditedDocument, docDir));

            var recentDocuments = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent))
                .Where(sc => Helper.IsShortcutLink(sc, "doc"))
                .Select(sc => Helper.GetShortcutTargetFile(sc))
                .ToArray();
            var lastAccessedFiles = recentDocuments
                .Where(f => Path.GetDirectoryName(f).Equals(docDir, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .ToArray();

            if (!lastAccessedFiles.Any())
            {
                InvokeShowMessage(string.Format(Res.CouldNotFindDocument, docDir));
                return null;
            }

            InvokeShowMessage(string.Format(Res.ProcessingFile, docDir));
            _docFileName = lastAccessedFiles.First();
            try
            {
                var fileInfo = new FileInfo(_docFileName);
                using (FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                }
            }
            catch (Exception ex)
            {
                InvokeShowMessage(string.Format(Res.ErrorMessageDocumentIsLocked, _docFileName));
                return null;
            }

            _currentWorkFileName = AastedHelper.ProcessedDocFileName(_docFileName, TempDir(_docFileName));
            try
            {
                _macro = new AastedPriceMacro(_docFileName, _currentWorkFileName);
                CanOverwrite = false;

                Overwrite();
                if (ValidDocument)
                {
                    InvokeShowMessage(string.Format(Res.DocumentProcessed, _docFileName));
                }
                else
                {
                    InvokeShowMessage(string.Format(Res.DocumentProcessedWithErrors, _docFileName, ValidationErrors));
                }
                result = _docFileName;
            }
            catch (Exception ex)
            {
                InvokeShowMessage(string.Format(Res.ErrorMessageCouldNotOpenDocument, _docFileName, ex.ToString()));
                return null;
            }

            return result;
        }

        public string Overwrite()
        {
            File.Copy(_currentWorkFileName, _docFileName, true);
            File.Delete(_currentWorkFileName);
            return _docFileName;
        }

        private int PostFixStart(string docFileName) => Helper.RegExMatchStart(DATEFORMAT_POSTFIX_REGEX, docFileName);
        private string BatchDir => Helper.CreateDirectoryIfNotExists(AppDirectory, "batch");
        private string TempDir(string docFileName) => Helper.CreateDirectoryIfNotExists(Path.GetDirectoryName(docFileName), Properties.Settings.Default.TempSubDirName);
    }
}
