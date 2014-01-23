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
                select (n - 1) * 2).ToArray();

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
            a.ExceptWith(_odd);
            Assert.That(a, Is.EqualTo(_even.Where(x => x <= _numbers.Max())));
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
