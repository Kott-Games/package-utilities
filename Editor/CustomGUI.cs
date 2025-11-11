using UnityEngine;

namespace Kott.Utilities
{
    public static class CustomGUI
    {
        private static Texture2D _whiteTexture;
        private static GUIStyle _whiteBoxStyle;

        public static void DrawBox(Rect position, Color color)
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            if (_whiteBoxStyle == null)
            {
                _whiteBoxStyle = new GUIStyle(GUIStyle.none);
                _whiteBoxStyle.normal.background = _whiteTexture;
            }

            Color guiBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(position, GUIContent.none, _whiteBoxStyle);
            GUI.backgroundColor = guiBackgroundColor;
        }
    }
}