using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortLib
{
    /// <summary>
    /// A balanced binary tree that maps keys (of type <typeparamref name="K"/>) to values
    /// (of type <typeparamref name="V"/>).
    /// </summary>
    /// <typeparam name="K">The type of the keys which have to be <see cref="IComparable{K}"/></typeparam>
    /// <typeparam name="V">Values to associate with keys.</typeparam>
    public class RedBlackMap<K, V> : RedBlackTree<KeyValuePair<K, V>> where K : IComparable<K>
    {
        private class KeyValueComparer : IComparer<KeyValuePair<K, V>>
        {
            private static readonly KeyValueComparer _singleton = new KeyValueComparer();
            private static readonly IComparer<K> _comp = Comparer<K>.Default;

            private KeyValueComparer()
            {
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public int Compare(KeyValuePair<K, V> x, KeyValuePair<K, V> y)
            {
                return _comp.Compare(x.Key, y.Key);
            }

            /// <summary>
            /// Gets the default comparer for key-value-pairs.
            /// </summary>
            public static KeyValueComparer Default
            {
                get
                {
                    return _singleton;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{K, V}"/> class.
        /// </summary>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IComparer<KeyValuePair<K, V>> comp = null)
            : base(comp ?? KeyValueComparer.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{K, V}"/> class with elements to be added.
        /// </summary>
        /// <param name="pairs">Pairs of key and value to add.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<KeyValuePair<K, V>> pairs, IComparer<KeyValuePair<K, V>> comp = null)
            : this(comp)
        {
            foreach (var pair in pairs)
            {
                Add(pair);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{K, V}"/> class with elements to be added.
        /// </summary>
        /// <param name="keys">The keys to use.</param>
        /// <param name="values">The values to associate with.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<K> keys, IEnumerable<V> values, IComparer<KeyValuePair<K, V>> comp = null)
            : this(keys.Zip(values, (k, v) => new KeyValuePair<K, V>(k, v)), comp)
        {
            if (keys.Count() != values.Count())
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{K, V}"/> class with elements to be added.
        /// </summary>
        /// <param name="tuples">The keys and values to store with.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<Tuple<K, V>> tuples, IComparer<KeyValuePair<K, V>> comp = null)
            : this(comp)
        {
            foreach (var tuple in tuples)
            {
                Add(tuple.Item1, tuple.Item2);
            }
        }

        /// <summary>
        /// Adds the specified key with a given value.
        /// </summary>
        /// <param name="key">The key for <paramref name="value"/>.</param>
        /// <param name="value">The value to store with <paramref name="key"/>.</param>
        public void Add(K key, V value)
        {
            Add(new KeyValuePair<K, V>(key, value));
        }
    }
}
