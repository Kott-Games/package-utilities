using System;
using NUnit.Framework;
using UnityEngine;

namespace Kott.Utilities.Tests
{
    public class NoiseTests
    {
        [Test]
        public void AsUInt()
        {
            CheckerAsUInt checker = new();
            uint[] rangeValues =
            {
                0u,
                1u,
                2u,
                4u,
                7u,
                0xFFFF0000u,
                0xFFFF0001u,
                0x80000001u,
                0xFFFFFFFEu,
                0xFFFFFFFFu
            };

            foreach (uint value in rangeValues)
            {
                checker.Init(value);
                CheckValues(checker);
                checker.Log();
            }

            foreach (uint valueA in rangeValues)
            foreach (uint valueB in rangeValues)
            {
                checker.Init(valueA, valueB);
                CheckValues(checker);
                checker.Log();
            }
        }
        
        [Test]
        public void AsInt()
        {
            CheckerAsInt checker = new();
            int[] rangeValues =
            {
                0,
                -1,
                -4,
                1,
                4,
                int.MinValue,
                int.MinValue + 1,
                int.MaxValue,
                int.MaxValue - 1,
            };

            foreach (int value in rangeValues)
            {
                checker.Init(value);
                CheckValues(checker);
                checker.Log();
            }

            foreach (int valueA in rangeValues)
            foreach (int valueB in rangeValues)
            {
                checker.Init(valueA, valueB);
                CheckValues(checker);
                checker.Log();
            }
        }
        
        [Test]
        public void AsFloat()
        {
            CheckerAsFloat checker = new();
            float[] rangeValues =
            {
                0f,
                -1.23456789f,
                -12345.6789f,
                -123456789f,
                1.23456789f,
                12345.6789f,
                123456789f,
            };
            
            checker.Init();
            CheckValues(checker);
            checker.Log();

            foreach (float value in rangeValues)
            {
                checker.Init(value);
                CheckValues(checker);
                checker.Log();
            }

            foreach (float valueA in rangeValues)
            foreach (float valueB in rangeValues)
            {
                checker.Init(valueA, valueB);
                CheckValues(checker);
                checker.Log();
            }
        }

        private static void CheckValues(IChecker checker)
        {
            for (uint seed = 0; seed < 10; seed++)
            for (int position = -100; position <= 100; position++)
            {
                NoiseValue sampleValue = NoiseUtility.SampleValue(position, seed);
                checker.Check(sampleValue);
            }

            checker.Check(new NoiseValue(0));
            checker.Check(new NoiseValue(1));
            checker.Check(new NoiseValue(uint.MaxValue));
            checker.Check(new NoiseValue(uint.MaxValue - 1));
        }

        private static string NoiseValueDebugString(NoiseValue sampleValue)
        {
            string hex = "0x" + sampleValue.Bits.ToString("X8");
            string binary = "0b" + Convert.ToString(sampleValue.Bits, 2).PadLeft(32, '0');
            return $"{sampleValue.Bits} {hex} {binary}";
        }

        private interface IChecker
        {
            void Check(NoiseValue sampleValue);
        }

        private class CheckerAsUInt : IChecker
        {
            private uint _counter;
            private ulong _sum;

            private uint? _min;
            private uint? _max;

            public void Init(uint max)
            {
                _counter = 0;
                _sum = 0;

                _min = null;
                _max = max;
            }

            public void Init(uint min, uint max)
            {
                _counter = 0;
                _sum = 0;

                _min = min;
                _max = max;
            }

