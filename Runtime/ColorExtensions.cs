using UnityEngine;

namespace Kott.Utilities
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new(color.r, color.g, color.b, alpha);
        }
    }
}
