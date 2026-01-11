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

        [SerializeField] private bool _hasNullValue;
        [SerializeField] private TValue _nullValue;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // if (_pairs == null) return;
            // _pairs.Clear();
            // if (_hasNullValue)
            // {
            //     _pairs.Add(new Pair { Key = default, Value = _nullValue });
            // }
            // _pairs.AddRange(this.Select(kvp => new Pair { Key = kvp.Key, Value = kvp.Value }));
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_pairs == null) return;

            this.Clear();
            _hasNullValue = false;

            foreach (Pair pair in _pairs)
            {
                if (pair.Key == null)
                {
                    _nullValue = pair.Value;
                    continue;
                }
                this[pair.Key] = pair.Value;
            }
        }
    }
}
