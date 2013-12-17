using SortLib;
using SortVis;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace n_squared
{
    /// <summary>
    /// Implementation of selection sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Shellsort")]
    public class ShellSort : SorterBase
    {
        /// <summary>
        /// Do the actual sorting work. Without the gap sizes, it would be the same as <see cref="InsertionSort"/>.
        /// </summary>
        protected override void SortIt()
        {
            int n = Numbers.Length;
            // We need all gap sizes that are smaller than the number of items to sort in decreasing order.
            var hs = Steps().TakeWhile(h => h < n).ToArray();
            Array.Reverse(hs);

            foreach (int gap in hs)
            {
                // A gap size of 1 (last iteration) makes this insertion sort, but it has to do less work then.
                for (int i = gap; i < n; ++i)
                {
                    int toInsert = Numbers[i];
                    int j = i - gap;
                    for (; j >= 0 && CompareNum(Numbers[j], toInsert) > 0; j -= gap)
                    {
                        Abort.ThrowIfCancellationRequested();
                        Shift(j, j + gap);
                    }
                    Write(toInsert, j + gap);
                    if (gap == 1)
                    {
                        SortedTo = i;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the step sizes according to Tokuda recursion.
        /// </summary>
        /// <returns>An increasing sequence of gap sizes.</returns>
        private IEnumerable<int> Steps()
        {
            int h = 1;
            for (;;)
            {
                yield return h;
                h = (int)Math.Ceiling(2.25f * h + 1.0f);
            }
        }
    }
}
