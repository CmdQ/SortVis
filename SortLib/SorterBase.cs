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

        private int[] _numbers;
        private CircularArray<int> _swapped;
        private bool? _stable;
        private int _min;
        private int _max;
        private string _name;
        private int _writes;
        private int _compares;
        private long _milliseconds;
        private bool _finished;


        /// <summary>
        /// Construct a sorter with default comparer.
        /// </summary>
        public SorterBase()
        {
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
        /// Sorts a list of <see cref="int"/>s.
        /// </summary>
        public void Sort()
        {
            Reset();

            _finished = false;
            var sw = Stopwatch.StartNew();
            SortIt();
            sw.Stop();
            Milliseconds = sw.ElapsedMilliseconds;

            if (SteppedExecution != null)
            {
                SteppedExecution.RemoveParticipant();
            }
            _swapped.Clear();

            CheckSortedness();

            _finished = !Abort.IsCancellationRequested;
        }

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        public void Reset()
        {
            _swapped = new CircularArray<int>(2);
            Writes = Compares = 0;
            Milliseconds = 0L;
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
        /// <param name="finished">Set <c>true</c> when finished.</param>
        /// <param name="min">A precomputed minimum, use <c>null</c> to be computed.</param>
        /// <param name="max">A precomputed maximum, use <c>null</c> to be computed.</param>
        /// <returns>A bitmap that can be displayed.</returns>
        public static Bitmap Draw(int[] numbers, Size size, bool finished = false, float? min = null, float? max = null)
        {
            // I want to use these colors.
            var background = Color.White;

            var blockBrush = new SolidBrush(finished ? _finishedGreen : _grayBlock);

            var bm = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(bm))
            {
                g.Clear(background);

                min = min ?? numbers.Min();
                max = max ?? numbers.Max();
                float range = max.Value - min.Value;

                float width = (float)size.Width / numbers.Length;

                for (int i = 0; i < numbers.Length; ++i)
                {
                    var num = numbers[i];

                    g.FillRectangle(blockBrush,
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
            var blockBrush = new SolidBrush(Color.SeaGreen);
            var bm = Draw(Numbers, size, _finished, _min, _max);
            bm.RotateFlip(RotateFlipType.RotateNoneFlipY);
            using (var g = Graphics.FromImage(bm))
            {
                float range = _max - _min;

                float width = (float)size.Width / Numbers.Length;

                for (int i = 0; i < Numbers.Length; ++i)
                {
                    if (!_swapped.Contains(i))
                    {
                        continue;
                    }
                    var num = Numbers[i];
                    g.FillRectangle(blockBrush,
                    i * width, 0,
                    width, range > 0.0f ? (num - _min) / range * size.Height : size.Height / 2);
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
                try
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
                catch (OperationCanceledException)
                {
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
