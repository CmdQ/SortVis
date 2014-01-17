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
            var sequence = new RedBlackTree<int, bool>(Enumerable.Range(1, 7), Enumerable.Repeat(true, 7));

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

                var rbt = new RedBlackTree<int, bool>(list.Zip(Enumerable.Repeat(false, list.Count),
                    (a, b) => Tuple.Create(a, b)));

                var depth = rbt.MaxDepth();
                var theory = Math.Log(list.Count, 2.0) * 2.0;
                Assert.That(depth, Is.LessThanOrEqualTo(theory),
                    "The tree is not well balanced.");
            });
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

        private void WithRandomNumbers(Action<SortedSet<int>> test)
        {
            List<int> lengths = Enumerable.Range(0, 10).ToList();
            lengths.AddRange(new int[] { 63, 64, 65, 66, 67 });

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
