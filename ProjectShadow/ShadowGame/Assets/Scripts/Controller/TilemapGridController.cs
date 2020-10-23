using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TilemapGridController : MonoBehaviour
{
    public static TilemapGridController instance;

    public GameObject currentGrid;
    public AT.SerializableDictionary.SerializableDictionary<string, GameObject> grids;

    public string MapName;
    public int Width;
    public int Height;
    
    public RuleTile lightBlock;
    public RuleTile shadowBlock;
    public RuleTile greyBlock;
    public Tile lightLadder;
    public Tile shadowLadder;
    public Tile greyLadder;
    
    public GameObject prefab;
    public List<Tilemap> layers;
    public List<string> layerNames;

    private void Awake()
    {
        instance = this;
        layers = new List<Tilemap>();
        grids = new AT.SerializableDictionary.SerializableDictionary<string, GameObject>();
    }

    public void Initiate(GameObject stage)
    {
        var obj = Instantiate(Resources.Load<GameObject>(string.Concat("Prefabs/Parents/Grid")), Vector3.zero, Quaternion.identity, stage.transform);
        grids.Add(stage.name, obj);
        currentGrid = obj;
        Initiate();
    }

    public void Initiate()
    {
        layers = new List<Tilemap>();
        for (int i = 0; i < 7; i++)
        {
            var layer = Instantiate(prefab, currentGrid.transform);
            layer.gameObject.name = layerNames[i];
            layers.Add(layer.GetComponent<Tilemap>());
            layer.GetComponent<TilemapRenderer>().sortingOrder = (i % 3 == 0 ? 1 : (i % 3 == 1 ? 2 : 3));
        }
        RefreshLayers();
    }

    public void Dispose()
    {
        WorldBehaviour.instance.TurnOnAllObjects();
        for (int i = 0; i < 7; i++)
        {
            var layer = layers[i];
            GameObject.Destroy(layer.gameObject);
            layers[i] = null;
        }
        layers.RemoveAll(x => x);
        layers.Clear();
        WorldBehaviour.instance.RefreshObjects();
    }

    public TileBase GetTile(int layer, Vector2 pos)
    {
        return layers[layer]?.GetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(layers[layer].transform.position.z)));
    }

    public void SetTile(int layer, Vector2 pos, string type, string name = "", string target = "none")
    {
        TileBase tile = null;
        bool isLight = false;
        bool isGrey = true;
        if (type == "null") return;
        if (type == "Door") type = "StartPoint";
        switch (type)
        {
            case "LightBlock": tile = lightBlock; break;
            case "ShadowBlock": tile = shadowBlock; break;
            case "GreyBlock": tile = greyBlock; break;
            case "LightLadder": tile = lightLadder; break;
            case "ShadowLadder": tile = shadowLadder; break;
            case "GreyLadder": tile = greyLadder; break;
        }

        if (type.Contains("Light"))
        {
            isGrey = false;
            isLight = true;
        }
        else if (type.Contains("Shadow"))
        {
            isGrey = false;
            isLight = false;
        }
        else if (type.Contains("Grey"))
            isGrey = true;

        if (layer == 6)
        {
            string d = "";
            try
            {
                var obj = Instantiate(Resources.Load<GameObject>(string.Concat("Prefabs/Gimmicks/", type)), pos * 16, Quaternion.identity, layers[layer].transform).GetComponent<ObjectBehaviour>();
                obj.gameObject.name = name == "" ? type : name;
                d = obj.gameObject.name;
                obj.target = target;
                obj.isLight = isLight;
                obj.isGrey = isGrey;
                var osr = obj.GetComponentInChildren<SpriteRenderer>();
                if (osr) osr.sortingOrder = isLight ? 1 : isGrey ? 3 : 2;

                if (type.Contains("Door") || type.Contains("StartPoint"))
                {
                    StageDoorPool.instance.stageDoorPool.Add(obj.gameObject.name, MapName);
                }
            }
            catch(System.ArgumentException) { Debug.LogError(string.Concat("Prefabs/Gimmicks/", d)); }
        }
        else if (tile != null) layers[layer].SetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(layers[layer].transform.position.z)), tile);
    }
    
    public void RefreshLayers()
    {
        foreach(Tilemap map in layers)
        {
            var obj = map.GetComponent<ObjectBehaviour>();
            switch (map.name)
            {
                case "LightBlocks":
                    map.gameObject.layer = LayerMask.NameToLayer("LightBlock");
                    obj.isLight = true;
                    obj.isGrey = false;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "ShadowBlocks":
                    map.gameObject.layer = LayerMask.NameToLayer("ShadowBlock");
                    obj.isLight = false;
                    obj.isGrey = false;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "GreyBlocks":
                    map.gameObject.layer = LayerMask.NameToLayer("GreyBlock");
                    obj.isLight = false;
                    obj.isGrey = true;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "LightLadders":
                    map.gameObject.layer = LayerMask.NameToLayer("LightLadder");
                    obj.isLight = true;
                    obj.isGrey = false;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "ShadowLadders":
                    map.gameObject.layer = LayerMask.NameToLayer("ShadowLadder");
                    obj.isLight = false;
                    obj.isGrey = false;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "GreyLadders":
                    map.gameObject.layer = LayerMask.NameToLayer("GreyLadder");
                    obj.isLight = false;
                    obj.isGrey = true;
                    WorldBehaviour.instance.RegistObject(obj);
                    break;
                case "Gimmicks":
                    obj.isGrey = true;
                    for(int i = 0; i < map.transform.childCount; i++)
                    {
                        WorldBehaviour.instance.RegistObject(map.transform.GetChild(i).GetComponent<ObjectBehaviour>());
                    }
                    break;
            }
        }
    }
}
