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

            if (n < 2)
            {
                return;
            }

            vector<value_type> swaps(n);

            // All "normal" bits starting with the least significant one (makes it a stable sort).
            for (int b = 0; b < bits; ++b)
            {
                auto front = swaps.begin();
                auto back = --swaps.end();

                partition(first, last, 1 << b, front, back);

                if (front == swaps.end())
                {
                    continue;
                }

                auto continueHere = move(swaps.begin(), front, first);
                move(swaps.rbegin(), reverse_iterator<Iter>(front), continueHere);
            }

            // For twos-complement numbers, we have to do a special pass for the highest bit.
            if (numeric_limits<value_type>::is_signed)
            {
                auto front = swaps.begin();
                auto back = --swaps.end();

                partition(first, last, numeric_limits<value_type>::lowest(), front, back);

                if (front != swaps.end())
                {
                    auto continueHere = move(swaps.rbegin(), reverse_iterator<Iter>(front), first);
                    move(swaps.begin(), front, continueHere);
                }
            }
        }
    }
}

#define _RADIX_H_
#endif//_RADIX_H_
