using SortLib;
using System;

namespace SortVis
{
    /// <summary>
    /// Partially provides functionality from <see cref="ISorter"/> to use by inheriting.
    /// </summary>
    public abstract partial class SorterBase : ISorter
    {
        /// <summary>
        /// Thrown when a sorter does not do its work correctly.
        /// </summary>
        [Serializable]
        public class NotSortedException : Exception
        {
            private const string _message = "The result of {0} is not correctly sorted.";

            /// <summary>
            /// Default construct with defaul message.
            /// </summary>
            /// <param name="sorterName">Name of the sorter that failed.</param>
            public NotSortedException(string sorterName)
                : base(string.Format(_message, sorterName))
            {
            }

            /// <summary>
            /// Construct with position indication.
            /// </summary>
            /// <param name="sorterName">Name of the sorter that failed.</param>
            /// <param name="at">The 0-based index that holds the first violation of sorting.</param>
            public NotSortedException(string sorterName, int at)
                : base(string.Format(string.Concat(_message, " The first violation is at index {1}."), sorterName, at))
            {
            }
        }

        /// <summary>
        /// Gets or sets the worst-case runtime class.
        /// </summary>
        public BigO BigO
        {
            get;
            set;
        }

        /// <summary>
        /// Sets whether to run a sorter or not.
        /// </summary>
        public bool Run
        {
            get;
            set;
        }
    }
}