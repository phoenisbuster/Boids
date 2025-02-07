using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePoolManager : MonoBehaviour
{
    [Header("Prefabs")]
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    private Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    
    private static NodePoolManager instance;

    public static NodePoolManager GetInstance()
    {
        return NodePoolManager.instance;
    }

    private void Awake() 
    {
        createInstance();
    }

    private void createInstance()
    {
        if(NodePoolManager.instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        NodePoolManager.instance = this;
    }

    public void addToPool(string key, GameObject prefab)
    {
        if(!prefabs.ContainsKey(key))
        {
            prefabs.Add(key, prefab);
        }
    }

    public void removeFromPool(string key)
    {
        if(pool.ContainsKey(key))
        {
            prefabs.Remove(key);
        }
    }

    public GameObject getFromPool(string key)
    {
        if(prefabs.ContainsKey(key))
        {
            return prefabs[key];
        }
        return null;
    }

    public GameObject createItem(string key)
    {
        if(pool.ContainsKey(key))
        {
            return pool[key][0];
        }
        return null;
    }
}
