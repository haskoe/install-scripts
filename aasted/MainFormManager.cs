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
            var recentDocuments = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent))
                .Where(sc => sc.Split('.').Length > 2 && sc.Split('.').Last().Equals("lnk", StringComparison.CurrentCultureIgnoreCase) && sc.Split('.')[sc.Split('.').Length - 2].Equals("doc", StringComparison.CurrentCultureIgnoreCase))
                .Select(sc => Helper.GetShortcutTargetFile(sc))
                .ToArray();
            var lastAccessedFiles = recentDocuments
                .Where(f => Path.GetDirectoryName(f).Equals(docDir, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .ToArray();
            
            if (!lastAccessedFiles.Any())
                ThrowError(Properties.Settings.Default.ErrorMessageNoDocs + " " + docDir);

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
                ThrowError(Properties.Settings.Default.ErrorMessageCouldNotOpenDoc + " " + _docFileName);
            }

            _currentWorkFileName = AastedHelper.ProcessedDocFileName(_docFileName, TempDir(_docFileName));
            try
            {
                _macro = new AastedPriceMacro(_docFileName, _currentWorkFileName);
                CanOverwrite = true;
                if (ValidDocument)
                {
                    Overwrite();
                    result = _docFileName;
                }

            }
            catch (Exception ex)
            {
                ThrowError(Properties.Settings.Default.ErrorMessageCouldNotOpenDoc + " " + _docFileName);
            }

            return result;
        }

        public string Overwrite()
        {
            if (!CanOverwrite)
                throw new ApplicationException("Overwrite called but cannot overwrite");

            File.Copy(_currentWorkFileName, _docFileName);
            File.Delete(_currentWorkFileName);
            return _docFileName;
        }
        private void ThrowError(string errorMessage)
        {
            throw new ApplicationException(errorMessage);
        }

        private int PostFixStart(string docFileName) => Helper.RegExMatchStart(DATEFORMAT_POSTFIX_REGEX, docFileName);
        private string BatchDir => Helper.CreateDirectoryIfNotExists(AppDirectory, "batch");
        private string TempDir(string docFileName) => Helper.CreateDirectoryIfNotExists(Path.GetDirectoryName(docFileName), Properties.Settings.Default.TempSubDirName);
    }
}
