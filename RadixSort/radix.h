#ifndef _RADIX_H_

#include <iterator>
#include <limits>
#include <vector>
#include <cstdint>
#include <type_traits>

namespace SortVis
{
    namespace RadixSort
    {
        namespace Details
        {
            template<typename T>
            struct TranslateBase
            {
                typedef T mask_type;

                static mask_type ZERO()
                {
                    return mask_type{};
                }

                static mask_type ONE()
                {
                    return 1;
                }

                static mask_type BIT_AND(mask_type const a, mask_type const b)
                {
                    return a & b;
                }
            };

            template<typename T>
            struct Translate : public TranslateBase<T>
            {
                static T FIRST_ONE()
                {
                    return FIRST_ONE(std::integral_constant<bool, std::numeric_limits<T>::is_signed>());
                }

            private:
                static T FIRST_ONE(std::integral_constant<bool, true>)
                {
                    return std::numeric_limits<T>::lowest();
                }

                static T FIRST_ONE(std::integral_constant<bool, false>)
                {
                    throw "Should never be reached.";
                }
            };

            template<>
            struct Translate<float> : public TranslateBase<int32_t>
            {
                typedef float value_type;

                static mask_type FIRST_ONE()
                {
                    return std::numeric_limits<mask_type>::lowest();
                }

                static mask_type BIT_AND(value_type const a, mask_type const b)
                {
                    return *reinterpret_cast<mask_type const *>(&a)& b;
                }
            };

            template<>
            struct Translate<double> : public TranslateBase<int64_t>
            {
                typedef double value_type;

                static mask_type FIRST_ONE()
                {
                    return std::numeric_limits<mask_type>::lowest();
                }

                static mask_type BIT_AND(value_type const a, mask_type const b)
                {
                    return *reinterpret_cast<mask_type const *>(&a)& b;
                }
            };

            template<typename Iter>
            inline void partition(Iter first, Iter const last,
                typename Translate<typename Iter::value_type>::mask_type const mask,
                Iter & front, Iter & back)
            {
                typedef Translate<Iter::value_type> translate;
                auto zero = translate::ZERO();

                for (; first != last; ++first)
                {
                    if (translate::BIT_AND(*first, mask) == zero)
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

            auto const one = Translate<value_type>::ONE();

            // All "normal" bits starting with the least significant one (makes it a stable sort).
            for (int b = 0; b < bits; ++b)
            {
                auto front = swaps.begin();
                auto back = --swaps.end();

                partition(first, last, one << b, front, back);

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

                partition(first, last, Translate<value_type>::FIRST_ONE(), front, back);

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
