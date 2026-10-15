using UnityEngine;

namespace Kott.Utilities
{
    public class NoiseValueGenerator
    {
        private readonly uint _seed;
        private int _position;

        public NoiseValueGenerator() : this(0)
        { }

        public NoiseValueGenerator(uint seed)
        {
            _seed = seed;
            _position = 0;
        }

        public NoiseValue Next()
        {
            return NoiseUtility.SampleValue(_position++, _seed);
        }
    }
}
