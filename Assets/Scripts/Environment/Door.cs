using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door
{
    public string name { get; set; }

    public int x { get; set; }

    public int y { get; set; }

    public string tile { get; set; }

    public bool can_collide { get; set; }

    public string state { get; set; }

    public int time_to_close { get; set; }
   
}
