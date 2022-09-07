using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>DestinationProperties</c> used in JSON deserialization. </summary>
[SerializeField]
public class DestinationProperties
{

    public int n_dests { get; set; }
    public int dwell_time { get; set; }
    public int spawn_frequency { get; set; }
    public bool spawn_in_other_zone { get; set; }
    public string spawn_mode { get; set; }

}
