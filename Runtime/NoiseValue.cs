using System;
using System.Runtime.CompilerServices;

namespace Kott.Utilities
{
    /// <summary>
    ///     Simple single-precision noise value wrapper, handling common conversions to arithmetic types.
    /// </summary>
    public readonly struct NoiseValue
    {
        public interface IDistribution
        {
            float Sample(float uniformValue);
        }

        public class LogitDistribution : IDistribution
        {
            private readonly double _p;
            private readonly double _s;

            public LogitDistribution(float power = 5f)
            {
                _p = 1f - power;

                double k = 0.5 * Math.Exp(power);
                _s = (k - 0.5) / (k + 0.5);
            }

            public float Sample(float uniformValue)
            {
                double g = _s * (uniformValue - 0.5) + 0.5;
                double x = _p * Math.Log(g / (1.0 - g));
                return (float)x;
            }
        }

        public uint Bits { get; }

        public NoiseValue(uint bits)
        {
            Bits = bits;
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="uint"/>
        ///     in the range [ 0 ; 4294967295 ].
        /// </summary>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public uint AsUInt(IDistribution distribution = null)
        {
            if (distribution == null)
            {
                return Bits;
            }

            float range01 = AsFloat01(distribution);
            return (uint)(uint.MaxValue * range01);
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="int"/>
        ///     in the range [ -2147483648 ; 2147483647 ].
        /// </summary>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public int AsInt(IDistribution distribution = null)
        {
            if (distribution == null)
            {
                return unchecked((int)Bits);
            }

            return int.MinValue + unchecked((int)AsUInt(distribution));
        }

        /// <summary>
        ///     Converts the noise value to a <see langword="float"/>
        ///     in the range [ 0.0 ; 1.0 ).
        /// </summary>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public float AsFloat01(IDistribution distribution = null)
        {
            const uint pattern = 0x3F800000u; // bit pattern for 1.0 in IEEE 754
            const uint mantissaMask = 0x7FFFFFu; // 23 bits float mantissa

            uint bitsFloat = pattern | (mantissaMask & Bits);
            float value12 = Unsafe.As<uint, float>(ref bitsFloat);

            // shift range from [ 1.0 ; 2.0 ) to [ 0.0 ; 1.0 )
            float value01 = value12 - 1f;

            return distribution?.Sample(value01) ?? value01;
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="uint"/>
        ///     in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public uint AsUInt(uint max, IDistribution distribution = null)
        {
            if (max == 0)
            {
                return 0;
            }

            if (max == uint.MaxValue)
            {
                return AsUInt(distribution);
            }

            float range01 = AsFloat01(distribution);

            // add 1 to catch the single unlikely case where range01 is exactly 1
            // then we would not round down and end with a wrong probability for max
            uint value = (uint)((max + 1) * range01);
            if (value == max + 1)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="uint"/>
        ///     in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public uint AsUInt(uint min, uint max, IDistribution distribution = null)
        {
            if (max < min)
            {
                return min;
            }

            uint delta = max - min;
            return min + AsUInt(delta, distribution);
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="int"/>
        ///     in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public int AsInt(int max, IDistribution distribution = null)
        {
            if (max < 0)
            {
                return 0;
            }

            uint delta = unchecked((uint)max);
            return unchecked((int)AsUInt(delta, distribution));
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="int"/>
        ///     in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        public int AsInt(int min, int max, IDistribution distribution = null)
        {
            if (max < min)
            {
                return min;
            }

            uint delta = unchecked((uint)(max - min));
            return min + unchecked((int)AsUInt(delta, distribution));
        }

        /// <summary>
        ///     Converts the noise value to a <see langword="float"/>
        ///     in the range [ 0.0 ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When <paramref name="max"/> is strictly less than 0.0.
        /// </exception>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is exactly equal to 0.0
        ///     the returned value will be equal to <paramref name="max"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     ignoring float precision errors.
        /// </remarks>
        public float AsFloat(float max, IDistribution distribution = null)
        {
            if (max < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            float range01 = AsFloat01(distribution);
            return range01 * max;
        }

        /// <summary>
        ///     Converts the noise value to a <see langword="float"/>
        ///     in the range [ <paramref name="min"/> ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When <paramref name="max"/> is strictly less than <paramref name="min"/>.
        /// </exception>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is exactly equal to <paramref name="min"/>
        ///     the returned value will be equal to <paramref name="max"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     ignoring float precision errors.
        /// </remarks>
        public float AsFloat(float min, float max, IDistribution distribution = null)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            float delta = max - min;
            return min + AsFloat(delta, distribution);
        }
    }
}