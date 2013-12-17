using SortLib;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Linear ascending number generator.
    /// </summary>
    [Export(typeof(IGenerator))]
    [ExportMetadata("Name", "Ascending")]
    public class AscendingGenerator : GeneratorBase
    {
        /// <summary>
        /// Generate numbers.
        /// </summary>
        protected override void Generate()
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                Numbers[i] = i + 1;
            }
        }
    }
}
