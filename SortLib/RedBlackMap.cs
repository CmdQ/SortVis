using System;
using System.Collections.Generic;
using System.Linq;

namespace SortLib
{
    /// <summary>
    /// A balanced binary tree that maps keys (of type <typeparamref name="K"/>) to values
    /// (of type <typeparamref name="V"/>).
    /// </summary>
    /// <typeparam name="K">The type of the keys which have to be <see cref="IComparable{K}"/></typeparam>
    /// <typeparam name="V">Values to associate with keys.</typeparam>
    public class RedBlackMap<K, V> : RedBlackTree<KeyValuePair<K, V>>, IDictionary<K, V>
        where K : IComparable<K>
        where V : new()
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
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
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
                throw new ArgumentException("The two sequences don't have the same length.");
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
        /// Gets or sets the value for a specified key.
        /// </summary>
        /// <param name="key">The key to use for lookup.</param>
        /// <value>The value to save for <paramref name="key"/>.</value>
        /// <returns>The value associated with <paramref name="key"/>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The map does not contain an entry
        /// for that key.</exception>
        public V this[K key]
        {
            get
            {
                var kvp = FindNode(key);
                if (kvp == null)
                {
                    throw new KeyNotFoundException("The map does not contain an entry for that key.");
                }
                return kvp.Item.Value;
            }
            set
            {
                Add(key, value);
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

        /// <summary>
        /// Determines whether the map contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(K key)
        {
            return base.Contains(new KeyValuePair<K, V>(key, new V()));
        }

        /// <summary>
        /// Determines whether the map contains a mapping with the specified mapping target.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(V value)
        {
            return ConstructList(_root).Any(kv => kv.Value.Equals(value));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns
        /// <c>false</c> if <paramref name="key"/> was not found in the
        /// original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public bool Remove(K key)
        {
            return base.Remove(new KeyValuePair<K, V>(key, new V()));
        }


        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys
        /// of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object
        /// that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<K> Keys
        {
            get
            {
                return ConstructList(_root).Select(kv => kv.Key).ToArray();
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.
        /// This parameter is passed uninitialized.</param>
        /// <returns>
        /// <c>true</c> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains
        /// an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(K key, out V value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values
        /// in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object
        /// that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<V> Values
        {
            get
            {
                return ConstructList(_root).Select(kv => kv.Value).ToArray();
            }
        }


        /// <summary>
        /// Copies the elements of the map to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied
        /// from the map. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            var list = ConstructList(_root);
            foreach (var elm in list)
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private Node FindNode(K key, Node node = null)
        {
            return FindNode(new KeyValuePair<K, V>(key, new V()), node);
        }
    }
}
