using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unittest
{
    public static class RedBlackTreeTestExtensions
    {
        public static int MaxDepth<K, V>(this RedBlackTree<K, V> tree) where K : IComparable<K>
        {
            return MaxDepth(tree._root);
        }

        public static bool AreNodesOrdered<K, V>(this RedBlackTree<K, V> tree) where K : IComparable<K>
        {
            return AreNodesOrdered(tree._root);
        }

        private static bool AreNodesOrdered<K, V>(RedBlackTree<K, V>.Node node) where K : IComparable<K>
        {
            if (node == null)
            {
                return true;
            }

            if (node.Left != null && node.Left.Key.CompareTo(node.Key) > 0)
            {
                return false;
            }
            if (node.Right != null && node.Right.Key.CompareTo(node.Key) < 0)
            {
                return false;
            }

            return true;
        }

        private static int MaxDepth<K, V>(RedBlackTree<K, V>.Node node) where K : IComparable<K>
        {
            return node == null ? 0 : 1 + Math.Max(MaxDepth(node.Left), MaxDepth(node.Right));
        }
    }
}
