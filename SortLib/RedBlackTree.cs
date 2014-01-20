using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SortLib
{
    /// <summary>
    /// A left-leaning variant of a red-black tree. Especially wrt. deletion it's
    /// supposed to be easier to implement.
    /// </summary>
    public abstract partial class RedBlackTree<T> : IEnumerable<T>
    {
        /// <summary>
        /// A comparer to use for our keys.
        /// </summary>
        private readonly IComparer<T> _comp;

        /// <summary>
        /// The root node of the tree.
        /// </summary>
        protected Node _root = null;

        /// <summary>
        /// Initializes an empty instance of the <see cref="RedBlackTree{T}"/> class.
        /// </summary>
        public RedBlackTree(IComparer<T> comparer = null)
        {
            _comp = comparer ?? Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{T}"/> class with elements to be added.
        /// </summary>
        /// <param name="elements">The elements to add.</param>
        /// <param name="comparer">An optional comparer to use for the elements.</param>
        public RedBlackTree(IEnumerable<T> elements, IComparer<T> comparer = null)
            : this(comparer)
        {
            foreach (var elm in elements)
            {
                Add(elm);
            }
        }

        /// <summary>
        /// Gets the <see cref="IComparer{T}"/> for elements in the tree.
        /// </summary>
        public IComparer<T> Comparer
        {
            get
            {
                return _comp;
            }
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
        /// Determines whether the specified <paramref name="key"/> is contained in the tree.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns><c>true</c> if it is, <c>false</c> otherwise.</returns>
        public bool Contains(T key)
        {
            return FindNode(key) != null;
        }

        /// <summary>
        /// Inserts the specified <paramref name="key"/> into the tree.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        public void Add(T key)
        {
            _root = Insert(_root, key);
            _root.Color = Node.BLACK;
        }

        /// <summary>
        /// Clears all data from the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerator(_root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Tries to find a node in the tree.
        /// </summary>
        /// <param name="node">The starting node.</param>
        /// <param name="key">The key to find.</param>
        /// <returns>The found node or <c>null</c> if not present.</returns>
        protected Node FindNode(T key, Node node = null)
        {
            node = node ?? _root;
            while (node != null)
            {
                int cmp = _comp.Compare(key, node.Item);
                if (cmp == 0)
                {
                    return node;
                }
                if (cmp < 0)
                {
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
            }
            return null;
        }

        private LinkedList<T> GetEnumerator(Node node, LinkedListNode<T> acc = null)
        {
            // First iteration.
            if (acc == null)
            {
                var ll = new LinkedList<T>();
                if (node == null)
                {
                    return ll;
                }
                var onlyNode = ll.AddFirst(node.Item);
                if (node.Left != null)
                {
                    GetEnumerator(node.Left, onlyNode);
                }
                if (node.Right != null)
                {
                    GetEnumerator(node.Right, onlyNode);
                }
                return ll;
            }
            else
            {
                if (node == null)
                {
                    return acc.List;
                }
                if (_comp.Compare(node.Item, acc.Value) < 0)
                {
                    var newNode = acc.List.AddBefore(acc, node.Item);
                    GetEnumerator(node.Left, newNode);
                    GetEnumerator(node.Right, newNode);
                }
                else
                {
                    var newNode = acc.List.AddAfter(acc, node.Item);
                    GetEnumerator(node.Left, newNode);
                    GetEnumerator(node.Right, newNode);
                }
            }

            return acc.List;
        }

        private int CountNodes(Node node)
        {
            return node == null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
        }

        private Node Insert(Node h, T key)
        {
            if (h == null)
            {
                return new Node(key);
            }

            if (IsRed(h.Left) && IsRed(h.Right))
            {
                ColorFlip(h);
            }

            int cmp = _comp.Compare(key, h.Item);
            if (cmp < 0)
            {
                h.Left = Insert(h.Left, key);
            }
            else if (cmp > 0)
            {
                h.Right = Insert(h.Right, key);
            }
            else
            {
                h.Item = key;
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
            return h != null && h.Color == Node.RED;
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
            h.Color = Node.RED;
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
