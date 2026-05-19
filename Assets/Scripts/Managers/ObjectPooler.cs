using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Can be DontDestroyOnLoad if managers are in a persistent scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                // Parent it to this manager to keep hierarchy clean
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // If the object is still active on screen, the pool is full. We expand it dynamically.
        if (objectToSpawn.activeInHierarchy)
        {
            poolDictionary[tag].Enqueue(objectToSpawn); // Put the active one back
            
            Pool currentPool = pools.Find(p => p.tag == tag);
            if (currentPool != null)
            {
                objectToSpawn = Instantiate(currentPool.prefab);
                objectToSpawn.transform.SetParent(transform);
            }
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void DeactivateAllObjects()
    {
        if (poolDictionary == null) return;
        foreach (var poolQueue in poolDictionary.Values)
        {
            foreach (var obj in poolQueue)
            {
                if (obj != null && obj.activeInHierarchy)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
