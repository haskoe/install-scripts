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
        internal const char TAB = '\t';

        internal static string ProcessedDocFileName(string docFileName, string tempDir)
        {
            string directoryPart = Path.GetDirectoryName(docFileName);

            string fileNamePart = Path.GetFileNameWithoutExtension(docFileName);

            return Path.Combine(tempDir ?? directoryPart, $"{fileNamePart}-{DateTime.Now.ToString(DATEFORMAT_POSTFIX)}{Path.GetExtension(docFileName)}");
        }

        internal static bool ValidAastedItemPrice(string aastedItemPrice)
        {
            if (string.IsNullOrEmpty(aastedItemPrice))
                return false;

            string[] splt = aastedItemPrice.Split(TAB);
            return splt.Length >= 2 && splt[splt.Length - 2] == "EUR" && !string.IsNullOrEmpty(splt.Last());
        }

        internal static string GetPriceFromAastedItemPrice(string aastedItemPrice) => ValidAastedItemPrice(aastedItemPrice) ? aastedItemPrice.Split(TAB).Last() : "";
    }
}
