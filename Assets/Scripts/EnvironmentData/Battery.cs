using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Battery</c> used in JSON deserialization. </summary>

[System.Serializable]
public class Battery
{
    public float charge_level { get; set; }
    public string name { get; set; }
    public string belongs_to { get; set; }
}
