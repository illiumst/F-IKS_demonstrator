using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentEpisode
{
    public List<EnvironmentTimeStep> steps { get; set; }

    public int episode { get; set; }

}