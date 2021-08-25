using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentState : MonoBehaviour
{
    public JSONReader JSONReader;
    public List<EnvironmentInfo> EnvironmentInfoList;

    public EnvironmentConstants environmentConstants;

    //public List<WallList> walls;

    public EnvironmentInfo CurrentEnvironmentState;


    // Start is called before the first frame update
    void Start()
    {
        JSONReader = GetComponent<JSONReader>();
        //EnvironmentInfoList = JSONReader.CreateEnvironmentInfoListFromFile();
        //CurrentEnvironmentState = EnvironmentInfoList[0];
        //Debug.Log("--------------Found some JSON: " + EnvironmentInfoList.Count);
        environmentConstants = JSONReader.ReadEnvironmentConstants();
        Debug.Log("--------------Found some JSON with walls: " + environmentConstants.walls.Count);

        Debug.Log("--------------Found some JSON with walls: " + environmentConstants.walls[0].name);

    }

    // Update is called once per frame
    void Update()
    {

    }

}
