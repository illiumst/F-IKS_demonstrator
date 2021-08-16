using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{

    private int size;
    private Vector3 position;

    GameObject TrashBoundarySpawnObject;

    ObjectPooler objectPooler;
    private int frames = 0;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        var prefab = Resources.Load("Prefabs/BoltM6") as GameObject;
        //index = GetComponent<ObjectSpawner>().GetTrashList().Count;
        objectPooler.pools.Add(new ObjectPooler.Pool(CreateTrashTag(), prefab, size * 50));

        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < objectPooler.pools[objectPooler.pools.Count - 1].size; i++)
        {
            GameObject obj = Instantiate(objectPooler.pools[objectPooler.pools.Count - 1].prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
        objectPooler.poolDictionary.Add(objectPooler.pools[objectPooler.pools.Count - 1].tag, objectPool);

        this.position = transform.position;
        Bounds bounds = getBounds(gameObject);
        transform.position = new Vector3(position.x + 0.5f * bounds.extents.x, position.y, position.z + 0.5f * bounds.extents.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frames++;
        //only spawn every 15 frames
        if (frames % 15 == 0)
        {
            spawnTrashCubePiece(position, size);
        }
    }

    public void spawnTrashCubePiece(Vector3 trashPosition, int size)
    {
        float xTrans = Random.Range(0, 2f * (float)size / Mathf.PI);
        float zTrans = Random.Range(0, 2f * (float)size / Mathf.PI);

        var position = new Vector3(trashPosition.x + xTrans, .3f, trashPosition.z + zTrans);
        var boundarySize = 2f * (float)size / Mathf.PI;
        var boundaryPosition = new Vector3((trashPosition.x + 0.5f * boundarySize), trashPosition.y, (trashPosition.z + 0.5f * boundarySize));

        objectPooler.SpawnFromPool("Trash" + index, position, Quaternion.identity);
    }

    Bounds getBounds(GameObject objeto)
    {
        Bounds bounds;
        Renderer childRender;
        bounds = getRenderBounds(objeto);
        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(objeto.transform.position, Vector3.zero);
            foreach (Transform child in objeto.transform)
            {
                childRender = child.GetComponent<Renderer>();
                if (childRender)
                {
                    bounds.Encapsulate(childRender.bounds);
                }
                else
                {
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }
    Bounds getRenderBounds(GameObject objeto)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if (render != null)
        {
            return render.bounds;
        }
        return bounds;
    }

    public string CreateTrashTag()
    {
        string tag = "Trash";
        tag += index;
        return tag;
    }

    public Vector3 getPosition()
    {
        return this.position;
    }

    public int getSize()
    {
        return this.size;
    }

    public void setSize(int size)
    {
        this.size = size;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }
}
