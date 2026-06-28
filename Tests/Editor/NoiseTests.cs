using System;
using NUnit.Framework;

namespace Kott.Utilities.Tests
{
    public class NoiseTests
    {
        [Test]
        public void AsFloat01()
        {
            CheckValues(CheckAsFloat01);
        }

        [Test]
        public void AsFloat()
        {
            CheckValues(CheckAsFloat);
        }

        private static void CheckAsFloat01(NoiseValue sampleValue)
        {
            float value = sampleValue.AsFloat01();
            Assert.GreaterOrEqual(value, 0f, NoiseValueDebugString(sampleValue));
            Assert.Less(value, 1f, NoiseValueDebugString(sampleValue));
        }

        private static void CheckAsFloat(NoiseValue sampleValue)
        {
            float[] rangeValues =
            {
                0f,
                -1.23456789f,
                -12345.6789f,
                -123456789f,
                1.23456789f,
                12345.6789f,
                123456789f
            };

            foreach (float value in rangeValues)
            {
                CheckAsFloat(sampleValue, value);
            }
            
            foreach (float valueA in rangeValues)
            foreach (float valueB in rangeValues)
            {
                CheckAsFloat(sampleValue, valueA, valueB);
            }
        }

        private static void CheckAsFloat(NoiseValue sampleValue, float max)
        {
            if (max < 0f)
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => sampleValue.AsFloat(max),
                    NoiseValueDebugString(sampleValue));
                return;
            }
            
            float value = sampleValue.AsFloat(max);

            if (max <= 0f)
            {
                Assert.AreEqual(value, 0f, NoiseValueDebugString(sampleValue));
                return;
            }
            
            Assert.GreaterOrEqual(value, 0f, NoiseValueDebugString(sampleValue));
            Assert.Less(value, max, NoiseValueDebugString(sampleValue));
        }

        private static void CheckAsFloat(NoiseValue sampleValue, float min, float max)
        {
            if (max < min)
            {
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => sampleValue.AsFloat(min, max),
                    NoiseValueDebugString(sampleValue));
                return;
            }
            
            float value = sampleValue.AsFloat(min, max);
            
            if (max <= min)
            {
                Assert.AreEqual(value, min, NoiseValueDebugString(sampleValue));
                return;
            }
            
            Assert.GreaterOrEqual(value, min, NoiseValueDebugString(sampleValue));
            Assert.Less(value, max, NoiseValueDebugString(sampleValue));
        }

        private static void CheckValues(Action<NoiseValue> checkAction)
        {
            for (uint seed = 0; seed < 10; seed++)
            for (int position = -100; position <= 100; position++)
            {
                NoiseValue sampleValue = NoiseUtility.SampleValue(position, seed);
                checkAction(sampleValue);
            }

            checkAction(new NoiseValue(0));
            checkAction(new NoiseValue(1));
            checkAction(new NoiseValue(uint.MaxValue));
            checkAction(new NoiseValue(uint.MaxValue - 1));
        }

        private static string NoiseValueDebugString(NoiseValue sampleValue)
        {
            string hex = "0x" + sampleValue.Bits.ToString("X8");
            string binary = "0b" + Convert.ToString(sampleValue.Bits, 2).PadLeft(32, '0');
            return $"{sampleValue.Bits} {hex} {binary}";
        }
    }
}