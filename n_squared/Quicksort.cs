using SortLib;
using SortVis;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace n_squared
{
    /// <summary>
    /// Implementation of quicksort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Quicksort")]
    public class QuickSort : SorterBase
    {
#if MEDIAN_OF_3
        private class PivotComarer : Comparer<Tuple<int,int>>
        {
            private Comparer<int> _comparer;

            public PivotComarer(Comparer<int> Comparer)
            {
                this._comparer = Comparer;
            }

            public override int Compare(Tuple<int, int> x, Tuple<int, int> y)
            {
                return _comparer.Compare(x.Item1, y.Item1);
            }
        }

        private Tuple<int,int>[] _pivots;
        private PivotComarer _pivotCompare;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSort"/> class.
        /// </summary>
        public QuickSort()
        {
            ConsideredBig = 8;
        }

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
#if MEDIAN_OF_3
            _pivots = new Tuple<int,int>[3];
            _pivotCompare = new PivotComarer(Comparer);
#endif
            SortIt(0, Numbers.Length);
        }

        private void SortIt(int lo, int hi)
        {
            Abort.ThrowIfCancellationRequested();

            if (hi - lo < ConsideredBig)
            {
                InsertionSort.Sort(Numbers, lo, hi, CompareNum, Shift, Write, Abort);
                return;
            }

            int pivot = Partition(lo, hi);
            SortIt(lo, pivot);
            SortIt(pivot, hi);
        }

        private int Partition(int lo, int hi)
        {
            int pivot = lo + (hi - lo) / 2;

#if MEDIAN_OF_3
            _pivots[0] = Tuple.Create(Numbers[0], 0);
            _pivots[1] = Tuple.Create(Numbers[pivot], pivot);
            _pivots[2] = Tuple.Create(Numbers[hi - 1], hi - 1);
            InsertionSort.Sort(_pivots, 0, 3, _pivotCompare.Compare);
            pivot = _pivots[1].Item2;
#endif

            Swap(pivot, --hi);
            pivot = hi;

            while (lo < hi)
            {
                Abort.ThrowIfCancellationRequested();
                while (CompareInArray(lo, pivot) <= 0)
                {
                    if (++lo == hi)
                    {
                        goto partition_done;
                    }
                }
                do
                {
                    if (--hi == lo)
                    {
                        goto partition_done;
                    }
                } while (CompareInArray(hi, pivot) >= 0);

                Swap(lo++, hi);
            }

        partition_done:
            Swap(lo, pivot);

            return lo;
        }
    }
}
