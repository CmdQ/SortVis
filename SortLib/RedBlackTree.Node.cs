﻿
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
        }
    }
}