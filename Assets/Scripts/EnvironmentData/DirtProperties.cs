using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>DirtProperties</c> used in JSON deserialization. </summary>

[System.Serializable]
public class DirtProperties
{
    public float initial_dirt_ratio { get; set; }
    public float initaial_dirt_spawn_r_var { get; set; }
    public float clean_amount { get; set; }
    public float max_spawn_ratio { get; set; }
    public float max_spawn_amount { get; set; }
    public float spawn_frequency { get; set; }
    public float max_local_amount { get; set; }
    public float max_global_amount { get; set; }
    public float dirt_smear_amount { get; set; }
    public bool done_when_clean { get; set; }
}
