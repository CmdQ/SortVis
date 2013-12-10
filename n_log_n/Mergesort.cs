using SortLib;
using SortVis;
using System;
using System.ComponentModel.Composition;

namespace n_log_n
{
    /// <summary>
    /// Implementation of merge sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Merge sort")]
    public class Mergesort : SorterBase
    {
        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            int n = Numbers.Length;
            var store = new int[n];

            for (int len = 1; len < n; len <<= 1)
            {
                int len2 = len << 1;
                for (int i = 0; !Abort.IsCancellationRequested && i < n; i += len2)
                {
                    int ilen = i + len;
                    if (ilen >= n)
                    {
                        break;
                    }
                    Merge(store, i, ilen, Math.Min(i + len2, n));
                }
            }
        }

        private void Merge(int[] store, int lo, int mi, int hi)
        {
            int cursor = lo;

            // Copy always the smaller element.
            int l = lo;
            int h = mi;
            while (!Abort.IsCancellationRequested && l < mi && h < hi)
            {
                if (CompareInArray(l, h) <= 0)
                {
                    store[cursor++] = Numbers[l++];
                }
                else
                {
                    store[cursor++] = Numbers[h++];
                }
            }

            if (l == h)
            {
                return;
            }

            // Copy the rest.
            Array.Copy(Numbers, l, store, cursor, mi - l);
            cursor += mi - l;
            Array.Copy(Numbers, h, store, cursor, hi - h);

            // Write back.
            for (mi = lo; mi < hi; ++mi)
            {
                Write(store[mi], mi);
            }
            Writes += (hi - lo) << 1;
        }
    }
}
