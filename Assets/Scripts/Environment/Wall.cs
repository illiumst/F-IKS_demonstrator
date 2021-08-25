using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wall
{
    public string name { get; set; }
    public int x { get; set; }
    public int y { get; set; }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ConvertCoordinates(int centerX, int centerY)
    {
        this.x -= centerX;
        this.y -= centerY;
    }
}