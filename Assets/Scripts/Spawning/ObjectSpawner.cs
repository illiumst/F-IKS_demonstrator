using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ObjectSpawner : MonoBehaviour
{

    GameObject RobotSpawnObject;
    GameObject TrashBoundarySpawnObject;
    GameObject WallSpawnObject;
    GameObject PickupItemSpawnObject;
    GameObject DropOffZoneSpawnObject;
    GameObject DoorObject;
    GameObject positionMarkerObject;
    GameObject AgentListItem;
    public GameObject AgentListContent;
    //ObjectPooler objectPooler;

    [SerializeField] GameObject WallObjects;
    [SerializeField] GameObject DoorObjects;
    [SerializeField] GameObject AgentObjects;
    [SerializeField] GameObject ItemRegister;
    [SerializeField] GameObject ZoneObjects;
    [SerializeField] GameObject DirtRegister;
    [SerializeField] GameObject FloorMarkerRegister;
    [SerializeField] Toggle floorMarkerToggle;


    Quaternion wallRotation;
    Quaternion doorRotation;


    GameObject system;


    List<GameObject> trashList = new List<GameObject>();
    List<GameObject> wallList = new List<GameObject>();
    List<GameObject> doorList = new List<GameObject>();

    int tempMaxWallX = 0;
    int tempMaxWallY = 0;
    Vector3 wallCenter;

    int maxWallX;
    int maxWallY;

    EnvironmentConstants constants;



    // Start is called before the first frame update
    void Start()
    {
        //objectPooler = ObjectPooler.Instance;
        system = GameObject.FindWithTag("System");

        RobotSpawnObject = Resources.Load("Prefabs/Robot") as GameObject;
        TrashBoundarySpawnObject = Resources.Load("Prefabs/TrashBoundary") as GameObject;
        AgentListItem = Resources.Load("Prefabs/AgentListItem") as GameObject;
        DoorObject = Resources.Load("Prefabs/SlideDoor") as GameObject;
        WallSpawnObject = Resources.Load("Prefabs/Walls/Wall3") as GameObject;
        PickupItemSpawnObject = Resources.Load("Prefabs/PickupItem") as GameObject;
        DropOffZoneSpawnObject = Resources.Load("Prefabs/DropOffZone") as GameObject;
        positionMarkerObject = Resources.Load("Prefabs/PositionMarker") as GameObject;
        wallRotation = transform.rotation;


        Debug.Log("Instantiating Object Spawner...");
    }

    private void Update()
    {
        FloorMarkerRegister.SetActive(floorMarkerToggle.isOn);
    }

    public void SpawnNewEpisode(EnvironmentConstants constantsInput, int episode)
    {
        constants = constantsInput;
        Debug.Log("#Walls Spawner Input: " + constantsInput.episodes[0].steps[0].WallTiles.Count);
        Debug.Log("#Walls Spawner: " + constants.episodes[0].steps[0].WallTiles.Count);
        spawnWalls(episode);
        spawnDoors(episode);
        spawnAgents(episode, 0);
        spawnPickUpItems(episode, 0);
        spawnDropOffZones(episode, 0);
        spawnDirtRegister(episode, 0);
        spawnFloorPositionMarkers(episode);
    }

    public void RemoveLastEpisode()
    {
        DestroyObjectsWithTag("Agent");
        DestroyObjectsWithTag("Wall");
        DestroyObjectsWithTag("PickupItem");
        DestroyObjectsWithTag("DropOffZone");
        DestroyObjectsWithTag("Trash");
        DestroyObjectsWithTag("TrashBoundary");
        DestroyObjectsWithTag("AgentListItem");
        DestroyObjectsWithTag("Door");
        wallList.Clear();
        trashList.Clear();
        doorList.Clear();
    }

    void DestroyObjectsWithTag(string tag)
    {

        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag) as GameObject[];
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
    }

    public void spawnObject(GameObject objectToSpawn, Vector3 SpawnPosition, string name)
    {

        if (objectToSpawn.tag == "PickupItem")
        {
            var newPickupItem = Instantiate(objectToSpawn, SpawnPosition, transform.rotation) as GameObject;
            newPickupItem.name = name;
            newPickupItem.transform.parent = ItemRegister.transform;
            system.GetComponent<EnvironmentStateMachine>().itemObjects.Add(newPickupItem);

        }
        if (objectToSpawn.tag == "DropOffZone")
        {
            var newDropOffZone = Instantiate(objectToSpawn, SpawnPosition, transform.rotation) as GameObject;
            newDropOffZone.name = name;
            newDropOffZone.transform.parent = ZoneObjects.transform;
            system.GetComponent<EnvironmentStateMachine>().zoneObjects.Add(newDropOffZone);

        }
    }

    private void spawnFloorPositionMarkers(int episode)
    {
        var tmpMaxWallX = 0;
        var tmpMaxWallY = 0;
        var tmpMinWallX = 0;
        var tmpMinWallY = 0;

        List<Wall> walls = constants.episodes[episode].steps[0].WallTiles;

        foreach (Wall wall in walls)
        {
            if (wall.x > tmpMaxWallX)
            {
                tmpMaxWallX = wall.x;
            }
            if (wall.x < tmpMinWallX)
            {
                tmpMinWallX = wall.x;
            }
            if (wall.y > tmpMaxWallY)
            {
                tmpMaxWallY = wall.y;
            }
            if (wall.y < tmpMinWallY)
            {
                tmpMinWallY = wall.y;
            }
        }

        for (int i = tmpMinWallX; i <= tmpMaxWallX; i++)
        {
            for (int j = tmpMinWallY; j <= tmpMaxWallY; j++)
            {
                var posMarker = Instantiate(positionMarkerObject) as GameObject;
                posMarker.transform.parent = FloorMarkerRegister.transform;
                posMarker.transform.position = GetRecalculatedPosition((float)i, 0f, (float)j);
                var canvas = posMarker.transform.GetChild(0).gameObject;
                var text = canvas.transform.GetChild(0).gameObject;
                text.GetComponent<Text>().text = "x:" + i + " y:" + j;
            }
        }
    }

    public void spawnAgent(Vector3 SpawnPosition, int x, int y, string name, string action, bool valid)
    {
        var newRobot = Instantiate(RobotSpawnObject, SpawnPosition, transform.rotation) as GameObject;
        newRobot.name = name;
        newRobot.transform.parent = AgentObjects.transform;

        var newListItem = Instantiate(AgentListItem) as GameObject;
        newListItem.transform.SetParent(AgentListContent.transform, false);
        var agentBody = newRobot.transform.GetChild(0).gameObject;
        var itemHeader = newListItem.transform.GetChild(0).gameObject;
        var itemHeaderText = itemHeader.transform.GetChild(1).gameObject;
        itemHeaderText.GetComponent<Text>().text = name;

        var itemContent = newListItem.transform.GetChild(1).gameObject;
        var data = itemContent.transform.GetChild(1).gameObject;
        var contentPositionText = data.transform.GetChild(0).gameObject;
        contentPositionText.GetComponent<Text>().text = "x: " + x + " y: " + y;
        var contentActionText = data.transform.GetChild(1).gameObject;
        contentActionText.GetComponent<Text>().text = action;
        var contentValidityText = data.transform.GetChild(2).gameObject;
        string validityString = "invalid";
        var color = Color.red;
        var textColor = Color.red;
        if (valid)
        {
            textColor = new Color32(0, 160, 20, 255);
            color = Color.green;
            validityString = "valid";
        }
        contentValidityText.GetComponent<Text>().text = validityString;
        contentValidityText.GetComponent<Text>().color = textColor;

        var validityLight = agentBody.transform.GetChild(5).gameObject;
        validityLight.GetComponent<Light>().color = color;

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTagCanvas = FindGameObjectInChildWithTag(agentBody.transform.GetChild(0).gameObject, "AgentCanvas");
        var nameTag = FindGameObjectInChildWithTag(nameTagCanvas, "NameTag");
        nameTag.GetComponent<TextMeshProUGUI>().SetText(GetAgentNumberFromNameAsString(name));
        //nameTag.GetComponent<TextMeshProUGUI>().color = textColor;
        system.GetComponent<EnvironmentStateMachine>().agentObjects.Add(newRobot);
        system.GetComponent<EnvironmentStateMachine>().agentListObjects.Add(newListItem);
    }

    string GetAgentNumberFromNameAsString(string name)
    {
        var foundS1 = name.IndexOf("[");
        var nameNew = name.Substring(foundS1 + 1, name.Length - 2 - foundS1);
        return nameNew;
    }

    public void spawnAgents(int episode, int step)
    {
        List<Agent> agents = constants.episodes[episode].steps[step].Agents;
        foreach (Agent agent in agents)
        {
            Vector3 newPos = GetRecalculatedPosition(agent.x, 0.2f, agent.y);
            spawnAgent(newPos, agent.x, agent.y, agent.name, agent.action, agent.valid);

        }
    }

    public void spawnPickUpItems(int episode, int step)
    {
        List<Item> items = constants.episodes[episode].steps[step].ItemRegister;
        foreach (Item item in items)
        {
            spawnObject(PickupItemSpawnObject, GetRecalculatedPosition(item.x, 0.3f, item.y), item.name);
        }
    }
    public void spawnDropOffZones(int episode, int step)
    {
        List<DropOffLocation> zones = constants.episodes[episode].steps[step].DropOffLocations;
        foreach (DropOffLocation zone in zones)
        {
            if (zone != null)
            {
                spawnObject(DropOffZoneSpawnObject, GetRecalculatedPosition(zone.x, 0.05f, zone.y), zone.name);
            }
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
        system.GetComponent<EnvironmentStateMachine>().dirtObjects.Add(newTrashBoundary);

    }

    public void spawnDirt(Vector3 trashPosition, double amount, string name)
    {
        var rand = Random.Range(0, 3);
        var randRot = Random.Range(0, 360);
        var dirtObj = Resources.Load("Prefabs/Puddles/Puddle1") as GameObject;
        switch (rand)
        {
            case 0: dirtObj = Resources.Load("Prefabs/Puddles/Puddle1") as GameObject; break;
            case 1: dirtObj = Resources.Load("Prefabs/Puddles/Puddle2") as GameObject; break;
            case 2: dirtObj = Resources.Load("Prefabs/Puddles/Puddle2") as GameObject; break;
            default: dirtObj = Resources.Load("Prefabs/Puddles/Puddle1") as GameObject; break;
        }
        var dirtSpawn = Instantiate(dirtObj, trashPosition, transform.rotation) as GameObject;
        dirtSpawn.name = name;
        dirtSpawn.transform.parent = DirtRegister.transform;
        var scaleFactor = Mathf.Sqrt((float)amount);
        dirtSpawn.transform.localScale = new Vector3(scaleFactor, 0.001f, scaleFactor);
        dirtSpawn.transform.eulerAngles = new Vector3(0, randRot, 0);
        trashList.Add(dirtSpawn);
        system.GetComponent<EnvironmentStateMachine>().dirtObjects.Add(dirtSpawn);
    }

    /*public void spawnDirtRegisterOld(int episode, int step)
    {
        List<Dirt> dirtPiles = constants.episodes[episode].steps[step].DirtRegister;
        foreach (Dirt dirt in dirtPiles)
        {
            if (dirt != null)
            {
                var pos = GetRecalculatedPosition(dirt.x, 0, dirt.y);
                var size = (int)(dirt.amount * 10);
                //TODO check sizing
                spawnTrashObject(pos, size);

            }
        }
    }*/

    public void spawnDirtRegister(int episode, int step)
    {
        List<Dirt> dirtPiles = constants.episodes[episode].steps[step].DirtRegister;
        foreach (Dirt dirt in dirtPiles)
        {
            if (dirt != null)
            {
                var pos = GetRecalculatedPosition(dirt.x, 0, dirt.y);
                spawnDirt(pos, dirt.amount, dirt.name);

            }
        }
    }

    public void spawnWalls(int episode)
    {
        List<Wall> walls = constants.episodes[episode].steps[0].WallTiles;
        wallCenter = GetWallCenter(walls, episode);

        foreach (Wall wall in walls)
        {
            //.Log("------Wallpiece: x: " + wall.x + " y: " + wall.y);
            wallRotation = Quaternion.Euler(0, 0, 0);

            if (!CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                //Debug.Log("------WallBlock");
                var wallPiece = Resources.Load("Prefabs/Walls/WallBlock");
                WallSpawnObject = wallPiece as GameObject;
            }
            if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                //Debug.Log("------Wall1");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (!CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                //Debug.Log("------Wall1 top hit");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                //Debug.Log("------Wall1 bottom hit");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                //Debug.Log("------Wall2");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (!CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                //Debug.Log("------Wall2 right hit");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                //Debug.Log("------Wall2 left hit");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
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
                wallRotation = Quaternion.Euler(0, 0, 0);

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
                wallRotation = Quaternion.Euler(0, 270, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
            {
                //Debug.Log("------Wall8");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                //Debug.Log("------Wall9");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
            {
                //Debug.Log("------Wall10");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
            {
                //Debug.Log("------Wall11");
                var wallPiece = Resources.Load("Prefabs/Walls/Wall3");
                WallSpawnObject = wallPiece as GameObject;
            }
            var newWallPiece = Instantiate(WallSpawnObject, new Vector3(wall.x, 1f, wall.y), wallRotation) as GameObject;
            newWallPiece.name = wall.name;
            newWallPiece.transform.parent = WallObjects.transform;
            wallList.Add(newWallPiece);
        }
        foreach (GameObject wall in wallList)
        {
            //Debug.Log("------Converting Wall coordinates...");
            wall.transform.position = GetRecalculatedPosition(wall.transform.position.x, wall.transform.position.y, wall.transform.position.z);
        }
    }

    public void spawnDoors(int episode)
    {
        List<Wall> walls = constants.episodes[episode].steps[0].WallTiles;
        List<Door> doors = constants.episodes[episode].steps[0].Doors;
        wallCenter = GetWallCenter(walls, episode);

        foreach (Door door in doors)
        {
            //.Log("------Wallpiece: x: " + wall.x + " y: " + wall.y);
            doorRotation = Quaternion.Euler(0, 0, 0);

            if (CheckIfWallExistsAtPosition(door.x - 1, door.y) && CheckIfWallExistsAtPosition(door.x + 1, door.y)
            && !CheckIfWallExistsAtPosition(door.x, door.y + 1) && !CheckIfWallExistsAtPosition(door.x, door.y - 1))
            {
                doorRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (!CheckIfWallExistsAtPosition(door.x - 1, door.y) && !CheckIfWallExistsAtPosition(door.x + 1, door.y)
            && CheckIfWallExistsAtPosition(door.x, door.y + 1) && CheckIfWallExistsAtPosition(door.x, door.y - 1))
            {
                doorRotation = Quaternion.Euler(0, 90, 0);
            }

            var newDoorPiece = Instantiate(DoorObject, new Vector3(door.x, 1f, door.y), doorRotation) as GameObject;
            var doorChild = newDoorPiece.gameObject.transform.GetChild(0);
            doorChild.transform.rotation = doorRotation;
            newDoorPiece.name = door.name;
            newDoorPiece.transform.parent = DoorObjects.transform;
            doorList.Add(newDoorPiece);
        }
        foreach (GameObject door in doorList)
        {
            door.transform.position = GetRecalculatedPosition(door.transform.position.x, door.transform.position.y, door.transform.position.z);
            system.GetComponent<EnvironmentStateMachine>().doorObjects.Add(door);
        }
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        return new Vector3(x - wallCenter.x, y, z - wallCenter.z);
    }

    public Vector3 GetWallCenter(List<Wall> walls, int episode)
    {
        tempMaxWallX = 0;
        tempMaxWallY = 0;

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
        maxWallX = tempMaxWallX;
        maxWallY = tempMaxWallY;
        return new Vector3(tempMaxWallX / 2, 0f, tempMaxWallY / 2);
    }
    public bool CheckIfWallExistsAtPosition(int x, int y)
    {
        List<Wall> walls = constants.episodes[0].steps[0].WallTiles;
        foreach (Wall wall in walls)
        {
            if (wall.x == x && wall.y == y)
            {
                return true;
            }
        }
        return false;
    }

    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }

        }

        return null;
    }

    public int GetMaxWallX()
    {
        return this.maxWallX;
    }

    public int GetMaxWallY()
    {
        return this.maxWallY;
    }
}
