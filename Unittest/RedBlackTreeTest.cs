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
            var perfect = new RedBlackSetTester<int>();
            perfect.Add(4);
            perfect.Add(2);
            perfect.Add(1);
            perfect.Add(3);
            perfect.Add(6);
            perfect.Add(5);
            perfect.Add(7);

            Assert.That(perfect.MaxDepth(), Is.EqualTo(3));
        }

        [TestCase]
        public void TestOrderedness()
        {
            WithRandomNumbers(list =>
                {
                    var rbt = new RedBlackSetTester<int>(list);
                    Assert.That(rbt.AreNodesOrdered(), Is.True);
                });
        }

        [TestCase]
        public void TestCtors()
        {
            var empty = new RedBlackSetTester<char>();
            Assert.That(empty.Count(), Is.EqualTo(0));
            Assert.That(empty.ToList(), Is.Empty);

            WithRandomNumbers(list =>
                {
                    var range = Enumerable.Range(42, list.Count);
                    var two = new RedBlackMapTester<int, int>(list, range);
                    var prs = new RedBlackMapTester<int, int>(list.Zip<int, int, KeyValuePair<int, int>>(range, (a, b) => new KeyValuePair<int, int>(a, b)));
                    Assert.That(two.ToArray(), Is.EqualTo(prs.ToList()));
                });

            Assert.That(() =>
                {
                    new RedBlackMapTester<int, int>(Enumerable.Range(1, 3), Enumerable.Range(1, 4));
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

                var rbt = new RedBlackSetTester<int>(list);

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

            var rbt = new RedBlackSetTester<int>(Enumerable.Range(1, 7));

            double max = 0.0;
            for (int i = 8; i <= up2; ++i)
            {
                rbt.Add(i);
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
                var rbt = list.Aggregate(new RedBlackSetTester<int>(), (t, n) =>
                {
                    t.Add(n);
                    return t;
                });

                Assert.That(list.Count, Is.EqualTo(rbt.Count));

                var flatSet = list.ToList();
                var flatTree = rbt.Select(e => e).ToList();

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
