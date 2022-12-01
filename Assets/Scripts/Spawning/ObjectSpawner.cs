using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Class <c>ObjectSpawner</c> is responsible for instantiating all 
/// GameObjects from predefined prefabs in the scene according to the JSON file. 
/// Objects of the same type (walls, agents, items etc.) are all stored in lists of GameObjects.</summary>
///
public class ObjectSpawner : MonoBehaviour
{
    GameObject RobotSpawnObject;
    GameObject WallSpawnObject;
    GameObject PillarTopSpawnObject;
    GameObject PickupItemSpawnObject;
    GameObject DropOffZoneSpawnObject;
    GameObject DoorObject;
    GameObject positionMarkerObject;
    GameObject AgentListItem;
    GameObject EpisodeItem;
    public GameObject AgentListContent;
    [SerializeField] GameObject WallObjects;
    [SerializeField] GameObject DoorObjects;
    [SerializeField] GameObject AgentObjects;
    [SerializeField] GameObject Items;
    [SerializeField] GameObject ZoneObjects;
    [SerializeField] GameObject DirtPiles;
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
        system = GameObject.FindWithTag("System");

        RobotSpawnObject = Resources.Load("Prefabs/Robot") as GameObject;
        AgentListItem = Resources.Load("Prefabs/AgentListItem") as GameObject;
        DoorObject = Resources.Load("Prefabs/SlideDoor") as GameObject;
        WallSpawnObject = Resources.Load("Prefabs/Walls/Wall3") as GameObject;
        PillarTopSpawnObject = Resources.Load("Prefabs/Walls/PillarTop") as GameObject;
        PickupItemSpawnObject = Resources.Load("Prefabs/PickupItem") as GameObject;
        DropOffZoneSpawnObject = Resources.Load("Prefabs/DropOffZone") as GameObject;
        positionMarkerObject = Resources.Load("Prefabs/PositionMarker") as GameObject;
        EpisodeItem = Resources.Load("Prefabs/EpisodeItem") as GameObject;
        wallRotation = transform.rotation;


