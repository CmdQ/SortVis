
namespace SortLib
{
    partial class RedBlackTree<T>
    {
        /// <summary>
        /// A class to store values in a tree.
        /// </summary>
        protected class Node
        {
            /// <summary>
            /// Boolean value for a red node.
            /// </summary>
            public const bool RED = true;

            /// <summary>
            /// Boolean value for a black node.
            /// </summary>
            public const bool BLACK = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.
            /// </summary>
            /// <param name="item">The item to store.</param>
            public Node(T item)
            {
                Item = item;
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
            /// Gets or sets the item stored in this node.
            /// </summary>
            public T Item { get; set; }

            /// <summary>
            /// Implicit conversion of the type <typeparamref name="T"/> to a Node.
            /// </summary>
            /// <param name="item">The item to convert.</param>
            /// <returns>A new node with <paramref name="item"/> in <see cref="Item"/>.</returns>
            public static implicit operator Node(T item)
            {
                return new Node(item);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                var left = DescribeChild(Left, "left");
                var right = DescribeChild(Right, "right");
                return string.Format("{{{0} {1} Node '{2}' with {3} and {4}}}",
                    Left == null && Right == null ? "empty" : (Left != null ? "LL" : "wrong"),
                    DescribeColor(this),
                    Item.ToString(),
                    left,
                    right);
            }

            private static string DescribeChild(Node node, string dir)
            {
                return node == null
                    ? string.Concat("NO ", dir)
                    : string.Format("{{{0} {1} '{2}'}}",
                        DescribeColor(node),
                        dir,
                        node.Item.ToString());
            }

            private static string DescribeColor(Node node)
            {
                return node == null || node.Color == BLACK ? "black" : "red";
            }
        }
    }
}