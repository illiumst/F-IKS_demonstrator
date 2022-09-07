using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class <c>BatteryBlinkPulse</c> created an outline around the agent that can blink. 
/// In the future it is intended to be used to indicate and draw attention to a low battery status. 
/// This script is assigned to the Robo3 of the Agent prefab.</summary>
public class BatteryBlinkPulse : MonoBehaviour
{
    private Outline outline;

    public float minimum = 0.0f;
    public float maximum = 1f;
    public float cyclesPerSecond = 1.0f;
    private float a;
    private bool increasing = true;

    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        a = maximum;
    }



    void Update()
    {
        float t = Time.deltaTime;
        if (a >= maximum) increasing = false;
        if (a <= minimum) increasing = true;
        a = increasing ? a += t * cyclesPerSecond * 2 : a -= t * cyclesPerSecond;
        outline.OutlineWidth = 10 * a;
    }
}