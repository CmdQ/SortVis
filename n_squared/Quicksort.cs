using SortLib;
using SortVis;
using System;
using System.ComponentModel.Composition;

namespace n_squared
{
    /// <summary>
    /// Implementation of quicksort that uses <see cref="InsertionSort"/> below a threshold (<see cref="SorterBase.ConsideredBig"/>)
    /// and limits stack size by only recursing in the smaller partition.
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
#else
        private Random _rand;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSort"/> class.
        /// </summary>
        public QuickSort()
        {
            ConsideredBig = 8;
#if !MEDIAN_OF_3
            _rand = new Random();
#endif
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
            while (lo < hi)
            {
                Abort.ThrowIfCancellationRequested();
                if (hi - lo < ConsideredBig)
                {
                    // If the interval gets to small, we don't bother with quicksort and use insertion sort.
                    InsertionSort.Sort(Numbers, lo, hi, CompareNum, Shift, Write, Abort);
                    return;
                }


                int pivot = Partition(lo, hi);

                if (pivot - lo > hi - pivot)
                {
                    // Left half is bigger, so recurse in smaller half...
                    SortIt(pivot, hi);
                    // ... and sort smaller by resetting bounds.
                    hi = pivot;
                }
                else
                {
                    // Right half is bigger, so recurse in smaller half.
                    SortIt(lo, pivot);
                    // ... and sort smaller by resetting bounds.
                    lo = pivot;
                }
            }
        }

        /// <summary>
        /// Partition an array into small and large values based on a pivot element.
        /// </summary>
        /// <param name="lo">Lower index to partition from.</param>
        /// <param name="hi">Exclusive upper index to partition to.</param>
        /// <returns>The position of the chosen pivot element.</returns>
        private int Partition(int lo, int hi)
        {
            // Deterministic middle element, easily tricked.
            //int pivot = lo + (hi - lo) / 2;

#if MEDIAN_OF_3
            // Extract the first, the last and the value in the middle into a tiny array remembering the positions.
            _pivots[0] = Tuple.Create(Numbers[0], 0);
            _pivots[1] = Tuple.Create(Numbers[pivot], pivot);
            _pivots[2] = Tuple.Create(Numbers[hi - 1], hi - 1);
            // Sort them wrt. the values.
            InsertionSort.Sort(_pivots, 0, 3, _pivotCompare.Compare);
            // The median of 3 is then the position of the tuple in the middle.
            int pivot = _pivots[1].Item2;
#else
            // A randomly chosen pivot cannot be tricked and results almost always in O(n*log(n)) run time.
            int pivot = _rand.Next(lo, hi);
            // It should also be faster than sorting 3 values every time.
#endif
            // Swap the pivot to the right and concentrate on the values to the left of it.
            Swap(pivot, --hi);
            pivot = hi;

            while (lo < hi)
            {
                Abort.ThrowIfCancellationRequested();
                while (CompareInArray(lo, pivot) <= 0)
                {
                    // Increase the "left" pointer while its values are smaller than the pivot.
                    if (++lo == hi)
                    {
                        // Pointers joined.
                        goto partition_done;
                    }
                }
                do
                {
                    // Decrease the "right" pointer while its values are greater than the pivot.
                    if (--hi == lo)
                    {
                        // Pointers joined.
                        goto partition_done;
                    }
                } while (CompareInArray(hi, pivot) >= 0);

                // "Left" now points to a big element in the left part and "right" points to a small element.
                Swap(lo++, hi);
            }

        partition_done:
            // We still have to swap our pivot in the middle of the two parts.
            Swap(lo, pivot);

            return lo;
        }
    }
}
 