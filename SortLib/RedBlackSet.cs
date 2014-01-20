using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
