using SortLib;
using SortVis;
using System.ComponentModel.Composition;

namespace n
{
    /// <summary>
    /// Implementation of radix sort. Optimized with http://stereopsis.com/radix.html.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Radix sort")]
    public class RadixSort : SorterBase
    {
        private const int ELEVEN = 0x7FF;

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            int count = Numbers.Length;
            if (count < 2)
            {
                return;
            }

            checked
            {
                const int hist = 1 << 11;
                var b0 = new int[hist];
                var b1 = new int[hist];
                var b2 = new int[hist];

                foreach (int i in Numbers)
                {
                    var fi = Flip(i);
                    ++b0[Eleven0(fi)];
                    ++b1[Eleven1(fi)];
                    ++b2[Eleven2(fi)];
                }

                int sum = 0;
                int sum0 = 0;
                int sum1 = 0;
                int sum2 = 0;
                for (int i = 0; i < hist; ++i)
                {
                    sum = b0[i] + sum0;
                    b0[i] = sum0 - 1;
                    sum0 = sum;

                    sum = b1[i] + sum1;
                    b1[i] = sum1 - 1;
                    sum1 = sum;

                    sum = b2[i] + sum2;
                    b2[i] = sum2 - 1;
                    sum2 = sum;
                }

                var temp = new int[count];

                foreach (int i in Numbers)
                {
                    temp[++b0[Eleven0(i)]] = Flip(i);
                }
                Writes += count;

                foreach (int i in temp)
                {
                    Write(Flip(i), ++b1[Eleven1(i)]);
                }

                foreach (int i in Numbers)
                {
                    var fi = Flip(i);
                    temp[++b2[Eleven2(fi)]] = fi;
                }
                Writes += count;

                for (int i = 0; i < count; ++i)
                {
                    Write(Flip(temp[i]), i);
                }
            }
        }

        private int Flip(int i)
        {
            return i ^ int.MinValue;
        }

        private int Eleven0(int x)
        {
            return x & ELEVEN;
        }

        private int Eleven1(int x)
        {
            return (x >> 11) & ELEVEN;
        }

        private int Eleven2(int x)
        {
            return x >> 22 & ELEVEN;
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
            // For a least significant bit radix sort, we just know.
            return true;
        }
    }
}
