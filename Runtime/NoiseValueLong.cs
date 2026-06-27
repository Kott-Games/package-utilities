namespace Kott.Utilities
{
    /// <summary>
    /// Simple double-precision noise value wrapper, handling common conversions to arithmetic types.
    /// </summary>
    public readonly struct NoiseValueLong
    {
        private readonly ulong _value;
		
        public NoiseValueLong(ulong value)
        {
            _value = value;
        }

        /// <summary>
        /// Converts the noise value to an <see langword="ulong"/>
        /// in the range [ 0 ; 18446744073709551615 ].
        /// </summary>
        public ulong AsULong()
        {
            return _value;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="long"/>
        /// in the range [ -9223372036854775808 ; 9223372036854775807 ].
        /// </summary>
        public long AsLong()
        {
            return unchecked((long)_value);
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="double"/>
        /// in the range [ 0.0 ; 1.0 ].
        /// </summary>
        public double AsDouble01()
        {
            // 52  bits double mantissa
            const ulong mantissaMask = 0xFFFFFFFFFFFFF;
            const double maxMantissaValue = mantissaMask;
		    
            double mantissaValue = _value & mantissaMask;
            return mantissaValue / maxMantissaValue;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="ulong"/>
        /// in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public ulong AsULong(ulong max)
        {
            if (max == 0) return 0;
            if (max == ulong.MaxValue) return _value;
			
            double range01 = AsDouble01();
			
            // add 1 to catch the single unlikely case where range01 is exactly 1
            // then we would not round down and end with a wrong probability for max
            ulong value = (ulong)((max + 1) * range01);
            if (value == max + 1) return max;
			
            return value;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="ulong"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public ulong AsULong(ulong min, ulong max)
        {
            if (max < min) return min;

            ulong delta = max - min;
            return min + AsULong(delta);
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="long"/>
        /// in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public long AsLong(long max)
        {
            if (max < 0) return 0;

            ulong delta = unchecked((ulong)max);
            return unchecked((long)AsULong(delta));
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="long"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public long AsLong(long min, long max)
        {
            if (max < min) return min;

            ulong delta = unchecked((ulong)(max - min));
            return min + unchecked((long)AsULong(delta));
        }
        
        /// <summary>
        /// Converts the noise value to an <see langword="double"/>
        /// in the range [ 0.0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public double AsDouble(double max)
        {
            if (max < 0f) return 0f;
            
            double range01 = AsDouble01();
            return range01 * max;
        }
        
        /// <summary>
        /// Converts the noise value to an <see langword="double"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public double AsDouble(double min, double max)
        {
            if (max < min) return min;
            
            double delta = max - min;
            return min + AsDouble(delta);
        }
    }
}