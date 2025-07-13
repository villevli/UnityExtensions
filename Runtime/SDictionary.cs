using System;
using System.Collections.Generic;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// Dictionary that can be serialized in Unity and displayed in inspector.
    /// Serializes into a list of key value pairs.
    /// </summary>
    [Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct SKeyValuePair
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<SKeyValuePair> _data = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _data.Clear();
            foreach (var kp in this)
            {
                _data.Add(new()
                {
                    Key = kp.Key,
                    Value = kp.Value
                });
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();
            foreach (var skp in _data)
            {
                var key = skp.Key;
                var okey = key;
                // Auto increment the key when serialized data contains a duplicate key (e.g. when adding an item in inspector)
                while (!TryAdd(key, skp.Value)
                     && TryIncrement(ref key)
                     // Some types like enum may wrap around, halt in that case
                     && !EqualityComparer<TKey>.Default.Equals(key, okey)) { }
            }
        }

        private static bool TryIncrement(ref TKey key)
        {
            if (key is string str)
            {
                key = (TKey)(object)(str + (str.Length > 0 ? str[^1] : 'a'));
                return true;
            }
            if (key is Enum e)
            {
                // TODO: Optimize by caching enum values per type
                var values = (TKey[])Enum.GetValues(typeof(TKey));
                TKey k = key;
                int idx = Array.FindIndex(values, x => EqualityComparer<TKey>.Default.Equals(x, k));
                if (idx != -1)
                    key = values[(idx + 1) % values.Length];
                else
                    key = (TKey)Enum.ToObject(typeof(TKey), Convert.ToInt32(e) + 1);
                return true;
            }
            if (key is byte b)
            {
                key = (TKey)(object)(byte)(b + 1);
                return true;
            }
            if (key is sbyte sb)
            {
                key = (TKey)(object)(sbyte)(sb + 1);
                return true;
            }
            if (key is char c)
            {
                key = (TKey)(object)(char)(c + 1);
                return true;
            }
            if (key is short s)
            {
                key = (TKey)(object)(short)(s + 1);
                return true;
            }
            if (key is ushort us)
            {
                key = (TKey)(object)(ushort)(us + 1);
                return true;
            }
            if (key is int i)
            {
                key = (TKey)(object)(i + 1);
                return true;
            }
            if (key is uint ui)
            {
                key = (TKey)(object)(ui + 1);
                return true;
            }
            if (key is long l)
            {
                key = (TKey)(object)(l + 1);
                return true;
            }
            if (key is ulong ul)
            {
                key = (TKey)(object)(ul + 1);
                return true;
            }

            return false;
        }
    }
}