        Debug.Log("Instantiating Object Spawner...");
    }

    private void Update()
    {
        FloorMarkerRegister.SetActive(false);
    }

    /// <summary>This method is called to spawn a specific episode from the JSON file. 
    /// In particular the methods <c>spawnWalls(episode)</c>, <c>spawnDoors(episode)</c>
    /// and <c>spawnAgents(episode, step)</c> are being called. Objects that are optional and
    /// don't necessarily appear in an episode are null-checked. These are <c>spawnPickUpItems(episode, step)</c>,
    /// <c>spawnDropOffZones(episode, step)</c> and <c>spawnDirtRegister(episode, step)</c>.
    /// The JSON input is stored in and extracted from an object of the type <c>EnvironmentConstants</c>. </summary>
    /// <param name="constantsInput">the <c>EnvironmentConstants</c> object that stores the JSON data.</param>
    /// <param name="episode">the episode number to be spawned</param>
    /// 
    public void SpawnNewEpisode(EnvironmentConstants constantsInput, int episode)
    {
        constants = constantsInput;
        Debug.Log("#Walls Spawner Input: " + constantsInput.header.rec_Walls.Count);
        Debug.Log("#Walls Spawner: " + constants.header.rec_Walls.Count);
        spawnWalls(episode);
        spawnDoors(episode);
        spawnAgents(episode, 0);
        if (constants.episodes[episode].steps[0].Items != null) { spawnPickUpItems(episode, 0); }
        if (constants.header.rec_DropOffLocations != null) { spawnDropOffZones(episode, 0); }
        if (constants.episodes[episode].steps[0].DirtPiles != null) { spawnDirtRegister(episode, 0); }
        spawnFloorPositionMarkers(episode);
    }
    /// <summary>This method is called to destroy all GameObjects in the scene of the previous episode and clear the
    /// wall- dirt- and dool-list. </summary>
    /// 
    public void RemoveLastEpisode()
    {
        DestroyObjectsWithTag("Agent");
        DestroyObjectsWithTag("Wall");
        DestroyObjectsWithTag("PickupItem");
        DestroyObjectsWithTag("DropOffZone");
        DestroyObjectsWithTag("Trash");
        DestroyObjectsWithTag("AgentListItem");
        DestroyObjectsWithTag("Door");
        wallList.Clear();
        trashList.Clear();
        doorList.Clear();
    }

    /// <summary>This method is called to spawn the different items in the top foldout scrollview for the episode selection. 
    /// It sets the episode's name, number of steps, number of agents and environment dimensions. </summary>
    /// 
    public void SpawnEpisodeSelection()
    {
        foreach (EnvironmentEpisode ep in constants.episodes)
        {
            var newEpisodeItem = Instantiate(EpisodeItem) as GameObject;
            var content = GameObject.FindWithTag("EpisodeScrollViewContent");
            newEpisodeItem.transform.SetParent(content.transform, false);
            var episodeName = FindGameObjectInChildWithTag(newEpisodeItem, "EpisodeItemName");
            episodeName.transform.GetComponent<Text>().text = "Episode " + ep.episode;
            var nrSteps = FindGameObjectInChildWithTag(newEpisodeItem, "EpisodeNrSteps");
            nrSteps.transform.GetComponent<Text>().text = "" + ep.steps.Count;
            var nrAgents = FindGameObjectInChildWithTag(newEpisodeItem, "EpisodeNrAgents");
            nrAgents.transform.GetComponent<Text>().text = "" + ep.steps[0].Agents.Count;
            var environmentSize = FindGameObjectInChildWithTag(newEpisodeItem, "EpisodeEnvironmentSize");
            //environmentSize.transform.GetComponent<Text>().text = GetMaxWallX(ep.episode) + " x " + GetMaxWallY(ep.episode);
            system.GetComponent<EnvironmentStateManager>().episodeItems.Add(newEpisodeItem);
        }
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
            newPickupItem.transform.parent = Items.transform;
            system.GetComponent<EnvironmentStateManager>().itemObjects.Add(newPickupItem);

        }
        if (objectToSpawn.tag == "DropOffZone")
        {
            var newDropOffZone = Instantiate(objectToSpawn, SpawnPosition, transform.rotation) as GameObject;
            newDropOffZone.name = name;
            newDropOffZone.transform.parent = ZoneObjects.transform;
            system.GetComponent<EnvironmentStateManager>().zoneObjects.Add(newDropOffZone);

        }
    }

    private void spawnFloorPositionMarkers(int episode)
    {
        var tmpMaxWallX = 0;
        var tmpMaxWallY = 0;
        var tmpMinWallX = 0;
        var tmpMinWallY = 0;

        List<Wall> walls = constants.header.rec_Walls;

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

        SetUpPositionMarkers(tmpMinWallX, tmpMinWallY, tmpMaxWallX, tmpMaxWallY);
    }

    void SetUpPositionMarkers(int tmpMinWallX, int tmpMinWallY, int tmpMaxWallX, int tmpMaxWallY)
    {
        for (int i = tmpMinWallX; i <= tmpMaxWallX; i++)
        {
            for (int j = tmpMinWallY; j <= tmpMaxWallY; j++)
            {
                var posMarker = Instantiate(positionMarkerObject) as GameObject;
                posMarker.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
                posMarker.transform.SetParent(FloorMarkerRegister.gameObject.transform);
                posMarker.transform.position = GetRecalculatedPosition((float)i, 0f, (float)j);
                var text = posMarker.transform.GetChild(0).gameObject;
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

        var canvas = agentBody.transform.GetChild(4).gameObject;
        var nameTagCanvas = FindGameObjectInChildWithTag(agentBody.transform.GetChild(0).gameObject, "AgentCanvas");
        var nameTag = FindGameObjectInChildWithTag(nameTagCanvas, "NameTag");
        nameTag.GetComponent<TextMeshProUGUI>().SetText(GetAgentNumberFromNameAsString(name));
        system.GetComponent<EnvironmentStateManager>().agentObjects.Add(newRobot);
        system.GetComponent<EnvironmentStateManager>().agentListObjects.Add(newListItem);
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
        List<Item> items = constants.episodes[episode].steps[step].Items;
        foreach (Item item in items)
        {
            spawnObject(PickupItemSpawnObject, GetRecalculatedPosition(item.x, 0.3f, item.y), item.name);
        }
    }
    public void spawnDropOffZones(int episode, int step)
    {
        List<DropOffLocation> zones = constants.header.rec_DropOffLocations;
        foreach (DropOffLocation zone in zones)
        {
            if (zone != null)
            {
                spawnObject(DropOffZoneSpawnObject, GetRecalculatedPosition(zone.x, 0.05f, zone.y), zone.name);
            }
        }
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
        dirtSpawn.transform.parent = DirtPiles.transform;
        var scaleFactor = Mathf.Sqrt((float)amount);
        dirtSpawn.transform.localScale = new Vector3(scaleFactor, 0.001f, scaleFactor);
        dirtSpawn.transform.eulerAngles = new Vector3(0, randRot, 0);
        trashList.Add(dirtSpawn);
        system.GetComponent<EnvironmentStateManager>().dirtObjects.Add(dirtSpawn);
    }

    public void spawnDirtRegister(int episode, int step)
    {
        List<Dirt> dirtPiles = constants.episodes[episode].steps[step].DirtPiles;
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
        List<Wall> walls = constants.header.rec_Walls;
        wallCenter = GetWallCenter(walls, episode);

        foreach (Wall wall in walls)
        {
            wallRotation = Quaternion.Euler(0, 0, 0);
            //PillarTop
            if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
             && CheckIfWallExistsAtPosition(wall.x + 1, wall.y + 1))
            {
                var pillarRotation = Quaternion.Euler(0, 0, 0);
                var newPillarTop = Instantiate(PillarTopSpawnObject, new Vector3(wall.x, 1.9f, wall.y), pillarRotation) as GameObject;
                newPillarTop.transform.parent = WallObjects.transform;
                newPillarTop.transform.position = GetRecalculatedPosition(newPillarTop.transform.position.x + 0.5f, newPillarTop.transform.position.y, newPillarTop.transform.position.z + 0.5f);
                //x+0.34 z+0.6
            }
            //Walls
            if (!CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
             && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/WallBlock");
                WallSpawnObject = wallPiece as GameObject;
            }
            if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (!CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (!CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall1");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
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
                var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 0, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y + 1)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, -90, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall4");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 180, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x + 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x - 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 270, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x, wall.y - 1)
            && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x + 1, wall.y))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 90, 0);

            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y + 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y - 1))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && !CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
            {
                var wallPiece = Resources.Load("Prefabs/Walls/Wall2");
                WallSpawnObject = wallPiece as GameObject;
                wallRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (CheckIfWallExistsAtPosition(wall.x - 1, wall.y) && CheckIfWallExistsAtPosition(wall.x + 1, wall.y)
            && CheckIfWallExistsAtPosition(wall.x, wall.y - 1) && CheckIfWallExistsAtPosition(wall.x, wall.y + 1))
            {
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
            wall.transform.position = GetRecalculatedPosition(wall.transform.position.x, wall.transform.position.y, wall.transform.position.z);
        }
    }

    public void spawnDoors(int episode)
    {
        List<Wall> walls = constants.header.rec_Walls;
        List<Door> doors = constants.episodes[episode].steps[0].Doors;
        wallCenter = GetWallCenter(walls, episode);

        foreach (Door door in doors)
        {
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
            system.GetComponent<EnvironmentStateManager>().doorObjects.Add(door);
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
        List<Wall> walls = constants.header.rec_Walls;
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

    public int GetMaxWallX(int episode)
    {
        var walls = constants.header.rec_Walls;
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
        return tempMaxWallX;
    }

    public int GetMaxWallY(int episode)
    {
        var walls = constants.header.rec_Walls;
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
        return tempMaxWallY;
    }

    public int GetMaxWallY()
    {
        return this.maxWallY;
    }
}
