#ifndef _RADIX_H_

#include <iterator>
#include <limits>
#include <vector>

namespace SortVis
{
    namespace RadixSort
    {
        namespace Details
        {
            template<typename Iter>
            inline void partition(Iter first, Iter const last,
                typename Iter::value_type const mask,
                Iter & front, Iter & back)
            {
                for (; first != last; ++first)
                {
                    if ((*first & mask) == 0)
                    {
                        *front++ = *first;
                    }
                    else
                    {
                        *back-- = *first;
                    }
                }
            }
        }

        template<typename Iter>
        void sort(Iter const first, Iter const last)
        {
            using namespace std;
            using namespace Details;

            typedef Iter::value_type value_type;

            int const bits = numeric_limits<value_type>::digits;
            auto const n = distance(first, last);

            vector<value_type> swaps(n);

            // All "normal" bits.
            for (int b = 0; b < bits; ++b)
            {
                auto front = swaps.begin();
                auto back = --swaps.end();

                partition(first, last, 1 << b, front, back);

                if (front == swaps.end())
                {
                    continue;
                }

                auto writeBack = first;
                for (auto iter = swaps.begin(); iter != front; ++iter)
                {
                    *writeBack++ = *iter;
                }

                for (auto iter = --swaps.end(); writeBack != last; --iter)
                {
                    *writeBack++ = *iter;
                }
            }

            if (numeric_limits<value_type>::is_signed)
            {
                auto front = swaps.begin();
                auto back = --swaps.end();

                partition(first, last, numeric_limits<value_type>::lowest(), front, back);

                if (front-- != swaps.end())
                {
                    auto writeBack = first;
                    for (auto iter = --swaps.end(); iter != front; --iter)
                    {
                        *writeBack++ = *iter;
                    }
                    for (auto iter = swaps.begin(); writeBack != last; ++iter)
                    {
                        *writeBack++ = *iter;
                    }

                }
            }
        }
    }
}

#define _RADIX_H_
#endif//_RADIX_H_
