using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>Wall</c> used in JSON deserialization. </summary>

[System.Serializable]
public class Wall
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }

    public void ConvertCoordinates(int centerX, int centerY)
    {
        this.x -= centerX;
        this.y -= centerY;
    }
}