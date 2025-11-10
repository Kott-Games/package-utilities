using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
    public class HackedScriptAttributeUtility
    {
        public class PropertyInfo
        {
            public Type Type;
            public FieldInfo Field;
            public PropertyAttribute[] PropertyAttributes;
        }

        private static Dictionary<int, PropertyInfo> _cachedPropertyData = new();

        public static PropertyInfo GetPropertyInfo(SerializedProperty property)
        {
            int hash = GetPropertyHash(property);
            if (!_cachedPropertyData.TryGetValue(hash, out PropertyInfo info))
            {
                info = new PropertyInfo();
                info.Field = ScriptAttributeUtility.GetFieldInfoFromProperty(property, out info.Type);
                info.PropertyAttributes = GetFieldAttributes(info.Field);
                _cachedPropertyData.Add(hash, info);
            }
            return info;
        }

        private static PropertyAttribute[] GetFieldAttributes(FieldInfo field)
        {
            if (field == null)
                return null;

            PropertyAttribute[] array = field.GetCustomAttributes<PropertyAttribute>(inherit: true).ToArray();
            Comparer<PropertyAttribute> comparer = Comparer<PropertyAttribute>
                .Create((PropertyAttribute p1, PropertyAttribute p2) => p1.order.CompareTo(p2.order));
            Array.Sort(array, comparer);

            return array;
        }

        private static int GetPropertyHash(SerializedProperty property)
        {
            if (property.serializedObject.targetObject == null)
            {
                return 0;
            }

            int num = property.serializedObject.targetObject.GetInstanceID() ^ property.hashCodeForPropertyPathWithoutArrayIndex;
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                num ^= property.objectReferenceInstanceIDValue;
            }

            return num;
        }
    }
}
