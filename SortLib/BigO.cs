
namespace SortLib
{
    /// <summary>
    /// Tells worst-case Big-O category of a <see cref="ISorter"/>.
    /// </summary>
    public enum BigO
    {
        /// <summary>
        /// Linear in size of array, i.e. <c>O(n)</c>.
        /// </summary>
        Linear,
        /// <summary>
        /// Squared in size of array, i.e. <c>O(n^2)</c>.
        /// </summary>
        Squared,
        /// <summary>
        /// Log-linear in size of array, i.e. <c>O(n * log(n))</c>.
        /// </summary>
        LogLinear,
    }
}
