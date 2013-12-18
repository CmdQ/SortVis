using SortLib;
using System;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Random number generator.
    /// </summary>
    [Export(typeof(IGenerator))]
    [ExportMetadata("Name", "Random")]
    public class RandomGenerator : GeneratorBase
    {
        private int _max;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomGenerator"/> class.
        /// </summary>
        protected RandomGenerator()
        {
            _max = (1 << 14) - 1;
        }

        /// <summary>
        /// Gets or sets the exclusive maximum that all generated numbers must obey.
        /// </summary>
        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (value <= 0 || value == _max)
                {
                    return;
                }
                _max = value;
                Generate();
            }
        }

        /// <summary>
        /// Gets or sets a seed value for the random numbers.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Generate numbers.
        /// </summary>
        protected override void Generate()
        {
            var rand = new Random(Seed);

            for (int i = Count - 1; i >= 0; --i)
            {
                Numbers[i] = rand.Next(0, Max);
            }
        }
    }
}
