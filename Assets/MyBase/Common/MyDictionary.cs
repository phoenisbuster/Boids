using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.Common
{
    #region My Custom Dictionary
    [Serializable]
    public class MyDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new();
        [SerializeField] private List<TValue> values = new();
        
        readonly List<KeyValuePair<TKey, TValue>> data = new();
        public int Count
        {
            get { return data.Count; }
        }

        /// <summary>
        /// Returns the pair of key and value at the specified index, base on the order of insertion.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KeyValuePair<TKey, TValue> ElementAt(int index)
        {
            return data[index];
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;

                throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");
            }
            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Returns true if the dictionary contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            KeyValuePair<TKey, TValue> element;
            for (int i = 0; i < data.Count; i++)
            {
                element = data[i];
                if (key.ToString() == element.Key.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the value associated with the specified key, or the default value if the key is not found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool TryGetValue(TKey key, out TValue value)
        {
            KeyValuePair<TKey, TValue> element;
            for (int i = 0; i < data.Count; i++)
            {
                element = data[i];
                if (key.ToString() == element.Key.ToString())
                {
                    value = element.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Returns the value associated with the specified key, or the default value if the key is not found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TValue GetValue(TKey key, TValue defaultValue = default)
        {
            KeyValuePair<TKey, TValue> element;
            for (int i = 0; i < data.Count; i++)
            {
                element = data[i];
                if (key.ToString() == element.Key.ToString())
                {
                    TValue value = element.Value;
                    return value;
                }
            }
            return defaultValue;
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                Debug.LogError($"Fail to add key {key} to dictionary, key already exists");
                return;
            }
            data.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Remove(TKey key)
        {
            KeyValuePair<TKey, TValue> element;
            int removeIdx = -1;
            for (int i = 0; i < data.Count; i++)
            {
                element = data[i];
                if(key.ToString() == element.Key.ToString())
                {
                    removeIdx = i;
                    break;
                }
            }

            if(removeIdx >= 0)
            {
                data.RemoveAt(removeIdx);
            }
            else
            {
                Debug.LogWarning($"Fail to remove key {key} from dictionary, key not found");
            }
        }

        public void Clear()
        {
            data.Clear();
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach(KeyValuePair<TKey, TValue> pair in data)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            if(keys.Count != values.Count)
            {
                Debug.LogError($"Fail to deserialize a SerialiableDictionary, {keys.Count}, {values.Count}");
            }
            
            int max = Mathf.Max(keys.Count, values.Count);
            for (int i = 0; i < max; i++)
            {
                if(i >= keys.Count)
                {
                    keys.Add(default);
                    Debug.LogWarning("Add default key");
                }
                if(i >= values.Count)
                {
                    values.Add(default);
                    Debug.LogWarning("Add default value");
                }
                Debug.Log($"key: {keys[i]}, value: {values[i]}");
                Add(keys[i], values[i]);
            }
        }
    }
    #endregion

    #region My Key
    [Serializable]
    public abstract class MyKey
    {
        public static bool operator ==(MyKey a, MyKey b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MyKey a, MyKey b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MyKey other = (MyKey)obj;
            return ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        
        public abstract override string ToString();
    }
    #endregion
}
