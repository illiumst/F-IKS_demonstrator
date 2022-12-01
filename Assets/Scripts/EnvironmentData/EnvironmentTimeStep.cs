using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary> MODEL Class <c>EnvironmentTimeStep</c> used in JSON deserialization. </summary>

[System.Serializable]
public class EnvironmentTimeStep
{
    public int step { get; set; }
    public List<Door>? Doors { get; set; }
    public List<Agent> Agents { get; set; }
    public List<Item>? Items { get; set; }
    public List<Inventory>? Inventories { get; set; }
    public List<Dirt>? DirtPiles { get; set; }
    public List<Destination>? Destinations { get; set; }
    public List<Destination>? ReachedDestinations { get; set; }
    public List<Battery>? Batteries { get; set; }

}
