// Define your strategy here.
#define MEDIAN_RAND

#if !(MEDIAN_OF_3 || MEDIAN_RAND || MEDIAN_MIDDLE)
#warning Define one of the above median strategies for quicksort.
#define MEDIAN_LAST
#endif

using SortLib;
using SortVis;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;

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
        /// <summary>
        /// Provides a place to store some code that must be executed when a block finishes (use with <c>using</c>).
        /// </summary>
        private class FinalAction : IDisposable
        {
            private readonly Action _finalAction;

            /// <summary>
            /// Registers code to be executed when this instance will be disposed.
            /// </summary>
            /// <param name="finalAction">The swap action to do on dying.</param>
            public FinalAction(Action finalAction)
            {
                _finalAction = finalAction;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing,
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _finalAction();
            }
        }

#if MEDIAN_OF_3
        private class PivotComarer : System.Collections.Generic.Comparer<Tuple<int,int>>
        {
            private System.Collections.Generic.Comparer<int> _comparer;

            public PivotComarer(System.Collections.Generic.Comparer<int> Comparer)
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
#elif MEDIAN_RAND
        private Random _rand;
#endif
        private readonly List<Tuple<int, int>> _sortedRanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSort"/> class.
        /// </summary>
        public QuickSort()
        {
            ConsideredBig = 8;
            _sortedRanges = new List<Tuple<int, int>>();
#if MEDIAN_RAND
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
            _sortedRanges.Clear();
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
                    _sortedRanges.Add(Tuple.Create(pivot, hi));
                    // ... and sort smaller by resetting bounds.
                    hi = pivot;
                }
                else
                {
                    // Right half is bigger, so recurse in smaller half.
                    SortIt(lo, pivot);
                    _sortedRanges.Add(Tuple.Create(lo, pivot));
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
            int pivot = ChoosePivot(lo, ref hi);

#if !MEDIAN_LAST
            // Swap the pivot to the right and concentrate on the values to the left of it.
            Swap(pivot, --hi);
            pivot = hi;
#endif

            using (var rs = new FinalAction(() => Swap(lo, pivot)))
            {
                while (lo < hi)
                {
                    Abort.ThrowIfCancellationRequested();
                    while (CompareInArray(lo, pivot) <= 0)
                    {
                        // Increase the "left" pointer while its values are smaller than the pivot.
                        if (++lo == hi)
                        {
                            // Pointers joined.
                            return lo;
                        }
                    }
                    do
                    {
                        // Decrease the "right" pointer while its values are greater than the pivot.
                        if (--hi == lo)
                        {
                            // Pointers joined.
                            return lo;
                        }
                    } while (CompareInArray(hi, pivot) >= 0);

                    // "Left" now points to a big element in the left part and "right" points to a small element.
                    Swap(lo++, hi);
                }
            }

            return lo;
        }

        /// <summary>
        /// Chooses a pivot element in the range [lo; hi[.
        /// </summary>
        /// <param name="lo">Inclusive lower bound.</param>
        /// <param name="hi">Exclusive upper bound.</param>
        /// <returns>An index where the chosen pivot element is to be found.</returns>
        protected virtual int ChoosePivot(int lo, ref int hi)
        {
#if MEDIAN_LAST
            return --hi;
#elif MEDIAN_MIDDLE || MEDIAN_OF_3
            // Deterministic middle element, easily tricked.
            int pivot = lo + (hi - lo) / 2;
#if MEDIAN_OF_3
            // Extract the first, the last and the value in the middle into a tiny array remembering the positions.
            _pivots[0] = Tuple.Create(Numbers[0], 0);
            _pivots[1] = Tuple.Create(Numbers[pivot], pivot);
            _pivots[2] = Tuple.Create(Numbers[hi - 1], hi - 1);
            // Sort them wrt. the values.
            InsertionSort.Sort(_pivots, 0, 3, _pivotCompare.Compare);
            // The median of 3 is then the position of the tuple in the middle.
            pivot = _pivots[1].Item2;
#endif//MEDIAN_OF_3
            return pivot;
#elif MEDIAN_RAND
            // A randomly chosen pivot cannot be tricked and results almost always in O(n*log(n)) run time.
            return _rand.Next(lo, hi);
            // It should also be faster than sorting 3 values every time.
#endif
        }

        /// <summary>
        /// Check whether an array index falls into a partially sorted range.
        /// </summary>
        /// <param name="i">An index into the array to be sorted.</param>
        /// <returns>
        ///   <c>true</c> if in a sorted part, <c>false</c> otherwise.
        /// </returns>
        protected override bool IsPartiallySorted(int i)
        {
            if (base.IsPartiallySorted(i))
            {
                return true;
            }

            return _sortedRanges.Exists(tup => i >= tup.Item1 && i < tup.Item2);
        }
    }
}
 