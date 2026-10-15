using UnityEngine;
using UnityEngine.UI;

namespace Kott.Utilities.UI
{
    public class UpdatableScrollRect : ScrollRect
    {
        /// <inheritdoc cref="SetContentPosition(Vector2)"/>
        public void SetContentPosition(RectTransform.Axis axis, float value)
        {
            Vector2 value2 = content.anchoredPosition;
            value2[(int)axis] = value;
            SetContentPosition(value2);
        }

        /// <summary>
        /// Set content position.
        /// Also works while dragging and during intertia movement.
        /// </summary>
        public void SetContentPosition(Vector2 value)
        {
            Vector2 offset = value - content.anchoredPosition;
            OffsetContentPosition(offset);
        }

        /// <inheritdoc cref="SetContentSize(Vector2)"/>
        public void SetContentSize(RectTransform.Axis axis, float value)
        {
            Vector2 value2 = content.sizeDelta;
            value2[(int)axis] = value;
            SetContentSize(value2);
        }

        /// <summary>
        /// Set content size.
        /// Also works while dragging and during intertia movement.
        /// </summary>
        public void SetContentSize(Vector2 value)
        {
            Vector2 offset = value - content.sizeDelta;
            OffsetContentSize(offset);
        }

        /// <inheritdoc cref="OffsetContentPosition(Vector2)"/>
        public void OffsetContentPosition(RectTransform.Axis axis, float offset)
        {
            Vector2 offset2 = Vector2.zero;
            offset2[(int)axis] = offset;
            OffsetContentPosition(offset2);
        }

        /// <summary>
        /// Offset content position by given offset.
        /// Also works while dragging and during intertia movement.
        /// </summary>
        public void OffsetContentPosition(Vector2 offset)
        {
            content.anchoredPosition += offset;
            m_ContentStartPosition += offset;
        }

        /// <inheritdoc cref="OffsetContentSize(Vector2)"/>
        public void OffsetContentSize(RectTransform.Axis axis, float offset)
        {
            Vector2 offset2 = Vector2.zero;
            offset2[(int)axis] = offset;
            OffsetContentSize(offset2);
        }

        /// <summary>
        /// Offset content size by given offset.
        /// Also works while dragging and during intertia movement.
        /// </summary>
        public void OffsetContentSize(Vector2 offset)
        {
            content.sizeDelta += offset;
        }
    }
}
