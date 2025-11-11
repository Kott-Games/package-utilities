using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kott.Utilities
{
    public static class RectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AddPadding(this Rect position,
            float paddingLeft, float paddingTop, float paddingRight, float paddingBottom)
        {
            position.x += paddingLeft;
            position.y += paddingTop;
            position.width -= paddingLeft + paddingRight;
            position.height -= paddingTop + paddingBottom;
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AddPadding(this Rect position, float padding)
        {
            return AddPadding(position, padding, padding, padding, padding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AddPadding(this Rect position, float paddingHorizontal, float paddingVertical)
        {
            return AddPadding(position, paddingHorizontal, paddingVertical, paddingHorizontal, paddingVertical);
        }

        /// <param name="padding">Coordinates [ X | Y ] map to [ Horizontal | Vertical ]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AddPadding(this Rect position, Vector2 padding)
        {
            return AddPadding(position, padding.x, padding.y, padding.x, padding.y);
        }

        /// <param name="padding">Coordinates [ X | Y | Z | W ] map to [ Left | Top | Right | Bottom ]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect AddPadding(this Rect position, Vector4 padding)
        {
            return AddPadding(position, padding.x, padding.y, padding.z, padding.w);
        }
    }
}
