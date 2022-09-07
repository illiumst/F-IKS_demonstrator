using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>ObservationProperties</c> used in JSON deserialization. </summary>

[System.Serializable]
public class ObservationProperties
{
    public string render_agents { get; set; }
    public bool omit_agent_self { get; set; }
    public PlaceHolder additional_agent_placeholder { get; set; }
    public bool cast_shadows { get; set; }
    public int frames_to_stack { get; set; }
    public int pomdp_r { get; set; }
    public bool indicate_door_area { get; set; }
    public bool show_global_position_info { get; set; }
}
