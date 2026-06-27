namespace Kott.Utilities
{
    /// <summary>
    /// Simple single-precision noise value wrapper, handling common conversions to arithmetic types.
    /// </summary>
    public readonly struct NoiseValue
    {
        private readonly uint _value;
		
        public NoiseValue(uint value)
        {
            _value = value;
        }

        /// <summary>
        /// Converts the noise value to an <see langword="uint"/>
        /// in the range [ 0 ; 4294967295 ].
        /// </summary>
        public uint AsUInt()
        {
            return _value;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="int"/>
        /// in the range [ -2147483648 ; 2147483647 ].
        /// </summary>
        public int AsInt()
        {
            return unchecked((int)_value);
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="float"/>
        /// in the range [ 0.0 ; 1.0 ].
        /// </summary>
        public float AsFloat01()
        {
            // 23 bits float mantissa
            const uint mantissaMask = 0x7FFFFF;
            const float maxMantissaValue = mantissaMask;
		    
            float mantissaValue = _value & mantissaMask;
            return mantissaValue / maxMantissaValue;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="uint"/>
        /// in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public uint AsUInt(uint max)
        {
            if (max == 0) return 0;
            if (max == uint.MaxValue) return _value;
			
            float range01 = AsFloat01();
			
            // add 1 to catch the single unlikely case where range01 is exactly 1
            // then we would not round down and end with a wrong probability for max
            uint value = (uint)((max + 1) * range01);
            if (value == max + 1) return max;
			
            return value;
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="uint"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public uint AsUInt(uint min, uint max)
        {
            if (max < min) return min;

            uint delta = max - min;
            return min + AsUInt(delta);
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="int"/>
        /// in the range [ 0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public int AsInt(int max)
        {
            if (max < 0) return 0;

            uint delta = unchecked((uint)max);
            return unchecked((int)AsUInt(delta));
        }
		
        /// <summary>
        /// Converts the noise value to an <see langword="int"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public int AsInt(int min, int max)
        {
            if (max < min) return min;

            uint delta = unchecked((uint)(max - min));
            return min + unchecked((int)AsUInt(delta));
        }
        
        /// <summary>
        /// Converts the noise value to an <see langword="float"/>
        /// in the range [ 0.0 ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="max">Inclusive upper bounds.</param>
        public float AsFloat(float max)
        {
            if (max < 0f) return 0f;
            
            float range01 = AsFloat01();
            return range01 * max;
        }
        
        /// <summary>
        /// Converts the noise value to an <see langword="float"/>
        /// in the range [ <paramref name="min"/> ; <paramref name="max"/> ].
        /// </summary>
        /// <param name="min">Inclusive lower bounds.</param>
        /// <param name="max">Inclusive upper bounds.</param>
        public float AsFloat(float min, float max)
        {
            if (max < min) return min;
            
            float delta = max - min;
            return min + AsFloat(delta);
        }
    }
}