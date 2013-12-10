using SortLib;
using System;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Ascending median of 3 number generator.
    /// </summary>
    [Export(typeof(IGenerator))]
    [ExportMetadata("Name", "Median of 3")]
    public class Median3Generator : GeneratorBase
    {
        /// <summary>
        /// Generate numbers.
        /// </summary>
        protected override void Generate()
        {
            Array.Clear(Numbers, 0, Numbers.Length);
            Recurse(0, Count - 1, 1);
        }

        private void Recurse(int lo, int hi, int num)
        {
            if (lo < hi)
            {
                int mi = lo + (hi - lo) / 2;
                if (Numbers[lo] == 0)
                {
                    Numbers[lo] = num;
                }
                if (Numbers[mi] == 0)
                {
                    Numbers[mi] = num;
                }
                if (Numbers[hi] == 0)
                {
                    Numbers[hi] = num;
                }
                Recurse(lo, mi, num + 1);
                Recurse(mi + 1, hi, num + 1);
            }
        }
    }
}
