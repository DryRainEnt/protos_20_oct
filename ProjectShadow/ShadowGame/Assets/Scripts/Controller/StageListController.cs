using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageListController : MonoBehaviour
{
    public static StageListController instance;

    List<string> stages = new List<string>();
    List<string> activeStages = new List<string>();
    
    GameObject togglePrefab;
    public Text t;

    private void Awake()
    {
        instance = this;

        togglePrefab = Resources.Load<GameObject>(string.Concat("Prefabs/Parents/StageToggle"));
        string dataPath = Application.persistentDataPath;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        try
        {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] files = info.GetFiles();

            foreach(FileInfo file in files)
            {
                if (file.Extension == ".json")
                    stages.Add(file.Name.Split('.')[0]);
            }
        }
        catch (System.Exception)
        {
        }

        foreach (string stage in stages)
        {
            var obj = Instantiate(togglePrefab, transform);
            obj.name = stage;
            obj.GetComponentInChildren<Text>().text = stage;
            obj.GetComponent<Toggle>().isOn = false;
            obj.GetComponent<Toggle>().onValueChanged.AddListener(ToggleStage);
        }
    }

    public void ToggleStage(bool onClick)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current.GetComponent<Toggle>().isOn)
        {
            activeStages.Add(current.name);
        }
        if (!current.GetComponent<Toggle>().isOn)
        {
            activeStages.Remove(current.name);
        }
    }

    public static bool WriteFile(string path, string fileName, string data)
    {
        bool retValue = false;
        string dataPath = path;
        try
        {
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            dataPath = System.IO.Path.Combine(path, fileName);
            StreamWriter writer = new StreamWriter(dataPath);
            try
            {
                writer.WriteLine(data);
                writer.Close();
                retValue = true;
            }
            catch (System.Exception ex)
            {
                string ErrorMessages = "File Write Error\n" + ex.Message;
                retValue = false;
                Debug.LogError(ErrorMessages);
            }
            return retValue;
        }
        catch (System.Exception) { }
        return retValue;
    }

    private void Update()
    {
        string data = string.Format("{0}\n", activeStages.Count);
        foreach(string active in activeStages)
        {
            data += active + "\n";
        }
        t.text = data;
    }

    public void WriteStages()
    {
        WriteFile(Application.persistentDataPath, "stages.txt", t.text);
    }
}
