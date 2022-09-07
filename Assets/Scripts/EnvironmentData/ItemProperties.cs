using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>ItemProperties</c> used in JSON deserialization. </summary>

[System.Serializable]
public class ItemProperties
{
    public int n_items { get; set; }
    public int spawn_frequency { get; set; }
    public int n_drop_off_locations { get; set; }
    public int max_dropoff_storage_size { get; set; }
    public int max_agent_inventory_capacity { get; set; }
}
