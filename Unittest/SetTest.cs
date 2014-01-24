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
                where s <= 100
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
