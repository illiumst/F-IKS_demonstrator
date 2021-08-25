using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentConstants
{
    public List<Wall> walls { get; set; }
    public int centerX = 5;
    public int centerY = 5;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Wall wall in walls)
        {
            wall.ConvertCoordinates(centerX, centerY);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}