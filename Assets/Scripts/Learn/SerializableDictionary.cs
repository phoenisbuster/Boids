using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this)
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
            }
            if(i >= values.Count)
            {
                values.Add(default);
            }
            
            Add(keys[i], values[i]);
    
        }
    }
}
