using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class EnvironmentStateManager : MonoBehaviour
{
    #region Object and Variable Declarations

    //========================================================================================================//
    //================================= GLOBAL OBJECTS =======================================================//
    //========================================================================================================//
    GameObject system;
    JSONReader JSONReader;
    ObjectSpawner objectSpawner;
    public EnvironmentConstants environmentConstants;

    //========================================================================================================//
    //================================= UI ELEMENTS ==========================================================//
    //========================================================================================================//

    //********************************** CANVASES / AREAS ***************************************************//

    GameObject UICanvas;
    GameObject PopUpDialog;
    GameObject LoadingCanvas;
    GameObject AgentScrollView;
    GameObject CursorCanvas;
    GameObject CameraControllerCanvas;
    GameObject ToggleMenuCanvas;

    //********************************** SLIDER **************************************************************//

    Slider slider;
    Slider bufferSlider;
    Image sliderFill;
    Image bufferFill;

    //********************************** SLIDER NAVIGATION ***************************************************//

    GameObject steptext;
    GameObject sliderHandleLabel;
    Button playButton;
    Button pauseButton;
    Button nextButton;
    Button nextNextButton;
    Button previousButton;
    Button previousPreviousButton;
    GameObject playBackSpeedDropdown;
    Toggle timelapseToggle;

    //********************************** STEP OVERVIEW *******************************************************//

    GameObject stepOverviewText;
    GameObject agentNrText;
    GameObject dirtNrText;
    GameObject itemNrText;
    GameObject validNrText;
    GameObject invalidNrText;

    //********************************** SEQUENCE SELECTION **************************************************//

    GameObject sequenceName;
    GameObject selectedEpisodeName;

    //========================================================================================================//
    //================================= OBJECT LISTS =========================================================//
    //========================================================================================================//

    public List<GameObject> agentObjects = new List<GameObject>();
    public List<GameObject> agentListObjects = new List<GameObject>();
    public List<GameObject> dirtObjects = new List<GameObject>();
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> zoneObjects = new List<GameObject>();
    public List<GameObject> doorObjects = new List<GameObject>();
    public List<GameObject> episodeItems = new List<GameObject>();
    public List<string> dropDownOptions = new List<string>();
    public List<Agent> agents = new List<Agent>();

    //========================================================================================================//
    //================================= HELPING FIELDS =======================================================//
    //========================================================================================================//

    int currentEpisode;
    int currentStep;
    int prevStep = 0;

    bool allAgentsInitialized = false;
    bool playingSequence = false;
    public bool buffering;
    bool lerping = false;
    bool timelapse = true;
    int stepsSkipped = 0;

    public float playBackSpeed = 0.25f;

    public Vector3 environmentCenter;

    int validCounter;
    int invalidCounter;

    #endregion

    void Start()
    {
        #region Initializing GameObjects by Tag

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
        selectedEpisodeName = GameObject.FindWithTag("SelectedEpisodeName");
        sliderFill = GameObject.FindWithTag("SliderFill").GetComponent<Image>();
        bufferFill = GameObject.FindWithTag("BufferFill").GetComponent<Image>();
        steptext = GameObject.FindWithTag("StepText");
        sliderHandleLabel = GameObject.FindWithTag("SliderHandleLabel");
        timelapseToggle = GameObject.FindWithTag("TimelapseToggle").GetComponent<Toggle>();

        #endregion

        validCounter = 0;
        invalidCounter = 0;

        UICanvas.SetActive(false);
        PopUpDialog.SetActive(false);
        JSONReader = GetComponent<JSONReader>();
        sequenceName.GetComponent<TextMeshProUGUI>().SetText(JSONReader.GetFileName());
        StartCoroutine(InitializeObjectSpawner());
    }

    #region Initializations

    IEnumerator InitializeObjectSpawner()
    {
        objectSpawner = system.GetComponent<ObjectSpawner>();
        yield return new WaitForSeconds(3);
        InitializeEnvironmentData();
    }

    void InitializeEnvironmentStateMachine()
    {
        //********************************** UI INITIALIZATION **********************************************//

        LoadingCanvas.SetActive(false);
        UICanvas.SetActive(true);
        system.GetComponent<UIShowAndHide>().InitializeUIShowAndHide();

        slider = GameObject.FindWithTag("Slider").GetComponent<Slider>();
        bufferSlider = GameObject.FindWithTag("BufferFillArea").GetComponent<Slider>();
        playBackSpeedDropdown = GameObject.FindWithTag("PlaybackSpeedDropdown");
        playButton = GameObject.FindWithTag("PlayButton").GetComponent<Button>();
        pauseButton = GameObject.FindWithTag("PauseButton").GetComponent<Button>();
        pauseButton.gameObject.SetActive(false);
        nextButton = GameObject.FindWithTag("NextButton").GetComponent<Button>();
        nextNextButton = GameObject.FindWithTag("NextNextButton").GetComponent<Button>();
        previousButton = GameObject.FindWithTag("PreviousButton").GetComponent<Button>();
        previousPreviousButton = GameObject.FindWithTag("PreviousPreviousButton").GetComponent<Button>();

        //********************************** SLIDER SETUP ***************************************************//

        InitializeTimelineSlider();
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        bufferSlider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        currentStep = 0;
        UpdateSliderLabel();
        UpdateStepOverview();
        steptext.SetActive(false);
        buffering = false;

        //********************************** BUTTON LISTENER SET-UP *****************************************//

        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);
        nextButton.onClick.AddListener(delegate { SkipSteps(1); });
        nextNextButton.onClick.AddListener(delegate { SkipSteps(10); });
        previousButton.onClick.AddListener(delegate { SkipSteps(-1); });
        previousPreviousButton.onClick.AddListener(delegate { SkipSteps(-10); });

        //********************************** AGENTS SET-UP **************************************************//

        agents.Clear();
        agents = environmentConstants.episodes[0].steps[0].Agents;

        //********************************** EPISODE SELECTION SET-UP ***************************************//

        SetUpEpisodeSelection();

        //********************************** SPEED DROP-DOWN ***********************************************//

        InitializePlaybackSpeedDropdown();
    }

    void SetUpEpisodeSelection()
    {
        objectSpawner.SpawnEpisodeSelection();
        selectedEpisodeName.GetComponent<Text>().text = "Episode 0";
        Button[] allChildren = GameObject.FindWithTag("EpisodeScrollViewContent").GetComponentsInChildren<Button>();

        foreach (Button child in allChildren)
        {
            child.onClick.AddListener(delegate { ChangeEpisode(Array.IndexOf(allChildren, child)); });
        }
        var selection = GameObject.FindWithTag("EpisodeSelectionScrollviewCanvas");
        for (int i = 0; i < episodeItems.Count; i++)
        {
            if (i == 0)
            {
                FindGameObjectInChildWithTag(episodeItems[i], "EpisodeItemOutline").SetActive(true);
            }
            else
            {
                FindGameObjectInChildWithTag(episodeItems[i], "EpisodeItemOutline").SetActive(false);
            }
        }
        selection.SetActive(false);
    }

    void InitializeEnvironmentData()
    {
        environmentConstants = JSONReader.constants;
        currentEpisode = 0;
        objectSpawner.SpawnNewEpisode(environmentConstants, currentEpisode);
        environmentCenter = objectSpawner.GetWallCenter(environmentConstants.episodes[0].steps[0].Walls, 0);
        InitializeEnvironmentStateMachine();
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

        var speedDrop = playBackSpeedDropdown.GetComponent<Dropdown>();
        speedDrop.onValueChanged.AddListener(delegate
        {
            playBackSpeed = SpeedDropdownValueChanged(speedDrop);
            Debug.Log("PlaybackSpeed set to: " + playBackSpeed);

        });
        speedDrop.value = 1;
    }


    void InitializeTimelineSlider()
    {
        slider.onValueChanged.AddListener(delegate
            {
                Debug.Log("Slider changed to " + slider.value + ", currentStep: " + currentStep);

                slider.value = Mathf.Round(slider.value * 10f) / 10f;
                //Debug.Log("Timestep slider: " + slider.value);
                if (!timelapseToggle.isOn)
                {
                    Debug.Log("Timelapse turned off...");
                    timelapse = false;
                    buffering = false;
                    prevStep = currentStep;
                    currentStep = (int)slider.value;
                }
                else
                {
                    if (((int)slider.value > currentStep + 2) && !buffering && !playingSequence)
                    {
                        StartCoroutine(WaitForFinalSliderPositionForward());
                    }
                    else if (((int)slider.value < currentStep - 2) && !buffering && !playingSequence)
                    {
                        StartCoroutine(WaitForFinalSliderPositionBackward());
                    }
                }
            });
    }

    IEnumerator WaitForFinalSliderPositionForward()
    {
        if (timelapse)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Buffering forward");
            StartBuffering();
            sliderFill.color = new Color32(0, 125, 255, 255);
            sliderFill.transform.SetSiblingIndex(0);
            bufferFill.color = Color.white;
        }
        else
        {
            yield return null;
            StartBuffering();
        }
    }

    IEnumerator WaitForFinalSliderPositionBackward()
    {
        if (timelapse)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Buffering backward");
            StartBuffering();
            sliderFill.color = Color.white;
            sliderFill.transform.SetSiblingIndex(1);
            bufferFill.color = new Color32(0, 125, 255, 255);
        }
        else
        {
            yield return null;
            StartBuffering();
        }
    }

    void StartBuffering()
    {
        Debug.Log("Start buffering...");
        stepsSkipped = ((int)(slider.value - currentStep));
        buffering = true;
        PauseSequence();
    }

    void StopBuffering()
    {
        buffering = false;
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
    #endregion

    void Update()
    {
        if (!allAgentsInitialized && environmentConstants == null || !allAgentsInitialized && agentObjects.Count == 0)
        {
        }
        else if (!allAgentsInitialized && agentObjects.Count > 0)
        {
            //TODO find better solution
            allAgentsInitialized = AllAgentObjectsInitialized(currentEpisode);
        }
        else
        {
            if (playingSequence)
            {
                HandleSequenceUpdate();
            }
            if (buffering)
            {
                if (stepsSkipped > 0)
                {
                    HandleBufferUpdateForward();
                }
                if (stepsSkipped < 0)
                {
                    HandleBufferUpdateBackward();
                }
            }
            Debug.Log("CurrentStep: " + currentStep + " PrevStep: " + prevStep);
            //if (!timelapse && (currentStep != prevStep) && !playingSequence)
            if (!timelapse && !playingSequence)

            {
                LoadNewTimeStepNoTimelapse(currentEpisode, currentStep);
            }
        }
    }

    void HandleSequenceUpdate()
    {
        if (checkIfAllAgentsFinished() && !lerping)
        {
            Debug.Log("______UPDATE: LOADING NEW TIMESTEP________");
            currentStep += 1;
            LoadNewTimeStep(currentEpisode, currentStep);
        }
    }

    void HandleBufferUpdateForward()
    {
        if (!timelapse)
        {
            Debug.Log("Buffer forward");
            currentStep = (int)slider.value;
            LoadNewTimeStepNoTimelapse(currentEpisode, currentStep);
            EnablePlayerInteraction(true);
            UpdateSliderLabel();
            StopBuffering();
            resetBufferAccelleration();
        }
        else
        {
            if (bufferSlider.value < slider.value)
            {
                EnablePlayerInteraction(false);
                if (checkIfAllAgentsFinished())
                {
                    currentStep += 1;
                    LoadNewTimeStepBufferForward(currentEpisode, currentStep);
                }
            }
            if (bufferSlider.value >= slider.value - 1)
            {
                EnablePlayerInteraction(true);
                UpdateSliderLabel();
                StopBuffering();
                resetBufferAccelleration();
            }
        }
    }

    void HandleBufferUpdateBackward()
    {
        if (!timelapse)
        {
            Debug.Log("Buffer backward");
            currentStep = (int)slider.value;
            LoadNewTimeStepNoTimelapse(currentEpisode, currentStep);
            EnablePlayerInteraction(true);
            UpdateSliderLabel();
            StopBuffering();
            resetBufferAccelleration();
        }
        else
        {
            if (bufferSlider.value > slider.value)
            {
                EnablePlayerInteraction(false);
                if (checkIfAllAgentsFinished())
                {
                    currentStep -= 1;
                    LoadNewTimeStepBufferBackward(currentEpisode, currentStep);
                }
            }
            if (bufferSlider.value <= slider.value + 1)
            {
                EnablePlayerInteraction(true);
                UpdateSliderLabel();
                StopBuffering();
                resetBufferAccelleration();
            }
        }
    }

    #region Major Control Functions
    //TODO check if matches concept of buffering
    void SkipSteps(int i)
    {
        Debug.Log("Skipping steps...");
        /* PauseSequence();
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
         LoadNewTimeStep(currentEpisode, currentStep);*/
    }

    IEnumerator AnimateBufferSliderOverTime(float start, float finish, float seconds)
    {
        float animationTime = 0f;
        while (animationTime < seconds)
        {
            lerping = true;
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            bufferSlider.value = Mathf.Lerp(start, finish, lerpValue);
            Debug.Log("LERPING buffer from " + start + " to " + finish + " = " + bufferSlider.value);

            yield return null;
            lerping = false;
        }
    }
    IEnumerator AnimateBothSlidersOverTime(float start, float finish, float seconds)
    {
        float animationTime = 0f;
        Debug.Log("START LERPING: currentStep: " + currentStep);
        while (animationTime < seconds)
        {
            lerping = true;
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            slider.value = Mathf.Lerp(start, finish, lerpValue);
            bufferSlider.value = Mathf.Lerp(start, finish, lerpValue);
            Debug.Log("LERPING slider from " + start + " to " + finish + " = " + slider.value);
            Debug.Log("LERPING buffer from " + start + " to " + finish + " = " + bufferSlider.value);

            yield return null;
            Debug.Log("FINISH LERPING");
            lerping = false;

        }
    }

    void ChangeEpisode(int change)
    {
        currentEpisode = change;
        selectedEpisodeName.GetComponent<Text>().text = "Episode " + change;
        slider.value = 0;
        bufferSlider.value = 0;
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        bufferSlider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        objectSpawner.RemoveLastEpisode();
        emptyObjectLists();
        agents.Clear();
        agents = environmentConstants.episodes[currentEpisode].steps[0].Agents;
        objectSpawner.SpawnNewEpisode(environmentConstants, currentEpisode);
        environmentCenter = objectSpawner.GetWallCenter(environmentConstants.episodes[0].steps[0].Walls, 0);
        UpdateSliderLabel();
        validCounter = 0;
        invalidCounter = 0;
        for (int i = 0; i < episodeItems.Count; i++)
        {
            if (i == change)
            {
                FindGameObjectInChildWithTag(episodeItems[i], "EpisodeItemOutline").SetActive(true);
            }
            else
            {
                FindGameObjectInChildWithTag(episodeItems[i], "EpisodeItemOutline").SetActive(false);
            }
        }
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

    public void LoadNewTimeStep(int episode, int step)
    {
        Debug.Log("Loading new timestep...");
        //currentStep = step;
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabel();
        UpdateStepOverview();
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();

            agentObjController.bufferAcceleration = 1;
        }
        var temp = currentStep;
        var from = temp - 1;
        var to = currentStep;
        var longestAgentAction = GetLongestAgentAction();
        Debug.Log("____ Longest action: " + longestAgentAction);
        StartCoroutine(AnimateBothSlidersOverTime(from, to, longestAgentAction));
    }

    public void LoadNewTimeStepNoTimelapse(int episode, int step)
    {
        Debug.Log("Loading new timestep no timelapse...step: " + currentStep);
        //currentStep = step;
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        UpdateAgentsNoTimelapse(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabel();
        UpdateStepOverview();
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            //agentObjController.finished = true;
            agentObjController.bufferAcceleration = 1;
        }
    }

    public void resetBufferAccelleration()
    {
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();

            agentObjController.bufferAcceleration = 1;
        }
    }

    public void LoadNewTimeStepBufferForward(int episode, int step)
    {
        Debug.Log("Loading new timestep buffer...stepsSkipped: " + stepsSkipped);
        //currentStep = step;
        //bufferSlider.value += 1;
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabelBuffer();
        UpdateStepOverview();
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();

            agentObjController.bufferAcceleration = stepsSkipped / 5;
        }
        var temp = currentStep;
        var from = temp - 1;
        bufferSlider.value = currentStep;
        //StartCoroutine(AnimateBufferSliderOverTime(from, currentStep, GetLongestAgentAction() * stepsSkipped / 10));
    }
    public void LoadNewTimeStepBufferBackward(int episode, int step)
    {
        Debug.Log("Loading new timestep buffer...stepsSkipped: " + stepsSkipped);
        //currentStep = step;
        //bufferSlider.value += 1;
        agents = environmentConstants.episodes[currentEpisode].steps[currentStep].Agents;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateDoors(episode, step);
        UpdateItemObjects(episode, step);
        UpdateSliderLabelBuffer();
        UpdateStepOverview();
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();

            agentObjController.bufferAcceleration = Mathf.Abs(stepsSkipped) / 5;
            agentObjController.backwardBuffer = true;

        }
        var temp = currentStep;
        var from = temp - 1;
        bufferSlider.value = currentStep;
        //StartCoroutine(AnimateBufferSliderOverTime(from, currentStep, GetLongestAgentAction() * stepsSkipped / 10));
    }


    bool checkIfAllAgentsFinished()
    {

        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();

            if (!agentObjController.finished)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region SpeedDropDown
    float SpeedDropdownValueChanged(Dropdown change)
    {
        var pbSpeed = 1f;
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
    #endregion

    #region UI
    void UpdateStepOverview()
    {
        var step = environmentConstants.episodes[currentEpisode].steps[currentStep];
        var nrAgents = step.Agents.Count;
        var nrDirt = 0;
        if (step.DirtRegister != null) { nrDirt = step.DirtRegister.Count; }
        var nrItems = 0;
        if (environmentConstants.episodes[currentEpisode].steps[currentStep].ItemRegister != null)
        {
            foreach (Item item in environmentConstants.episodes[currentEpisode].steps[currentStep].ItemRegister)
            {
                if (item.x != -9999 && item.y != -9999)
                {
                    nrItems += 1;
                }
            }
        }
        agentNrText.gameObject.GetComponent<Text>().text = "" + nrAgents;
        dirtNrText.gameObject.GetComponent<Text>().text = "" + nrDirt;
        itemNrText.gameObject.GetComponent<Text>().text = "" + nrItems;
        //TODO reflect what i want to show here
        validNrText.gameObject.GetComponent<Text>().text = "" + validCounter;
        invalidNrText.gameObject.GetComponent<Text>().text = "" + invalidCounter;

    }

    void UpdateSliderLabel()
    {
        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        steptext.GetComponent<Text>().text = "" + (int)slider.value;
        sliderHandleLabel.GetComponent<Text>().text = "Step " + currentStep;

    }
    void UpdateSliderLabelBuffer()
    {
        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        steptext.GetComponent<Text>().text = "" + (int)slider.value;
        sliderHandleLabel.GetComponent<Text>().text = "Loading step " + currentStep + "/" + (int)slider.value;

    }

    void EnablePlayerInteraction(bool active)
    {
        playButton.interactable = active;
        nextButton.interactable = active;
        nextNextButton.interactable = active;
        previousButton.interactable = active;
        previousPreviousButton.interactable = active;
    }

    public void ToggleSliderTimelapse()
    {
        if (timelapseToggle.isOn)
        {
            timelapse = true;
        }
        else
        {
            timelapse = false;
        }
    }
    #endregion

    #region Update Objects - Functions
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

        if (items != null)
        {
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

    }

    public void UpdateAgents(int episode, int step)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            //********************************** FIND AGENT CONTROLLERS ************************************//
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            agentObjController.SetAgentModel(agents[i]);

            //********************************** UPDATE AGENT POSITIONS ************************************//
            UpdateAgentPositions(agents[i], agentObjController);

            //********************************** UPDATE AGENT UI *******************************************//
            agentObjController.UpdateAgentListItems(agentObjects[i], agentListObjects[i],
            agents[i].x, agents[i].y, agents[i].name, agents[i].action, agents[i].valid);

            //********************************** UPDATE AGENT ACTION ***************************************//
            agentObjController.UpdateAgentAction();
        }
    }

    public void UpdateAgentsNoTimelapse(int episode, int step)
    {
        for (int i = 0; i < agents.Count; i++)
        {
            //********************************** FIND AGENT CONTROLLERS ************************************//
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            agentObjController.SetAgentModel(agents[i]);

            //********************************** UPDATE AGENT POSITIONS ************************************//
            var pos = GetRecalculatedPosition(agents[i].x, 0, agents[i].y);
            agentObjController.SetRobotPosition(pos);

            //********************************** UPDATE AGENT UI *******************************************//
            agentObjController.UpdateAgentListItems(agentObjects[i], agentListObjects[i],
            agents[i].x, agents[i].y, agents[i].name, agents[i].action, agents[i].valid);

            //********************************** UPDATE AGENT ACTION ***************************************//
            agentObjController.UpdateAgentAction();
        }
    }

    private void UpdateAgentPositions(Agent agent, AgentController controller)
    {
        Vector3 recalcPos = GetRecalculatedPosition(agent.x, 0, agent.y);
        BroadCastSliderValueToAgentObj(controller);
        controller.goalPosition = recalcPos;
    }

    public void UpdateDirtPiles(int episode, int step)
    {
        List<Dirt> dirtPiles = environmentConstants.episodes[episode].steps[step].DirtRegister;

        if (dirtPiles != null)
        {
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
    }

    IEnumerator DeleteDirtObject(GameObject dirt)
    {
        yield return new WaitForSeconds(1);
        dirt.SetActive(false);
    }

    #endregion

    #region Sequence

    void PlaySequence()
    {
        Debug.Log("Play Sequence...");
        playingSequence = true;
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        buffering = false;
        slider.enabled = false;

        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = true;
        }
    }

    public void PauseSequence()
    {
        Debug.Log("Pause Sequence...");
        playingSequence = false;
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        slider.enabled = true;


        foreach (GameObject agentObj in agentObjects)
        {
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.playingSequence = false;
        }
    }

    #endregion

    #region Helper Methods
    void RepositionAgent(GameObject agent, Vector3 pos)
    {
        agent.transform.position = pos;
    }


    public void SpawnCleaningBubbles(Vector3 pos)
    {
        StartCoroutine(BubbleSpawn(pos));
    }

    IEnumerator BubbleSpawn(Vector3 pos)
    {
        //********************************** SPAWN BUBBLES ************************************************//
        var particleRes = Resources.Load("Prefabs/CleaningParticlesNew") as GameObject;
        var particlePos = new Vector3(pos.x, 0.3f, pos.z);
        GameObject particles = Instantiate(particleRes, particlePos, Quaternion.identity, null) as GameObject;
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();

        //********************************** WAIT ************************************************//
        yield return new WaitForSeconds(2);

        //********************************** DESTROY BUBBLES ***********************************************//
        Destroy(particles);
    }

    public Vector3 RequestAgentPosition(GameObject agent, int episode, int step)
    {
        int agentIndex = agentObjects.IndexOf(agent);
        Vector3 recalcPos = GetRecalculatedPosition(agents[agentIndex].x, 0, agents[agentIndex].y);
        return recalcPos;
    }

    bool CheckForWallCollision(Agent agent, int episode, int step)
    {
        int index = agents.IndexOf(agent);
        var agentObjController = agentObjects[index].transform.GetChild(0).GetComponent<AgentController>();

        var x = agent.x;
        var y = agent.y;

        switch (agent.action)
        {
            case AgentConstants.ACTION_NORTH: return CheckForWall(x, y - 1, episode, agentObjController);
            case AgentConstants.ACTION_NORTHEAST: return CheckForWall(x + 1, y - 1, episode, agentObjController);
            case AgentConstants.ACTION_EAST: return CheckForWall(x + 1, y, episode, agentObjController);
            case AgentConstants.ACTION_SOUTHEAST: return CheckForWall(x + 1, y + 1, episode, agentObjController);
            case AgentConstants.ACTION_SOUTH: return CheckForWall(x, y + 1, episode, agentObjController);
            case AgentConstants.ACTION_SOUTHWEST: return CheckForWall(x - 1, y + 1, episode, agentObjController);
            case AgentConstants.ACTION_WEST: return CheckForWall(x - 1, y, episode, agentObjController);
            case AgentConstants.ACTION_NORTHWEST: return CheckForWall(x - 1, y - 1, episode, agentObjController);
            default: return false; ;
        }
    }

    bool CheckForWall(int x, int y, int episode, AgentController ctr)
    {
        bool wallExists = false;
        List<Wall> walls = environmentConstants.episodes[episode].steps[0].Walls;
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

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
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
        var temp = currentStep;
        if (currentStep > 0)
        {
            foreach (Dirt dirt in environmentConstants.episodes[currentEpisode].steps[temp - 1].DirtRegister)
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

    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }

    public void SetStepTextActive(bool active)
    {
        steptext.SetActive(active);
    }

    public void AddToValidCounter(int i)
    {
        this.validCounter += i;
    }

    public void AddToInValidCounter(int i)
    {
        this.invalidCounter *= i;
    }

    float GetLongestAgentAction()
    {
        float longest = 0f;
        for (int i = 0; i < agentObjects.Count; i++)
        {
            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            var time = agentObjController.GetCurrentActionLength();

            if (time > longest)
            {
                longest = time;
            }
        }
        return longest;
    }

    #endregion

}

//==========================================================================================================//
//================================ HELPER CLASS - WAIT FRAME AMOUNT ========================================//
//==========================================================================================================//

public static class WaitFor
{
    public static IEnumerator Frames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }
}
