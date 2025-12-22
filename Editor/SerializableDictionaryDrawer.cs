using UnityEditor;
using UnityEngine;

namespace Kott.Utilities
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty pairsProperty = property.FindPropertyRelative("_pairs");
            return EditorGUI.GetPropertyHeight(pairsProperty, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty pairsProperty = property.FindPropertyRelative("_pairs");
            EditorGUI.PropertyField(position, pairsProperty, label, true);
        }
    }
}
