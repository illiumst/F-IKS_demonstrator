using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Item</c> used in JSON deserialization. </summary>
[System.Serializable]
public class Item
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string tile { get; set; }
    public bool can_collide { get; set; }
    public int auto_despawn { get; set; }


}