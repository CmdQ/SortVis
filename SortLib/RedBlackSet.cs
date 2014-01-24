using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace SortLib
{
    /// <summary>
    /// A set in form of a balanced binary tree.
    /// </summary>
    /// <typeparam name="T">The type of the values which have to be <see cref="IComparable{K}"/></typeparam>
    public class RedBlackSet<T> : RedBlackTree<T>, ISet<T> where T : IComparable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">An optional comparer for the values.</param>
        public RedBlackSet(IComparer<T> comparer = null)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackSet{T}"/> class with elements to be added.
        /// </summary>
        /// <param name="elements">The elements to add.</param>
        /// <param name="comparer">An optional comparer to use for the elements.</param>
        public RedBlackSet(IEnumerable<T> elements, IComparer<T> comparer = null)
            : base(elements, comparer)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the set is empty.
        /// </summary>
        public bool Empty
        {
            get
            {
                return _root == null;
            }
        }

        /// <summary>
        /// Inserts the specified <paramref name="item"/> into the tree.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <returns><c>true</c> if the item was inserted, i.e. the tree was changed; <c>false</c>
        /// if it was already present.</returns>
        public new bool Add(T item)
        {
            return AddItem(item);
        }


        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <remarks>
        /// This method is an O(n) operation, where <c>n</c> is the number of elements
        /// in the <paramref name="other"/> parameter.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            foreach (T elm in other)
            {
                Remove(elm);
            }
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <remarks>This method ignores any duplicate elements in <paramref name="other"/>.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            var otherSet = new RedBlackSet<T>(other);

            var copy = new T[Count];
            CopyTo(copy, 0);

            foreach (var item in copy)
            {
                if (!otherSet.Contains(item))
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the current set is a proper subset of <paramref name="other"/>, <paramref name="other"/> must have
        /// at least one element that the current set does not have.
        /// <para>
        /// An empty set is a proper subset of any other collection. Therefore, this method
        /// returns <c>true</c> if the current set is empty, unless the
        /// <paramref name="other"/> parameter is also an empty set.
        /// </para>
        /// <para>
        /// This method always returns <c>false</c> if the current set has more or the same number
        /// of elements than <paramref name="other"/>.</para>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            
            // Early exit.
            if (Empty)
            {
                var e = other.GetEnumerator();
                return e.MoveNext();
            }

            return IsSomeSubsetOf(new RedBlackSet<T>(other), 1);
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a proper superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the current set is a proper superset of <paramref name="other"/>, the current set must have at least
        /// one element that <paramref name="other"/> does not have.
        /// </para>
        /// <para>
        /// An empty set is a proper superset of any other collection. Therefore, this method
        /// returns <c>true</c> if the collection represented by the <paramref name="other"/> parameter is empty,
        /// unless the current set is also empty.
        /// </para>
        /// <para>
        /// This method always returns <c>false</c> if the number of elements in the current set is less
        /// than or equal to the number of elements in <paramref name="other"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            // Early exit.
            var e = other.GetEnumerator();
            if (!e.MoveNext())
            {
                return !Empty;
            }

            return (new RedBlackSet<T>(other)).IsSomeSubsetOf(this, 1);
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="other"/> contains the same elements as the current set,
        /// the current set is still considered a subset of <paramref name="other"/>.
        /// </para>
        /// <para>
        /// This method always returns <c>false</c> if the current set has elements that are not
        /// in <paramref name="other"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return IsSomeSubsetOf(new RedBlackSet<T>(other), 0);
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="other"/> contains the same elements as the current set,
        /// the current set is still considered a superset of <paramref name="other"/>.
        /// </para>
        /// <para>
        /// This method always returns <c>false</c> if the current set has fewer elements than
        /// <paramref name="other"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            var otherSet = new RedBlackSet<T>(other);

            return otherSet.IsSomeSubsetOf(this, 0);
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set and <paramref name="other"/> share at least one common element;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>Any duplicate elements in <paramref name="other"/> are ignored.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            // TODO: Use Distinct()?
            return other.Any(e => Contains(e));
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>true if the current set is equal to <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method ignores the order of elements and any duplicate elements in <paramref name="other"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            var otherSet = new RedBlackSet<T>(other);

            return FlatList.SequenceEqual(otherSet.FlatList, EqualityComparer);
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are
        /// present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <remarks>Any duplicate elements in <paramref name="other"/> are ignored.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that
        /// are present in either the current set or the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <remarks>Any duplicate elements in <paramref name="other"/> are ignored.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// This checks if this set is a subset of <paramref name="other"/>. It works
        /// for both regular and proper subsets, depending on the value of <paramref name="minDifference"/>.
        /// </summary>
        /// <param name="other">The set to compare to the current set.</param>
        /// <param name="minDifference">A minimum difference that has to be observed in the number of elements
        /// in the set.</param>
        /// <returns><c>true</c> if the set is a (proper) subset of <paramref name="other"/>.</returns>
        private bool IsSomeSubsetOf(RedBlackSet<T> other, int minDifference)
        {
            Debug.Assert(minDifference == 0 || minDifference == 1, "Other values don't make sense.");
            if (Count + minDifference > other.Count)
            {
                return false;
            }

            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
