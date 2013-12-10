namespace SortLib
{
    /// <summary>
    /// Holds metadata about sorters to use with MEF.
    /// </summary>
    public interface ISorterMetadata
    {
        /// <summary>
        /// Gets the name of a sorter.
        /// </summary>
        string Name { get; }
    }
}
