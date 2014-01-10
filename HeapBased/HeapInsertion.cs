using n_log_n;
using SortLib;
using SortVis;
using System.ComponentModel.Composition;

namespace n_squared
{
    /// <summary>
    /// Implementation of insertion sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Heap insertion")]
    public class HeapInsertion : Heapsort
    {
        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            if (Numbers.Length > 1)
            {
                // This heapify puts large values near the beginning...
                Heapify();
                // ... so we reverse afterwards.
                Reverse();

                // Then we use an inverse comparer...
                var org = Comparer;
                Comparer = Comparer.Invert();
                // ... so that this heapify puts small values near the beginning.
                Heapify();
                // We now have done some work and know that the first position is final.
                SortedTo = 1;
                // So with the original comparer, we make use of insertion sort, which does well on partially sorted fields.
                Comparer = org;
                InsertionSort.Sort(Numbers, 1, Numbers.Length, org.Compare, Shift, Write, Abort, t => SortedTo = t);
            }
        }

        /// <summary>
        /// Reverse an array making use of our <see cref="SorterBase.Swap"/>.
        /// </summary>
        private void Reverse()
        {
            int n = Numbers.Length;
            for (int i = n / 2; i >= 0; --i)
            {
                Abort.ThrowIfCancellationRequested();
                Swap(i, n - i - 1);
            }
        }
    }
}
