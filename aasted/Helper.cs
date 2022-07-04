using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace aasted
{
    public static class Helper
    {
        public static string[] DynamicToStringArray(object o) => ((Array)o).Cast<string>().ToArray();

        public static string RemoveWhiteSpace(string s) => new string(s.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());

        public static int RegExMatchStart(Regex re, string aString)
        {
            var mtch = re.Match(aString);
            return mtch.Success ? aString.IndexOf(mtch.Value) : -1;
        }

        public static string CreateDirectoryIfNotExists(string parentDir, string subDir)
        {
            string resultingDir = Path.Combine(parentDir, subDir);
            if (!Directory.Exists(resultingDir))
                Directory.CreateDirectory(resultingDir);
            return resultingDir;
        }
    }
}
