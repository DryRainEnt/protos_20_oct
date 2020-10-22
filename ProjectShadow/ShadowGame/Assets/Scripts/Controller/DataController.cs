using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;
    public MapData currentMap;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Initiate()
    {
        var grid = TilemapGridController.instance;
        currentMap = new MapData(grid.MapName, grid.Width, grid.Height);
    }

    public void SaveJson()
    {
        WriteFile(Application.streamingAssetsPath, currentMap.Name + ".json", JsonUtility.ToJson(currentMap));
    }

    public void LoadJson()
    {
        string data = JsonUtility.ToJson(new MapData("", 0, 0));
        ReadFile(Application.streamingAssetsPath, TilemapGridController.instance.MapName + ".json", out data);
        currentMap = JsonUtility.FromJson<MapData>(data);

        TilemapGridController.instance.Initiate();

        foreach (Token token in currentMap.LightBlocks.map) TilemapGridController.instance.SetTile(0, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.ShadowBlocks.map) TilemapGridController.instance.SetTile(1, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.GreyBlocks.map) TilemapGridController.instance.SetTile(2, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.LightLadders.map) TilemapGridController.instance.SetTile(3, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.ShadowLadders.map) TilemapGridController.instance.SetTile(4, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.GreyLadders.map) TilemapGridController.instance.SetTile(5, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.Gimmicks.map) TilemapGridController.instance.SetTile(6, new Vector2(token.x, token.y), token.type);

        TilemapGridController.instance.RefreshLayers();
    }

    public void LoadJson(string data)
    {
        currentMap = JsonUtility.FromJson<MapData>(data);

        TilemapGridController.instance.Initiate();

        foreach (Token token in currentMap.LightBlocks.map) TilemapGridController.instance.SetTile(0, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.ShadowBlocks.map) TilemapGridController.instance.SetTile(1, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.GreyBlocks.map) TilemapGridController.instance.SetTile(2, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.LightLadders.map) TilemapGridController.instance.SetTile(3, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.ShadowLadders.map) TilemapGridController.instance.SetTile(4, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.GreyLadders.map) TilemapGridController.instance.SetTile(5, new Vector2(token.x, token.y), token.type);
        foreach (Token token in currentMap.Gimmicks.map) TilemapGridController.instance.SetTile(6, new Vector2(token.x, token.y), token.type);

    }

    public void Add(int layer, Vector2 pos, string type)
    {
        Layer target;
        switch (layer)
        {
            case 0: target = currentMap.LightBlocks; break;
            case 1: target = currentMap.ShadowBlocks; break;
            case 2: target = currentMap.GreyBlocks; break;
            case 3: target = currentMap.LightLadders; break;
            case 4: target = currentMap.ShadowLadders; break;
            case 5: target = currentMap.GreyLadders; break;
            default: target = currentMap.Gimmicks; break;
        }
        if (TilemapGridController.instance.GetTile(layer, pos))
            Remove(layer, pos);
        target.map.Add(new Token(pos, type));
        TilemapGridController.instance.SetTile(layer, pos, type);
    }

    public void Remove(int layer, Vector2 pos)
    {
        Layer target;
        switch (layer)
        {
            case 0: target = currentMap.LightBlocks; break;
            case 1: target = currentMap.ShadowBlocks; break;
            case 2: target = currentMap.GreyBlocks; break;
            case 3: target = currentMap.LightLadders; break;
            case 4: target = currentMap.ShadowLadders; break;
            case 5: target = currentMap.GreyLadders; break;
            default: target = currentMap.Gimmicks; break;
        }
        target.map.RemoveAll(tile => tile.x == pos.x && tile.y == pos.y);
        TilemapGridController.instance.SetTile(layer, pos, "null");
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
                Debug.LogError(ErrorMessages);
            }
            return retValue;
        }
        catch (System.Exception)
        {
            data = "";
        }
        return retValue;
    }

}

[Serializable]
public class MapData
{
    public string Name;
    public int Width;
    public int Height;
    public Layer LightBlocks;
    public Layer ShadowBlocks;
    public Layer GreyBlocks;
    public Layer LightLadders;
    public Layer ShadowLadders;
    public Layer GreyLadders;
    public Layer Gimmicks;

    public MapData(string name, int w, int h)
    {
        Name = name;
        Width = w;
        Height = h;

        LightBlocks = new Layer();
        ShadowBlocks = new Layer();
        GreyBlocks = new Layer();
        LightLadders = new Layer();
        ShadowLadders = new Layer();
        GreyLadders = new Layer();
        Gimmicks = new Layer();
    }
}

[Serializable]
public class Layer
{
    [SerializeField]
    public List<Token> map;

    public Layer()
    {
        map = new List<Token>();
    }
}

[Serializable]
public class Token
{
    public int x;
    public int y;
    public string type;

    public Token(Vector2 pos, string type)
    {
        this.x = Mathf.FloorToInt(pos.x);
        this.y = Mathf.FloorToInt(pos.y);
        this.type = type;
    }
}
