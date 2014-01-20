using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unittest
{
    internal class RedBlackTreeTestExtensions<T> : RedBlackTree<T>
    {
        public int MaxDepth()
        {
            return MaxDepth(_root);
        }

        public bool AreNodesOrdered()
        {
            return AreNodesOrdered(_root);
        }

        private bool AreNodesOrdered(RedBlackTree<T>.Node node)
        {
            if (node == null)
            {
                return true;
            }

            if (node.Left != null && Comparer.Compare(node.Left.Key, node.Key) > 0)
            {
                return false;
            }
            if (node.Right != null && Comparer.Compare(node.Right.Key, node.Key) < 0)
            {
                return false;
            }

            return true;
        }

        private static int MaxDepth(RedBlackTree<T>.Node node)
        {
            return node == null ? 0 : 1 + Math.Max(MaxDepth(node.Left), MaxDepth(node.Right));
        }
    }
}
