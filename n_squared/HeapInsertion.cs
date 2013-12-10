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
                Comparer = org;
                InsertionSort.Sort(Numbers, 0, Numbers.Length, org.Compare, Shift, Write, Abort);
            }
        }

        private void Reverse()
        {
            int n = Numbers.Length;
            for (int i = n / 2; i >= 0 && !Abort.IsCancellationRequested; --i)
            {
                Swap(i, n - i - 1);
            }
        }
    }
}
