using NUnit.Framework;
using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unittest
{
    [TestFixture]
    public class IntervalTest
    {
        [TestCase]
        public void AddingWithoutOverlap()
        {
            var a = new Intervals<int>();
            a.Add(0, 5);
            Assert.That(a.Contains(-1), Is.False);
            Assert.That(a.Contains(0));
            Assert.That(a.Contains(1));
            Assert.That(a.Contains(2));
            Assert.That(a.Contains(3));
            Assert.That(a.Contains(4));
            Assert.That(a.Contains(5), Is.False);
            a.Add(6, 10);
            Assert.That(a.Contains(6));
            Assert.That(a.Contains(7));
            Assert.That(a.Contains(8));
            Assert.That(a.Contains(9));
            Assert.That(a.Contains(10), Is.False);
            a.Add(-2, -1);
            Assert.That(a.Contains(-3), Is.False);
            Assert.That(a.Contains(2));
            Assert.That(a.Contains(-1), Is.False);
        }

        [TestCase]
        public void PerfectFill()
        {
            var a = new Intervals<float>();
            a.Add(0, 5);
            Assert.That(a.Contains(-float.Epsilon), Is.False);
            Assert.That(a.Contains(0));
            Assert.That(a.Contains(4.999f));
            Assert.That(a.Contains(5), Is.False);
            a.Add(6, 10);
            Assert.That(a.Contains(6));
            Assert.That(a.Contains(9.999f));
            Assert.That(a.Contains(10), Is.False);

            Assert.That(a.Count(), Is.EqualTo(2));
            a.Add(5, 6);
            Assert.That(a.Count(), Is.EqualTo(1));
        }

        [TestCase]
        public void PredecessorOfRightChild()
        {
            var a = MakeTree();
            Assert.That(a.Contains(0.5 * (25 + 29)));
            Assert.That(a.Contains(0.5 * 4));
        }

        [TestCase]
        public void PredecessorOfLeftChild()
        {
            var a = MakeTree();
            Assert.That(a.Contains(0.5 * (-100 - 50)));
            Assert.That(a.Contains(-101), Is.False);
        }

        [TestCase]
        public void PredecessorOfRoot()
        {
            var a = MakeTree();
            Assert.That(a.Contains(11.9));
            Assert.That(a.Contains(12), Is.False);
            Assert.That(a.Contains(24.9), Is.False);
            Assert.That(a.Contains(25));
            Assert.That(a.Contains(28.9));
            Assert.That(a.Contains(29), Is.False);
        }

        [TestCase]
        public void TestMergingAdds()
        {
            var a = MakeTree();
            a.Add(0, 4.9);
            Assert.That(a.Count(), Is.EqualTo(5));
            a.Add(0, 6);
            Assert.That(a.Count(), Is.EqualTo(5));
            a.Add(41, 43);
            a.Add(48.5, 50);
            Assert.That(a.Contains(-double.Epsilon), Is.False);
            Assert.That(a.Contains(0));
            Assert.That(a.Contains(5.99999));
            Assert.That(a.Contains(40.99999), Is.False);
            Assert.That(a.Contains(41));
            Assert.That(a.Contains(42));
            Assert.That(a.Contains(43));
            Assert.That(a.Contains(44));
            Assert.That(a.Contains(48));
            Assert.That(a.Contains(49));
            Assert.That(a.Contains(49.99999));
            Assert.That(a.Contains(50), Is.False);
            a.Add(-200, 200);
            Assert.That(a.Count(), Is.EqualTo(1));
            Assert.That(a.Contains(-200.00001), Is.False);
            Assert.That(a.Contains(-200));
            Assert.That(a.Contains(199.99999));
            Assert.That(a.Contains(200), Is.False);
        }

        /// <summary>
        /// This makes the following tree.
        /// <para><code>
        ///                25/29
        ///             /         \
        ///          0/5          42/49
        ///       /       \
        /// -100/-50     10/12
        /// </code></para>
        /// </summary>
        /// <returns></returns>
        private static Intervals<double> MakeTree()
        {
            var a = new Intervals<double>();
            a.Add(0, 5);
            a.Add(-100, -50);
            a.Add(25, 29);
            a.Add(10, 12);
            a.Add(42, 49);
            Assert.That(a.Count(), Is.EqualTo(5));
            return a;
        }
    }
}
