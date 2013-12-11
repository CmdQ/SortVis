using SortLib;
using SortVis;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;

namespace n_squared
{
    /// <summary>
    /// Implementation of insertion sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Insertion sort")]
    public class InsertionSort : SorterBase
    {
        /// <summary>
        /// Sorts an array with insertion sort.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="numbers">The numbers to sort.</param>
        /// <param name="lo">Lower inclusive bound of the array to sort.</param>
        /// <param name="hi">Upper exclusive bound of the array to sort.</param>
        /// <param name="compare">Comparison function like that of <see cref="IComparable{T}"/>.</param>
        /// <param name="shift">A function that does shifting of values.</param>
        /// <param name="write">A function to write a value to a position.</param>
        /// <param name="abort">To see whether we want to cancel sorting.</param>
        /// <param name="updateRange">Give this to have a sorted range updated.</param>
        public static void Sort<T>(T[] numbers, int lo, int hi,
            Func<T,T,int> compare = null,
            Action<int, int> shift = null,
            Action<T, int> write = null,
            CancellationToken? abort = null,
            Action<int> updateRange = null)
        {
            compare = compare ?? Comparer<T>.Default.Compare;
            shift = shift ?? ((from, to) => numbers[to] = numbers[from]);
            write = write ?? ((val, pos) => numbers[pos] = val);

            int n = hi - lo;
            for (int i = 1; i < n && (abort == null || !abort.Value.IsCancellationRequested); ++i)
            {
                T temp = numbers[lo + i];
                int j = i - 1;
                for (; j >= 0 && compare(numbers[lo + j], temp) > 0 && (abort == null || !abort.Value.IsCancellationRequested); j--)
                {
                    shift(lo + j, lo + j + 1);
                }
                write(temp, lo + j + 1);

                if (updateRange != null)
                {
                    updateRange(i);
                }
            }
        }

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            Sort(Numbers, 0, Numbers.Length, CompareNum, Shift, Write, Abort, t => SortedTo = t);
        }
    }
}
