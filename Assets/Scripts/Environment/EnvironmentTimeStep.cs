using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentTimeStep
{
    public int step { get; set; }
    public List<Wall> WallTiles { get; set; }
    //public Floor FloorTiles { get; set; }
    //TODO doors sind in neuer JSON weg
    //public List<Door> Doors { get; set; }
    public List<Agent> Agents { get; set; }
    public List<PlaceHolder> PlaceHolders { get; set; }
    public List<Dirt> DirtRegister { get; set; }
    public List<DropOffLocation> DropOffLocations { get; set; }
    public List<Item> ItemRegister { get; set; }
    public List<InventoryItem> Inventories { get; set; }
    public int episode { get; set; }

}
