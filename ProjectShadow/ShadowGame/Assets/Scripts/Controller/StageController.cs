using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public static StageController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LoadStage(string name)
    {
        this.gameObject.name = name;

        string data = "";
        if (DataController.ReadFile(Application.streamingAssetsPath, name + ".json", out data))
        {
            TilemapGridController.instance.MapName = name;
            DataController.instance.LoadJson(data);
            WorldBehaviour.player.ResetPosition();
            return true;
        }
        return false;
    }
    
}
