using System.Linq;
using System.Windows.Forms;

namespace SortVis
{
    /// <summary>
    /// Extension methods for <see cref="DataGridViewRow"/>.
    /// </summary>
    public static class DataGridViewRowExtensions
    {
        /// <summary>
        /// Extract a single image cell from a row.
        /// </summary>
        /// <param name="row">The row to extract from.</param>
        /// <returns>An image cell if there's exactly one, or throws.</returns>
        public static DataGridViewImageCell SingleImageCell(this DataGridViewRow row)
        {
            return row.Cells.OfType<DataGridViewImageCell>().Single();
        }
    }
}
