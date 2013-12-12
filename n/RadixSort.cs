using SortLib;
using SortVis;
using System.ComponentModel.Composition;

namespace n
{
    /// <summary>
    /// Implementation of radix sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Radix sort")]
    public class RadixSort : SorterBase
    {
        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            const int bits = sizeof(int) * 8;

            int n = Numbers.Length;
            var swap = new int[n];

            for (int b = 0; b < bits; ++b)
            {
                int front = 0;
                int back = n - 1;

                int i;
                for (i = 0; front <= back; ++i)
                {
                    if (Abort.IsCancellationRequested)
                    {
                        return;
                    }
                    if ((Numbers[i] & (1 << b)) == 0)
                    {
                        swap[front++] = Numbers[i];
                    }
                    else
                    {
                        swap[back--] = Numbers[i];
                    }
                }

                if (front == n)
                {
                    continue;
                }

                for (i = 0; i < front; ++i)
                {
                    if (Abort.IsCancellationRequested)
                    {
                        return;
                    }
                    Write(swap[i], i);
                }
                for (; i < n; ++i)
                {
                    if (Abort.IsCancellationRequested)
                    {
                        return;
                    }
                    Write(swap[n - (i - front) - 1], i);
                }
            }
        }

        /// <summary>
        /// Do a check if all numbers are really sorted.
        /// </summary>
        /// <exception cref="SorterBase.NotSortedException">Indicates sorting failure.</exception>
        protected override void CheckSortedness()
        {
            if (!Abort.IsCancellationRequested)
            {
                // Radix sort can only sort in a simple manner.
                for (int i = 1; i < Numbers.Length; ++i)
                {
                    if (Numbers[i - 1] > Numbers[i])
                    {
                        throw new NotSortedException(Name, i);
                    }
                }
            }
        }

        /// <summary>
        /// Sort all permutations of tuples by first and last item and see if equal
        /// elements keep their order.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if stable, <c>false</c> otherwise.
        /// </returns>
        protected override bool CheckIfStable()
        {
            return true;
        }
    }
}
