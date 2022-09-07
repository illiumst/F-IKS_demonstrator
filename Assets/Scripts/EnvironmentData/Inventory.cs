using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Inventory</c> used in JSON deserialization. </summary>

[System.Serializable]
public class Inventory
{
    public bool is_blocking_light { get; set; }
    public bool can_be_shadowed { get; set; }
    public bool can_collide { get; set; }
    public int capacity { get; set; }
    public List<Item> items { get; set; }
    public string name { get; set; }
    public string belongs_to { get; set; }

}