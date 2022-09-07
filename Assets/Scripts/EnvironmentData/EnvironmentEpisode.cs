using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary> MODEL Class <c>EnvironmentEpisode</c> used in JSON deserialization. 
/// Contains a list of all the steps. </summary>
public class EnvironmentEpisode
{
    public List<EnvironmentTimeStep> steps { get; set; }

    public int episode { get; set; }

}