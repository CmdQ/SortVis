using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortLib
{
    internal class RedBlackSameMap<T> : RedBlackMap<T, T>
        where T : IComparable<T>, new()
    {
        public KeyValuePair<T, T>? PredecessorOrNode(T x)
        {
            Node n = _root;
            Node parent = null;
            var xp = new KeyValuePair<T, T>(x, new T());

            while (n != null)
            {
                int cmp = Comparer.Compare(xp, n.Item);
                if (cmp < 0)
                {
                    n = n.Left;
                }
                else if (cmp > 0)
                {
                    parent = n;
                    n = n.Right;
                }
                else
                {
                    return n.Item;
                }
            }
            if (parent == null)
            {
                return null;
            }
            return parent.Item;
        }

        public KeyValuePair<T, T>? Successor(T x)
        {
            var found = Successor(new Node(new KeyValuePair<T, T>(x, new T())));
            if (found == null)
            {
                return null;
            }
            return found.Item;
        }
    }
}
