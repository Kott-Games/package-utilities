using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kott.Utilities
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Pair
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField] private List<Pair> _pairs = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _pairs.Clear();
            _pairs.AddRange(this.Select(kvp => new Pair { Key = kvp.Key, Value = kvp.Value }));
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();
            _pairs.ForEach(pair => this[pair.Key] = pair.Value);
        }
    }
}
