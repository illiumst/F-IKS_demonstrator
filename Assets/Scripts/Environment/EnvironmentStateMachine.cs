using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentStateMachine : MonoBehaviour
{
    GameObject system;
    Slider slider;

    public JSONReader JSONReader;
    public EnvironmentConstants environmentConstants;

    public List<GameObject> agentObjects = new List<GameObject>();
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
        //Debug.Log("--------------Found some JSON : #Episodes: " + environmentConstants.episodes.Count);
        //Debug.Log("--------------Found some JSON : #Timesteps episode 1: " + environmentConstants.episodes[0].steps.Count);


    }

    // Update is called once per frame
    void Update()
    {
        if(environmentConstants.episodes[0].steps[0].Agents.Count == agentObjects.Count){
            //InitializeAllAgentPositions(0);
        }
        //TODO load new step depending on slider and playButton
    }

    void InitializeAllAgentPositions(int episodeNr){
        var episode = environmentConstants.episodes[episodeNr];
        foreach (EnvironmentTimeStep step in episode.steps){
            for(int i=0; i<step.Agents.Count; i++){
                var pos = GetRecalculatedPosition(step.Agents[i].x, 0, step.Agents[i].y);
                agentObjects[i].transform.GetChild(0).GetComponent<AgentController>().positions.Add(pos);
            }
        }
        /*positions.Add(new Vector3(0, 0, 0));
        positions.Add(new Vector3(1, 0, 3));
        positions.Add(new Vector3(-5, 0, 0));
        positions.Add(new Vector3(-4, 0, 10));
        positions.Add(new Vector3(15, 0, 0));
        positions.Add(new Vector3(8, 0, -2));*/
    }
    public Vector3 GetRecalculatedPosition(float x, float y, float z)
    {
        Vector3 center = environmentCenter;
        return new Vector3(x - center.x, y, z - center.z);
    }
}
