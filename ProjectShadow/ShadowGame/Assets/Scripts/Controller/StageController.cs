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
        foreach (string s in DataController.instance.stages)
            LoadStage(s);
        ActivateStage(Constants.stages[0]);
        currentStage = Constants.stages[0];
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
        if (DataController.ReadFile(Application.streamingAssetsPath, name + ".json", out data))
        {
            TilemapGridController.instance.MapName = name;
            var stage = new GameObject();
            stage.name = name;
            stages.Add(name, stage);
            DataController.instance.LoadJson(data);
            res = true;
        }
        DeactivateStage(name);
        return res;
    }
    
    public void ActivateStage(string name)
    {
        if (!stages.ContainsKey(name)) return;
        if (currentStage == name) return;
        
        stages[name].SetActive(true);
    }

    public void DeactivateStage(string name)
    {
        if (!stages.ContainsKey(name)) return;
        if (currentStage == name) return;
        
        stages[name].SetActive(false);
    }

    public void SetCurrentStage(string name)
    {
        string temp = currentStage;
        currentStage = name;
        DeactivateStage(temp);
    }
}
