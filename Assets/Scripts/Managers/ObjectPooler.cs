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

    [Header("Boss Bullet Custom Materials")]
    [Tooltip("Asigna exactamente 6 materiales, uno para cada nivel (Nivel 1 al 6/Secreto)")]
    public Material[] bossBulletMaterials;

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

        // Apply custom level-based material if it's an enemy bullet
        if (tag == "EnemyBullet" && bossBulletMaterials != null && bossBulletMaterials.Length > 0)
        {
            int levelMatIndex = GetCurrentLevelMaterialIndex();
            if (levelMatIndex >= 0 && levelMatIndex < bossBulletMaterials.Length)
            {
                Material targetMat = bossBulletMaterials[levelMatIndex];
                if (targetMat != null)
                {
                    SpriteRenderer sr = objectToSpawn.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.material = targetMat;
                    }
                    else
                    {
                        Renderer r = objectToSpawn.GetComponent<Renderer>();
                        if (r != null)
                        {
                            r.material = targetMat;
                        }
                    }
                }
            }
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    private int GetCurrentLevelMaterialIndex()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
        {
            if (int.TryParse(sceneName.Replace("Level", ""), out int levelNum))
            {
                return Mathf.Clamp(levelNum - 1, 0, 5);
            }
        }
        else if (sceneName == "SecretLevel")
        {
            return 5; // Secret level gets the 6th material (index 5)
        }
        return 0; // Default fallback
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
