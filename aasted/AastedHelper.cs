using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aasted
{
    class AastedHelper
    {
        internal const string DATEFORMAT_POSTFIX = "yyyyMMddhhmmss";

        internal static string ProcessedDocFileName(string docFileName, string tempDir)
        {
            string directoryPart = Path.GetDirectoryName(docFileName);

            string fileNamePart = Path.GetFileNameWithoutExtension(docFileName);

            return Path.Combine(tempDir ?? directoryPart, $"{fileNamePart}-{DateTime.Now.ToString(DATEFORMAT_POSTFIX)}{Path.GetExtension(docFileName)}");
        }
    }
}
