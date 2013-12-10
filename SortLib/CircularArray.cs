using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SortLib
{
    /// <summary>
    /// A circular array of fixed size.
    /// </summary>
    /// <typeparam name="T">The type of the contained elements.</typeparam>
    public class CircularArray<T> : IEnumerable<T>
    {
        private readonly T[] _elms;
        private readonly int _size;
        private int _base;
        private int _count;

        /// <summary>
        /// Initializes an empty container.
        /// </summary>
        /// <param name="size">The maximum number of elements to be stored.</param>
        public CircularArray(int size)
        {
            _elms = new T[size];
            _size = size;
            Clear();
        }

        /// <summary>
        /// Gets the number of elements.
        /// </summary>
        int Count
        {
            get
            {
                return Math.Min(_base, _size);
            }
        }

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> at the specified index.
        /// </summary>
        /// <param name="index">The index to access.</param>
        [SuppressMessage("Microsoft.Design",
            "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public T this[int index]
        {
            get
            {
                if (index >= _base)
                {
                    throw new IndexOutOfRangeException();
                }
                return _elms[index % _base];
            }
            set
            {
                if (index >= _base)
                {
                    throw new IndexOutOfRangeException();
                }
                _elms[index % _base] = value;
            }
        }

        /// <summary>
        /// Add <paramref name="elm"/> to the array, possibly overwriting older entries.
        /// </summary>
        /// <param name="elm">The value to add.</param>
        public void Add(T elm)
        {
            _elms[_base++ % _size] = elm;
            ++_count;
        }

        /// <summary>
        /// Adds the specified <paramref name="elms"/> to the array, possibly overwriting older entries.
        /// </summary>
        /// <param name="elms">The values to add.</param>
        public void Add(params T[] elms)
        {
            foreach (var elm in elms)
            {
                Add(elm);
            }
        }

        /// <summary>
        /// Clears this the contents.
        /// </summary>
        public void Clear()
        {
            _count = _base = 0;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" />
        /// that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            int count = Math.Min(_count, _size);
            for (int i = _base; i < _base + count; ++i)
            {
                yield return _elms[i % _size];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" />
        /// object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
