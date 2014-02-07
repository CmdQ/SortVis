#ifndef _BIT_STUFF_H

#include <type_traits>
#include <limits>
#include <cstdint>
#include <cstring>

namespace RadixSort
{
    namespace Details
    {
        template<typename T>
        struct FloatBits
        {
            static const bool FLOAT_32BIT = std::is_same<T, float>::value;
            static_assert(FLOAT_32BIT || std::is_same<T, double>::value, "This only works for float and double.");

            static const bool NECESSARRY = true;

            static const auto MASK = std::conditional<FLOAT_32BIT,
                std::integral_constant<uint32_t, 0x80000000>,
                std::integral_constant<uint64_t, 0x8000000000000000L >> ::type::value;
            typedef typename std::remove_cv<decltype(MASK)>::type integer_type;
            typedef typename std::make_signed<integer_type>::type signed_integer_type;
            static const int SIGN_SHIFT = std::numeric_limits<signed_integer_type>::digits;

            static integer_type to_bits(T f)
            {
                integer_type re;
                std::memcpy(&re, &f, sizeof(T));
                return re;
            }

            static T from_bits(integer_type i)
            {
                T re;
                std::memcpy(&re, &i, sizeof(T));
                return re;
            }
        };

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
                static_assert((MASK & ((signed char)-1)) == 0xFF
                    && (MASK & ((char)-1)) == 0xFF
                    && (MASK & ((unsigned char)255)) == 0xFF,
                    "Mask doesn't work for values.");
                assert(n < HISTS);
                return (x >> (n * RADIX)) & MASK;
            }

            static size_type radix(double x, size_type n)
            {
                assert(n < HISTS);
                return (FloatBits<double>::to_bits(x) >> (n * RADIX)) & MASK;
            }

        private:
            static size_type const MASK = 0xFF;
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
                return (x >> (n * RADIX)) & MASK;
            }

            static size_type radix(float x, size_type n)
            {
                assert(n < HISTS);
                return (FloatBits<float>::to_bits(x) >> (n * RADIX)) & MASK;
            }

        private:
            static size_type const MASK = 0x7FF;
        };

        template<typename T, bool IS_SIGNED, bool IS_IEC559>
        struct BitFlipSwitch
        {
        };

        template<typename T>
        struct BitFlipSwitch<T, true, false>
        {
            static const bool NECESSARRY = true;

            T operator()(T x) const
            {
                return x ^ std::numeric_limits<T>::min();
            }

            T back(T x) const
            {
                // Symmetric operation.
                return operator()(x);
            }
        };

        template<typename T>
        struct BitFlipSwitch<T, false, false>
        {
            static const bool NECESSARRY = false;

            T operator()(T x) const
            {
                // noop operation.
                return x;
            }

            T back(T x) const
            {
                // Symmetric noop operation.
                return operator()(x);
            }
        };

        template <typename T>
        struct BitFlipSwitch<T, true, true> : public FloatBits<T>
        {
            typedef FloatBits<T> base_type;
            typedef typename base_type::integer_type integer_type;
            typedef typename base_type::signed_integer_type signed_integer_type;

            integer_type operator()(T f) const
            {
                auto asInt = to_bits(f);
                integer_type mask = -static_cast<signed_integer_type>(asInt >> SIGN_SHIFT) | MASK;
                return asInt ^ mask;
            }

            T back(integer_type i) const
            {
                integer_type const mask = ((i >> SIGN_SHIFT) - 1) | MASK;
                i ^= mask;
                return from_bits(i);
            }
        };

        template<typename T>
        struct BitFlip : public BitFlipSwitch<T, std::numeric_limits<T>::is_signed, std::numeric_limits<T>::is_iec559>
        {
        };
    }
}

#define _BIT_STUFF_H
#endif//_BIT_STUFF_H
