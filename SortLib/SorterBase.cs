using SortLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SortVis
{
    /// <summary>
    /// Abstract base for a general number sorter.
    /// </summary>
    public abstract partial class SorterBase : ISorter
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private static Color _finishedGreen = Color.LawnGreen;
        private static Color _grayBlock = Color.FromArgb(64, 64, 64);
        private static Color _swapColor = Color.OrangeRed;

        private int[] _numbers;
        private CircularArray<int> _swapped;
        private bool? _stable;
        private int _min;
        private int _max;
        private string _name;
        private int _writes;
        private int _compares;
        private long _milliseconds;
        private int _sortedFrom;
        private int _sortedTo;

        /// <summary>
        /// Construct a sorter with default comparer.
        /// </summary>
        public SorterBase()
        {
            ConsideredBig = 1;
            _stable = null;
            _numbers = null;
            Comparer = Comparer<int>.Default;
            Reset();
        }

        /// <summary>
        /// Sets the numbers that are to be sorted.
        /// </summary>
        public int[] Numbers
        {
            set
            {
                if (_numbers != value)
                {
                    if (value == null)
                    {
                        _numbers = null;
                    }
                    else
                    {
                        {
                            _numbers = new int[value.Length];
                            if (value.Length > 0)
                            {
                                Array.Copy(value, _numbers, value.Length);
                                _min = _numbers.Min();
                                _max = _numbers.Max();
                            }
                        }
                    }
                    OnPropertyChanged();
                }
            }
            protected get
            {
                return _numbers;
            }
        }

        /// <summary>
        /// Gets or sets the name of the sorter.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a comparer to use for sorting.
        /// </summary>
        public Comparer<int> Comparer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a barrier used for stepped parallel execution.
        /// </summary>
        public Barrier SteppedExecution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a cancellation token to provide a possibility to abort sorting.
        /// </summary>
        public CancellationToken Abort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of array writes that happened during sorting.
        /// </summary>
        public int Writes
        {
            get
            {
                return _writes;
            }
            protected set
            {
                if (value != _writes)
                {
                    _writes = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the number of number comparisons that happened during sorting.
        /// </summary>
        public int Compares
        {
            get
            {
                return _compares;
            }
            private set
            {
                if (value != _compares)
                {
                    _compares = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the time sorting took in milliseconds.
        /// </summary>
        public long Milliseconds
        {
            get
            {
                return _milliseconds;
            }
            private set
            {
                if (value != _milliseconds)
                {
                    _milliseconds = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Tells whether this sorter is stable.
        /// </summary>
        public bool Stable
        {
            get
            {
                _stable = _stable ?? CheckIfStable();
                return _stable.Value;
            }
        }

        /// <summary>
        /// Tells, which range of a sorter is already sorted.
        /// </summary>
        /// <see cref="SortedTo"/>
        protected int SortedFrom
        {
            get
            {
                return _sortedFrom;
            }
            set
            {
                if (value != _sortedFrom)
                {
                    _sortedFrom = value;
                    if (SortedTo < value)
                    {
                        SortedTo = value;
                    }
                }
            }
        }

        /// <summary>
        /// Tells, which range of a sorter is already sorted.
        /// </summary>
        /// <see cref="SortedFrom"/>
        protected int SortedTo
        {
            get
            {
                return _sortedTo;
            }
            set
            {
                if (value != _sortedTo)
                {
                    _sortedTo = value;
                    if (SortedFrom > value)
                    {
                        SortedFrom = value;
                    }
                }
            }
        }

        /// <summary>
        /// Lengths of this length are not considered small, i.e. the "real"
        /// algorithm works on them instead of a specialized sort for small arrays.
        /// </summary>
        protected int ConsideredBig
        {
            get;
            set;
        }

        /// <summary>
        /// Sorts a list of <see cref="int"/>s.
        /// </summary>
        public void Sort()
        {
            Reset();

            try
            {
                var sw = Stopwatch.StartNew();
                SortIt();
                sw.Stop();
                Milliseconds = sw.ElapsedMilliseconds;

                if (SteppedExecution != null)
                {
                    SteppedExecution.RemoveParticipant();
                    SteppedExecution = null;
                }

                CheckSortedness();

                // If we come here, the full range is correctly sorted.
                SortedFrom = 0;
                SortedTo = Numbers.Length;
            }
            catch (OperationCanceledException)
            {
            }

            _swapped.Clear();
        }

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        public void Reset()
        {
            _swapped = new CircularArray<int>(2);
            Writes = Compares = 0;
            Milliseconds = 0L;
            SortedTo = 0;
        }

        /// <summary>
        /// Compares two numbers in the array.
        /// </summary>
        /// <param name="i">Position of 1st number.</param>
        /// <param name="j">Position of 2nd number.</param>
        /// <returns>-1 if first number is smaller, 1 if larger or 0 if they're equal.</returns>
        protected int CompareInArray(int i, int j)
        {
            ++Compares;
            return Comparer.Compare(Numbers[i], Numbers[j]);
        }

        /// <summary>
        /// Compares two numbers.
        /// </summary>
        /// <param name="a">1st number.</param>
        /// <param name="b">2nd number.</param>
        /// <returns>-1 if first number is smaller, 1 if larger or 0 if they're equal.</returns>
        protected int CompareNum(int a, int b)
        {
            ++Compares;
            return Comparer.Compare(a, b);
        }

        /// <summary>
        /// Puts number in one slot to another.
        /// </summary>
        /// <param name="from">Source slot.</param>
        /// <param name="to">Destination slot.</param>
        /// <remarks>When implementing a sorter, use this to shift elements (e.g. as in insertion sort when
        /// making room for the element to be inserted).</remarks>
        /// <seealso cref="Write"/>
        /// <seealso cref="Swap"/>
        protected void Shift(int from, int to)
        {
            _swapped.Add(from, to);
            Numbers[to] = Numbers[from];
            ++Writes;
            WaitIfNecessary();
        }

        /// <summary>
        /// Puts a number into a slot.
        /// </summary>        
        /// <param name="value">Value to put.</param>
        /// <param name="pos">Slot to write to.</param>
        /// <remarks>When implementing a sorter, use this to write a value to a position (e.g. like the
        /// inserted element in insertion sort).</remarks>
        /// <seealso cref="Shift"/>
        /// <seealso cref="Swap"/>
        protected void Write(int value, int pos)
        {
            Numbers[pos] = value;
            _swapped.Add(pos);
            ++Writes;
            WaitIfNecessary();
        }

        /// <summary>
        /// Swaps the numbers of two slots.
        /// </summary>
        /// <param name="i">1st slot.</param>
        /// <param name="j">2nd slot.</param>
        /// <remarks>When implementing a sorter, use this to swap two values (e.g. as in bubble sort).</remarks>
        /// <seealso cref="Write"/>
        /// <seealso cref="Shift"/>
        protected void Swap(int i, int j)
        {
            if (i == j)
            {
                return;
            }

            int tmp = Numbers[i];
            Numbers[i] = Numbers[j];
            Numbers[j] = tmp;
            Writes += 2;
            _swapped.Add(i, j);
            WaitIfNecessary();
            WaitIfNecessary();
        }

        /// <summary>
        /// Draws an array of numbers as bar diagram into a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="numbers">The numbers to draw.</param>
        /// <param name="size">The size of the output bitmap.</param>
        /// <param name="sortedFrom">The index where the sorted part begins.</param>
        /// <param name="sortedTo">The index where the sorted part ends.</param>
        /// <param name="min">A precomputed minimum, use <c>null</c> to be computed.</param>
        /// <param name="max">A precomputed maximum, use <c>null</c> to be computed.</param>
        /// <returns>A bitmap that can be displayed.</returns>
        public static Bitmap Draw(int[] numbers, Size size, int sortedFrom = 0, int sortedTo = 0, float? min = null, float? max = null)
        {
            // I want to use these colors.
            var background = Color.White;

            var blockBrush = new SolidBrush(_grayBlock);
            var finishedBrush = new SolidBrush(_finishedGreen);

            var bm = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(bm))
            {
                g.Clear(background);

                min = min ?? numbers.Min();
                max = max ?? numbers.Max();
                float range = max.Value - min.Value;

                if (numbers == null || numbers.Length == 0)
                {
                    return bm;
                }

                float width = (float)size.Width / numbers.Length;

                for (int i = 0; i < numbers.Length; ++i)
                {
                    var num = numbers[i];

                    g.FillRectangle(i >= sortedFrom && i < sortedTo ? finishedBrush : blockBrush,
                    i * width, 0,
                    width, range > 0.0f ? (num - min.Value) / range * size.Height : size.Height / 2);
                }
            }
            bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bm;
        }

        /// <summary>
        /// Draws an image of the current sort state of the numbers to be sorted.
        /// </summary>
        /// <param name="size">Describes the size of the image to render.</param>
        /// <returns>
        /// A bitmap that can be displayed.
        /// </returns>
        public Bitmap Draw(Size size)
        {
            var swapBrush = new SolidBrush(_swapColor);
            var sortedBrush = new SolidBrush(_finishedGreen);

            var bm = Draw(Run ? Numbers : null, size, SortedFrom, SortedTo, _min, _max);
            bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
            using (var g = Graphics.FromImage(bm))
            {
                float range = _max - _min;

                if (Numbers == null || Numbers.Length == 0 || !Run)
                {
                    return bm;
                }

                float width = (float)size.Width / Numbers.Length;

                for (int i = 0; i < Numbers.Length; ++i)
                {
                    Brush brush;
                    if (_swapped.Contains(i))
                    {
                        brush = swapBrush;
                    }
                    else if (i >= SortedFrom && i < SortedTo)
                    {
                        brush = sortedBrush;
                    }
                    else
                    {
                        continue;
                    }
                    g.FillRectangle(brush,
                    i * width, 0,
                    width, range > 0.0f ? (Numbers[i] - _min) / range * size.Height : size.Height / 2);
                }
            }
            bm.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bm;
        }

        /// <summary>
        /// Causes the sorter to pause, if we run under observation.
        /// </summary>
        protected void WaitIfNecessary()
        {
            if (SteppedExecution != null)
            {
                {
                    if (Abort != null)
                    {
                        SteppedExecution.SignalAndWait(Abort);
                    }
                    else
                    {
                        SteppedExecution.SignalAndWait();
                    }
                }
            }
        }

        /// <summary>
        /// Do a check if all numbers are really sorted.
        /// </summary>
        /// <exception cref="NotSortedException">Indicates sorting failure.</exception>
        protected virtual void CheckSortedness()
        {
            if (!Abort.IsCancellationRequested)
            {
                for (int i = 1; i < Numbers.Length; ++i)
                {
                    if (Comparer.Compare(Numbers[i - 1], Numbers[i]) > 0)
                    {
                        throw new NotSortedException(Name, i);
                    }
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            if (prop == null)
            {
                throw new ArgumentNullException("prop", "Compiler should give us this.");
            }
            var handler = PropertyChanged;
            if (handler != null)
            {
                var ea = new PropertyChangedEventArgs(prop);
                PropertyChanged(this, ea);
            }
        }

        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected abstract void SortIt();
    }
}
