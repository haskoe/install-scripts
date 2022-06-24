using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aasted
{
    public static class LinqHelper
    {
        public static IEnumerable<T> MakeGenetator<T>(IEnumerable<T> enu)
        {
            foreach (var t in enu)
            {
                yield return t;
            }
        }

        public static T FirstNotNull<T>(IEnumerable<T> enu)
        {
            foreach (var t in enu)
            {
                if (null != t)
                    return t;
            }

            return default(T);
        }
    }
}
