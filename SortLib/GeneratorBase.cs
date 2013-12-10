using SortLib;

namespace SortVis
{
    /// <summary>
    /// Abstract base for a general number generator.
    /// </summary>
    public abstract class GeneratorBase : IGenerator
    {
        private int _count;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected GeneratorBase()
        {
            _count = 0;
            Numbers = new int[0];
        }

        /// <summary>
        /// Gets or sets the name of the generator.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of elements to generate.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value >= 0)
                {
                    _count = value;
                    Numbers = new int[_count];
                    Generate();
                }
            }
        }

        /// <summary>
        /// Gets the generated numbers.
        /// </summary>
        public int[] Numbers
        {
            get;
            private set;
        }

        /// <summary>
        /// Fill <see cref="Numbers"/> with generated numbers.
        /// </summary>
        protected abstract void Generate();
    }
}
