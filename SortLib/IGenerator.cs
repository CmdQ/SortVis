namespace SortLib
{
    /// <summary>
    /// Interface for a general number generator.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Gets or sets the name of the generator.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of elements to generate.
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// Gets the generated numbers.
        /// </summary>
        int[] Numbers { get; }
    }
}
