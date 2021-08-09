using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{

    GameObject RobotSpawnObject;
    GameObject TrashBoundarySpawnObject;
    GameObject WallSpawnObject;
    GameObject AgentListItem;
    public GameObject AgentListContent;

    List<GameObject> TrashList = new List<GameObject>();

    ObjectPooler objectPooler;

    private int frames = 0;


    // Start is called before the first frame update
    void Start()
    {
        var robot = Resources.Load("Prefabs/Robot");
        var trashBoundary = Resources.Load("Prefabs/TrashBoundary");
        var listItem = Resources.Load("Prefabs/AgentListItem");
        //var trash = Resources.Load("Prefabs/TrashCube");

        RobotSpawnObject = robot as GameObject;
        TrashBoundarySpawnObject = trashBoundary as GameObject;
        AgentListItem = listItem as GameObject;

        spawnObject(RobotSpawnObject, new Vector3(RobotSpawnObject.transform.position.x, 0.5f, RobotSpawnObject.transform.position.z));
        spawnTrashObject(new Vector3(2, 0, 3), 2);
        //spawnObject(RobotSpawnObject, new Vector3(-3, 0, 10));

        objectPooler = ObjectPooler.Instance;


    }

    // Update is called once per frame -> used for trash object pooler
    void FixedUpdate()
    {
        frames++;
        //only spawn every 15 frames
        if (frames % 15 == 0)
        {
            //spawnTrashCube(this.transform.position, 1);
            //spawnTrashCubePiece(new Vector3(2, 0, 3), 2);

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
        var newTrashBoundary = Instantiate(TrashBoundarySpawnObject, boundaryPosition, transform.rotation) as GameObject;
        //var newTrashBoundary = Instantiate(TrashBoundarySpawnObject, trashPosition, transform.rotation) as GameObject;
        newTrashBoundary.transform.localScale = new Vector3(boundarySize, boundarySize, boundarySize);
    }

    public void spawnObject(GameObject objectToSpawn, Vector3 SpawnPosition)
    {

        if (objectToSpawn.tag == "Agent")
        {
            var newRobot = Instantiate(objectToSpawn, SpawnPosition, transform.rotation) as GameObject;
            Debug.Log("--------Spawned Robot: " + newRobot.name);
            var newListItem = Instantiate(AgentListItem) as GameObject;
            //AgentListContent.AddComponent(AgentListItem);
            newListItem.transform.SetParent(AgentListContent.transform, false);
            var agentBody = newRobot.transform.GetChild(1).gameObject;
            var itemContent = newListItem.transform.GetChild(1).gameObject;
            agentBody.GetComponent<AgentCollision>().setWarningText(SpawnHelperClass.FindComponentInChildWithTag<Text>(itemContent, "WarningText"));
            agentBody.GetComponent<AgentController>().setPositionText(SpawnHelperClass.FindComponentInChildWithTag<Text>(itemContent, "UpcomingPositionText"));

            Debug.Log("Trying to Instantiate List item....");
        }
    }

    public void spawnTrashObject(Vector3 trashPosition, int size)
    {
        var trash = new Trash(trashPosition, size);
    }

}
