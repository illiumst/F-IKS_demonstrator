using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementProperties
{

    public bool allow_square_movement { get; set; }
    public bool allow_diagonal_movement { get; set; }
    public bool allow_no_op { get; set; }
}
