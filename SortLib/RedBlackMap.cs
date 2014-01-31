using System;
using System.Collections.Generic;
using System.Linq;

namespace SortLib
{
    /// <summary>
    /// A balanced binary tree that maps keys (of type <typeparamref name="TKey"/>) to values
    /// (of type <typeparamref name="TValue"/>).
    /// </summary>
    /// <typeparam name="TKey">The type of the keys which have to be <see cref="IComparable{K}"/></typeparam>
    /// <typeparam name="TValue">Values to associate with keys.</typeparam>
    public class RedBlackMap<TKey, TValue> : RedBlackTree<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
        where TValue : new()
    {
        /// <summary>
        /// Provides convenience creation method for <see cref="KeyValuePair{TKey, TValue}"/>s.
        /// </summary>
        public static class KeyValuePair
        {
            /// <summary>
            /// Create a <see cref="KeyValuePair{K, V}"/> with a <paramref name="key"/> and a <paramref name="value"/>.
            /// </summary>
            /// <typeparam name="K">Type of the key.</typeparam>
            /// <typeparam name="V">Type of the value.</typeparam>
            /// <param name="key">The key to use.</param>
            /// <param name="value">The value to associate with it.</param>
            /// <returns>A new <see cref="KeyValuePair{K, V}"/>.</returns>
            public static KeyValuePair<K, V> Create<K, V>(K key, V value)
            {
                return new KeyValuePair<K, V>(key, value);
            }

            /// <summary>
            /// Create a <see cref="KeyValuePair{K, V}"/> with a <paramref name="key"/> and a
            /// default value.
            /// </summary>
            /// <typeparam name="K">Type of the key.</typeparam>
            /// <param name="key">The key to use.</param>
            /// <returns>A new <see cref="KeyValuePair{K, V}"/>.</returns>
            public static KeyValuePair<K, TValue> Create<K>(K key)
            {
                return new KeyValuePair<K, TValue>(key, new TValue());
            }
        }

        private class KeyValueComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            private static readonly KeyValueComparer _singleton = new KeyValueComparer();
            private static readonly IComparer<TKey> _comp = Comparer<TKey>.Default;

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
            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
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
        /// Initializes a new instance of the <see cref="RedBlackMap{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IComparer<KeyValuePair<TKey, TValue>> comp = null)
            : base(comp ?? KeyValueComparer.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{TKey, TValue}"/> class with elements to be added.
        /// </summary>
        /// <param name="pairs">Pairs of key and value to add.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<KeyValuePair<TKey, TValue>> pairs, IComparer<KeyValuePair<TKey, TValue>> comp = null)
            : this(comp)
        {
            AddRange(pairs);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{TKey, TValue}"/> class with elements to be added.
        /// </summary>
        /// <param name="keys">The keys to use.</param>
        /// <param name="values">The values to associate with.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<TKey> keys, IEnumerable<TValue> values, IComparer<KeyValuePair<TKey, TValue>> comp = null)
            : this(keys.Zip(values, KeyValuePair.Create), comp)
        {
            if (keys.Count() != values.Count())
            {
                throw new ArgumentException("The two sequences don't have the same length.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackMap{TKey, TValue}"/> class with elements to be added.
        /// </summary>
        /// <param name="tuples">The keys and values to store with.</param>
        /// <param name="comp">An optional comparer to use.</param>
        public RedBlackMap(IEnumerable<Tuple<TKey, TValue>> tuples, IComparer<KeyValuePair<TKey, TValue>> comp = null)
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
        public TValue this[TKey key]
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
        public void Add(TKey key, TValue value)
        {
            Add(KeyValuePair.Create(key, value));
        }

        /// <summary>
        /// Determines whether the map contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return base.Contains(KeyValuePair.Create(key));
        }

        /// <summary>
        /// Determines whether the map contains a mapping with the specified mapping target.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
        public bool ContainsValue(TValue value)
        {
            return FlatList.Any(kv => kv.Value.Equals(value));
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
        public bool Remove(TKey key)
        {
            return base.Remove(KeyValuePair.Create(key));
        }


        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys
        /// of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object
        /// that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get
            {
                return FlatList.Select(kv => kv.Key).ToArray();
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
        public bool TryGetValue(TKey key, out TValue value)
        {
            var node = FindNode(key);
            if (node != null)
            {
                value = node.Item.Value;
                return true;
            }
            value = new TValue();
            return false;
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values
        /// in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object
        /// that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values
        {
            get
            {
                return FlatList.Select(kv => kv.Value).ToArray();
            }
        }

        private Node FindNode(TKey key, Node node = null)
        {
            return FindNode(KeyValuePair.Create(key), node);
        }
    }
}
