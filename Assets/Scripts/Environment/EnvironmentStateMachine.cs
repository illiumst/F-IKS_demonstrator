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

    int currentEpisode;
    int currentStep;
    bool allAgentsInitialized = false;
    bool playingSequence = false;

    public JSONReader JSONReader;
    public EnvironmentConstants environmentConstants;

    public List<GameObject> agentObjects = new List<GameObject>();
    public List<GameObject> agentListObjects = new List<GameObject>();
    public List<GameObject> dirtObjects = new List<GameObject>();
    public List<GameObject> itemObjects = new List<GameObject>();
    public List<GameObject> zoneObjects = new List<GameObject>();

    public Vector3 environmentCenter;



    void Start()
    {
        system = GameObject.FindWithTag("System");
        slider = system.GetComponent<UIGlobals>().slider;

        JSONReader = GetComponent<JSONReader>();
        environmentConstants = JSONReader.ReadEnvironmentConstants();
        slider.maxValue = environmentConstants.episodes[0].steps.Count;
        Debug.Log("________________Episode 0 has #steps: " + slider.maxValue);
        currentEpisode = 0;
        currentStep = (int)slider.value;

        playButton = system.GetComponent<UIGlobals>().playButton;
        pauseButton = system.GetComponent<UIGlobals>().pauseButton;
        playButton.onClick.AddListener(PlaySequence);
        pauseButton.onClick.AddListener(PauseSequence);
        UpdateSliderLabel();
    }

    // Update is called once per frame
    void Update()
    {
        if(!allAgentsInitialized){
            allAgentsInitialized = AllAgentObjectsInitialized(currentEpisode);
        }
        else{
            slider.onValueChanged.AddListener(delegate
            {
                currentStep = (int)slider.value;
                UpdateAgents(currentEpisode, currentStep);
                UpdateSliderLabel();
            });

            if(playingSequence){
                HandleSequencePlayUpdate();
            }
        }

    }
    void UpdateSliderLabel(){

        var sliderHandleArea = slider.transform.GetChild(2).gameObject;
        var handle = sliderHandleArea.transform.GetChild(0).gameObject;
        var steptext = handle.transform.GetChild(0).gameObject;
        steptext.GetComponent<Text>().text= "Step "+slider.value;
        system.GetComponent<UIGlobals>().sliderStepCount.gameObject.GetComponent<Text>().text = slider.value+"/"+slider.maxValue;
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
        //UpdateAgents(episode, step);
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
        //amount
        //spawn new piles
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
                slider.value = 0f;
            }
        }
        UpdateAgentPositions(currentEpisode, (int) slider.value);
        UpdateAgentUI(currentEpisode, (int) slider.value);
    }

    void PlaySequence()
    {
        foreach(GameObject agentObj in agentObjects){
        var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
        controller.playingSequence = true;
        playingSequence = true;
        }
    }

    void PauseSequence()
    {
        foreach(GameObject agentObj in agentObjects){
        var controller = agentObj.transform.GetChild(0).GetComponent<AgentController>();
        controller.playingSequence = false;
        playingSequence = false;
        }
    }


}
