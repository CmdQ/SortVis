#ifndef _RADIX_H_

#include <iterator>
#include <limits>
#include <vector>
#include <cstdint>
#include <cassert>
#include <type_traits>
#include <array>
#ifdef _DEBUG
#include <iostream>
#endif

namespace RadixSort
{
    namespace Details
    {
        template<int B>
        struct Bits
        {
            typedef std::size_t size_type;

            static size_type const RADIX = 8;
            static size_type const HISTS = B / RADIX;
            static size_type const HIST_SIZE = 1 << RADIX;

            static_assert(B % RADIX == 0, "Cannot be divided evenly.");

            template<typename T>
            static size_type radix(T x, size_type n)
            {
                static_assert((_mask & ((signed char)-1)) == 0xFF
                    && (_mask & ((char)-1)) == 0xFF
                    && (_mask & ((unsigned char)255)) == 0xFF,
                    "Mask doesn't work for values.");
                assert(n < HISTS);
                return (x >> (n * RADIX)) & _mask;
            }

        private:
            static size_type const _mask = 0xFF;
        };

        template<>
        struct Bits<32>
        {
            typedef Bits<0>::size_type size_type;

            static size_type const RADIX = 11;
            static size_type const HISTS = 3;
            static size_type const HIST_SIZE = 1 << RADIX;

            template<typename T>
            static size_type radix(T x, size_type n)
            {
                assert(n < HISTS);
                return (x >> (n * RADIX)) & 0x7FF;
            }
        };

        template<typename T, bool IS_SIGNED>
        struct BitFlip
        {
        };

        template<typename T>
        struct BitFlip<T, true>
        {
            static const bool NECESSARRY = true;

            T operator()(T x)
            {
                return x ^ std::numeric_limits<T>::min();
            }
        };

        template<typename T>
        struct BitFlip<T, false>
        {
            static const bool NECESSARRY = false;

            T operator()(T x)
            {
                return x;
            }
        };

        template <>
        struct BitFlip<float, true>
        {
            typedef std::uint32_t integer_type;

            static const bool NECESSARRY = true;

            integer_type operator()(float x)
            {
                auto asInt = *reinterpret_cast<integer_type *>(&x);
                return asInt ^ _mask;
            }

        private:
            static const integer_type _mask = 0x80000000;
        };

        template <>
        struct BitFlip<double, true>
        {
            typedef std::uint64_t integer_type;

            static const bool NECESSARRY = true;

            integer_type operator()(double x)
            {
                auto asInt = *reinterpret_cast<integer_type *>(&x);
                return asInt ^ _mask;
            }

        private:
            static const integer_type _mask = 0x8000000000000000L;
        };

        template<typename T>
        struct RadixSorter
        {
            RadixSorter()
                : _histograms{}
            {
            }

            template<typename Cont>
            void operator()(Cont & container)
            {
                int const n = container.size();
                if (n >= 2)
                {
                    RadixSorter<Cont::value_type> sorter;
                    operator()(begin(container), end(container), n);
                }
            }

            template<typename Iter>
            void operator()(Iter const & first, Iter const & last, typename Iter::difference_type const n);

        private:
            template<typename Iter>
            bool histogram(Iter first, Iter const & last);

            typedef Bits<sizeof(T)* 8> bit_info;
            std::array<std::array<std::ptrdiff_t, bit_info::HIST_SIZE>, bit_info::HISTS> _histograms;
            typedef BitFlip<T, std::numeric_limits<T>::is_signed> flip_type;
            flip_type _flip;
        };

        template<typename T>
        template<typename Iter>
        void RadixSorter<T>::operator()(Iter const & first, Iter const & last,
            typename Iter::difference_type const n)
        {
            using namespace std;

            if (!histogram(first, last))
            {
                // Range was sorted and no histograms were built.
                return;
            }

            // We need a vector that can hold the return values of that flip function.
            typedef vector<result_of<flip_type(T)>::type> intermediate_vector;
            intermediate_vector temp(n);

            // First iteration that also flips values to bit malleable type.
            for (auto iter = first; iter != last; ++iter)
            {
                temp[++_histograms[0][bit_info::radix(*iter, 0)]] = _flip(*iter);
            }

            {
                // Rest of the iterations. Save copying by swapping pointers.
                auto one = temp.data();
                auto two = reinterpret_cast<decltype(one)>(&*first);
                int const rest = bit_info::HISTS;
#pragma warning(suppress: 6294) // Only holds for single byte types. Compiler should optimize away.
                for (size_t h = 1; h < rest; h++)
                {
                    for (int i = 0; i < n; ++i)
                    {
                        auto const si = one[i];
                        two[++_histograms[h][bit_info::radix(si, h)]] = si;
                    }
                    swap(one, two);
                }

                /* Depending if we did an even or odd number of pointer swaps
                * two might not point to the passed array where we want to write
                * the sorted values back. In this case, we move the current state
                * from the passed array to our temporary vector.
                */
                if (two == temp.data())
                {
                    // If we don't have to do a flip back (as with unsigned integers), then we're already done.
                    if (!_flip.NECESSARRY)
                    {
                        return;
                    }
                    move(one, one + n, temp.begin());
                }
            }

            // Flip values and write them back to the passed array.
            transform(temp.cbegin(), temp.cend(), first, [this](intermediate_vector::value_type x)
            {
                return _flip(x);
            });
        }

        template<typename T>
        template<typename Iter>
        bool RadixSorter<T>::histogram(Iter first, Iter const & last)
        {
            int const histograms = bit_info::HISTS;

            bool sorted = true;
            auto prev = *first;
            for (; first != last; ++first)
            {
                sorted &= prev <= *first;
                auto flipped = _flip(*first);
                for (size_t h = 0; h < histograms; ++h)
                {
                    ++_histograms[h][bit_info::radix(flipped, h)];
                }
            }

            if (sorted)
            {
                return false;
            }

            typedef Iter::difference_type count_type;

            count_type sum = 0;
            array<count_type, histograms> sums{};
            for (size_t h = 0; h < histograms; ++h)
            {
                for (count_type i = 0; i < bit_info::HIST_SIZE; ++i)
                {
                    sum = _histograms[h][i] + sums[h];
                    _histograms[h][i] = sums[h] - 1;
                    sums[h] = sum;
                }
            }

            return true;
        }
    }

    template<typename Iter>
    void sort(Iter const & first, Iter const & last)
    {
        auto const n = std::distance(first, last);

        if (n >= 2)
        {
            Details::RadixSorter<Iter::value_type> sorter;
            sorter(first, last, n);
        }
    }

    template<typename Cont>
    void sort(Cont & container)
    {
        auto const n = container.size();

        if (n >= 2)
        {
            Details::RadixSorter<Cont::value_type> sorter;
            sorter(std::begin(container), std::end(container), n);
        }
    }
}

#define _RADIX_H_
#endif//_RADIX_H_
