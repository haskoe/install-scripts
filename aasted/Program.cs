using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aasted
{
    class Program
    {
        static void Main(string[] args)
        {
            string wordFile = Environment.ExpandEnvironmentVariables(Path.Combine("%USERPROFILE%", "Downloads", "t.doc"));

            var doc = new AastedPriceMacro(wordFile);
        }
    }
}