            public void Log()
            {
                if (_max == null)
                {
                    throw new NotImplementedException();
                }

                double average = (double)_sum / _counter;

                if (_min == null)
                {
                    Debug.Log($"max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
                    return;
                }

                Debug.Log($"min: {_min.Value}, max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
            }

            public void Check(NoiseValue sampleValue)
            {
                if (_max == null)
                {
                    throw new NotImplementedException();
                }

                if (_min == null)
                {
                    Track(CheckMax(sampleValue, _max.Value));
                    return;
                }

                Track(CheckMinMax(sampleValue, _min.Value, _max.Value));
            }

            private void Track(uint? value)
            {
                if (value == null)
                {
                    return;
                }

                _counter++;
                _sum += value.Value;
            }

            private static uint? CheckMax(NoiseValue sampleValue, uint max)
            {
                uint value = sampleValue.AsUInt(max);

                if (max <= 0u)
                {
                    Assert.AreEqual(value, 0u, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, 0u, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }

            private static uint? CheckMinMax(NoiseValue sampleValue, uint min, uint max)
            {
                uint value = sampleValue.AsUInt(min, max);

                if (max <= min)
                {
                    Assert.AreEqual(value, min, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, min, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }
        }
        
        private class CheckerAsInt : IChecker
        {
            private uint _counter;
            private long _sum;

            private int? _min;
            private int? _max;

            public void Init(int max)
            {
                _counter = 0;
                _sum = 0;

                _min = null;
                _max = max;
            }

            public void Init(int min, int max)
            {
                _counter = 0;
                _sum = 0;

                _min = min;
                _max = max;
            }

            public void Log()
            {
                if (_max == null)
                {
                    throw new NotImplementedException();
                }

                double average = (double)_sum / _counter;

                if (_min == null)
                {
                    Debug.Log($"max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
                    return;
                }

                Debug.Log($"min: {_min.Value}, max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
            }

            public void Check(NoiseValue sampleValue)
            {
                if (_max == null)
                {
                    throw new NotImplementedException();
                }

                if (_min == null)
                {
                    Track(CheckMax(sampleValue, _max.Value));
                    return;
                }

                Track(CheckMinMax(sampleValue, _min.Value, _max.Value));
            }

            private void Track(int? value)
            {
                if (value == null)
                {
                    return;
                }

                _counter++;
                _sum += value.Value;
            }

            private static int? CheckMax(NoiseValue sampleValue, int max)
            {
                int value = sampleValue.AsInt(max);

                if (max <= 0u)
                {
                    Assert.AreEqual(value, 0, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, 0, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }

            private static int? CheckMinMax(NoiseValue sampleValue, int min, int max)
            {
                int value = sampleValue.AsInt(min, max);

                if (max <= min)
                {
                    Assert.AreEqual(value, min, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, min, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }
        }
        
        private class CheckerAsFloat : IChecker
        {
            private uint _counter;
            private double _sum;

            private float? _min;
            private float? _max;

            public void Init()
            {
                _counter = 0;
                _sum = 0;

                _min = null;
                _max = null;
            }
            
            public void Init(float max)
            {
                _counter = 0;
                _sum = 0;

                _min = null;
                _max = max;
            }

            public void Init(float min, float max)
            {
                _counter = 0;
                _sum = 0;

                _min = min;
                _max = max;
            }

            public void Log()
            {
                double average = (double)_sum / _counter;
                
                if (_max == null)
                {
                    Debug.Log($"01 - count: {_counter}, sum: {_sum}, average: {average}");
                    return;
                }

                if (_min == null)
                {
                    Debug.Log($"max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
                    return;
                }

                {
                    Debug.Log($"min: {_min.Value}, max: {_max.Value} - count: {_counter}, sum: {_sum}, average: {average}");
                }
            }

            public void Check(NoiseValue sampleValue)
            {
                float value = sampleValue.AsFloat01();
                
                if (_max == null)
                {
                    Track(Check01(sampleValue));
                    return;
                }

                if (_min == null)
                {
                    Track(CheckMax(sampleValue, _max.Value));
                    return;
                }

                Track(CheckMinMax(sampleValue, _min.Value, _max.Value));
            }

            private void Track(float? value)
            {
                if (value == null)
                {
                    return;
                }

                _counter++;
                _sum += value.Value;
            }

            private static float? Check01(NoiseValue sampleValue)
            {
                float value = sampleValue.AsFloat01();
                
                Assert.GreaterOrEqual(value, 0f, NoiseValueDebugString(sampleValue));
                Assert.Less(value, 1f, NoiseValueDebugString(sampleValue));
                
                return value;
            }

            private static float? CheckMax(NoiseValue sampleValue, float max)
            {
                float value = sampleValue.AsFloat(max);

                if (max <= 0f)
                {
                    Assert.AreEqual(value, 0f, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, 0f, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }

            private static float? CheckMinMax(NoiseValue sampleValue, float min, float max)
            {
                float value = sampleValue.AsFloat(min, max);

                if (max <= min)
                {
                    Assert.AreEqual(value, min, NoiseValueDebugString(sampleValue));
                    return value;
                }

                Assert.GreaterOrEqual(value, min, NoiseValueDebugString(sampleValue));
                Assert.Less(value, max, NoiseValueDebugString(sampleValue));

                return value;
            }
        }
    }
}