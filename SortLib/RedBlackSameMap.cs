using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortLib
{
    internal class RedBlackSameMap<T> : RedBlackMap<T, T>
        where T : IComparable<T>, new()
    {
        internal KeyValuePair<T, T>? PredecessorOrNode(T x)
        {
            Node n = _root;
            Node parent = null;
            var xp = KeyValuePair.Create(x);

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

        internal KeyValuePair<T, T>? Successor(T x)
        {
            var found = Successor(new Node(KeyValuePair.Create(x)));
            if (found == null)
            {
                return null;
            }
            return found.Item;
        }

        internal void RemoveOverlapped(T lo, T hi)
        {
            var completelyOverlapped = (
                from kpv in FlatList
                where kpv.Key.CompareTo(lo) >= 0 && kpv.Value.CompareTo(hi) <= 0
                select kpv).ToArray();


            if (completelyOverlapped.Length == Count)
            {
                Clear();
            }
            else
            {
                RemoveRange(completelyOverlapped);
            }
        }
    }
}
