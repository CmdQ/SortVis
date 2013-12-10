using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace SortLib
{
    /// <summary>
    /// Interface for a general number sorter.
    /// </summary>
    public interface ISorter : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a comparer to use for sorting.
        /// </summary>
        Comparer<int> Comparer { get; set; }

        /// <summary>
        /// Gets or sets a barrier used for stepped parallel execution.
        /// </summary>
        Barrier SteppedExecution { get; set; }

        /// <summary>
        /// Gets or sets a cancellation token to provide a possibility to abort sorting.
        /// </summary>
        CancellationToken Abort { get; set; }

        /// <summary>
        /// Gets or sets the worst-case runtime class.
        /// </summary>
        BigO BigO { get; set; }

        /// <summary>
        /// Sets the numbers that are to be sorted.
        /// </summary>
        int[] Numbers { set; }

        /// <summary>
        /// Gets or sets the name of the sorter.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the number of array writes that happened during sorting.
        /// </summary>
        int Writes { get; }

        /// <summary>
        /// Gets the number of number comparisons that happened during sorting.
        /// </summary>
        int Compares { get; }

        /// <summary>
        /// Gets the time sorting took in milliseconds.
        /// </summary>
        long Milliseconds { get; }

        /// <summary>
        /// Tells whether this sorter is stable.
        /// </summary>
        bool Stable { get; }

        /// <summary>
        /// Sorts a list of ints.
        /// </summary>
        void Sort();

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        void Reset();

        /// <summary>
        /// Draws an image of the current sort state of the numbers to be sorted.
        /// </summary>
        /// <param name="size">Describes the size of the image to render.</param>
        /// <returns>A bitmap that can be displayed.</returns>
        Bitmap Draw(Size size);
    }
}
