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
        public void TestInsertAndContains()
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

                var rbt = new RedBlackTree<int, bool>();
                foreach (var num in nums)
                {
                    rbt.Insert(num, true);
                }

                Assert.That(rbt.Count() == len);

                foreach (var num in nums)
                {
                    Assert.That(rbt.Contains(num));
                }
            }
        }
    }
}
