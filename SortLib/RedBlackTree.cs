using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortLib
{
    /// <summary>
    /// A left-leaning variant of a red-black tree. Especially wrt. deletion it's
    /// supposed to be easier to implement.
    /// </summary>
    public class RedBlackTree<K, V> : IEnumerable<KeyValuePair<K, V>> where K : IComparable<K>
    {
        internal class Node
        {
            private readonly K _key;

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="key">The fixed key which we associate a <paramref name="value"/> with.</param>
            /// <param name="value">The value for that <paramref name="key"/>.</param>
            public Node(K key, V value)
            {
                _key = key;
                Value = value;
                Color = RED;
                Left = Right = null;
            }

            /// <summary>
            /// Gets or sets the color of this node.
            /// </summary>
            /// <value>
            ///   <c>true</c> if red; <c>false</c> for black.
            /// </value>
            /// <seealso cref="RED"/>
            /// <seealso cref="BLACK"/>
            public bool Color { get; set; }

            /// <summary>
            /// Gets or sets the left child.
            /// </summary>
            public Node Left { get; set; }

            /// <summary>
            /// Gets or sets the right child.
            /// </summary>
            public Node Right { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public V Value { get; set; }

            /// <summary>
            /// Gets the key.
            /// </summary>
            public K Key
            {
                get
                {
                    return _key;
                }
            }
        }

        private const bool RED = true;
        private const bool BLACK = false;

        internal Node _root;

        /// <summary>
        /// Initializes an empty instance of the <see cref="RedBlackTree{K, V}"/> class.
        /// </summary>
        public RedBlackTree()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{K, V}"/> class with contents.
        /// </summary>
        /// <param name="keys">The keys to insert.</param>
        /// <param name="values">According values to insert.</param>
        public RedBlackTree(IEnumerable<K> keys, IEnumerable<V> values)
            : this(keys.Zip(values, (k, v) => Tuple.Create(k, v)))
        {
            if (keys.Count() != values.Count())
            {
                throw new ArgumentException("The sequences are not equal in length.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{K, V}"/> class with contents.
        /// </summary>
        /// <param name="pairs">The key-value-pairs to insert.</param>
        public RedBlackTree(IEnumerable<KeyValuePair<K, V>> pairs)
            : this(pairs.Select(p => Tuple.Create(p.Key, p.Value)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{K, V}"/> class with contents.
        /// </summary>
        /// <param name="pairs">The key-value-pairs to insert.</param>
        public RedBlackTree(IEnumerable<Tuple<K, V>> pairs)
        {
            foreach (var pair in pairs)
            {
                Add(pair.Item1, pair.Item2);
            }
        }

        /// <summary>
        /// Determines whether the specified <paramref name="key"/> is contained in the tree.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns><c>true</c> if it is, <c>false</c> otherwise.</returns>
        public bool Contains(K key)
        {
            Node n = _root;
            while (n != null)
            {
                int cmp = key.CompareTo(n.Key);
                if (cmp == 0)
                {
                    return true;
                }
                if (cmp < 0)
                {
                    n = n.Left;
                }
                else
                {
                    n = n.Right;
                }
            }
            return false;
        }

        /// <summary>
        /// Inserts the specified <paramref name="key"/> into the tree.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        /// <param name="value">The value associated with it.</param>
        public void Add(K key, V value)
        {
            _root = Insert(_root, key, value);
            _root.Color = BLACK;
        }

        /// <summary>
        /// Gets the number of elements in this tree.
        /// </summary>
        public int Count
        {
            get
            {
                return CountNodes(_root);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return GetEnumerator(_root, new LinkedList<KeyValuePair<K, V>>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private LinkedList<KeyValuePair<K, V>> GetEnumerator(Node node, LinkedList<KeyValuePair<K, V>> acc, int side = 0)
        {
            if (node == null)
            {
                return acc;
            }

            if (side < 0)
            {
                acc.AddFirst(new KeyValuePair<K, V>(node.Key, node.Value));
            }
            else
            {
                acc.AddLast(new KeyValuePair<K, V>(node.Key, node.Value));
            }

            if (node.Left != null)
            {
                GetEnumerator(node.Left, acc, -1);
            }
            if (node.Right != null)
            {
                GetEnumerator(node.Right, acc, 1);
            }

            return acc;
        }

        private int CountNodes(Node node)
        {
            return node == null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
        }

        private Node Insert(Node h, K key, V value)
        {
            if (h == null)
            {
                return new Node(key, value);
            }

            if (IsRed(h.Left) && IsRed(h.Right))
            {
                ColorFlip(h);
            }

            int cmp = key.CompareTo(h.Key);
            if (cmp == 0)
            {
                h.Value = value;
            }
            else if (cmp < 0)
            {
                h.Left = Insert(h.Left, key, value);
            }
            else
            {
                h.Right = Insert(h.Right, key, value);
            }

            if (IsRed(h.Right) && !IsRed(h.Left))
            {
                h = RotateLeft(h);
            }
            if (IsRed(h.Left) && IsRed(h.Left.Left))
            {
                h = RotateRight(h);
            }

            return h;
        }

        private static bool IsRed(Node h)
        {
            return h != null && h.Color == RED;
        }

        private Node RotateRight(Node h)
        {
            Node x = h.Left;
            h.Left = x.Right;
            x.Right = h;
            return FixColorAfterRotation(h, x);
        }

        private static Node FixColorAfterRotation(Node h, Node x)
        {
            x.Color = h.Color;
            h.Color = RED;
            return x;
        }

        private Node RotateLeft(Node h)
        {
            Node x = h.Right;
            h.Right = x.Left;
            x.Left = h;
            return FixColorAfterRotation(h, x);
        }

        private void ColorFlip(Node h)
        {
            Debug.Assert(h.Left.Color == h.Right.Color && h.Left.Color == !h.Color);
            h.Left.Color = h.Right.Color = h.Color;
            h.Color = !h.Color;
        }
    }
}
