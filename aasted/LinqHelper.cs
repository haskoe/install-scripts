using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aasted
{
    public static class LinqHelper
    {
        public static IEnumerable<T> MakeGenerator<T>(IEnumerable<T> enu)
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

        public static IEnumerable<T> ToEnumerable<T>(Func<int> getCount, Func<int, T> getItem) => Enumerable.Range(0, getCount())
            .Select(i => getItem(i));

        public static IEnumerable<T2> ToEnumerable<T1, T2>(Func<bool> continueFunc, Func<T1> getT1, Func<T2> getT2, Func<T1, bool> addCond = null, Func<T2,int> dupKeyFunc = null)
        {
            HashSet<int> dupKeys = new HashSet<int>();
            bool doContinue = continueFunc();
            while (doContinue)
            {
                T1 t1 = getT1();
                if (null == addCond || addCond(t1))
                {
                    T2 item = getT2();
                    yield return item;
                    if (null != dupKeyFunc)
                    {
                        int dupKey = dupKeyFunc(item);
                        if (!dupKeys.Contains(dupKey))
                            dupKeys.Add(dupKey);
                        else
                            break;
                    }
                }
                doContinue = continueFunc();
            }
        }

        public static IEnumerable<string> GetDuplicates(IEnumerable<string> lst) => lst.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);


        public static IEnumerable<(int index, T value)> Enumerate<T>(IEnumerable<T> coll)
                    => coll.Select((i, val) => (val, i));
    }
}
