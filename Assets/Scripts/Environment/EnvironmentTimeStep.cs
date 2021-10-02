using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentTimeStep
{
    public int step { get; set; }
    public List<Door> Doors { get; set; }
    public List<Agent> Agents { get; set; }
    public List<Dirt> DirtRegister { get; set; }
    public List<DropOffLocation> DropOffLocations { get; set; }
    public List<Item> ItemRegister { get; set; }
    public int episode { get; set; }

}
