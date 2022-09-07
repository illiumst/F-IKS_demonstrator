using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Destination</c> used in JSON deserialization. </summary>

[System.Serializable]
public class Destination
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string tile { get; set; }
    public bool can_collide { get; set; }
    public List<Agent>? per_agent_times { get; set; }
    public float dwell_time { get; set; }
}
