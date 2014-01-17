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
        public void TestDepth()
        {
            WithRandomNumbers(list =>
            {
                if (list.Count <= 1)
                {
                    return;
                }

                var rbt = new RedBlackTree<int, bool>(list.Zip(Enumerable.Repeat(false, list.Count), (a, b) => Tuple.Create(a, b)));

                var depth = rbt.MaxDepth();
                Assert.That(depth, Is.LessThanOrEqualTo(Math.Log(list.Count, 2.0) * 2.0));
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

                var flatSet = list.ToList();
                var flatTree = rbt.Select(e => e.Key).ToList();

                Assert.That(flatSet, Is.EqualTo(flatTree));
            });
        }

        private void WithRandomNumbers(Action<SortedSet<int>> test)
        {
            List<int> lengths = Enumerable.Range(0, 10).ToList();
            lengths.AddRange(new int[] { 64, 65, 66, 67 });

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
