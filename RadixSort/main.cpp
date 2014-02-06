#include <iostream>
#include <vector>
#include <algorithm>
#include "radix.h"
#include "bench.h"

using namespace std;

template<typename F, typename T>
vector<T> cast(vector<F> const & src)
{
    vector<T> re(src.size());
    transform(src.cbegin(), src.cend(), re.begin(), [](F in)
    {
        return static_cast<T>(in);
    });
    return re;
}

template<typename T>
void test(vector<T> nums)
{
    RadixSort::sort(nums.begin(), nums.end());

    if (!is_sorted(nums.begin(), nums.end()))
    {
        for (auto iter : nums)
        {
            cout << iter << "   ";
        }
        cout << endl;
    }
}

int main()
{
    vector<char> empty;
    vector<char> tiny(1);
    vector<int> positive{ 541, 212, 5125, 6342, 61, 243, 15, 99, 1234, 123, 1524 };
    vector<int> negative{ 541, -212, 5125, 6342, -61, -243, 15, -99, 1234, -123, 1524 };

    test(empty);
    test(tiny);
    test(cast<int, unsigned short>(positive));
    test(cast<int, short>(positive));
    test(cast<int, unsigned int>(positive));
    test(positive);
    test(cast<int, unsigned long>(positive));
    test(cast<int, long>(positive));
    test(cast<int, unsigned long long>(positive));
    test(cast<int, long long>(positive));

    test(cast<int, short>(negative));
    test(negative);
    test(cast<int, long>(negative));
    test(cast<int, long long>(negative));

    test(cast<int, float>(positive));
    test(cast<int, double>(positive));
    test(cast<int, float>(negative));
    test(cast<int, double>(positive));

    RadixSort::Details::RadixSorter<int> r;
    RadixSort::sort(positive);
    r(positive);

    cout << "Radix sort test done.\n";

    int const length = 999999;
    {
        cout << "Benchmarking float...\n";
        auto res = benchmark<float>(length);
        cout << "std::sort took " << res.first.count() << " s.\n";
        cout << "radixsort took " << res.second.count() << " s.\n";
    }
    {
        cout << "Benchmarking double...\n";
        auto res = benchmark<double>(length);
        cout << "std::sort took " << res.first.count() << " s.\n";
        cout << "radixsort took " << res.second.count() << " s.\n";
    }
    {
        cout << "Benchmarking uint...\n";
        auto res = benchmark<uint32_t>(length);
        cout << "std::sort took " << res.first.count() << " s.\n";
        cout << "radixsort took " << res.second.count() << " s.\n";
    }
    {
        cout << "Benchmarking int...\n";
        auto res = benchmark<int32_t>(length);
        cout << "std::sort took " << res.first.count() << " s.\n";
        cout << "radixsort took " << res.second.count() << " s.\n";
    }

    return 0;
}
