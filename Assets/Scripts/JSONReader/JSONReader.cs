using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JSONReader : MonoBehaviour
{
    public TextAsset jsonFile;

    public List<EnvironmentInfo> CreateEnvironmentInfoListFromFile()
    {
        List<List<double>> agent_coords = new List<List<double>>();
        List<EnvironmentInfo> environmentInfoList = JsonConvert.DeserializeObject<List<EnvironmentInfo>>(jsonFile.text);
        return environmentInfoList;
    }
    public EnvironmentConstants ReadEnvironmentConstants()
    {
        EnvironmentConstants contants = JsonConvert.DeserializeObject<EnvironmentConstants>(jsonFile.text);
        return contants;
    }
}
