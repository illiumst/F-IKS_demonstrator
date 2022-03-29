using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

public class EnvironmentStateMachine : MonoBehaviour
{
    GameObject system;
    Slider slider;
    Slider bufferSlider;
    public Image sliderFill;
    public Image bufferFill;

    public GameObject steptext;
    Button playButton;
    Button pauseButton;
    Button nextButton;
    Button nextNextButton;
    Button previousButton;
    Button previousPreviousButton;

    GameObject episodeSelectionDropdown;
    GameObject playBackSpeedDropdown;
    GameObject stepOverviewText;
    GameObject agentNrText;
    GameObject dirtNrText;
    GameObject itemNrText;
    GameObject validNrText;
    GameObject invalidNrText;
    GameObject sequenceName;



    int currentEpisode;
    int currentStep;
    bool allAgentsInitialized = false;
    bool playingSequence = false;
    [SerializeField] bool buffering;
    int stepsSkipped = 0;

    public JSONReader JSONReader;
    public ObjectSpawner objectSpawner;
    public EnvironmentConstants environmentConstants;
    public float playBackSpeed = 0.25f;

    public List<GameObject> agentObjects = new List<GameObject>();
    public List<GameObject> agentListObjects = new List<GameObject>();
    public List<GameObject> dirtObjects = new List<GameObject>();
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> zoneObjects = new List<GameObject>();
    public List<GameObject> doorObjects = new List<GameObject>();

    public List<string> dropDownOptions = new List<string>();

    List<Agent> agents = new List<Agent>();

    public Vector3 environmentCenter;

    GameObject UICanvas;
    GameObject PopUpDialog;
    GameObject LoadingCanvas;
    GameObject AgentScrollView;
    GameObject CursorCanvas;
    GameObject CameraControllerCanvas;
    GameObject ToggleMenuCanvas;

    int validCounter;
    int invalidCounter;



    void Start()
    {
        system = GameObject.FindWithTag("System");
        UICanvas = GameObject.FindWithTag("UICanvas");
        PopUpDialog = GameObject.FindWithTag("PopUpDialog");
        LoadingCanvas = GameObject.FindWithTag("LoadingCanvas");
        AgentScrollView = GameObject.FindWithTag("RobotScrollview");
        CursorCanvas = GameObject.FindWithTag("CameraCursor");
        CameraControllerCanvas = GameObject.FindWithTag("CameraControlsPanel");
        ToggleMenuCanvas = GameObject.FindWithTag("ToggleMenu");
        agentNrText = GameObject.FindWithTag("NrAgentsText");
        itemNrText = GameObject.FindWithTag("NrItemsText");
        dirtNrText = GameObject.FindWithTag("NrDirtText");
        validNrText = GameObject.FindWithTag("NrValidText");
        invalidNrText = GameObject.FindWithTag("NrInvalidText");
        sequenceName = GameObject.FindWithTag("SequenceName");

        validCounter = 0;
        invalidCounter = 0;

        UICanvas.SetActive(false);
        PopUpDialog.SetActive(false);
        JSONReader = GetComponent<JSONReader>();
        sequenceName.GetComponent<TextMeshProUGUI>().SetText(JSONReader.GetFileName());
        StartCoroutine(InitializeObjectSpawner());
    }

    IEnumerator InitializeObjectSpawner()
    {
        objectSpawner = system.GetComponent<ObjectSpawner>();
        yield return new WaitForSeconds(3);
        InitializeEnvironmentData();
    }
    void InitializeEnvironmentStateMachine()
    {
        //UI Initialization
        LoadingCanvas.SetActive(false);
        UICanvas.SetActive(true);
        system.GetComponent<UIShowAndHide>().InitializeUIShowAndHide();

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
        InitializeTimelineSlider();
        InitializeBufferSlider();
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        bufferSlider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        currentStep = (int)bufferSlider.value;
        UpdateSliderLabel();
        UpdateStepOverview();
        steptext.SetActive(false);
        buffering = false;

        //Button Listner set-up
        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);
        nextButton.onClick.AddListener(delegate { SkipSteps(1); });
        nextNextButton.onClick.AddListener(delegate { SkipSteps(10); });
        previousButton.onClick.AddListener(delegate { SkipSteps(-1); });
        previousPreviousButton.onClick.AddListener(delegate { SkipSteps(-10); });

