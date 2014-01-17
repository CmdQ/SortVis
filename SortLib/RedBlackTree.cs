using System;
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
    public class RedBlackTree<K, V> where K : IComparable<K>
    {
        private class Node
        {
            private readonly K _key;

            public bool Color { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public V Value { get; set; }

            public Node(K key, V value)
            {
                _key = key;
                Value = value;
                Color = RED;
                Left = Right = null;
            }

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

        private Node _root;

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
        public void Insert(K key, V value)
        {
            _root = Insert(_root, key, value);
            _root.Color = BLACK;
        }

        /// <summary>
        /// Counts the elements in this tree.
        /// </summary>
        /// <returns>The number of elements found.</returns>
        public int Count()
        {
            return Count(_root);
        }

        private int Count(Node node)
        {
            return node == null ? 0 : 1 + Count(node.Left) + Count(node.Right);
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
            Node left = h.Left;
            h.Left = left.Right;
            left.Right = h;
            return left;
        }

        private Node RotateLeft(Node h)
        {
            Node right = h.Right;
            h.Right = right.Left;
            right.Left = h;
            return right;
        }

        private void ColorFlip(Node h)
        {
            Debug.Assert(h.Left.Color == h.Right.Color && h.Left.Color == !h.Color);
            h.Left.Color = h.Right.Color = h.Color;
            h.Color = !h.Color;
        }
    }
}
