using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentState : MonoBehaviour
{
    public JSONReader JSONReader;
    public List<EnvironmentInfo> EnvironmentInfoList;

    public EnvironmentInfo CurrentEnvironmentState;

    // Start is called before the first frame update
    void Start()
    {
        JSONReader = GetComponent<JSONReader>();
        EnvironmentInfoList = JSONReader.createEnvironmentInfoListFromFile();
        CurrentEnvironmentState = EnvironmentInfoList[0];
        Debug.Log("--------------Found some JSON: " + EnvironmentInfoList.Count);

    }

    // Update is called once per frame
    void Update()
    {

    }

}
