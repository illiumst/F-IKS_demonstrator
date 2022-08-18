using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentConstants
{

    public int n_episodes { get; set; }

    public Header header { get; set; }
    
    public List<EnvironmentEpisode> episodes { get; set; }

}