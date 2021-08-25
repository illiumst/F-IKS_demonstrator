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
    ObjectPooler objectPooler;

    List<GameObject> trashList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        var robot = Resources.Load("Prefabs/Robot");
        var trashBoundary = Resources.Load("Prefabs/TrashBoundary");
        var listItem = Resources.Load("Prefabs/AgentListItem");

        RobotSpawnObject = robot as GameObject;
        TrashBoundarySpawnObject = trashBoundary as GameObject;
        AgentListItem = listItem as GameObject;

        spawnObject(RobotSpawnObject, new Vector3(RobotSpawnObject.transform.position.x, 0.5f, RobotSpawnObject.transform.position.z));
        //spawnTrashObject(new Vector3(2, 0, 3), 2);
        //spawnTrashObject(new Vector3(-10, 0, 3), 1);
        //spawnTrashObject(new Vector3(-2, 0, -2), 2);
        //spawnTrashObject(new Vector3(-6, 0, -12), 3);




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
        }
    }

    public void spawnTrashObject(Vector3 trashPosition, int size)
    {
        var boundarySize = 2f * (float)size / Mathf.PI;
        var newTrashBoundary = Instantiate(TrashBoundarySpawnObject, trashPosition, transform.rotation) as GameObject;
        trashList.Add(newTrashBoundary);
        newTrashBoundary.name = "Trash" + trashList.Count;
        newTrashBoundary.GetComponent<Trash>().setSize(size);
        newTrashBoundary.GetComponent<Trash>().setIndex(trashList.Count);
        newTrashBoundary.transform.localScale = new Vector3(boundarySize, boundarySize, boundarySize);

    }
}
