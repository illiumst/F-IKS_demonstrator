using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class EnvironmentTimeStep
{
    public int step { get; set; }
    public List<Wall>? Walls { get; set; }
    public Floor Floors { get; set; }
    public List<Door> Doors { get; set; }
    public List<Agent> Agents { get; set; }
    public DestinationProperties? Destinations { get; set; }
    public DestinationProperties? ReachedDestinations { get; set; }
    public List<BatteryProperties>? BatteriesRegister { get; set; }
    public List<Pod>? ChargePods { get; set; }
    public List<Dirt>? DirtRegister { get; set; }
    public List<DropOffLocation>? DropOffLocations { get; set; }
    public List<Item>? ItemRegister { get; set; }
    [JsonIgnore]
    public InventoryItem? Inventories { get; set; }
    public int episode { get; set; }

}
