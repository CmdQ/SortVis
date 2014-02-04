#ifndef _RADIX_H_

#include <iterator>
#include <limits>
#include <vector>
#include <cstdint>
#include <type_traits>
#include <array>
#ifdef _DEBUG
#include <iostream>
#endif

namespace SortVis
{
    namespace RadixSort
    {
        namespace Details
        {
            template<int B>
            struct Bits
            {
                static std::size_t const RADIX = 8;
                static std::size_t const HISTS = B / RADIX;
                static std::size_t const HIST_SIZE = 1 << RADIX;

                static_assert(B % RADIX == 0, "Cannot be divided evenly.");
            };

            template<>
            struct Bits<32>
            {
                static std::size_t const RADIX = 11;
                static std::size_t const HISTS = 3;
                static std::size_t const HIST_SIZE = 1 << RADIX;
            };

            template<typename T>
            struct RadixSorter
            {
                RadixSorter()
                    : _histograms()
                {
                }

                template<typename Iter>
                void operator()(Iter const first, Iter const last)
                {
                    using namespace std;
                    cout << "Hello: " << _histograms[0][0] << endl;
                }

            private:
                typedef Bits<sizeof(T)* 8> bit_info;
                std::array<std::array<std::size_t, bit_info::HIST_SIZE>, bit_info::HISTS> _histograms;
            };
        }

        template<typename Iter>
        void sort(Iter const first, Iter const last)
        {
            using namespace std;
            using namespace Details;

            typedef Iter::value_type value_type;

            int const bits = numeric_limits<value_type>::digits;
            auto const n = distance(first, last);

            if (n < 2)
            {
                return;
            }

            vector<value_type> swaps(n);
        }
    }
}

#define _RADIX_H_
#endif//_RADIX_H_
