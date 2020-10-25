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
    private AT.SerializableDictionary.SerializableDictionary<string, MapData> Maps;

    public string[] stages;

    private void Awake()
    {
        instance = this;

        string dataPath = Application.persistentDataPath;
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        try
        {
            dataPath = System.IO.Path.Combine(Application.persistentDataPath, "stages.txt");
            StreamReader reader = new StreamReader(dataPath);

            int count = int.Parse(reader.ReadLine());
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadLine());
            }
            reader.Close();
            stages = list.ToArray();
        }
        catch (System.Exception)
        {
            stages = new string[0];
            WriteFile(Application.persistentDataPath, "stages.txt", "0");
        }

        Maps = new AT.SerializableDictionary.SerializableDictionary<string, MapData>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initiate()
    {
        var grid = TilemapGridController.instance;
        currentMap = new MapData(grid.MapName, grid.Width, grid.Height);
    }

    public void SaveJson()
    {
        WriteFile(Application.persistentDataPath, currentMap.Name + ".json", JsonUtility.ToJson(currentMap));
    }
    
    public void LoadJson(string data)
    {
        currentMap = JsonUtility.FromJson<MapData>(data);
        Maps.Add(currentMap.Name, currentMap);

        TilemapGridController.instance.Initiate(StageController.instance.stages[currentMap.Name]);

        foreach (Token token in currentMap.LightBlocks.map) TilemapGridController.instance.SetTile(0, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.ShadowBlocks.map) TilemapGridController.instance.SetTile(1, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.GreyBlocks.map) TilemapGridController.instance.SetTile(2, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.LightLadders.map) TilemapGridController.instance.SetTile(3, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.ShadowLadders.map) TilemapGridController.instance.SetTile(4, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.GreyLadders.map) TilemapGridController.instance.SetTile(5, new Vector2(token.x, token.y), token.type, token.name, token.target);
        foreach (Token token in currentMap.Gimmicks.map) TilemapGridController.instance.SetTile(6, new Vector2(token.x, token.y), token.type, token.name, token.target);
        TilemapGridController.instance.FillBackgrounds();

        TilemapGridController.instance.RefreshLayers();
    }

    public MapData GetMapData(string name)
    {
        if (Maps.ContainsKey(name)) return Maps[name];
        else return new MapData("null", 0, 0);
    }

    public void Add(int layer, Vector2 pos, string type)
    {
        Layer target = GetLayer(layer);
        if (TilemapGridController.instance.GetTile(layer, pos))
            Remove(layer, pos);
        target.map.Add(new Token(pos, type));
        TilemapGridController.instance.SetTile(layer, pos, type);
    }

    public void Remove(int layer, Vector2 pos)
    {
        Layer target = GetLayer(layer);
        target.map.RemoveAll(tile => tile.x == pos.x && tile.y == pos.y);
        TilemapGridController.instance.SetTile(layer, pos, "null");
    }

    public Layer GetLayer(int layer)
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
        return target;
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

    public Layer[] Layers()
    {
        return new Layer[7] { LightBlocks, ShadowBlocks, GreyBlocks, LightLadders, ShadowLadders, GreyLadders, Gimmicks };
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
    public string name;
    public string target;
    public int x;
    public int y;
    public string type;

    public Token(Vector2 pos, string type)
    {
        this.name = type;
        this.target = "none";
        this.x = Mathf.FloorToInt(pos.x);
        this.y = Mathf.FloorToInt(pos.y);
        this.type = type;
    }

    public Token(Vector2 pos, string type, string name, string target = "none")
    {
        this.name = name;
        this.target = target;
        this.x = Mathf.FloorToInt(pos.x);
        this.y = Mathf.FloorToInt(pos.y);
        this.type = type;
    }
}
