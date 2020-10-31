using System.IO;
using System.Diagnostics;
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
    public int count;
    
    GameObject togglePrefab;
    List<GameObject> toggles = new List<GameObject>();
    public Text t;
    public Button b;

    private void Awake()
    {
        instance = this;

        togglePrefab = Resources.Load<GameObject>(string.Concat("Prefabs/Parents/StageToggle"));

        RefreshMapList();
    }

    private void Start()
    {
        RefreshMapList();
    }

    public void AddInitialStages()
    {
        string stagesData = "8\n";
        for (int i = 1; i < 9; i++)
        {
            string data = "";
            ReadFile(Application.streamingAssetsPath, string.Format("STAGE{0}.json", i), out data);
            WriteFile(Application.persistentDataPath, string.Format("STAGE{0}.json", i), data);
            stagesData += string.Format("STAGE{0}\n", i);
        }
        WriteFile(Application.persistentDataPath, string.Format("stages.txt"), stagesData);
    }

    public void RefreshMapList()
    {
        stages.Clear();
        activeStages.Clear();
        foreach(GameObject obj in toggles)
        {
            Destroy(obj);
        }
        toggles.Clear();

        string dataPath = Application.persistentDataPath;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
            AddInitialStages();
        }

        try
        {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] files = info.GetFiles();
            bool exists = false;

            foreach (FileInfo file in files)
            {
                exists = true;
                if (file.Extension == ".json")
                {
                    stages.Add(file.Name.Split('.')[0]);
                }
            }

            if (!exists)
                AddInitialStages();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(string.Format("{0} >> {1}", toggles.Count, e));
        }

        foreach (string stage in stages)
        {
            var obj = Instantiate(togglePrefab, transform);
            obj.name = stage;
            obj.GetComponentInChildren<Text>().text = stage;
            obj.GetComponent<Toggle>().isOn = false;
            obj.GetComponent<Toggle>().onValueChanged.AddListener(ToggleStage);
            toggles.Add(obj);
        }

        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, toggles.Count * 30f);

        int count = 0;
        string s = "";
        try
        {
            dataPath = System.IO.Path.Combine(Application.persistentDataPath, "stages.txt");
            StreamReader reader = new StreamReader(dataPath);

            count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; i++)
            {
                s = reader.ReadLine();
                var tog = toggles.Find(t => t.name == s).GetComponent<Toggle>();
                tog.Select();
                tog.isOn = true;
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(string.Format("{0} >> {1}", s, e));
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
                UnityEngine.Debug.LogError(ErrorMessages);
            }
            return retValue;
        }
        catch (System.Exception) { }
        return retValue;
    }

    public static bool ReadFile(string path, string fileName, out string data)
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
            StreamReader reader = new StreamReader(dataPath);
            try
            {
                data = reader.ReadLine();
                reader.Close();
                retValue = true;
            }
            catch (System.Exception ex)
            {
                data = "";
                string ErrorMessages = "File Write Error\n" + ex.Message;
                retValue = false;
            }
            return retValue;
        }
        catch (System.Exception)
        {
            data = "";
        }
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
        count = activeStages.Count;
    }

    public void WriteStages()
    {
        WriteFile(Application.persistentDataPath, "stages.txt", t.text);
    }

    public void OpenDirectory()
    {
        OpenFolder(Application.persistentDataPath);
    }

    private void OpenFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        Process.Start(folderPath);
    }
    
}
