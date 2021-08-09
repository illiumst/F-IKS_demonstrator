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

    public Trash(Vector3 position, int size)
    {
        this.size = size;
        this.position = position;
    }

    // Start is called before the first frame update
    void Start()
    {
        var trashBoundary = Resources.Load("Prefabs/TrashBoundary");
        TrashBoundarySpawnObject = trashBoundary as GameObject;
        objectPooler = ObjectPooler.Instance;

        var boundarySize = 2f * (float)size / Mathf.PI;
        var boundaryPosition = new Vector3((position.x + 0.5f * boundarySize), position.y, (position.z + 0.5f * boundarySize));
        var newTrashBoundary = Instantiate(TrashBoundarySpawnObject, boundaryPosition, transform.rotation) as GameObject;
        newTrashBoundary.transform.localScale = new Vector3(boundarySize, boundarySize, boundarySize);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frames++;
        //only spawn every 15 frames
        if (frames % 15 == 0)
        {
            spawnTrashCubePiece(new Vector3(2, 0, 3), 2);
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
}
