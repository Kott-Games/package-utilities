using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Kott.Utilities.JsonConverters
{
    public class BoundsIntConverter : JsonConverter<BoundsInt>
    {
        public override bool CanRead => false;

        public override BoundsInt ReadJson(JsonReader reader, Type objectType, BoundsInt existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, BoundsInt value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("center");
            serializer.Serialize(writer, value.center, typeof(Vector3Int));
            writer.WritePropertyName("size");
            serializer.Serialize(writer, value.size, typeof(Vector3Int));
            writer.WriteEndObject();
        }
    }
}
