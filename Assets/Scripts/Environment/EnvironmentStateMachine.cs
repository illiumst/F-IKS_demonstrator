using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentStateMachine : MonoBehaviour
{
    GameObject system;
    Slider slider;
    Slider bufferSlider;
    Button playButton;
    Button pauseButton;
    Button nextButton;
    Button nextNextButton;
    Button previousButton;
    Button previousPreviousButton;
    GameObject episodeSelectionDropdown;
    GameObject playBackSpeedDropdown;
    public GameObject stepOverviewText;
    public GameObject agentNrText;
    public GameObject dirtNrText;
    public GameObject itemNrText;



    int currentEpisode;
    int currentStep;
    bool allAgentsInitialized = false;
    bool playingSequence = false;
    bool buffering = false;
    int stepsSkipped = 0;

    public JSONReader JSONReader;
    public EnvironmentConstants environmentConstants;
    public float playBackSpeed = 0.25f;

    public List<GameObject> agentObjects = new List<GameObject>();
    public List<GameObject> agentListObjects = new List<GameObject>();
    public List<GameObject> dirtObjects = new List<GameObject>();
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> zoneObjects = new List<GameObject>();
    public List<GameObject> doorObjects = new List<GameObject>();

    public List<string> dropDownOptions = new List<string>();

    public Vector3 environmentCenter;



    void Start()
    {
        system = GameObject.FindWithTag("System");
        JSONReader = GetComponent<JSONReader>();
        environmentConstants = JSONReader.ReadEnvironmentConstants();
        currentEpisode = 0;

        //UI Initialization
        slider = system.GetComponent<UIGlobals>().slider;
        bufferSlider = system.GetComponent<UIGlobals>().bufferSlider;
        episodeSelectionDropdown = GameObject.FindWithTag("EpisodeSelectionDropdown");
        playBackSpeedDropdown = GameObject.FindWithTag("PlaybackSpeedDropdown");
        playButton = system.GetComponent<UIGlobals>().playButton;
        pauseButton = system.GetComponent<UIGlobals>().pauseButton;
        nextButton = system.GetComponent<UIGlobals>().nextButton;
        nextNextButton = system.GetComponent<UIGlobals>().nextNextButton;
        previousButton = system.GetComponent<UIGlobals>().previousButton;
        previousPreviousButton = system.GetComponent<UIGlobals>().previousPreviousButton;

        //slider
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        bufferSlider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        currentStep = (int)bufferSlider.value;
        UpdateSliderLabel();
        UpdateStepOverview();

        //Button Listner set-up
        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);
        nextButton.onClick.AddListener(delegate { SkipSteps(1); });
        nextNextButton.onClick.AddListener(delegate { SkipSteps(10); });
        previousButton.onClick.AddListener(delegate { SkipSteps(-1); });
        previousPreviousButton.onClick.AddListener(delegate { SkipSteps(-10); });

        //Dropdown
        InitializeEpisodeSelectionDropdown();
        InitializePlaybackSpeedDropdown();
        var drop = episodeSelectionDropdown.GetComponent<Dropdown>();
        drop.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(drop);
        });
        var speedDrop = playBackSpeedDropdown.GetComponent<Dropdown>();
        speedDrop.onValueChanged.AddListener(delegate
        {
            playBackSpeed = SpeedDropdownValueChanged(speedDrop);
            Debug.Log("PlaybackSpeed set to: " + playBackSpeed);

        });
        speedDrop.value = 3;

    }

    void Update()
    {
        if (!allAgentsInitialized)
        {
            allAgentsInitialized = AllAgentObjectsInitialized(currentEpisode);
        }
        else
        {
            slider.onValueChanged.AddListener(delegate
            {
                LoadNewTimeStep(currentEpisode, currentStep);
                if ((int)slider.value > currentStep + 1 || (int)slider.value < currentStep - 1)
                {
                    stepsSkipped = ((int)slider.value - currentStep);
                    buffering = true;
                    PauseSequence();
                }
            });

            bufferSlider.onValueChanged.AddListener(delegate
            {
                LoadNewTimeStep(currentEpisode, currentStep);
            });

            if (playingSequence)
            {
                HandleSequencePlayUpdate();
            }
            if (buffering)
            {
                HandleBufferSequenceUpdate(stepsSkipped);
            }
        }

    }

    //TODO check if matches concept of buffering
    void SkipSteps(int i)
    {
        PauseSequence();
        if (i > 0)
        {
            if (slider.value + i > slider.maxValue)
            {
                slider.value = (slider.value + i) - slider.maxValue;
            }
            else
            {
                slider.value = slider.value + i;
            }
        }
        else
        {
            if (slider.value + i < 0)
            {
                slider.value = (slider.value + i) + slider.maxValue;
            }
            else
            {
                slider.value = slider.value + i;
            }
        }
        LoadNewTimeStep(currentEpisode, currentStep);
    }

    void DropdownValueChanged(Dropdown change)
    {
        currentEpisode = change.value;
        slider.value = 0;
        bufferSlider.value = 0;
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        bufferSlider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        system.GetComponent<ObjectSpawner>().RemoveLastEpisode();
        emptyObjectLists();
        system.GetComponent<ObjectSpawner>().SpawnNewEpisode(currentEpisode);
        UpdateSliderLabel();
    }

    float SpeedDropdownValueChanged(Dropdown change)
    {
        var pbSpeed = 1f;
        switch (change.value)
        {
            case 0: pbSpeed = 0.25f; break;
            case 1: pbSpeed = 0.5f; break;
            case 2: pbSpeed = 0.75f; break;
            case 3: pbSpeed = 1f; break;
            case 4: pbSpeed = 1.25f; break;
            case 5: pbSpeed = 1.5f; break;
            case 6: pbSpeed = 1.75f; break;
            default: pbSpeed = 1f; break;
        }
        return pbSpeed;
    }

    void emptyObjectLists()
    {
        agentObjects.Clear();
        agentListObjects.Clear();
        dirtObjects.Clear();
        itemObjects.Clear();
        zoneObjects.Clear();
        doorObjects.Clear();
    }

    void InitializeEpisodeSelectionDropdown()
    {
        var drop = episodeSelectionDropdown.GetComponent<Dropdown>();
        drop.ClearOptions();
        for (int i = 0; i < environmentConstants.episodes.Count; i++)
        {
            dropDownOptions.Add("Episode " + (i + 1));
        }
        drop.AddOptions(dropDownOptions);
    }

    void InitializePlaybackSpeedDropdown()
    {
        var options = new List<string>();
        options.Add("0.25x");
        options.Add("0.5x");
        options.Add("0.75x");
        options.Add("Normal");
        options.Add("1.25x");
        options.Add("1.5x");
        options.Add("1.75x");
        var drop = playBackSpeedDropdown.GetComponent<Dropdown>();
        drop.AddOptions(options);
    }

    void UpdateStepOverview()
    {
        stepOverviewText.gameObject.GetComponent<Text>().text = "Overview: Step " + currentStep;
        var step = environmentConstants.episodes[currentEpisode].steps[currentStep];
        var nrAgents = step.Agents.Count;
        var nrDirt = step.DirtRegister.Count;
        var nrItems = 0;
        foreach (Item item in environmentConstants.episodes[currentEpisode].steps[currentStep].ItemRegister)
        {
            if (item.x != -9999 && item.y != -9999)
            {
                nrItems += 1;
            }
        }
        agentNrText.gameObject.GetComponent<Text>().text = "#" + nrAgents;
        dirtNrText.gameObject.GetComponent<Text>().text = "#" + nrDirt;
        itemNrText.gameObject.GetComponent<Text>().text = "#" + nrItems;

    }

    void UpdateSliderLabel()
    {

        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        var handle = sliderHandleArea.transform.GetChild(0).gameObject;
        var steptext = handle.transform.GetChild(0).gameObject;
        steptext.GetComponent<Text>().text = "Step " + currentStep;
        system.GetComponent<UIGlobals>().sliderStepCount.gameObject.GetComponent<Text>().text = "Step " + currentStep + " / " + slider.maxValue;
    }

    bool AllAgentObjectsInitialized(int episode)
    {
        return (environmentConstants.episodes[episode].steps[0].Agents.Count == agentObjects.Count)
        && (environmentConstants.episodes[episode].steps[0].Agents.Count == agentListObjects.Count);
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void LoadNewTimeStep(int episode, int step)
    {
        currentStep = (int)bufferSlider.value;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabel();
        UpdateStepOverview();
    }

    public void UpdateAgents(int episode, int step)
    {
        UpdateAgentUI(episode, step);
        UpdateAgentPositions(episode, step);
        UpdateAgentAction(episode, step);
    }

    private void UpdateDoors(int episode, int step)
    {
        var doors = environmentConstants.episodes[episode].steps[step].Doors;

        for (int i = 0; i < doors.Count; i++)
        {
            if (step > 0)
            {
                var doorsPrev = environmentConstants.episodes[episode].steps[step - 1].Doors;
                if (!doors[i].state.Equals(doorsPrev[i].state))
                {
                    var doorAn = doorObjects[i].GetComponentInChildren<Animator>();
                    doorAn.SetTrigger("OpenClose");
                }
            }
        }
    }

    private void UpdateItemObjects(int episode, int step)
    {
        var items = environmentConstants.episodes[episode].steps[step].ItemRegister;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].x == -9999 && items[i].y == -9999)
            {
                itemObjects[i].SetActive(false);
            }
            else
            {
                itemObjects[i].SetActive(true);
            }
        }
    }

    private void UpdateAgentPositions(int episode, int step)
    {
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

        for (int i = 0; i < agents.Count; i++)
        {
            Vector3 recalcPos = GetRecalculatedPosition(agents[i].x, 0, agents[i].y);
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            BroadCastSliderValueToAgentObj(agentObjController);
            agentObjController.goalPosition = recalcPos;
        }
    }

    private void UpdateAgentAction(int episode, int step)
    {
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;
        for (int i = 0; i < agents.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            if (!agents[i].valid)
            {
                if (CheckForWallCollision(agents[i], episode, step))
                {
                    agentObjController.animator.SetTrigger("collision");
                }
            }
            else if (agents[i].action.Equals("Action[CLEAN_UP]") && agents[i].valid)
            {
                agentObjController.animator.SetTrigger("foundTrash");
            }
            else if (agents[i].action.Equals("Action[ITEM_ACTION]") && agents[i].valid)
            {
                agentObjController.animator.SetTrigger("foundTrash");
            }
        }
    }

    bool CheckForWallCollision(Agent agent, int episode, int step)
    {
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;
        int index = agents.IndexOf(agent);
        var agentObjController = agentObjects[index].transform.GetChild(0).GetComponent<AgentController>();

        var x = agent.x;
        var y = agent.y;

        switch (agent.action)
        {
            case "Action[NORTH]": return CheckForWall(x, y - 1, episode);
            case "Action[NORTHEAST]": return CheckForWall(x + 1, y - 1, episode);
            case "Action[EAST]": return CheckForWall(x + 1, y, episode);
            case "Action[SOUTHEAST]": return CheckForWall(x + 1, y + 1, episode);
            case "Action[SOUTH]": return CheckForWall(x, y + 1, episode);
            case "Action[SOUTHWEST]": return CheckForWall(x - 1, y + 1, episode);
            case "Action[WEST]": return CheckForWall(x - 1, y, episode);
            case "Action[NORTHWEST]": return CheckForWall(x - 1, y - 1, episode);
            default: return false; ;
        }
    }

    bool CheckForWall(int x, int y, int episode)
    {
        bool wallExists = false;
        List<Wall> walls = environmentConstants.episodes[episode].steps[0].WallTiles;
        foreach (Wall wall in walls)
        {
            if (wall.x == x && wall.y == y)
            {
                wallExists = true;
            }
        }
        return wallExists;
    }

    private void BroadCastSliderValueToAgentObj(AgentController controller)
    {
        controller.sliderValue = bufferSlider.value;
    }

    private void UpdateAgentUI(int episode, int step)
    {
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

        for (int i = 0; i < agents.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            agentObjController.UpdateAgentListItems(agentObjects[i], agentListObjects[i],
            agents[i].x, agents[i].y, agents[i].name, agents[i].action, agents[i].valid);
        }
    }

    public Vector3 RequestAgentPosition(GameObject agent, int episode, int step)
    {
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

        int agentIndex = agentObjects.IndexOf(agent);
        Vector3 recalcPos = GetRecalculatedPosition(agents[agentIndex].x, 0, agents[agentIndex].y);
        return recalcPos;
    }

    public void UpdateDirtPiles(int episode, int step)
    {
        List<Dirt> dirtPiles = environmentConstants.episodes[episode].steps[step].DirtRegister;

        foreach (Dirt dirt in dirtPiles)
        {
            if (GetDirtObjectByName(dirt.name) != null)
            {
                var dirtInPrevStep = GetDirtByNamePrevStep(dirt.name);
                if (dirtInPrevStep != null && dirtInPrevStep.amount != dirt.amount)
                {
                    var dirtObj = GetDirtObjectByName(dirt.name);
                    var scaleFactor = Mathf.Sqrt((float)dirt.amount);
                    dirtObj.transform.localScale = new Vector3(scaleFactor, 0.001f, scaleFactor);
                }
            }
            else
            {
                var pos = GetRecalculatedPosition(dirt.x, 0, dirt.y);
                system.GetComponent<ObjectSpawner>().spawnDirt(pos, dirt.amount, dirt.name);
            }
        }
        foreach (GameObject dirtObj in dirtObjects)
        {
            if (GetDirtByName(dirtObj.name) == null)
            {
                dirtObj.SetActive(false);
            }
        }
    }

    public void HandleSequencePlayUpdate()
    {

        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();

            if (slider.value == 0f)
            {
                slider.value = 1f;
                bufferSlider.value = slider.value;
            }
            else if (controller.currentpos == controller.goalPosition && slider.value < (slider.maxValue - 1f))
            {
                slider.value = slider.value + (playBackSpeed / 10);
                bufferSlider.value = slider.value;
            }
            else if ((controller.currentpos == controller.goalPosition) && (slider.value == (slider.maxValue - 1f)))
            {
                var pos = RequestAgentPosition(agentObj, currentEpisode, 0);
                RepositionAgent(agentObj, pos);
                slider.value = 0f;
                bufferSlider.value = slider.value;
            }
        }
    }

    public void HandleBufferSequenceUpdate(int stepsSkipped)
    {
        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = true;

            var setPBSpeed = SpeedDropdownValueChanged(playBackSpeedDropdown.GetComponent<Dropdown>());
            playBackSpeed = setPBSpeed * 10;
            if (bufferSlider.value == 0f)
            {
                bufferSlider.value = 1f;
            }
            else if (controller.currentpos == controller.goalPosition && bufferSlider.value < slider.value)
            {
                bufferSlider.value = bufferSlider.value + (playBackSpeed / 10);
            }
            else if ((int)bufferSlider.value == (int)slider.value)
            {
                Debug.Log("Finished Buffering");
                controller.playingSequence = false;
                stepsSkipped = 0;
                buffering = false;
                playBackSpeed = setPBSpeed;
            }
        }
    }

    void RepositionAgent(GameObject agent, Vector3 pos)
    {
        agent.transform.position = pos;
    }

    void PlaySequence()
    {
        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = true;
            playingSequence = true;
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }
    }

    void PauseSequence()
    {
        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = false;
            playingSequence = false;
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }
    }

    public Agent GetAgentByName(string name)
    {
        foreach (Agent agent in environmentConstants.episodes[currentEpisode].steps[currentStep].Agents)
        {
            if (agent.name.Equals(name))
            {
                return agent;
            }
        }
        return null;
    }
    public Dirt GetDirtByName(string name)
    {
        foreach (Dirt dirt in environmentConstants.episodes[currentEpisode].steps[currentStep].DirtRegister)
        {
            if (dirt.name.Equals(name))
            {
                return dirt;
            }
        }
        return null;
    }

    public Dirt GetDirtByNamePrevStep(string name)
    {
        if (currentStep > 0)
        {
            foreach (Dirt dirt in environmentConstants.episodes[currentEpisode].steps[currentStep - 1].DirtRegister)
            {
                if (dirt.name.Equals(name))
                {
                    return dirt;
                }
            }
        }
        return null;
    }

    public GameObject GetDirtObjectByName(string name)
    {
        foreach (GameObject dirt in dirtObjects)
        {
            if (dirt.name.Equals(name))
            {
                return dirt;
            }
        }
        return null;
    }

    public Item GetItemByName(string name)
    {
        foreach (Item item in environmentConstants.episodes[currentEpisode].steps[currentStep].ItemRegister)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }
        return null;
    }


}
