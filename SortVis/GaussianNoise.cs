using SortLib;
using System;
using System.ComponentModel.Composition;

namespace SortVis
{
    /// <summary>
    /// Generates numbers of a Gaussian distribution.
    /// </summary>
    [Export(typeof(IGenerator))]
    [ExportMetadata("Name", "Gaussian distribution")]
    public class GaussianNoise : RandomGenerator
    {
        /// <summary>
        /// Generate numbers.
        /// </summary>
        protected override void Generate()
        {
            var rand = new Random(Seed);

            double mean = Max / 2.0;
            double stddev = mean / 4.0;

            for (int i = Count - 1; i >= 1; --i)
            {
                double a = rand.NextDouble();
                double b = rand.NextDouble();

                double sqrtCos = Math.Sqrt(-2.0 * Math.Log(a)) * Math.Cos(2.0 * Math.PI * b);
                double sqrtSin = Math.Sqrt(-2.0 * Math.Log(a)) * Math.Sin(2.0 * Math.PI * b);

                Numbers[i] = (int)Math.Round(mean + stddev * sqrtCos, MidpointRounding.AwayFromZero);
                Numbers[i - 1] = (int)Math.Round(mean + stddev * sqrtSin, MidpointRounding.AwayFromZero);
            }

            double c = rand.NextDouble();
            double d = rand.NextDouble();

            double sqrt = Math.Sqrt(-2.0 * Math.Log(c)) * Math.Cos(2.0 * Math.PI * d);
            Numbers[0] = (int)Math.Round(mean + stddev * sqrt, MidpointRounding.AwayFromZero);
        }
    }
}
