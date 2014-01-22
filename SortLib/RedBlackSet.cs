﻿using System;
using System.Collections.Generic;

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
        public new bool Add(T item)
        {
            FlatList = null;
            bool inserted;
            _root = Insert(_root, item, out inserted);
            _root.Color = Node.BLACK;
            return inserted;
        }


        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <remarks>
        /// This method is an O(n) operation, where <c>n</c> is the number of elements
        /// in the other parameter.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <remarks>This method ignores any duplicate elements in other.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a proper subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the current set is a proper subset of other, other must have at least one
        /// element that the current set does not have.
        /// <para>
        /// An empty set is a proper subset of any other collection. Therefore, this method
        /// returns true if the current set is empty, unless the other parameter is also an empty set.
        /// </para>
        /// <para>
        /// This method always returns false if the current set has more or the same number
        /// of elements than other.</para>
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a proper superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If the current set is a proper superset of other, the current set must have at least
        /// one element that other does not have.
        /// </para>
        /// <para>
        /// An empty set is a proper superset of any other collection. Therefore, this method
        /// returns true if the collection represented by the other parameter is empty, unless the
        /// current set is also empty.
        /// </para>
        /// <para>
        /// This method always returns false if the number of elements in the current set is less
        /// than or equal to the number of elements in other.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a subset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If other contains the same elements as the current set, the current set is still considered
        /// a subset of other.
        /// </para>
        /// <para>
        /// This method always returns false if the current set has elements that are not in <paramref name="other"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set is a superset of <paramref name="other"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// If other contains the same elements as the current set, the current set is still considered
        /// a superset of other.
        /// </para>
        /// <para>
        /// This method always returns false if the current set has fewer elements than <paramref name="other"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns><c>true</c> if the current set and other share at least one common element;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>Any duplicate elements in other are ignored.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>true if the current set is equal to other;
        /// otherwise, false.</returns>
        /// <remarks>
        /// This method ignores the order of elements and any duplicate elements in other.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are
        /// present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <remarks>Any duplicate elements in other are ignored.</remarks>
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
        /// <remarks>Any duplicate elements in other are ignored.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>null</c>.</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            throw new NotImplementedException();
        }
    }
}
