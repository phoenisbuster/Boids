using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.Common
{
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
    
    [Serializable]
    public class MyDictionary<TKey, TValue>
    {
        readonly List<KeyValuePair<TKey, TValue>> data = new();
        public int Count
        {
            get { return data.Count; }
        }

        public KeyValuePair<TKey, TValue> ElementAt(int index)
        {
            return data[index];
        }

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

        public void TryGetValue(TKey key, out TValue value)
        {
            KeyValuePair<TKey, TValue> element;
            for (int i = 0; i < data.Count; i++)
            {
                element = data[i];
                if (key.ToString() == element.Key.ToString())
                {
                    value = element.Value;
                    return;
                }
            }
            value = default;
        }

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
        }

        public void Clear()
        {
            data.Clear();
        }
    }
}
