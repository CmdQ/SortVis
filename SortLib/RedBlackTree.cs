#if !TREE23 && !TREE234
#warning Define either TREE23 or TREE234.
#define TREE23
#elif TREE23 && TREE234
#error You cannot define them both.
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SortLib
{
    /// <summary>
    /// A left-leaning variant of a red-black tree. Especially wrt. deletion it's
    /// supposed to be easier to implement.
    /// </summary>
    public abstract partial class RedBlackTree<T> : ICollection<T>
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
        /// This is a cached flattened variant of the tree. Set to <c>null</c> whenever you modify the tree!
        /// </summary>
        private LinkedList<T> _cache;

        /// <summary>
        /// Initializes an empty instance of the <see cref="RedBlackTree{T}"/> class.
        /// </summary>
        /// <param name="comparer">An optional comparer to use for the the order of the elements.</param>
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
            AddRange(elements);
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
        /// Gets a value indicating whether the tree is empty.
        /// </summary>
        public bool Empty
        {
            get
            {
                return _root == null;
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
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the elements in the tree as a flat list.
        /// </summary>
        /// <remarks>
        /// To clear/reset the cache, e.g. before/after a modifying operation, assign <c>null</c>.
        /// </remarks>
        protected LinkedList<T> FlatList
        {
            get
            {
                return _cache = _cache ?? GetEnumerator(_root);
            }
            set
            {
                if (value != null)
                {
                    throw new ArgumentException("To reset the cache, assign null.");
                }
                _cache = null;
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
            AddItem(item);
        }

        /// <summary>
        /// Adds all elements in <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <remarks>Duplicate items are ignored.</remarks>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Clears all data from the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            FlatList = null;
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
            if (!Contains(item))
            {
                return false;
            }

            if (!IsRed(_root.Left) && !IsRed(_root.Right))
            {
                _root.Color = Node.RED;
            }

            _root = Remove(_root, item);

            if (_root != null)
            {
                _root.Color = Node.BLACK;
            }

            FlatList = null;

            return true;
        }

        /// <summary>
        /// Removes a range of elements from the tree.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (T item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return FlatList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of the tree to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied
        /// from the tree. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            FlatList.CopyTo(array, arrayIndex);
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

        /// <summary>
        /// Finds the successor node of a given node in the tree.
        /// </summary>
        /// <param name="node">The node whose successor we shall find.</param>
        /// <param name="root">An optional point in the tree to start from; if <c>null</c>
        /// then we start from the root.</param>
        /// <returns>The found successor node or <c>null</c> if a successor doesn't exist.</returns>
        protected Node Successor(Node node, Node root = null)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Left != null)
            {
                return Max(node.Left);
            }

            root = root ?? _root;

            Node succ = null;
            while (root != null)
            {
                int cmp = _comparer.Compare(node.Item, root.Item);
                if (cmp < 0)
                {
                    succ = root;
                    root = root.Left;
                }
                else if (cmp > 0)
                {
                    root = root.Right;
                }
                else
                {
                    break;
                }
            }
            return succ;
        }

        /// <summary>
        /// Inserts the specified <paramref name="item"/> into the tree.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <returns><c>true</c> if the item was inserted, i.e. the tree was changed; <c>false</c>
        /// if it was already present.</returns>
        protected bool AddItem(T item)
        {
            bool inserted;
            (_root = Insert(_root, item, out inserted)).Color = Node.BLACK;
            FlatList = null;
            return inserted;
        }

        /// <summary>
        /// Constructs a <see cref="LinkedList{T}"/> with all elements of a particular sub-tree.
        /// </summary>
        /// <param name="node">The node to start with.</param>
        /// <param name="acc">An accumulator argument. Don't supply anything other than <c>null</c>!</param>
        /// <returns>A list containing all elements under and including <paramref name="node"/>.</returns>
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

        private Node Remove(Node node, T item)
        {
            Debug.Assert(Contains(item), "When item is not in the tree, running this will break it.");

            if (_comparer.Compare(item, node.Item) < 0)
            {
                if (TwiceLeftColor(node) == Node.BLACK)
                {
                    node = MoveRedLeft(node);
                }
                node.Left = Remove(node.Left, item);
            }
            else
            {
                if (IsRed(node.Left))
                {
                    node = RotateRight(node);
                }
                if (_comparer.Compare(item, node.Item) == 0 && node.Right == null)
                {
                    return null;
                }
                else if (!IsRed(node.Right) && !IsRed(node.Right.Left))
                {
                    node = MoveRedRight(node);
                }
                if (_comparer.Compare(item, node.Item) == 0)
                {
                    node.Item = Min(node.Right).Item;
                    node.Right = RemoveMin(node.Right);
                }
                else
                {
                    node.Right = Remove(node.Right, item);
                }
            }

            return FixUp(node);
        }

        /// <summary>
        /// Checks if both the left child and its left child have the same color.
        /// </summary>
        /// <param name="node">The node that is both parent and grand parent.</param>
        /// <returns><c>null</c> if the colors differ, otherwise a boolean color
        /// (<see cref="Node.RED"/> and <see cref="Node.BLACK"/>).</returns>
        private bool? TwiceLeftColor(Node node)
        {
            bool first = IsRed(node.Left);
            if (node.Left == null)
            {
                Debug.Assert(!first);
                return null;
            }
            if (first != IsRed(node.Left.Left))
            {
                return null;
            }
            return first;
        }

        private Node FixUp(Node node)
        {
            if (IsRed(node.Right))
            {
#if TREE234
                if (IsRed(node.Right.Left))
                {
                    node.Right = RotateRight(node.Right);
                }
#endif
                node = RotateLeft(node);
            }

            if (TwiceLeftColor(node) == Node.RED)
            {
                node = RotateRight(node);
            }

#if TREE23
            if (HasRedChildren(node))
            {
                ColorFlip(node);
            }
#endif

            return node;
        }

        private bool HasRedChildren(Node node)
        {
            return IsRed(node.Left) && IsRed(node.Right);
        }

        private Node Min(Node node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        private Node Max(Node node)
        {
            while (node.Right != null)
            {
                node = node.Right;
            }

            return node;
        }

        private Node RemoveMin(Node node)
        {
            if (node.Left == null)
            {
                return null;
            }

            if (TwiceLeftColor(node) == Node.BLACK)
            {
                node = MoveRedLeft(node);
            }

            node.Left = RemoveMin(node.Left);

            return FixUp(node);
        }

        private Node MoveRedLeft(Node node)
        {
            Debug.Assert(IsRed(node) && !IsRed(node.Left) && !IsRed(node.Left.Left));
            ColorFlip(node);
            if (IsRed(node.Right.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                ColorFlip(node);

#if TREE234
                if (IsRed(node.Right.Right))
                {
                    node.Right = RotateLeft(node.Right);
                }
#endif
            }

            return node;
        }

        private Node MoveRedRight(Node node)
        {
            Debug.Assert(IsRed(node) && !IsRed(node.Right) && !IsRed(node.Right.Left));
            ColorFlip(node);
            if (IsRed(node.Left.Left))
            {
                node = RotateRight(node);
                ColorFlip(node);
            }

            return node;
        }

        private int CountNodes(Node node)
        {
            return node == null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
        }

        /// <summary>
        /// Inserts an <paramref name="item"/> below a <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The part of the tree to insert in.</param>
        /// <param name="item">The item to insert.</param>
        /// <param name="inserted">if set to <c>true</c> a new node in the tree was created;
        /// it's <c>false</c> for an unchanged tree.</param>
        /// <returns>The node containing <paramref name="item"/>.</returns>
        protected Node Insert(Node node, T item, out bool inserted)
        {
            if (node == null)
            {
                inserted = true;
                return item;
            }

#if TREE234
            if (HasRedChildren(node))
            {
                ColorFlip(node);
            }
#endif

            int cmp = _comparer.Compare(item, node.Item);
            if (cmp < 0)
            {
                node.Left = Insert(node.Left, item, out inserted);
            }
            else if (cmp > 0)
            {
                node.Right = Insert(node.Right, item, out inserted);
            }
            else
            {
                inserted = false;
                node.Item = item;
            }

            if (IsRightLeaning(node))
            {
                node = RotateLeft(node);
            }
            if (TwiceLeftColor(node) == Node.RED)
            {
                node = RotateRight(node);
            }

#if TREE23
            if (HasRedChildren(node))
            {
                ColorFlip(node);
            }
#endif

            return node;
        }

        private static bool IsRightLeaning(Node node)
        {
            return !IsRed(node.Left) && IsRed(node.Right);
        }

        private static bool IsRed(Node node)
        {
            return node != null && node.Color == Node.RED;
        }

        private Node RotateRight(Node node)
        {
            Debug.Assert(IsRed(node.Left));
            Node x = node.Left;
            node.Left = x.Right;
            x.Right = node;
            return FixColorAfterRotation(node, x);
        }

        private static Node FixColorAfterRotation(Node node, Node parent)
        {
            Debug.Assert(parent.Left == node || parent.Right == node, "These nodes are not directly related.");
            parent.Color = node.Color;
            node.Color = Node.RED;
            return parent;
        }

        private Node RotateLeft(Node node)
        {
            Debug.Assert(IsRed(node.Right));
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
