using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Action</c> used in JSON deserialization.
/// This is the top level object that holds the entire JSON informnation. </summary>
[System.Serializable]
public class EnvironmentConstants
{

    public int n_episodes { get; set; }

    public EnvironmentParameters env_params { get; set; }

    public Header header { get; set; }

    public List<EnvironmentEpisode> episodes { get; set; }

}