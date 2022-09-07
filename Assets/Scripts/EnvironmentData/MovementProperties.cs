using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>MovementPRoperties</c> used in JSON deserialization. </summary>
[System.Serializable]
public class MovementProperties
{
    public bool allow_square_movement { get; set; }
    public bool allow_diagonal_movement { get; set; }
    public bool allow_no_op { get; set; }
}
