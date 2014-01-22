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
        private readonly IComparer<T> _comparer;

        /// <summary>
        /// The root node of the tree.
        /// </summary>
        protected Node _root = null;

        /// <summary>
        /// Initializes an empty instance of the <see cref="RedBlackTree{T}"/> class.
        /// </summary>
        public RedBlackTree(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? Comparer<T>.Default;
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
                return _comparer;
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
        /// Determines whether the specified <paramref name="item"/> is contained in the tree.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><c>true</c> if it is, <c>false</c> otherwise.</returns>
        public bool Contains(T item)
        {
            return FindNode(item) != null;
        }

        /// <summary>
        /// Inserts the specified <paramref name="item"/> into the tree.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        public void Add(T item)
        {
            _root = Insert(_root, item);
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
        /// Removes an item from the tree.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is found and successfully removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(T item)
        {
            bool found = false;
            var newRoot = Remove(_root, item, ref found);
            if (found)
            {
                (_root = newRoot).Color = Node.BLACK;
            }
            return found;
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
        /// <param name="item">The item to find.</param>
        /// <returns>The found node or <c>null</c> if not present.</returns>
        protected Node FindNode(T item, Node node = null)
        {
            node = node ?? _root;
            while (node != null)
            {
                int cmp = _comparer.Compare(item, node.Item);
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

        private Node Remove(Node node, T item, ref bool found)
        {
            if (_comparer.Compare(item, node.Item) < 0)
            {
                if (!IsRed(node.Left) && !IsRed(node.Left.Left))
                {
                    node = MoveRedLeft(node);
                }
                node.Left = Remove(node.Left, item, ref found);
            }
            else
            {
                if (IsRed(node.Left))
                {
                    node = RotateRight(node);
                }
                if (_comparer.Compare(item, node.Item) == 0 && node.Right == null)
                {
                    found = true;
                    return node.Right;
                }
                if (node.Right == null)
                {
                    return node;
                }
                else if (!IsRed(node.Right) && !IsRed(node.Right.Left))
                {
                    node = MoveRedRight(node);
                }
                if (_comparer.Compare(item, node.Item) == 0)
                {
                    found = true;
                    node.Item = Min(node.Right).Item;
                    node.Right = RemoveMin(node.Right);
                }
                else
                {
                    node.Right = Remove(node.Right, item, ref found);
                }
            }

            return FixUp(node);
        }

        private Node FixUp(Node node)
        {
            if (IsRed(node.Right))
            {
                node = RotateLeft(node);
            }

            if (node.Left != null && IsRed(node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                ColorFlip(node);
            }

            return node;
        }

        private Node Min(Node node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        private Node RemoveMin(Node node)
        {
            if (node.Left == null)
            {
                return null;
            }

            if (!IsRed(node.Left) && !IsRed(node.Left.Left))
            {
                node = MoveRedLeft(node);
            }

            node.Left = RemoveMin(node.Left);

            return FixUp(node);
        }

        private Node MoveRedLeft(Node node)
        {
            ColorFlip(node);
            if (IsRed(node.Right.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                ColorFlip(node);
            }

            return node;
        }

        private Node MoveRedRight(Node node)
        {
            ColorFlip(node);
            if (IsRed(node.Left.Left))
            {
                node = RotateRight(node);
                ColorFlip(node);
            }

            return node;
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
                if (_comparer.Compare(node.Item, acc.Value) < 0)
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

        private Node Insert(Node node, T item)
        {
            if (node == null)
            {
                return new Node(item);
            }

            if (IsRed(node.Left) && IsRed(node.Right))
            {
                ColorFlip(node);
            }

            int cmp = _comparer.Compare(item, node.Item);
            if (cmp < 0)
            {
                node.Left = Insert(node.Left, item);
            }
            else if (cmp > 0)
            {
                node.Right = Insert(node.Right, item);
            }
            else
            {
                node.Item = item;
            }

            if (IsRed(node.Right) && !IsRed(node.Left))
            {
                node = RotateLeft(node);
            }
            if (IsRed(node.Left) && IsRed(node.Left.Left))
            {
                node = RotateRight(node);
            }

            return node;
        }

        private static bool IsRed(Node node)
        {
            return node != null && node.Color == Node.RED;
        }

        private Node RotateRight(Node node)
        {
            Node x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            return FixColorAfterRotation(node, x);
        }

        private static Node FixColorAfterRotation(Node node, Node parent)
        {
            parent.Color = node.Color;
            node.Color = Node.RED;
            return parent;
        }

        private Node RotateLeft(Node node)
        {
            Node x = node.Right;
            node.Right = x.Left;
            x.Left = node;
            return FixColorAfterRotation(node, x);
        }

        private void ColorFlip(Node node)
        {
            Debug.Assert(node.Left.Color == node.Right.Color && node.Left.Color == !node.Color);
            node.Left.Color = node.Right.Color = node.Color;
            node.Color = !node.Color;
        }
    }
}
