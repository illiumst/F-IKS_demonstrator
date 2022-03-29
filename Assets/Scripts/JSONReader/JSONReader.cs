using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using SFB;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;


public class JSONReader : MonoBehaviour
{
    private string _path;
    private string _filename;

    TextAsset environmentData;
    public TextAsset episodeData;
    public string episodeDataString;

    public List<List<EnvironmentTimeStep>> environmentTimeSteps;

    public EnvironmentConstants constants;


    private void Start()
    {
        if (FileBrowserNew.selectedFileName == null)
        {
            _filename = "recorder_out_DQN.json";
        }
        else
        {
            _filename = FileBrowserNew.selectedFileName;
        }
        ReadSelectedJSONFile(_filename);
        ReadEnvironmentConstants();
    }

    private void ReadSelectedJSONFile(string _filename)
    {
        string JSONpath = null;
        episodeDataString = "";
#if UNITY_EDITOR
     JSONpath = "Assets/Resources/JSON/"+_filename;
#endif
#if UNITY_STANDALONE
         // You cannot add a subfolder, at least it does not work for me
         JSONpath = "FIKS_Demonstrator_Data/JSON_Sequences/"+_filename;
#endif

        var info = new DirectoryInfo("FIKS_Demonstrator_Data/JSON_Sequences");
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            if (file.Name.Contains(_filename))
            {
                var source = new StreamReader(file.FullName);
                episodeDataString = source.ReadToEnd();
                source.Close();
                //Debug.Log("File content: " + episodeDataString);
            }
        }
#if UNITY_EDITOR
     UnityEditor.AssetDatabase.Refresh ();
#endif
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
    private void ReadEnvironmentConstants()
    {
        constants = JsonConvert.DeserializeObject<EnvironmentConstants>(episodeDataString);
    }

    public void ReadEnvironmentData()
    {
        environmentTimeSteps = JsonConvert.DeserializeObject<List<List<EnvironmentTimeStep>>>(environmentData.text);
    }

    public void CleanBackSlashes(TextAsset textAsset)
    {
        //textAsset.text = textAsset.text.Replace("\\", ""); // --> not working since .text is ReadOnly.... 
    }

    public string GetFileName()
    {
        return this._filename;
    }
}

