﻿using SortLib;
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
            SortedFrom = SortedTo = n;
            do
            {
                int newN = 1;
                for (int i = 0; !Abort.IsCancellationRequested && i < n - 1; ++i)
                {
                    if (CompareInArray(i, i + 1) > 0)
                    {
                        Swap(i, i + 1);
                        newN = i + 1;
                    }
                }
                SortedFrom = n = newN;
            } while (!Abort.IsCancellationRequested && n > 1);
        }
    }
}
