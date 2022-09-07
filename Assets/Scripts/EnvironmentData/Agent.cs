using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Agent</c> used in JSON deserialization. </summary>

[System.Serializable]
public class Agent
{
    public string name { get; set; }

    public int x { get; set; }

    public int y { get; set; }

    public string tile { get; set; }

    public bool can_collide { get; set; }

    public bool valid { get; set; }

    public string action { get; set; }


}
