using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour
{

    GameObject RobotSpawnObject;
    GameObject DirtSpawnObject;
    GameObject WallSpawnObject;
    GameObject AgentListItem;
    public GameObject AgentListContent;

    ObjectPooler objectPooler;

    private int frames = 0;


    // Start is called before the first frame update
    void Start()
    {
        var robot = Resources.Load("Prefabs/Robot");
        var dirt = Resources.Load("Prefabs/Trash");
        var listItem = Resources.Load("Prefabs/AgentListItem");
        //var trash = Resources.Load("Prefabs/TrashCube");

        RobotSpawnObject = robot as GameObject;
        DirtSpawnObject = dirt as GameObject;
        AgentListItem = listItem as GameObject;

        spawnObject(RobotSpawnObject, RobotSpawnObject.transform.position);
        //spawnObject(RobotSpawnObject, new Vector3(-3, 0, 10));

        objectPooler = ObjectPooler.Instance;


    }

    // Update is called once per frame -> used for trash object pooler
    void FixedUpdate()
    {
        frames++;
        if (frames % 15 == 0)
        {
            //spawnTrashCube(this.transform.position, 1);
            //spawnTrashCube(new Vector3(2, 0, 3), 1);

        }
    }

    public void spawnTrashCube(Vector3 trashPosition, int size)
    {
        float xTrans = Random.Range(0, size);
        float zTrans = Random.Range(0, size);

        var position = new Vector3(trashPosition.x + xTrans, .3f, trashPosition.z + zTrans);

        objectPooler.SpawnFromPool("TrashCube", position, Quaternion.identity);

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

}
