using n_log_n;
using SortLib;
using SortVis;
using System;
using System.Collections.Generic;
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
                Heapify();
                Reverse();
                var org = Comparer;
                Comparer = Comparer.Invert();
                Heapify();
                SortedTo = 1;
                Comparer = org;
                InsertionSort.Sort(Numbers, 1, Numbers.Length, org.Compare, Shift, Write, Abort, t => SortedTo = t);
            }
        }

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
