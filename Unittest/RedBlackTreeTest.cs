using NUnit.Framework;
using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unittest
{
    [TestFixture]
    public class RedBlackTreeTest
    {
        [TestCase]
        public void TestBalancedness()
        {
            var perfect = new RedBlackTree<int, bool>();
            perfect.Add(4, false);
            perfect.Add(2, false);
            perfect.Add(1, true);
            perfect.Add(3, true);
            perfect.Add(6, false);
            perfect.Add(5, true);
            perfect.Add(7, true);

            Assert.That(perfect.MaxDepth(), Is.EqualTo(3));
        }

        [TestCase]
        public void TestOrderedness()
        {
            WithRandomNumbers(list =>
                {
                    var rbt = new RedBlackTree<int, bool>(list, Enumerable.Repeat(true, list.Count));
                    Assert.That(rbt.AreNodesOrdered(), Is.True);
                });
        }

        [TestCase]
        public void TestCtors()
        {
            var empty = new RedBlackTree<char, string>();
            Assert.That(empty.Count(), Is.EqualTo(0));
            Assert.That(empty.ToList(), Is.Empty);

            WithRandomNumbers(list =>
                {
                    var range = Enumerable.Range(42, list.Count);
                    var two = new RedBlackTree<int, int>(list, range);
                    var prs = new RedBlackTree<int, int>(list.Zip<int, int, KeyValuePair<int, int>>(range, (a, b) => new KeyValuePair<int, int>(a, b)));
                    Assert.That(two.ToArray(), Is.EqualTo(prs.ToList()));
                });

            Assert.That(() =>
                {
                    new RedBlackTree<int, int>(Enumerable.Range(1, 3), Enumerable.Range(1, 4));
                }, Throws.ArgumentException);
        }

        [TestCase]
        public void TestDepth()
        {
            WithRandomNumbers(list =>
            {
                if (list.Count <= 1)
                {
                    return;
                }

                var rbt = new RedBlackTree<int, bool>(list.Zip(Enumerable.Repeat(false, list.Count), Tuple.Create));

                var depth = rbt.MaxDepth();
                var theory = Math.Log(list.Count, 2.0) * 2.0;
                Assert.That(depth, Is.LessThanOrEqualTo(theory),
                    "The tree is not well balanced.");
            }, new int[] { 255, 256, 257, 9999, 99999 });
        }

        [TestCase]
        public void TestDepthOfLinearInsertion()
        {
            const int up2 = 9999;

            var rbt = new RedBlackTree<int, bool>(Enumerable.Range(1, 7).Zip(Enumerable.Repeat(true, 7), Tuple.Create));

            double max = 0.0;
            for (int i = 8; i <= up2; ++i)
            {
                rbt.Add(i, false);
                double actual = rbt.MaxDepth();
                double perfect = Math.Log(i, 2.0);
                double ratio = actual / perfect;
                max = Math.Max(max, ratio);
            }

            Assert.That(rbt.Count, Is.EqualTo(up2));
            Assert.That(max, Is.LessThanOrEqualTo(2.0));
        }

        [TestCase]
        public void TestInsertAndContains()
        {
            WithRandomNumbers(list =>
            {
                var rbt = list.Aggregate(new RedBlackTree<int, bool>(), (t, n) =>
                {
                    t.Add(n, false);
                    return t;
                });

                Assert.That(list.Count, Is.EqualTo(rbt.Count));

                var flatSet = list.ToList();
                var flatTree = rbt.Select(e => e.Key).ToList();

                Assert.That(flatSet, Is.EqualTo(flatTree));
            });
        }

        private void WithRandomNumbers(Action<SortedSet<int>> test, IEnumerable<int> additional = null)
        {
            List<int> lengths = Enumerable.Range(0, 10).ToList();
            lengths.AddRange(new int[] { 63, 64, 65, 66, 67 });
            if (additional != null)
            {
                lengths.AddRange(additional);
            }

            var rand = new Random();

            foreach (var len in lengths)
            {
                var nums = new SortedSet<int>();
                while (nums.Count < len)
                {
                    nums.Add(rand.Next());
                }

                test(nums);
            }
        }
    }
}
