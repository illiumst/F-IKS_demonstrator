using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using SimpleFileBrowser;
using TMPro;


public class FileBrowserCustom : MonoBehaviour
{

    private string _path;
    private string _filename;
    public static string selectedFileName;
    private string episodeDataString;

    public GameObject dropdown;

    public GameObject StartButton;
    public GameObject SequenceSelectionWarningText;
    public List<string> dropDownOptions = new List<string>();
    public List<string> fileNames = new List<string>();
    public List<string> filePaths = new List<string>();

    public List<GameObject> itemButtons = new List<GameObject>();

    public GameObject ListItemObject;

    public GameObject FileScrollViewContent;





    // Start is called before the first frame update
    void Start()
    {
        if (SequenceSelectionWarningText != null)
        {
            SequenceSelectionWarningText.SetActive(false);
        }
        FileScrollViewContent = GameObject.FindWithTag("FileScrollViewContent");
        ListItemObject = Resources.Load("Prefabs/FileItemButton") as GameObject;

        FillScrollView();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenFileBrowser()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("JSON Files", ".json", ".JSON"));
        StartCoroutine(ShowLoadDialogCoroutine());

    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Import a Sequence", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            _filename = "";
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
            }

            // Read the bytes of the first file via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            episodeDataString = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result[0]);
            _filename = Path.GetFileName(FileBrowser.Result[0]);
            SaveAsJSONToResources(_filename, episodeDataString);

        }
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
                FillScrollView();
                SequenceSelectionWarningText.SetActive(false);
    }

    public void StartDemonstrator(string fileName)
    {
        selectedFileName = fileName;
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
                    });
                    if (dropDownOptions.Count >= 1)
                    {
                        drop.value = dropDownOptions.Count - 1;
                        selectedFileName = dropDownOptions[dropDownOptions.Count - 1];
                    }
                }
    }

    void FillScrollView()
    {
                if (FileScrollViewContent != null && Directory.Exists("FIKS_Demonstrator_Data/JSON_Sequences"))
                {
                    string JSONpath = null;
        #if UNITY_EDITOR
                JSONpath = "Assets/Resources/JSON";
        #endif
        #if UNITY_STANDALONE
                // You cannot add a subfolder, at least it does not work for me
                JSONpath = "FIKS_Demonstrator_Data/JSON_Sequences";
        #endif
                    var info = new DirectoryInfo(JSONpath);
                    var fileInfo = info.GetFiles();
                    foreach (FileInfo file in fileInfo)
                    {
                        if (file.Name.Contains(".json") && !fileNames.Contains(file.Name))
                        {
                            //JSONReader reader = new JSONReader();
                            //EnvironmentConstantsHeaderOnly const = reader.ReadEnvironmentConstantsHeaderOnly(reader.ReadSelectedJSONFileHeader(file.Name));
                            var newItem = Instantiate(ListItemObject) as GameObject;
                            newItem.transform.SetParent(FileScrollViewContent.transform, false);
                            newItem.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().SetText(file.Name);
                            newItem.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().SetText(JSONpath + "/" + file.Name);
                            itemButtons.Add(newItem);
                            newItem.GetComponent<Button>().onClick.AddListener(()
                            => StartDemonstrator(file.Name));
                        }
                    }
        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh ();
        #endif
                    if (itemButtons.Count > 0)
                    {
                        itemButtons[0].GetComponent<Button>().Select();
                    }

                }
            }
        }