        //Dropdown
        agents.Clear();
        agents = environmentConstants.episodes[0].steps[0].Agents;
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
        speedDrop.value = 1;
    }

    void InitializeEnvironmentData()
    {
        environmentConstants = JSONReader.constants;
        currentEpisode = 0;
        objectSpawner.SpawnNewEpisode(environmentConstants, currentEpisode);
        environmentCenter = objectSpawner.GetWallCenter(environmentConstants.episodes[0].steps[0].WallTiles, 0);
        InitializeEnvironmentStateMachine();
    }


    void Update()
    {
        if (!allAgentsInitialized && environmentConstants == null || !allAgentsInitialized && agentObjects.Count == 0)
        {
        }
        else if (!allAgentsInitialized && agentObjects.Count > 0)
        {
            //TODO find solution later
            allAgentsInitialized = AllAgentObjectsInitialized(currentEpisode);
        }
        else
        {
            //Debug.Log("bufferSlider: " + bufferSlider.value + " slider: " + slider.value);
            if ((int)bufferSlider.value != (int)slider.value)
            {
                Debug.Log("____TEST: set to buffering");
                buffering = true;
            }

            if (playingSequence)
            {
                Debug.Log("____TEST: playing sequence case");

                HandleSequencePlayUpdate();
            }
            if (buffering)
            {
                Debug.Log("____TEST: buffering case");

                HandleBufferSequenceUpdate(stepsSkipped);
                steptext.SetActive(true);
            }

        }

    }

    void InitializeTimelineSlider()
    {
        slider.onValueChanged.AddListener(delegate
            {
                slider.value = Mathf.Round(slider.value * 10f) / 10f;
                Debug.Log("Timestep slider: " + slider.value);

                if ((int)slider.value > currentStep + 1)
                {
                    stepsSkipped = ((int)slider.value - currentStep);
                    buffering = true;
                    PauseSequence();
                    sliderFill.color = new Color(0, 125, 255, 255);
                    sliderFill.transform.SetSiblingIndex(0);
                    bufferFill.color = Color.white;
                    Debug.Log("Buffering forward: steps skipped: " + stepsSkipped);

                }
                else if ((int)slider.value < currentStep - 1)
                {
                    stepsSkipped = ((int)slider.value - currentStep);
                    buffering = true;
                    PauseSequence();
                    sliderFill.color = Color.white;
                    sliderFill.transform.SetSiblingIndex(1);
                    bufferFill.color = new Color(0, 125, 255, 255);
                    Debug.Log("Buffering backwards: steps skipped: " + stepsSkipped);
                }
            });
    }

    void InitializeBufferSlider()
    {
        bufferSlider.onValueChanged.AddListener(delegate
        {
            //var bufferCheckValue = Mathf.Round(bufferSlider.value * 100f) / 100f;
            bufferSlider.value = Mathf.Round(bufferSlider.value * 10f) / 10f;
            Debug.Log("Timestep buffer: " + bufferSlider.value + " slider: " + slider.value + " currentstep: " + currentStep);
            if (bufferSlider.value % 1f == 0)
            {
                Debug.Log("bufferSlider.value % 1 == 0");
                if (bufferSlider.value == currentStep + 1f)
                {
                    LoadNewTimeStep(currentEpisode, currentStep);
                }
            }
        });
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
        objectSpawner.RemoveLastEpisode();
        emptyObjectLists();
        agents.Clear();
        agents = environmentConstants.episodes[currentEpisode].steps[0].Agents;
        objectSpawner.SpawnNewEpisode(environmentConstants, currentEpisode);
        environmentCenter = objectSpawner.GetWallCenter(environmentConstants.episodes[0].steps[0].WallTiles, 0);
        UpdateSliderLabel();
        validCounter = 0;
        invalidCounter = 0;
    }

    float SpeedDropdownValueChanged(Dropdown change)
    {
        var pbSpeed = 1f;
        //adjust slider values to make sure values reach whole numbers
        Debug.Log("Speed dropdown value changed");
        slider.value = (int)slider.value;
        bufferSlider.value = (int)bufferSlider.value;
        switch (change.value)
        {
            case 0: pbSpeed = 0.5f; break;
            case 1: pbSpeed = 1f; break;
            case 2: pbSpeed = 2f; break;
            case 3: pbSpeed = 5f; break;
            default: pbSpeed = 1f; break;
        }
        return pbSpeed;
    }

    float GetSpeedDropDownValue(Dropdown change)
    {
        var pbSpeed = 1f;
        switch (change.value)
        {
            case 0: pbSpeed = 0.5f; break;
            case 1: pbSpeed = 1f; break;
            case 2: pbSpeed = 2f; break;
            case 3: pbSpeed = 5f; break;
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
        options.Add("0.5x");
        options.Add("Normal");
        options.Add("2x");
        options.Add("5x");
        var drop = playBackSpeedDropdown.GetComponent<Dropdown>();
        drop.AddOptions(options);
    }

    void UpdateStepOverview()
    {
        //stepOverviewText.gameObject.GetComponent<Text>().text = "Overview: Step " + currentStep;
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
        agentNrText.gameObject.GetComponent<Text>().text = "" + nrAgents;
        dirtNrText.gameObject.GetComponent<Text>().text = "" + nrDirt;
        itemNrText.gameObject.GetComponent<Text>().text = "" + nrItems;
        //TODO refelct what i want to show here
        validNrText.gameObject.GetComponent<Text>().text = "" + validCounter;
        invalidNrText.gameObject.GetComponent<Text>().text = "" + invalidCounter;

    }

    void UpdateSliderLabel()
    {
        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        steptext.GetComponent<Text>().text = "" + (int)slider.value;
        system.GetComponent<UIGlobals>().sliderStepCount.gameObject.GetComponent<Text>().text = "Step " + currentStep + " / " + slider.maxValue;
    }

    bool AllAgentObjectsInitialized(int episode)
    {
        if (environmentConstants == null || agentObjects == null)
        {
            return false;
        }
        else
        {
            return (environmentConstants.episodes[episode].steps[0].Agents.Count == agentObjects.Count)
             && (environmentConstants.episodes[episode].steps[0].Agents.Count == agentListObjects.Count);
        }
    }

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void LoadNewTimeStep(int episode, int step)
    {
        currentStep = (int)bufferSlider.value;
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabel();
        UpdateStepOverview();
        Debug.Log("Loading new time step.....step: " + currentStep);
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

                    if (doorsPrev[i].state.Equals("closed"))
                    {
                        doorAn.SetTrigger("doorOpen");
                    }
                    else
                    {
                        doorAn.SetTrigger("doorClose");
                    }
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
        //List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

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
        //List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;
        for (int i = 0; i < agents.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            agentObjController.UpdateSpeechBubble(agents[i].action, agents[i].valid);
            agentObjController.speechBubblePositionText.GetComponent<Text>().text = "x:" + agents[i].x + " y:" + agents[i].y;


            if (!agents[i].valid)
            {
                agentObjController.valid = false;
                agentObjController.speed = 0;
                agentObjController.animator.SetBool("moving", false);
                invalidCounter++;
                /*if (CheckForWallCollision(agents[i], episode, step))
                {
                    agentObjController.animator.SetTrigger("collision");
                }*/
            }
            if (agents[i].valid)
            {
                agentObjController.valid = true;
                validCounter++;
                if (agents[i].action.Equals("Action[CLEAN_UP]"))
                {
                    agentObjController.animator.SetTrigger("foundTrash");
                    SpawnCleaningBubbles(agentObjController.goalPosition);
                }
                if (agents[i].action.Equals("Action[ITEM_ACTION]"))
                {
                    agentObjController.animator.SetTrigger("foundTrash");
                }
            }
        }
    }

    bool CheckForWallCollision(Agent agent, int episode, int step)
    {
        //List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;
        int index = agents.IndexOf(agent);
        var agentObjController = agentObjects[index].transform.GetChild(0).GetComponent<AgentController>();

        var x = agent.x;
        var y = agent.y;

        switch (agent.action)
        {
            case "Action[NORTH]": return CheckForWall(x, y - 1, episode, agentObjController);
            case "Action[NORTHEAST]": return CheckForWall(x + 1, y - 1, episode, agentObjController);
            case "Action[EAST]": return CheckForWall(x + 1, y, episode, agentObjController);
            case "Action[SOUTHEAST]": return CheckForWall(x + 1, y + 1, episode, agentObjController);
            case "Action[SOUTH]": return CheckForWall(x, y + 1, episode, agentObjController);
            case "Action[SOUTHWEST]": return CheckForWall(x - 1, y + 1, episode, agentObjController);
            case "Action[WEST]": return CheckForWall(x - 1, y, episode, agentObjController);
            case "Action[NORTHWEST]": return CheckForWall(x - 1, y - 1, episode, agentObjController);
            default: return false; ;
        }
    }

    bool CheckForWall(int x, int y, int episode, AgentController ctr)
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
        //List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

        for (int i = 0; i < agents.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            agentObjController.UpdateAgentListItems(agentObjects[i], agentListObjects[i],
            agents[i].x, agents[i].y, agents[i].name, agents[i].action, agents[i].valid);
        }
    }

    public Vector3 RequestAgentPosition(GameObject agent, int episode, int step)
    {
        //List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

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
                objectSpawner.spawnDirt(pos, dirt.amount, dirt.name);
            }
        }
        foreach (GameObject dirtObj in dirtObjects)
        {
            if (GetDirtByName(dirtObj.name) == null)
            {
                StartCoroutine(DeleteDirtObject(dirtObj));
            }
        }
    }

    IEnumerator DeleteDirtObject(GameObject dirt)
    {
        yield return new WaitForSeconds(1);
        dirt.SetActive(false);
    }

    void HandleSequencePlayUpdate()
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
                slider.value = slider.value + (playBackSpeed / 10f);
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

    void HandleBufferSequenceUpdate(int stepsSkipped)
    {
        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            playingSequence = false;
            controller.playingSequence = true;
            float setPBSpeed = GetSpeedDropDownValue(playBackSpeedDropdown.GetComponent<Dropdown>());
            //Debug.Log("Buffering sequence update: buffer: " + bufferSlider.value + " slider: " + slider.value);
            var factor = 0f;
            if ((stepsSkipped <= 30 && stepsSkipped > 0) || (stepsSkipped >= -30 && stepsSkipped < 0))
            {
                factor = 10f;
            }
            else if ((stepsSkipped <= 100 && stepsSkipped > 0) || (stepsSkipped >= -100 && stepsSkipped < 0))
            {
                factor = 5f;
            }
            else if ((stepsSkipped > 100 && stepsSkipped > 0) || (stepsSkipped < -100 && stepsSkipped < 0))
            {
                factor = 2f;
            }
            //buffering forward
            if (stepsSkipped > 0)
            {
                controller.backwardBuffer = false;
                /*if (bufferSlider.value == 0f)
                {
                    bufferSlider.value = 1f;
                }*/
                if (controller.currentpos == controller.goalPosition && bufferSlider.value < slider.value)
                {
                    float add = (playBackSpeed / factor);
                    Debug.Log("Adjusting buffer slider forward: adding " + add + " to " + bufferSlider.value);
                    bufferSlider.value = bufferSlider.value + add;
                    //bufferSlider.value = bufferSlider.value + 1f;
                }
                else if (bufferSlider.value == slider.value)
                {
                    Debug.Log("Finished Buffering forward");
                    controller.playingSequence = false;
                    stepsSkipped = 0;
                    buffering = false;
                    controller.backwardBuffer = false;
                    playBackSpeed = setPBSpeed;
                }
            }
            //buffering backwards
            else if (stepsSkipped < 0)
            {
                controller.backwardBuffer = true;

                if (controller.currentpos == controller.goalPosition && bufferSlider.value > slider.value)
                {
                    bufferSlider.value = bufferSlider.value - (playBackSpeed / factor);
                    Debug.Log("Adjusting buffer slider backwards: " + bufferSlider.value);

                }
                else if (bufferSlider.value == slider.value)
                {
                    Debug.Log("Finished Buffering backward");
                    controller.playingSequence = false;
                    controller.backwardBuffer = false;
                    stepsSkipped = 0;
                    buffering = false;
                    playBackSpeed = setPBSpeed;
                    sliderFill.color = Color.blue;
                    sliderFill.transform.SetSiblingIndex(0);
                    bufferFill.color = Color.white;
                }
            }
        }
    }

    void RepositionAgent(GameObject agent, Vector3 pos)
    {
        agent.transform.position = pos;
    }

    void PlaySequence()
    {
        Debug.Log("Playing Sequence.....");
        playingSequence = true;
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        bufferSlider.value = (int)bufferSlider.value;
        slider.value = bufferSlider.value;
        LoadNewTimeStep(currentEpisode, currentStep);
        buffering = false;

        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = true;
        }
    }

    public void PauseSequence()
    {
        playingSequence = false;
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);

        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = false;
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void SpawnCleaningBubbles(Vector3 pos)
    {
        StartCoroutine(BubbleSpawn(pos));
    }

    IEnumerator BubbleSpawn(Vector3 pos)
    {
        //spawn bubbles
        Debug.Log("Spawning Bubbles....");
        var particleRes = Resources.Load("Prefabs/CleaningParticlesNew") as GameObject;
        var particlePos = new Vector3(pos.x, 0.3f, pos.z);
        GameObject particles = Instantiate(particleRes, particlePos, Quaternion.identity, null) as GameObject;
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(2);
        //destroy bubbles
        Destroy(particles);
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
