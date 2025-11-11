using System.Runtime.CompilerServices;
using UnityEngine;

namespace Kott.Utilities
{
    public static class RectIntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt AddPadding(this RectInt position,
            int paddingLeft, int paddingTop, int paddingRight, int paddingBottom)
        {
            position.x += paddingLeft;
            position.y += paddingTop;
            position.width -= paddingLeft + paddingRight;
            position.height -= paddingTop + paddingBottom;
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt AddPadding(this RectInt position, int padding)
        {
            return AddPadding(position, padding, padding, padding, padding);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt AddPadding(this RectInt position, int paddingHorizontal, int paddingVertical)
        {
            return AddPadding(position, paddingHorizontal, paddingVertical, paddingHorizontal, paddingVertical);
        }

        /// <param name="padding">Coordinates [ X | Y ] map to [ Horizontal | Vertical ]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt AddPadding(this RectInt position, Vector2Int padding)
        {
            return AddPadding(position, padding.x, padding.y, padding.x, padding.y);
        }

        /// <param name="padding">Coordinates [ X | Y | Width | Height ] map to [ Left | Top | Right | Bottom ]</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt AddPadding(this RectInt position, RectInt padding)
        {
            return AddPadding(position, padding.x, padding.y, padding.width, padding.height);
        }
    }
}
