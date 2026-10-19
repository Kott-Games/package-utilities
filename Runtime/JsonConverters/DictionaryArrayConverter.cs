using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Kott.Utilities.JsonConverters
{
    public class DictionaryArrayConverter : JsonConverter<IDictionary>
    {
        public override IDictionary ReadJson(JsonReader reader, Type objectType, IDictionary existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            existingValue ??= (IDictionary)Activator.CreateInstance(objectType);
            existingValue.Clear();

            Type genericType = Array.Find(
                objectType.GetInterfaces(),
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (genericType != null)
            {
                Type[] genericArguments = genericType.GetGenericArguments();
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];

                Type itemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
                Type listType = typeof(List<>).MakeGenericType(itemType);

                PropertyInfo keyProperty = itemType.GetProperty("Key");
                PropertyInfo valueProperty = itemType.GetProperty("Value");

                IList list = (IList)serializer.Deserialize(reader, listType);
                foreach (object item in list)
                {
                    object key = keyProperty.GetValue(item);
                    object value = valueProperty.GetValue(item);
                    existingValue.Add(key, value);
                }
            }
            else
            {
                List<KeyValuePair<object, object>> list =
                    serializer.Deserialize<List<KeyValuePair<object, object>>>(reader);
                foreach (KeyValuePair<object, object> item in list)
                    existingValue.Add(item.Key, item.Value);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, IDictionary value, JsonSerializer serializer)
        {
            List<object> list = new();
            foreach (object item in (IEnumerable)value)
                list.Add(item);
            serializer.Serialize(writer, list);
        }
    }
}
