using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentStateMachine : MonoBehaviour
{
    public JSONReader JSONReader;
    //public List<EnvironmentInfo> EnvironmentInfoList;

    public EnvironmentConstants environmentConstants;

    //public List<WallList> walls;

    //public EnvironmentInfo CurrentEnvironmentState;


    // Start is called before the first frame update
    void Start()
    {
        JSONReader = GetComponent<JSONReader>();
        //EnvironmentInfoList = JSONReader.CreateEnvironmentInfoListFromFile();
        //CurrentEnvironmentState = EnvironmentInfoList[0];
        //Debug.Log("--------------Found some JSON: " + EnvironmentInfoList.Count);
        environmentConstants = JSONReader.ReadEnvironmentConstants();
        Debug.Log("--------------Found some JSON : #Episodes: " + environmentConstants.episodes.Count);
        Debug.Log("--------------Found some JSON : #Timesteps episode 1: " + environmentConstants.episodes[0].steps.Count);
        //Debug.Log("--------------Found some JSON : #Timesteps episode 1 Door 1: " + environmentConstants.episodes[0].steps[0].WallTiles[0].name);


    }

    // Update is called once per frame
    void Update()
    {

    }

}
