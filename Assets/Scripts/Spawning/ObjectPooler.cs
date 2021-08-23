using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public Pool(string tag, GameObject prefab, int size)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.size = size;
        }
        public string tag;
        public GameObject prefab;
        public int size;
        public int steps;
    }

    #region Singleton

    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    Pool GetPoolByTag(string tag)
    {
        foreach (Pool pool in pools)
        {
            if (pool.tag.Equals(tag))
            {
                return pool;
            }
        }
        return null;
    }
    void UpdapdatePoolDictionary(string key)
    {
        Pool pool = GetPoolByTag(key);
        Debug.Log("----------------------Update Pool Dictionary : " + pool.tag);


        if (pool != null)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
            poolDictionary[key] = objectPool;
        }
    }

    /*public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        if (poolDictionary[tag].Count == 0)
        {
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        //poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;

    }*/

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool active)
    {
        //UpdapdatePoolDictionary(tag);
        Debug.Log("----------------------SpawnFromPool dictionary count : " + poolDictionary.Count);

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        if (poolDictionary[tag].Count == 0)
        {
            return null;
        }
        /*switch (fillAmount)
        {
            case 2: poolDictionary[tag].Dequeue(); break;
            case 1: poolDictionary[tag].Dequeue(); poolDictionary[tag].Dequeue(); break;
            case 0: poolDictionary[tag].Clear(); break;
            default: break;
        }*/
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(active);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;

    }

    public Pool getPoolByTag(string tag)
    {
        Pool returnPool = null;
        foreach (Pool pool in pools)
        {
            if (pool.tag.Equals(tag))
            {
                returnPool = pool;
            }
        }
        return returnPool;
    }

}
