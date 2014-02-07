#ifndef _RADIX_H_
#define _RADIX_H_

#include <iterator>
#include <limits>
#include <vector>
#include <cstdint>
#include <type_traits>
#include <array>
#include <cassert>
#include <cstring>
#include "bit_stuff.h"

namespace RadixSort
{
    namespace Details
    {
        template<typename T>
        struct RadixSorter
        {
            RadixSorter()
            {
                for (std::size_t i = 0; i < bit_info::HISTS; ++i)
                {
                    _histograms[i] = std::vector<std::ptrdiff_t>(bit_info::HIST_SIZE);
                }
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
            std::array<std::vector<std::ptrdiff_t>, bit_info::HISTS> _histograms;
            BitFlip<T> _flip;
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
            typedef vector<result_of<BitFlip<T>(T)>::type> intermediate_vector;
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
#pragma warning(suppress: 6294) // Only holds for single byte types. Compiler should optimize away.
                for (size_t h = 1; h < bit_info::HISTS; h++)
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
                return _flip.back(x);
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

#endif//_RADIX_H_
