using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MODEL Class <c>EnvironmentParameters</c> used in JSON deserialization. </summary>

[System.Serializable]
public class EnvironmentParameters
{
    public BatteryProperties? btry_prop { get; set; }
    public RewardsDestination? rewards_dest { get; set; }
    public DestinationProperties? dest_prop { get; set; }
    public DirtProperties? dirt_prop { get; set; }
    public RewardsDirt? rewards_dirt { get; set; }
    public ItemProperties? item_prop { get; set; }
    public RewardsItem? rewards_item { get; set; }
    public long env_seed { get; set; }
    public MovementProperties mv_prop { get; set; }
    public ObservationProperties obs_prop { get; set; }
    public RewardsBase rewards_base { get; set; }
    public string level_name { get; set; }
    public bool verbose { get; set; }
    public int n_agents { get; set; }
    public int max_steps { get; set; }
    public bool done_at_collision { get; set; }
    public bool parse_doors { get; set; }
    public bool doors_have_area { get; set; }
    public bool individual_rewards { get; set; }
    public string class_name { get; set; }
}
