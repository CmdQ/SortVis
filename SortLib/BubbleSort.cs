using SortLib;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Implementation of bubble sort.
    /// </summary>
    /// <remarks>
    /// Implemented in executing assembly so that at least one sorter is always found.
    /// </remarks>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Bubblesort")]
    public class BubbleSort : SorterBase
    {
        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            int n = Numbers.Length;
            do
            {
                // The sorted range will grow from the right.
                SortedFrom = n;
                int newN = 1;
                // Run an index from the left to the beginning of the sorted part.
                for (int i = 0; i < n - 1; ++i)
                {
                    Abort.ThrowIfCancellationRequested();
                    if (CompareInArray(i, i + 1) > 0)
                    {
                        // Swap neighbors if the left element is bigger than the right.
                        Swap(i, i + 1);
                        // Remember the biggest position where an element was swapped to.
                        newN = i + 1;
                    }
                }
                // We now know that we don't have to go further than the right-most swap participant.
                n = newN;
            } while (n > 1);
        }
    }
}
