using System;
using System.Collections.Generic;

namespace SortLib
{
    /// <summary>
    /// A set in form of a balanced binary tree.
    /// </summary>
    /// <typeparam name="T">The type of the values which have to be <see cref="IComparable{K}"/></typeparam>
    public class RedBlackSet<T> : RedBlackTree<T> where T : IComparable<T>
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
    }
}
