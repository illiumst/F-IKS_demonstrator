using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>DropOffLocation</c> used in JSON deserialization. </summary>

[System.Serializable]
public class DropOffLocation
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string tile { get; set; }
    public bool can_collide { get; set; }

}