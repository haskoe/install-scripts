using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aasted
{
    public static class Helper
    {
        public static string[] DynamicToStringArray(object o) => ((Array)o).Cast<string>().ToArray();

        public static string RemoveWhiteSpace(string s) => new string(s.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }
}
