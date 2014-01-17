using n_squared;
using SortLib;
using System;
using System.ComponentModel.Composition;

namespace n_log_n
{
    /// <summary>
    /// A version of <see cref="QuickSort"/> that always chooses a good pivot
    /// point and hence becomes <c>O(n * log(n))</c>.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Balanced Quicksort")]
    public class BalancedQuicksort : QuickSort
    {
        private int[] _median;

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            _median = new int[Numbers.Length];
            base.SortIt();
        }

        /// <summary>
        /// Chooses a pivot element in the range [lo; hi[.
        /// </summary>
        /// <param name="lo">Inclusive lower bound.</param>
        /// <param name="hi">Exclusive upper bound.</param>
        /// <returns>
        /// An index where the chosen pivot element is to be found.
        /// </returns>
        protected override int ChoosePivot(int lo, ref int hi)
        {
            int count = hi - lo;
            Array.Copy(Numbers, lo, _median, lo, count);
            Writes += count;

            int pivotValue = Recurse(lo, hi);
            return Array.IndexOf(Numbers, pivotValue, lo);
        }

        private int Recurse(int lo, int hi)
        {
            const int fixedLength = 5;
            int gather = lo;
            for (int i = lo; i < hi; i += fixedLength)
            {
                int upper = Math.Min(hi - 1, i + fixedLength);
                InsertionSort.Sort<int>(_median, i, upper, CompareNum,
                    (from, to) =>
                    {
                        _median[to] = _median[from];
                        ++Writes;
                    }, (value, pos) =>
                    {
                        _median[pos] = value;
                        ++Writes;
                    }, Abort);
                _median[gather++] = _median[i + (upper - i) / 2];
                ++Writes;
            }
            if (hi - lo <= fixedLength)
            {
                return _median[lo];
            }
            return Recurse(lo, gather);
        }
    }
}
