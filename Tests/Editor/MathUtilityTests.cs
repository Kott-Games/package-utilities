using NUnit.Framework;

namespace Kott.Utilities.Tests
{
    public class MathUtilityTests
    {
        [Test]
        public void TransformUniformToNormal01_OneSigmaQuantile_IsMeanPlusOneStdDev()
        {
            // regression: mean and standard deviation were ignored, every input returned 0.5
            Assert.That(MathUtility.TransformUniformToNormal01(0.8413447), Is.EqualTo(0.5 + 0.5 / 3).Within(1e-3));
        }
    }
}
