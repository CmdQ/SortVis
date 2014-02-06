#ifndef _BIT_STUFF_H

#include <type_traits>
#include <limits>
#include <cstdint>
#include <cstring>

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

        template<typename T, typename integer_type>
        struct FloatBits
        {
            integer_type to_bits(T f) const
            {
                integer_type re;
                std::memcpy(&re, &f, sizeof(T));
                return re;
            }

            T from_bits(integer_type i) const
            {
                T re;
                std::memcpy(&re, &i, sizeof(T));
                return re;
            }
        };

        template<typename T, bool IS_SIGNED, bool IS_IEC559>
        struct BitFlip
        {
        };

        template<typename T>
        struct BitFlip<T, true, false>
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
        struct BitFlip<T, false, false>
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

        template <>
        struct BitFlip<float, true, true> : private FloatBits<float, std::uint32_t>
        {
            typedef std::uint32_t integer_type;
            typedef std::int32_t signed_int;

            static const bool NECESSARRY = true;

            integer_type operator()(float f) const
            {
                auto asInt = to_bits(f);
                integer_type mask = -static_cast<signed_int>(asInt >> sign_shift) | _mask;
                return asInt ^ mask;
            }

            float back(integer_type i) const
            {
                integer_type const mask = ((i >> sign_shift) - 1) | _mask;
                i ^= mask;
                return from_bits(i);
            }

        private:
            static const int sign_shift = std::numeric_limits<signed_int>::digits;
            static const integer_type _mask = 0x80000000;
        };

        template <>
        struct BitFlip<double, true, true> : private FloatBits<double, std::uint64_t>
        {
            typedef std::uint64_t integer_type;
            typedef std::int64_t signed_int;

            static const bool NECESSARRY = true;

            integer_type operator()(double d) const
            {
                auto asInt = to_bits(d);
                integer_type mask = -static_cast<signed_int>(asInt >> sign_shift) | _mask;
                return asInt ^ mask;
            }

            double back(integer_type i) const
            {
                integer_type const mask = ((i >> sign_shift) - 1) | _mask;
                i ^= mask;
                return from_bits(i);
            }

        private:
            static const int sign_shift = std::numeric_limits<signed_int>::digits;
            static const integer_type _mask = 0x8000000000000000L;
        };
    }
}

#define _BIT_STUFF_H
#endif//_BIT_STUFF_H
