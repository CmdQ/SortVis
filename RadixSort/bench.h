#ifndef _BENCH_H_
#define _BENCH_H_

#include <chrono>
#include <utility>
#include <vector>
#include <random>
#include <algorithm>
#include <functional>
#include <cassert>

template<typename T>
std::vector<T> generate(int const n)
{
    using namespace std;

    default_random_engine re(42);
    uniform_real_distribution<> uniform(0.0, 31337);
    auto mean = uniform(re);
    normal_distribution<> normal(mean, 31337);

    vector<T> ret(n);
    generate(ret.begin(), ret.end(), bind(normal, ref(re)));
    return ret;
}

template<typename T>
std::pair<std::chrono::duration<double>, std::chrono::duration<double>> benchmark(int const n)
{
    using namespace std;
    using namespace std::chrono;

    auto const numbers = generate<T>(n);

    vector<T> copy1(numbers.cbegin(), numbers.cend());
    auto start1 = high_resolution_clock::now();
    std::sort(copy1.begin(), copy1.end());
    auto stop1 = high_resolution_clock::now();

    vector<T> copy2(numbers.cbegin(), numbers.cend());
    auto start2 = high_resolution_clock::now();
    RadixSort::sort(copy2.begin(), copy2.end());
    auto stop2 = high_resolution_clock::now();

    assert(copy1 == copy2);

    return make_pair(duration_cast<duration<double>>(stop1 - start1), duration_cast<duration<double>>(stop2 - start2));
}

#endif//_BENCH_H_
