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

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        this.position = transform.position;
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

        objectPooler.SpawnFromPool("TrashCube", position, Quaternion.identity);
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
}
