using System;
using System.Collections.Generic;
using Kott.Utilities.JsonConverters;
using Newtonsoft.Json;
using UnityEngine;

namespace Kott.Utilities
{
    [Serializable]
    [JsonConverter(typeof(DictionaryArrayConverter))]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Pair
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<Pair> _pairs = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_pairs == null) return;

            this.Clear();

            foreach (Pair pair in _pairs)
            {
                if (pair.Key == null) continue;
                this[pair.Key] = pair.Value;
            }
        }
    }
}
