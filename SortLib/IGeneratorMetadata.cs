namespace SortLib
{
    /// <summary>
    /// Holds metadata about generators to use with MEF.
    /// </summary>
    public interface IGeneratorMetadata
    {
        /// <summary>
        /// Gets the name of a sorter.
        /// </summary>
        string Name { get; }
    }
}
