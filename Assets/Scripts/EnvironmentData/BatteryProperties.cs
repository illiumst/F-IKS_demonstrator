using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BatteryProperties
{
    public float initial_charge { get; set; }
    public float charge_rate { get; set; }
    public int charge_locations { get; set; }
    public float per_action_costs { get; set; }
    public bool done_when_discharged { get; set; }
    public bool multi_charge { get; set; }

}


