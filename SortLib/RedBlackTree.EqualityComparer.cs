using System.Collections.Generic;

namespace SortLib
{
    partial class RedBlackTree<T>
    {
        private class ComparerBasedEqualityComparer : IEqualityComparer<T>
        {
            private readonly IComparer<T> _comparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ComparerBasedEqualityComparer"/> class.
            /// </summary>
            /// <param name="comparer">The comparer to use.</param>
            public ComparerBasedEqualityComparer(IComparer<T> comparer)
            {
                _comparer = comparer;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type <typeparamref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <typeparamref name="T" /> to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <exception cref="System.NotImplementedException"></exception>
            public bool Equals(T x, T y)
            {
                return _comparer.Compare(x, y) == 0;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            /// <exception cref="System.NotImplementedException"></exception>
            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}