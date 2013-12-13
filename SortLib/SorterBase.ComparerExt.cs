using System.Collections.Generic;

namespace SortLib
{
    /// <summary>
    /// Extension methods for <see cref="Comparer{T}"/>
    /// </summary>
    public static class ComparerExt
    {
        private class InvertedComparer<T> : Comparer<T>
        {
            private Comparer<T> _comparer;

            public InvertedComparer(Comparer<T> comp)
            {
                this._comparer = comp;
            }

            public override int Compare(T x, T y)
            {
                return _comparer.Compare(y, x);
            }
        }

        /// <summary>
        /// Create an inverted <see cref="Comparer{T}"/> that sorts backwards.
        /// </summary>
        /// <typeparam name="T">The type of the elements to compare.</typeparam>
        /// <param name="comparer">A comparer that should be inverted.</param>
        /// <returns>A new comparer that behaves like a negative <paramref name="comparer"/>.</returns>
        public static Comparer<T> Invert<T>(this Comparer<T> comparer)
        {
            return new InvertedComparer<T>(comparer);
        }
    }
}
