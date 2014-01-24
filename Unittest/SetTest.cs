using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unittest
{
    [TestFixture]
    public class SetTest
    {
        private readonly int[] _numbers = Enumerable.Range(1, 100).ToArray();
        private readonly int[] _odd;
        private readonly int[] _even;
        private readonly int[] _squares;
        private readonly int[] _primes;

        public SetTest()
        {
            _even = (
                from n in _numbers
                select (n - 1) * 2).Skip(1).ToArray();

            _odd = (
                from n in _numbers
                select (n - 1) * 2 + 1).ToArray();

            _squares = (
                from n in _numbers
                let s = n * n
                where UpTo100(s)
                select s).ToArray();

            _primes = (
                from n in _numbers
                where IsPrime(n)
                select n).ToArray();
        }

        [TestCase]
        public void TestExceptWith()
        {
            var a = new RedBlackSetTester<int>(_numbers);
            Assert.That(() => a.ExceptWith(null), Throws.InstanceOf<ArgumentNullException>());
            a.ExceptWith(_odd);
            Assert.That(a, Is.EqualTo(_even.Where(x => x <= _numbers.Max())));
            a.ExceptWith(_primes);
            Assert.That(a, Is.EqualTo(_even.Where(x => x > 2 && x <= _numbers.Max())));
        }

        [TestCase]
        public void TestIntersectWith()
        {
            var a = new RedBlackSetTester<int>(_even);
            Assert.That(() => a.IntersectWith(null), Throws.InstanceOf<ArgumentNullException>());
            a.IntersectWith(_primes);
            Assert.That(a, Has.Count.EqualTo(1));
            Assert.That(a.Contains(2));
            a.IntersectWith(_odd);
            Assert.That(a, Is.Empty);
        }

        [TestCase]
        public void TestIsProperSubsetOf()
        {
            var a = new RedBlackSetTester<int>();
            Assert.That(() => a.IsProperSubsetOf(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.IsProperSubsetOf(Enumerable.Range(1, 1)));
            Assert.That(a.IsProperSubsetOf(Enumerable.Empty<int>()), Is.False);
            a.AddRange(_primes);
            Assert.That(a.IsProperSubsetOf(_numbers));
            a.AddRange(_even.Where(UpTo100));
            Assert.That(a.IsProperSubsetOf(_numbers));
            a.AddRange(_odd.Where(UpTo100));
            Assert.That(a.IsProperSubsetOf(_numbers), Is.False);
        }

        [TestCase]
        public void TestIsSubsetOf()
        {
            var a = new RedBlackSetTester<int>();
            Assert.That(() => a.IsSubsetOf(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.IsSubsetOf(Enumerable.Range(1, 1)));
            Assert.That(a.IsSubsetOf(Enumerable.Empty<int>()));
            a.AddRange(_primes);
            Assert.That(a.IsSubsetOf(_numbers));
            a.AddRange(_even.Where(UpTo100));
            Assert.That(a.IsSubsetOf(_numbers));
            a.AddRange(_odd.Where(UpTo100));
            Assert.That(a.IsSubsetOf(_numbers));
        }

        [TestCase]
        public void TestIsSupersetOf()
        {
            var a = new RedBlackSetTester<int>();
            Assert.That(() => a.IsSupersetOf(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.IsSupersetOf(Enumerable.Empty<int>()));
            Assert.That(a.IsSupersetOf(Enumerable.Range(1, 1)), Is.False);
            a.AddRange(_odd);
            Assert.That(a.IsSupersetOf(_primes), Is.False);
            a.Add(2);
            Assert.That(a.IsSupersetOf(_primes));
        }

        [TestCase]
        public void TestIsProperSupersetOf()
        {
            var a = new RedBlackSetTester<int>();
            Assert.That(() => a.IsProperSupersetOf(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.IsProperSupersetOf(Enumerable.Empty<int>()), Is.False);
            Assert.That(a.IsProperSupersetOf(Enumerable.Range(1, 1)), Is.False);
            a.Add(2);
            Assert.That(a.IsProperSupersetOf(Enumerable.Empty<int>()));
            a.AddRange(_odd);
            Assert.That(a.IsProperSupersetOf(_primes));
        }

        [TestCase]
        public void TestOverlap()
        {
            var a = new RedBlackSetTester<int>(_odd);
            Assert.That(() => a.Overlaps(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.Overlaps(_even), Is.False);
            a.AddRange(_primes);
            Assert.That(a.Overlaps(_even));
        }

        [TestCase]
        public void TestSetEquals()
        {
            var a = new RedBlackSetTester<int>(_odd.Where(UpTo100));
            Assert.That(() => a.SetEquals(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(a.SetEquals(_numbers), Is.False);
            a.AddRange(_primes);
            Assert.That(a.SetEquals(_numbers), Is.False);
            a.AddRange(_even.Where(UpTo100));
            Assert.That(a.SetEquals(_numbers));
        }

        private bool UpTo100(int n)
        {
            return n <= 100;
        }

        private bool IsPrime(int n)
        {
            if (n < 2)
            {
                return false;
            }
            for (int i = 2; i <= Math.Sqrt(n); ++i)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
