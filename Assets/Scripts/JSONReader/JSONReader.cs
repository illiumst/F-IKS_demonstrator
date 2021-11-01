using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JSONReader : MonoBehaviour
{
    public TextAsset environmentData;
    public TextAsset episodeData;

    public List<List<EnvironmentTimeStep>> environmentTimeSteps;

    private void Start()
    {
        //ReadEnvironmentData();
        //CleanBackSlashes(episodeData);
    }

    public List<EnvironmentInfo> CreateEnvironmentInfoListFromFile()
    {
        List<List<double>> agent_coords = new List<List<double>>();
        var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
        List<EnvironmentInfo> environmentInfoList = JsonConvert.DeserializeObject<List<EnvironmentInfo>>(environmentData.text, settings);
        return environmentInfoList;
    }
    public EnvironmentConstants ReadEnvironmentConstants()
    {
        EnvironmentConstants contants = JsonConvert.DeserializeObject<EnvironmentConstants>(episodeData.text);
        return contants;
    }

    public void ReadEnvironmentData()
    {
        environmentTimeSteps = JsonConvert.DeserializeObject<List<List<EnvironmentTimeStep>>>(environmentData.text);
    }

    public void CleanBackSlashes(TextAsset textAsset)
    {
        //textAsset.text = textAsset.text.Replace("\\", ""); // --> not working since .text is ReadOnly.... 
    }
}
