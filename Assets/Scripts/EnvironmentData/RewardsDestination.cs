using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>RewardsDestination</c> used in JSON deserialization. </summary>

[System.Serializable]
public class RewardsDestination
{
    public float WAIT_VALID { get; set; }
    public float WAIT_FAIL { get; set; }
    public float DEST_REACHED { get; set; }

}
