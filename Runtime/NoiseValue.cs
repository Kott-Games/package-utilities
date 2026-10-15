using System;

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
            private readonly double _s0;

            public LogitDistribution(float power = 5f)
            {
                _p = 1.0 / power;

                double k = 0.5 * Math.Exp(power);
                _s = (k - 0.5) / (k + 0.5);
                _s0 = 0.5 - _s * 0.5;
            }

            public float Sample(float uniformValue)
            {
                double g = _s * uniformValue + _s0;
                double x = _p * Math.Log(g / (1.0 - g)) * 0.5 + 0.5;
                float v = (float)x;
                return v < 1f && v >= 0f ? v : 0f;
            }
        }

        public uint Bits { get; }

        public NoiseValue(uint bits)
        {
            Bits = bits;
        }

        /// <summary>
        ///     Converts the noise value to aa <see langword="uint"/>
        ///     in the range [ 0 ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to 0
        ///     the returned value will be 0, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public uint AsUInt(uint max, IDistribution distribution = null)
        {
            if (max <= 0)
            {
                return 0;
            }

            if (distribution == null)
            {
                ulong overflowValue = (ulong)Bits * max;
                return unchecked((uint)(overflowValue >> 32));
            }

            float range01 = AsFloat01(distribution);
            return (uint)(range01 * max);
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="uint"/>
        ///     in the range [ <paramref name="min"/> ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to <paramref name="min"/>
        ///     the returned value will be <paramref name="min"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public uint AsUInt(uint min, uint max, IDistribution distribution = null)
        {
            if (max <= min)
            {
                return min;
            }

            if (distribution == null)
            {
                ulong overflowValue = (ulong)Bits * (max - min);
                return min + unchecked((uint)(overflowValue >> 32));
            }

            float range01 = AsFloat01(distribution);
            return min + (uint)(range01 * (max - min));
        }

        /// <summary>
        ///     Converts the noise value to aa <see langword="int"/>
        ///     in the range [ 0 ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to 0
        ///     the returned value will be 0, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public int AsInt(int max, IDistribution distribution = null)
        {
            if (max <= 0)
            {
                return 0;
            }

            if (distribution == null)
            {
                const int iOffset = int.MinValue;
                const uint uOffset = unchecked((uint)iOffset);

                uint uMax = unchecked((uint)(max + int.MinValue));

                ulong overflowValue = (ulong)Bits * (uMax - uOffset);
                uint uVal = uOffset + unchecked((uint)(overflowValue >> 32));

                return unchecked((int)(uVal + uOffset));
            }

            float range01 = AsFloat01(distribution);
            return (int)(range01 * max);
        }

        /// <summary>
        ///     Converts the noise value to an <see langword="int"/>
        ///     in the range [ <paramref name="min"/> ; <paramref name="max"/> ).
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Exclusive upper bounds.</param>
        /// <param name="distribution">
        ///     Optional distribution to transform the output value.
        ///     <see langword="null"/> means uniform distribution.
        /// </param>
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to <paramref name="min"/>
        ///     the returned value will be <paramref name="min"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public int AsInt(int min, int max, IDistribution distribution = null)
        {
            if (max <= min)
            {
                return min;
            }

            if (distribution == null)
            {
                const int iOffset = int.MinValue;
                const uint uOffset = unchecked((uint)iOffset);

                uint uMax = unchecked((uint)(max + iOffset));
                uint uMin = unchecked((uint)(min + iOffset));

                ulong overflowValue = (ulong)Bits * (uMax - uMin);
                uint uVal = uMin + unchecked((uint)(overflowValue >> 32));

                return unchecked((int)(uVal + uOffset));
            }

            float range01 = AsFloat01(distribution);
            return min + (int)(range01 * (max - min));
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
            float value12 = BitConverter.Int32BitsToSingle((int)bitsFloat);

            // shift range from [ 1.0 ; 2.0 ) to [ 0.0 ; 1.0 )
            float value01 = value12 - 1f;

            return distribution?.Sample(value01) ?? value01;
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
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to 0.0
        ///     the returned value will be equal to <paramref name="max"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public float AsFloat(float max, IDistribution distribution = null)
        {
            if (max <= 0f)
            {
                return 0f;
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
        /// <remarks>
        ///     In the case where <paramref name="max"/> is less or equal to <paramref name="min"/>
        ///     the returned value will be equal to <paramref name="min"/>, violating mathematical
        ///     correctness (the range [ x ; x ) would be considered empty) for the sake of
        ///     developer quality-of-life.
        /// </remarks>
        public float AsFloat(float min, float max, IDistribution distribution = null)
        {
            if (max <= min)
            {
                return min;
            }

            float delta = max - min;
            return min + AsFloat(delta, distribution);
        }
    }
}