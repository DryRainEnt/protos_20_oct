using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public static StageController instance;

    public AT.SerializableDictionary.SerializableDictionary<string, GameObject> stages;

    public string currentStage;

    private void Awake()
    {
        instance = this;
        stages = new AT.SerializableDictionary.SerializableDictionary<string, GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var firstStage = "null";
        foreach (string s in DataController.instance.stages)
        {
            if (firstStage == "null") firstStage = s;
            LoadStage(s);
        }
        ActivateStage(firstStage);
        currentStage = firstStage;
        WorldBehaviour.player.ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LoadStage(string name)
    {
        bool res = false;

        string data = "";
        if (DataController.ReadFile(Application.persistentDataPath, name + ".json", out data))
        {
            TilemapGridController.instance.MapName = name;
            var stage = new GameObject();
            stage.name = name;
            stages.Add(name, stage);
            DataController.instance.LoadJson(data);
            res = true;
        }

        Debug.Log(string.Format("Stage [{0}] Loaded", name));
        DeactivateStage(name);
        return res;
    }
    
    public void ActivateStage(string name)
    {
        if (!stages.ContainsKey(name))
        {
            Debug.Log(string.Format("Stage [{0}] Not in Pool!", name));
            return;
        }
        if (currentStage == name) return;
        
        stages[name].SetActive(true);
        DataController.instance.currentMap = DataController.instance.GetMapData(name);

        Debug.Log(string.Format("Stage [{0}] Activated", name));
    }

    public void DeactivateStage(string name)
    {
        if (!stages.ContainsKey(name)) return;
        if (currentStage == name) return;
        
        stages[name].SetActive(false);

        Debug.Log(string.Format("Stage [{0}] Deactivated", name));
    }

    public void SetCurrentStage(string name)
    {
        string temp = currentStage;
        currentStage = name;
        DeactivateStage(temp);

        Debug.Log(string.Format("Now Current Stage is [{0}]", name));
    }
}
