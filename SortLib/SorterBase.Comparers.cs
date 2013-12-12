using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortVis
{
    public abstract partial class SorterBase : ISorter
    {
        private class TenE0Comparer : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                x %= 10;
                y %= 10;

                return Comparer<int>.Default.Compare(x, y);
            }
        }

        private class TenE1Comparer : TenE0Comparer
        {
            public override int Compare(int x, int y)
            {
                x = (x / 10) % 10;
                y = (y / 10) % 10;

                return Comparer<int>.Default.Compare(x, y);
            }
        }

        private readonly TenE0Comparer _ten0 = new TenE0Comparer();
        private readonly TenE1Comparer _ten1 = new TenE1Comparer();

        /// <summary>
        /// Sort all permutations of tuples by first and last item and see if equal
        /// elements keep their order.
        /// </summary>
        /// <returns><c>true</c> if stable, <c>false</c> otherwise.</returns>
        protected virtual bool CheckIfStable()
        {
            var three = Task.Factory.StartNew(() => { return CheckIfStable(3); });
            var four  = Task.Factory.StartNew(() => { return CheckIfStable(4); });
            return three.Result && four.Result;
        }

        private bool CheckIfStable(int maxNum)
        {
            // TODO 7
            var nums = (
                from a in Enumerable.Range(1, maxNum)
                from b in Enumerable.Range(1, maxNum)
                select a * 10 + b).Take(7).ToArray();

            // Do this in another instance, otherwise we get nasty race conditions.
            var thisSorter = (SorterBase)Activator.CreateInstance(GetType());
            thisSorter.Numbers = nums;
            return thisSorter.RecursiveCheckIfStable(0);
        }

        private bool RecursiveCheckIfStable(int i)
        {
            int n = Numbers.Length;
            if (i >= n - 1)
            {
                return CheckIfStable(Numbers);
            }

            if (!RecursiveCheckIfStable(i + 1))
            {
                return false;
            }
            for (int j = i + 1; j < n; ++j)
            {
                int temp = Numbers[i];
                Numbers[i] = Numbers[j];
                Numbers[j] = temp;
                if (!RecursiveCheckIfStable(i + 1))
                {
                    return false;
                }
                Numbers[j] = Numbers[i];
                Numbers[i] = temp;
            }

            return CheckIfStable(Numbers);
        }

        private bool CheckIfStable(int[] nums)
        {
            return CheckIfStable(nums, _ten0, _ten1) && CheckIfStable(nums, _ten1, _ten0);
        }

        private bool CheckIfStable(int[] nums, Comparer<int> first, Comparer<int> second)
        {
            Comparer = first;
            Numbers = nums;
            Sort();
            Comparer = second;
            Sort();

            for (int i = 1; i < nums.Length; ++i)
            {
                int left = Numbers[i - 1];
                int right = Numbers[i];
                if (second.Compare(left, right) > 0)
                {
                    throw new NotSortedException(Name, i);
                }
                if (second.Compare(left, right) == 0 && first.Compare(left, right) > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
