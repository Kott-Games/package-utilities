using System;
using UnityEngine;

namespace Kott.Utilities
{
    public static class VersionUtility
    {
        public static Version GetAppVersion()
        {
            string versionString = Application.version;
            if (!Version.TryParse(versionString, out Version version))
            {
                version = new Version(0, 0, 0);
            }

            return version;
        }
        
        public static string GetPseudoRandomVersionString(Version version)
        {
            int versionNumber = (version.Major << 24) + (version.Minor << 12) + version.Build;
            NoiseValue versionNoise = NoiseUtility.SampleValue(versionNumber, 0);
            return versionNoise.Bits.ToString("X8");
        }
    }
}