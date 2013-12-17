using SortLib;
using SortVis;
using System.ComponentModel.Composition;

namespace n_log_n
{
    /// <summary>
    /// Implementation of heapsort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Heapsort")]
    public class Heapsort : SorterBase
    {
        private int _n;

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            Heapify();
            SortedTo = _n;
            for (int i = _n - 1; i >= 0; --i)
            {
                Abort.ThrowIfCancellationRequested();
                ExtractMax();
            }
        }

        private void ExtractMax()
        {
            Swap(0, _n - 1);
            SortedFrom = --_n;
            BubbleDown(0);
        }

        /// <summary>
        /// Turns an array into a binary heap.
        /// </summary>
        protected void Heapify()
        {
            _n = Numbers.Length;
            for (int i = (_n - 1) / 2; i >= 0; --i)
            {
                Abort.ThrowIfCancellationRequested();
                BubbleDown(i);
            }
        }

        private int BubbleDown(int i)
        {
            int child = 2 * i + 1;
            if (child < _n)
            {
                int right = child + 1;
                if (right < _n && CompareInArray(child, right) < 0)
                {
                    child = right;
                }
                
                if (CompareInArray(i, child) < 0)
                {
                    Swap(i, child);
                    return BubbleDown(child);
                }
            }
            return i;
        }
    }
}
