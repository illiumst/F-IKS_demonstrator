using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentStateMachine : MonoBehaviour
{
    GameObject system;
    Slider slider;
    Button playButton;
    Button pauseButton;
    Button nextButton;
    Button nextNextButton;
    Button previousButton;
    Button previousPreviousButton;
    GameObject episodeSelectionDropdown;
    GameObject playBackSpeedDropdown;



    int currentEpisode;
    int currentStep;
    bool allAgentsInitialized = false;
    //bool episodeSpawned = false;
    bool playingSequence = false;

    public JSONReader JSONReader;
    public EnvironmentConstants environmentConstants;
    public float playBackSpeed = 1f;

    public List<GameObject> agentObjects = new List<GameObject>();
    public List<GameObject> agentListObjects = new List<GameObject>();
    public List<GameObject> dirtObjects = new List<GameObject>();
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> zoneObjects = new List<GameObject>();
    
    public List<string> dropDownOptions = new List<string>();

    public Vector3 environmentCenter;



    void Start()
    {
        system = GameObject.FindWithTag("System");
        slider = system.GetComponent<UIGlobals>().slider;
        episodeSelectionDropdown = GameObject.FindWithTag("EpisodeSelectionDropdown");
        playBackSpeedDropdown = GameObject.FindWithTag("PlaybackSpeedDropdown");


        JSONReader = GetComponent<JSONReader>();
        environmentConstants = JSONReader.ReadEnvironmentConstants();
        currentEpisode = 0;
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
       // Debug.Log("________________Episode 0 has #steps: " + slider.maxValue);
        currentStep = (int)slider.value;

        playButton = system.GetComponent<UIGlobals>().playButton;
        pauseButton = system.GetComponent<UIGlobals>().pauseButton;
        nextButton = system.GetComponent<UIGlobals>().nextButton;
        nextNextButton = system.GetComponent<UIGlobals>().nextNextButton;
        previousButton = system.GetComponent<UIGlobals>().previousButton;
        previousPreviousButton = system.GetComponent<UIGlobals>().previousPreviousButton;
        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);

        nextButton.onClick.AddListener(delegate {SkipSteps(1);});
        nextNextButton.onClick.AddListener(delegate {SkipSteps(10);});
        previousButton.onClick.AddListener(delegate {SkipSteps(-1);});
        previousPreviousButton.onClick.AddListener(delegate {SkipSteps(-10);});

        UpdateSliderLabel();
        InitializeEpisodeSelectionDropdown();
        InitializePlaybackSpeedDropdown();
        var drop = episodeSelectionDropdown.GetComponent<Dropdown>();
        drop.onValueChanged.AddListener(delegate {
                DropdownValueChanged(drop);
            });
        var speedDrop = playBackSpeedDropdown.GetComponent<Dropdown>();
        speedDrop.onValueChanged.AddListener(delegate {
                SpeedDropdownValueChanged(speedDrop);
            });
        speedDrop.value = 3;

    }

    // Update is called once per frame
    void Update()
    {
        if(!allAgentsInitialized){
            allAgentsInitialized = AllAgentObjectsInitialized(currentEpisode);
        }
        else{
            /*if(!episodeSpawned){
                system.GetComponent<ObjectSpawner>().SpawnNewEpisode(currentEpisode);
            }*/
            slider.onValueChanged.AddListener(delegate
            {
                LoadNewTimeStep(currentEpisode, currentStep);
            });

            if(playingSequence){
                HandleSequencePlayUpdate();
            }
        }

    }

    void SkipSteps(int i){
        PauseSequence();
        if(i>0){
            if(slider.value+i>slider.maxValue){
                slider.value = (slider.value+i)-slider.maxValue;
            }
            else{
                slider.value = slider.value+i;
            }
        }
        else{
             if(slider.value+i<0){
                 slider.value = (slider.value+i)+slider.maxValue;
             }
             else{
                 slider.value = slider.value+i;
             }
        }
        LoadNewTimeStep(currentEpisode, currentStep);
    }

    void DropdownValueChanged(Dropdown change)
    {
        currentEpisode = change.value;
        slider.value = 0;
        slider.maxValue = environmentConstants.episodes[currentEpisode].steps.Count;
        system.GetComponent<ObjectSpawner>().RemoveLastEpisode();
        emptyObjectLists();
        system.GetComponent<ObjectSpawner>().SpawnNewEpisode(currentEpisode);
        UpdateSliderLabel();
    }
    void SpeedDropdownValueChanged(Dropdown change)
    {
        playBackSpeed = (change.value+1f)/4f;
        Debug.Log("PlaybackSpeed set to: "+playBackSpeed);
    }

    void emptyObjectLists(){
        agentObjects.Clear();
        agentListObjects.Clear();
        dirtObjects.Clear();
        itemObjects.Clear();
        zoneObjects.Clear();
    }

    void InitializeEpisodeSelectionDropdown(){
        Debug.Log("---------------DROPDOWN Initialization: #episodes "+environmentConstants.episodes.Count);
        var drop = episodeSelectionDropdown.GetComponent<Dropdown>();
        drop.ClearOptions();
        for(int i = 0; i<environmentConstants.episodes.Count; i++){
            dropDownOptions.Add("Episode "+(i+1));
        }
        drop.AddOptions(dropDownOptions);
    }

    void InitializePlaybackSpeedDropdown(){
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

    void UpdateSliderLabel(){

        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        var handle = sliderHandleArea.transform.GetChild(0).gameObject;
        var steptext = handle.transform.GetChild(0).gameObject;
        steptext.GetComponent<Text>().text= "Step "+slider.value;
        system.GetComponent<UIGlobals>().sliderStepCount.gameObject.GetComponent<Text>().text = "Step "+slider.value+" / "+slider.maxValue;
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
        currentStep = (int)slider.value;
        UpdateAgents(episode, step);
        UpdateDirtPiles(episode, step);
        UpdateSliderLabel();    
                
    }

    public void UpdateAgents(int episode, int step)
    {
        UpdateAgentUI(episode, step);
        UpdateAgentPositions(episode, step);
        //action
    }

    private void UpdateAgentPositions(int episode, int step)
    {
        //Debug.Log("_______________ Updating Agent Position");
        List<Agent> agents = environmentConstants.episodes[episode].steps[step].Agents;

        for (int i = 0; i < agents.Count; i++)
        {
            Vector3 recalcPos = GetRecalculatedPosition(agents[i].x, 0, agents[i].y);
            //Debug.Log("_______________ Updating Agent Positions: " + agents[i].name + " originalPos: " + agents[i].x + " " + agents[i].y + " recalc: " +
            //recalcPos.x + " " + recalcPos.y + " " + recalcPos.z);

            var agentObjController = agentObjects[i].transform.GetChild(0).GetComponent<AgentController>();
            BroadCastSliderValueToAgentObj(agentObjController);

            agentObjController.endGoalPosition = recalcPos;
        }
    }

    private void BroadCastSliderValueToAgentObj(AgentController controller)
    {
        controller.sliderValue = slider.value;
    }

    private void UpdateAgentUI(int episode, int step)
    {
        //Debug.Log("_______________ Updating Agent UI");
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

        foreach(Dirt dirt in dirtPiles){
            if(GetDirtObjectByName(dirt.name)!=null){
                var dirtInPrevStep = GetDirtByNamePrevStep(dirt.name);
                if(dirtInPrevStep!=null && dirtInPrevStep.amount!=dirt.amount){
                    var dirtObj = GetDirtObjectByName(dirt.name);
                    var scaleFactor = Mathf.Sqrt((float)dirt.amount);
                    dirtObj.transform.localScale = new Vector3(scaleFactor, 0.001f, scaleFactor);
                }
            }
            else{
                var pos = GetRecalculatedPosition(dirt.x, 0, dirt.y);
                system.GetComponent<ObjectSpawner>().spawnDirt(pos, dirt.amount, dirt.name);
            }
        }
        foreach(GameObject dirtObj in dirtObjects){
            if(GetDirtByName(dirtObj.name)==null){

                Debug.Log("Dirt disappeared: "+dirtObj.name+" in step: "+currentStep);
                dirtObj.SetActive(false);
                //dirtObjects.Remove(dirtObj);
                //Destroy(dirtObj);
            }
        }
    }

    public void HandleSequencePlayUpdate()
    {

        foreach(GameObject agentObj in agentObjects){
            var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
            controller.goalPosition = controller.endGoalPosition;
            Debug.Log(" Agents current pos: "+controller.currentpos.x+", "+controller.currentpos.y +", "+controller.currentpos.z);
            Debug.Log(" Agents end goal pos: "+controller.endGoalPosition.x+", "+controller.endGoalPosition.y +", "+controller.endGoalPosition.z);
            Debug.Log(" Agents goal pos: "+controller.goalPosition.x+", "+controller.goalPosition.y +", "+controller.goalPosition.z);


            if (slider.value == 0f)
            {
                slider.value = 1f;
            }
            else if (controller.currentpos == controller.goalPosition && slider.value < (slider.maxValue -1f))
            {
                slider.value += 1f;
            }
            else if ((controller.currentpos == controller.goalPosition) && (slider.value == (slider.maxValue - 1f)))
            {
                Debug.Log("-----------Playback slider value finished list: " + slider.value);
                //TODO respawn robot here -> so it doesnt move through walls
                var pos = RequestAgentPosition(agentObj, currentEpisode, 0);
                RepositionAgent(agentObj, pos);
                slider.value = 0f;
            }
        }
        UpdateAgentPositions(currentEpisode, (int) slider.value);
        UpdateAgentUI(currentEpisode, (int) slider.value);
    }

    void RepositionAgent(GameObject agent, Vector3 pos){
        agent.transform.position = pos;
    }

    void PlaySequence()
    {
        foreach(GameObject agentObj in agentObjects){
        var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
        controller.playingSequence = true;
        playingSequence = true;
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        }
    }

    void PauseSequence()
    {
        foreach(GameObject agentObj in agentObjects){
        var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
        controller.playingSequence = false;
        playingSequence = false;
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        }
    }

    public Agent GetAgentByName(string name){
        foreach(Agent agent in environmentConstants.episodes[currentEpisode].steps[currentStep].Agents){
            if(agent.name.Equals(name)){
                return agent;
            }
        }
        return null;
    }
    public Dirt GetDirtByName(string name){
        foreach(Dirt dirt in environmentConstants.episodes[currentEpisode].steps[currentStep].DirtRegister){
            if(dirt.name.Equals(name)){
                return dirt;
            }
        }
        return null;
    }

    public Dirt GetDirtByNamePrevStep(string name){
        if(currentStep>0){
            foreach(Dirt dirt in environmentConstants.episodes[currentEpisode].steps[currentStep-1].DirtRegister){
                if(dirt.name.Equals(name)){
                    return dirt;
                }
            }
        }
        return null;
    }

    public GameObject GetDirtObjectByName(string name){
        foreach(GameObject dirt in dirtObjects){
            if(dirt.name.Equals(name)){
                return dirt;
            }
        }
        return null;
    }

     public Item GetItemByName(string name){
        foreach(Item item in environmentConstants.episodes[currentEpisode].steps[currentStep].ItemRegister){
            if(item.name.Equals(name)){
                return item;
            }
        }
        return null;
    }


}
