using SortLib;
using System;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Linear descending number generator.
    /// </summary>
    [Export(typeof(IGenerator))]
    [ExportMetadata("Name", "Descending")]
    public class DescendingGenerator : GeneratorBase
    {
        /// <summary>
        /// Generate numbers.
        /// </summary>
        protected override void Generate()
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                Numbers[i] = Count - i;
            }
        }
    }
}
