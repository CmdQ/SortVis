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
            private V _value;
            public Node _left;
            public Node _right;
            public bool _color;

            public Node(K key, V value)
            {
                _key = key;
                _value = value;
                _color = RED;
            }

            public K Key
            {
                get
                {
                    return _key;
                }
            }

            public V Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }

        private const bool RED = true;
        private const bool BLACK = false;

        private Node _root;

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
                    n = n._left;
                }
                else
                {
                    n = n._right;
                }
            }
            return false;
        }

        public void Insert(K key, V value)
        {
            _root = Insert(_root, key, value);
            _root._color = BLACK;
        }

        private Node Insert(Node h, K key, V value)
        {
            if (h == null)
            {
                return new Node(key, value);
            }

            if (IsRed(h._left) && IsRed(h._right))
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
                h._left = Insert(h._left, key, value);
            }
            else
            {
                h._right = Insert(h._right, key, value);
            }

            if (IsRed(h._right) && !IsRed(h._left))
            {
                h = RotateLeft(h);
            }
            if (IsRed(h._left) && IsRed(h._left._left))
            {
                h = RotateRight(h);
            }

            return h;
        }

        private static bool IsRed(Node h)
        {
            return h != null && h._color == RED;
        }

        private Node RotateRight(Node h)
        {
            Node left = h._left;
            h._left = left._right;
            left._right = h;
            return left;
        }

        private Node RotateLeft(Node h)
        {
            Node right = h._right;
            h._right = right._left;
            right._left = h;
            return right;
        }

        private void ColorFlip(Node h)
        {
            Debug.Assert(h._left._color == h._right._color && h._left._color == !h._color);
            h._left._color = h._right._color = h._color;
            h._color = !h._color;
        }
    }
}
