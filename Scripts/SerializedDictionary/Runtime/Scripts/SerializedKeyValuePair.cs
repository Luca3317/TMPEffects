using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.SerializedCollections
{
    [System.Serializable]
    internal struct SerializedKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
