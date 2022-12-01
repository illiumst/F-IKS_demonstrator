using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>RewardBase</c> used in JSON deserialization. </summary>

[System.Serializable]
public class RewardsBase
{
    public float MOVEMENTS_VALID { get; set; }
    public float MOVEMENTS_FAIL { get; set; }
    public float NOOP { get; set; }
    public float USE_DOOR_VALID { get; set; }
    public float USE_DOOR_FAIL { get; set; }
    public float COLLISION { get; set; }
}
