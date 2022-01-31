using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using SFB;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FileBrowserOld : MonoBehaviour
{

    private string _path;
    private string _filename;
    public static string selectedFileName;
    private string episodeDataString;

    public GameObject dropdown;

    public GameObject StartButton;
    public GameObject SequenceSelectionWarningText;
    public List<string> dropDownOptions = new List<string>();



    // Start is called before the first frame update
    void Start()
    {
        if (SequenceSelectionWarningText != null)
        {
            SequenceSelectionWarningText.SetActive(false);
        }
        InitializeDropdown();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenFileBrowser()
    {
        /*
        // Open file with filter
        var extensions = new[] { new ExtensionFilter("JSON Files", "JSON", "json") };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

        WriteResult(paths);

        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
        */
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = new UnityWebRequest(url);
        loader.downloadHandler = new DownloadHandlerBuffer();
        yield return loader.SendWebRequest();

        episodeDataString = loader.downloadHandler.text;
        SaveAsJSONToResources(_filename, episodeDataString);
    }
    public void WriteResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return;
        }

        _path = "";
        _filename = "";
        for (int i = 0; i < paths.Length; i++)
        {
            _path += paths[i] + "\n";
        }
        _filename = Path.GetFileName(_path);
    }

    public void WriteResult(string path)
    {
        _path = path;
    }

    public void SaveAsJSONToResources(string filename, string content)
    {
        string JSONpath = null;
#if UNITY_EDITOR
     JSONpath = "Assets/Resources/JSON/"+filename;
#endif
#if UNITY_STANDALONE
         // You cannot add a subfolder, at least it does not work for me
         JSONpath = "FIKS_Demonstrator_Data/JSON_Sequences/"+filename;
         try
         {
            if(!Directory.Exists("FIKS_Demonstrator_Data/JSON_Sequences")){
                Directory.CreateDirectory("FIKS_Demonstrator_Data/JSON_Sequences");
            }
         }
         catch (IOException ex)
        {
            Debug.Log(ex.Message);
        }
#endif

        string str = content;
        using (FileStream fs = new FileStream(JSONpath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
            }
        }
#if UNITY_EDITOR
     UnityEditor.AssetDatabase.Refresh ();
#endif
        InitializeDropdown();
        SequenceSelectionWarningText.SetActive(false);
    }

    public void StartDemonstrator()
    {
        if (selectedFileName == null)
        {
            SequenceSelectionWarningText.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    void InitializeDropdown()
    {
        if (dropdown != null && Directory.Exists("FIKS_Demonstrator_Data/JSON_Sequences"))
        {
            var drop = dropdown.GetComponent<Dropdown>();
            drop.ClearOptions();
            string JSONpath = null;
#if UNITY_EDITOR
        JSONpath = "Assets/Resources/JSON";
#endif
#if UNITY_STANDALONE
         // You cannot add a subfolder, at least it does not work for me
         JSONpath = "FIKS_Demonstrator_Data/JSON_Sequences";
#endif
            //Debug.Log("JSON path: " + JSONpath);
            var info = new DirectoryInfo(JSONpath);
            var fileInfo = info.GetFiles();
            dropDownOptions.Add("Select a Sequence");
            foreach (FileInfo file in fileInfo)
            {
                if (file.Name.Contains(".json") && !dropDownOptions.Contains(file.Name))
                {
                    dropDownOptions.Add(file.Name);
                }
            }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh ();
#endif
            drop.AddOptions(dropDownOptions);
            drop.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(drop);
            });
            if (dropDownOptions.Count >= 1)
            {
                drop.value = dropDownOptions.Count - 1;
                selectedFileName = dropDownOptions[dropDownOptions.Count - 1];
            }
        }
    }

    void DropdownValueChanged(Dropdown change)
    {
        selectedFileName = dropDownOptions[change.value];
        if (change.value == 0)
        {
            SequenceSelectionWarningText.SetActive(true);
        }
        else
        {
            SequenceSelectionWarningText.SetActive(false);
        }
    }
}