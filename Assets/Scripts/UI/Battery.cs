using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Battery : MonoBehaviour
{
    Image agent_battery_fill;
    Image charging_station_battery_fill;
    [SerializeField] float fill_amount = 1;
    Agent agent;

    void Start()
    {
        agent_battery_fill = this.GetComponent<Image>();
    }
    void Update()
    {
        //TODO get battery fill amount from agent
        //TODO not sure if it really needs to be in update; only needs to load in every new step
        UpdateAgentBatteryFill();
    }

    void UpdateAgentBatteryFill()
    {

        if (agent_battery_fill != null)
        {
            agent_battery_fill.fillAmount = fill_amount;
            if (fill_amount > 0.6f)
            {
                agent_battery_fill.color = new Color32(0, 255, 55, 255);
            }
            if (fill_amount < 0.6f)
            {
                agent_battery_fill.color = new Color32(255, 225, 55, 255);
            }
            if (fill_amount < 0.3f)
            {
                agent_battery_fill.color = new Color32(255, 0, 55, 255);
            }
        }
    }

    void UpdateChargingStationFill()
    {

    }
}
