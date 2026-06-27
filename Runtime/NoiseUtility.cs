namespace Kott.Utilities
{
	/// <summary>
	/// Utility class to calculate pseudo-random noise value.
	/// </summary>
    public static class NoiseUtility
    {
	    /// <summary>
	    /// Samples a pseudo-random noise value from an infinite virtual 1-dimensional world
	    /// at the given <paramref name="position"/>.
	    /// Each <paramref name="seed"/> defines a unique world to sample from.
	    /// </summary>
	    public static NoiseValue SampleValue(int position, uint seed)
	    {
		    uint value = SquirrelNoise5(position, seed);
		    return new NoiseValue(value);
	    }
	    
	    /// <inheritdoc cref="SampleValue"/>
	    public static NoiseValueLong SampleValueLong(int position, uint seed)
	    {
		    ulong value0 = SquirrelNoise5(position, seed);
		    ulong value1 = SquirrelNoise5(position, seed + 1);
		    ulong value = (value1 << 32) | value0;
		    return new NoiseValueLong(value);
	    }
	    
	    /// <remarks>
	    /// <see href="https://gist.github.com/kevinmoran/0198d8e9de0da7057abe8b8b34d50f86">Source</see>
	    /// </remarks>
        private static uint SquirrelNoise5(int position, uint seed)
        {
        	uint SQ5_BIT_NOISE1 = 0xd2a80a3f; // 11010010101010000000101000111111
        	uint SQ5_BIT_NOISE2 = 0xa884f197; // 10101000100001001111000110010111
        	uint SQ5_BIT_NOISE3 = 0x6C736F4B; // 01101100011100110110111101001011
        	uint SQ5_BIT_NOISE4 = 0xB79F3ABB; // 10110111100111110011101010111011
        	uint SQ5_BIT_NOISE5 = 0x1b56c4f5; // 00011011010101101100010011110101
        
        	uint mangledBits = unchecked((uint)position);
        	mangledBits *= SQ5_BIT_NOISE1;
        	mangledBits += seed;
        	mangledBits ^= (mangledBits >> 9);
        	mangledBits += SQ5_BIT_NOISE2;
        	mangledBits ^= (mangledBits >> 11);
        	mangledBits *= SQ5_BIT_NOISE3;
        	mangledBits ^= (mangledBits >> 13);
        	mangledBits += SQ5_BIT_NOISE4;
        	mangledBits ^= (mangledBits >> 15);
        	mangledBits *= SQ5_BIT_NOISE5;
        	mangledBits ^= (mangledBits >> 17);
        	return mangledBits;
        }
    }
}