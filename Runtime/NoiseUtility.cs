namespace Kott.Utilities
{
	/// <summary>
	/// Utility class to calculate pseudo-random noise value.
	/// </summary>
    public static class NoiseUtility
    {
	    public static uint GetUInt(int position, uint seed)
	    {
		    return PseudoRandomNoiseValue(position, seed);
	    }
	    
	    public static uint GetUIntRange(int position, uint seed, uint min, uint max)
	    {
		    if (min == max) return min;
		    if (min > max) (min, max) = (max, min);
		    
		    uint delta = max - min;
		    
		    float range01 = GetFloat01(position, seed);
		    uint randomDelta = (uint)((delta + 1) * range01);
		    if (randomDelta == delta + 1) randomDelta = delta;
		    
		    return min + randomDelta;
	    }
	    
	    public static int GetInt(int position, uint seed)
	    {
		    return unchecked((int)PseudoRandomNoiseValue(position, seed));
	    }
	    
	    public static int GetIntRange(int position, uint seed, int min, int max)
	    {
		    if (min == max) return min;
		    if (min > max) (min, max) = (max, min);
		    
		    uint delta = (uint)(max - min);
		    
		    float range01 = GetFloat01(position, seed);
		    int randomDelta = (int)((delta + 1) * range01);
		    if (randomDelta == delta + 1) randomDelta = delta;
		    
		    return min + randomDelta;
	    }
	    
	    public static float GetFloat01(int position, uint seed)
	    {
		    // 23 bits float mantissa
		    const uint mantissaMask = 0x7FFFFF;
		    const float maxMantissaValue = (float)mantissaMask;
		    
		    uint bits = PseudoRandomNoiseValue(position, seed);
		    float mantissaValue = bits & mantissaMask;
		    
		    return mantissaValue / maxMantissaValue; 
	    }
	    
	    public static uint PseudoRandomNoiseValue(int position, uint seed)
	    {
		    return SquirrelNoise5(position, seed);
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