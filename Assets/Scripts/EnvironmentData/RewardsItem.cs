using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>RewardsItem</c> used in JSON deserialization. </summary>

[System.Serializable]
public class RewardsItem
{
    public float DROP_OFF_VALID { get; set; }
    public float DROP_OFF_FAIL { get; set; }
    public float PICK_UP_FAIL { get; set; }
    public float PICK_UP_VALID { get; set; }
}
