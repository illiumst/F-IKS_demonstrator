using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceHolder
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string tile { get; set; }
    public bool can_collide { get; set; }
}