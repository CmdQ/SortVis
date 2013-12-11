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
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            int n = Numbers.Length;
            var hs = Steps().TakeWhile(h => h < n).ToArray();
            Array.Reverse(hs);

            foreach (int gap in hs)
            {
                for (int i = gap; !Abort.IsCancellationRequested && i < n; ++i)
                {
                    int temp = Numbers[i];
                    int j = i - gap;
                    for (; !Abort.IsCancellationRequested && j >= 0 && CompareNum(Numbers[j], temp) > 0; j -= gap)
                    {
                        Shift(j, j + gap);
                    }
                    Write(temp, j + gap);
                    if (gap == 1)
                    {
                        SortedTo = i;
                    }
                }
                if (Abort.IsCancellationRequested)
                {
                    break;
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
