using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ns_YAUtils
{
    public class Algorithms
    {
        static void subBinarySearch(int begin, int end, out int idx, Func<int, bool> func)
        {
            if (begin == end || begin == end-1)
            {
                idx = end;
            }
            else
            {
                int mid = (begin + end) / 2;
                if (func(mid))
                {
                    subBinarySearch(mid, end, out idx, func);//tail optimas, of couse
                }
                else
                {
                    subBinarySearch(begin, mid, out idx, func);
                }
            }
        }

        public static int binarySearch(int st, Func<int, bool> func)
        {
            int ret = 0;
            subBinarySearch(0, st, out ret, func);
            return ret;
        }
    }
}
