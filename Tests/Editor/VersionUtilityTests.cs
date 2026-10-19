using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kott.Utilities.Tests
{
    public class VersionUtilityTests
    {
        [Test]
        public void UniqueVersionString()
        {
            HashSet<string> versionStrings = new();

            const int majorCount = 10;
            const int minorCount = 20;
            const int buildCount = 50;

            for (int majorVersion = 0; majorVersion < majorCount; majorVersion++)
            for (int minorVersion = 0; minorVersion < minorCount; minorVersion++)
            for (int buildVersion = 0; buildVersion < buildCount; buildVersion++)
            {
                Version version = new(majorVersion, minorVersion, buildVersion);
                string versionString = VersionUtility.GetPseudoRandomVersionString(version);

                Assert.IsTrue(versionStrings.Add(versionString),
                    $"version: {version}, versionString: {versionString}");
            }
        }
    }
}
