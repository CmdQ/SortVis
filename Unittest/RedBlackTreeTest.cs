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
        public void TestDoubleInsertionMap()
        {
            var map = new RedBlackMap<char, short>
            {
                { 'a', 1 },
                { 'b', 2 },
                { 'c', 3 },
                { 'd', 5 },
            };

            Assert.That(map['a'], Is.EqualTo(1));
            Assert.That(map['b'], Is.EqualTo(2));
            Assert.That(map['c'], Is.EqualTo(3));
            Assert.That(map['d'], Is.EqualTo(5));

            var before = map.Count;
            map['d'] = 4;
            Assert.That(map.Count, Is.EqualTo(before));
            Assert.That(map['d'], Is.EqualTo(4));
        }

        [TestCase]
        public void TestDoubleInsertionSet()
        {
            var set = new RedBlackSet<char>
            {
                'a',
                'b',
                'c',
                'd',
            };
            Assert.That(set.Empty, Is.False);
            Assert.That(set.Count, Is.EqualTo(4));
            Assert.That(set.Contains('e'), Is.False);
            set.Add('e');
            Assert.That(set.Contains('e'));
            Assert.That(set.Count, Is.EqualTo(5));
            set.Add('e');
            Assert.That(set.Contains('e'));
            Assert.That(set.Count, Is.EqualTo(5));
        }

        [TestCase]
        public void TestSetClearing()
        {
            var set = new RedBlackSet<char>
            {
                'a',
                'b',
                'c',
                'd',
            };
            Assert.That(set.Count, Is.GreaterThan(0));
            set.Clear();
            Assert.That(set.Empty, Is.True);
            set.Add('z');
            Assert.That(set.Count, Is.EqualTo(1));
        }

        [TestCase]
        public void TestSetRemove()
        {
            var set = new RedBlackSet<char>
            {
                'a',
                'b',
                'c',
                'd',
                'e',
                'f',
                'g',
                'h',
            };
            Assert.That(set.Count, Is.EqualTo(8));
            Assert.That(set.Contains('f'), Is.True);
            set.Remove('f');
            Assert.That(set.Count, Is.EqualTo(7));
            Assert.That(set.Contains('f'), Is.False);
        }

        [TestCase]
        public void TestMapRemove()
        {
            var map = new RedBlackMap<char, ushort>
            {
                { 'a', 1 },
                { 'b', 2 },
                { 'c', 3 },
                { 'd', 4 },
                { 'e', 5 },
                { 'f', 6 },
                { 'g', 7 },
                { 'h', 8 },
            };

            for (ushort i = 1; i <= 8; ++i)
            {
                Assert.That(map.ContainsValue(i), Is.True);
            }

            Assert.That(map.Count, Is.EqualTo(map['h']));
            Assert.That(map.ContainsKey('f'), Is.True);
            Assert.That(map.ContainsKey('h'), Is.True);
            Assert.That(map.Remove('f'), Is.True);
            Assert.That(map.Remove('h'), Is.True);
            Assert.That(map.Remove('h'), Is.False);
            Assert.That(map.Count, Is.EqualTo(6));
            Assert.That(map.ContainsKey('f'), Is.False);
            Assert.That(map.ContainsKey('h'), Is.False);

            Assert.That(map.ContainsValue(6), Is.False);
            Assert.That(map.ContainsValue(8), Is.False);

            Assert.That(map.Keys, Is.EqualTo(new char[] { 'a', 'b', 'c', 'd', 'e', 'g' }));
            Assert.That(map.Values, Is.EqualTo(new ushort[] { 1, 2, 3, 4, 5, 7 }));
        }

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
            Assert.That(empty.Empty, Is.True);
            Assert.That(empty.Count(), Is.EqualTo(0));
            Assert.That(empty.ToList(), Is.Empty);
            empty.Add('1');
            Assert.That(empty.Empty, Is.False);
            Assert.That(empty.Count(), Is.EqualTo(1));

            WithRandomNumbers(list =>
                {
                    var range = Enumerable.Range(42, list.Count);
                    var two = new RedBlackMapTester<int, int>(list, range);
                    var prs = new RedBlackMapTester<int, int>(list.Zip<int, int, KeyValuePair<int, int>>(range, (a, b) => new KeyValuePair<int, int>(a, b)));
                    var tup = new RedBlackMapTester<int, int>(list.Zip<int, int, Tuple<int, int>>(range, Tuple.Create));
                    Assert.That(two.ToArray(), Is.EqualTo(prs.ToList()));
                    Assert.That(two.ToArray(), Is.EqualTo(tup.ToList()));
                });

            Assert.That(() =>
                {
                    new RedBlackMapTester<int, int>(Enumerable.Range(1, 3), Enumerable.Range(1, 4));
                }, Throws.ArgumentException);
        }

        [TestCase]
        public void TestDepthWithRandomNumbers()
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
