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

    Quaternion wallRotation;

    GameObject system;


    List<GameObject> trashList = new List<GameObject>();
    List<GameObject> wallList = new List<GameObject>();

    int tempMaxWallX = 0;
    int tempMaxWallY = 0;



    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        system = GameObject.FindWithTag("System");
        var robot = Resources.Load("Prefabs/Robot");
        var trashBoundary = Resources.Load("Prefabs/TrashBoundary");
        var listItem = Resources.Load("Prefabs/AgentListItem");
        var wallPiece = Resources.Load("Prefabs/Walls/Wall3");
        wallRotation = transform.rotation;

        RobotSpawnObject = robot as GameObject;
        TrashBoundarySpawnObject = trashBoundary as GameObject;
        AgentListItem = listItem as GameObject;
        WallSpawnObject = wallPiece as GameObject;

        //spawnObject(RobotSpawnObject, new Vector3(RobotSpawnObject.transform.position.x, 0.5f, RobotSpawnObject.transform.position.z));
        //spawnWalls();
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

    /* public void spawnWalls()
     {
         List<Wall> walls = system.GetComponent<EnvironmentState>().environmentConstants.walls;
         foreach (Wall wall in walls)
         {
             Debug.Log("------Wallpiece: x: " + wall.x + " y: " + wall.y);
             wallRotation = Quaternion.Euler(0, 0, 0);


             if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
             {
                 Debug.Log("------Wall1");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
             }
             else if (!CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
             {
                 Debug.Log("------Wall1 top hit");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
             }
             else if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
             {
                 Debug.Log("------Wall1 bottom hit");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
             {
                 Debug.Log("------Wall2");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 90, 0);
             }
             else if (!CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
             {
                 Debug.Log("------Wall2 right hit");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 90, 0);
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
             {
                 Debug.Log("------Wall2 left hit");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 90, 0);
             }
             else if (CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
             {
                 //Debug.Log("------Wall3");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 90, 0);

             }
             else if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
             {
                 //Debug.Log("------Wall4");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                 WallSpawnObject = wallPiece as GameObject;
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y + 1)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
             {
                 //Debug.Log("------Wall5");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, -90, 0);

             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
             {
                 //Debug.Log("------Wall6");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 180, 0);

             }
             else if (CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
             {
                 //Debug.Log("------Wall7");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 180, 0);

             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
             {
                 //Debug.Log("------Wall8");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                 WallSpawnObject = wallPiece as GameObject;
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
             {
                 //Debug.Log("------Wall9");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, 90, 0);
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
             {
                 //Debug.Log("------Wall10");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                 WallSpawnObject = wallPiece as GameObject;
                 wallRotation = Quaternion.Euler(0, -90, 0);
             }
             else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
             {
                 //Debug.Log("------Wall11");
                 var wallPiece = Resources.Load("Prefabs/Walls/Wall3");
                 WallSpawnObject = wallPiece as GameObject;
             }
             var newWallPiece = Instantiate(WallSpawnObject, new Vector3(wall.x, 2f, wall.y), wallRotation) as GameObject;
             wallList.Add(newWallPiece);
         }
         foreach (GameObject wall in wallList)
         {
             Debug.Log("------Converting Wall coordinates...");
             wall.transform.position = new Vector3(wall.transform.position.x - GetWallCenter().x,
                 wall.transform.position.y, wall.transform.position.z - GetWallCenter().z);
         }
     }

    public Vector3 GetWallCenter()
    {
        List<Wall> walls = system.GetComponent<EnvironmentState>().environmentConstants.walls;

        foreach (Wall wall in walls)
        {
            if (wall.x > tempMaxWallX)
            {
                tempMaxWallX = wall.x;
            }
            if (wall.y > tempMaxWallY)
            {
                tempMaxWallY = wall.y;
            }
        }
        return new Vector3(tempMaxWallX / 2, 0f, tempMaxWallY / 2);
    }
    public bool CheckIfWallExistsAtPosition(int x, int y)
    {
        List<Wall> walls = system.GetComponent<EnvironmentState>().environmentConstants.walls;
        foreach (Wall wall in walls)
        {
            if (wall.x == x && wall.y == y)
            {
                return true;
            }
        }
        return false;
    }*/
}
