using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Header</c> used in JSON deserialization. It contains all fixed components of the environment 
/// like walls, floors, dropOffLocations, charging pods...</summary>

[System.Serializable]
public class Header
{
    public int rec_step { get; set; }
    public List<Wall>? rec_Walls { get; set; }
    public List<Floor>? rec_Floors { get; set; }
    public List<DropOffLocation>? rec_DropOffLocations { get; set; }
    public List<Pod>? rec_ChargePods { get; set; }
    public List<Action>? actions { get; set; }

}
