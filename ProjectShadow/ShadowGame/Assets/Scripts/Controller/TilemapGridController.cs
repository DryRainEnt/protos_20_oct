using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TilemapGridController : MonoBehaviour
{
    public static TilemapGridController instance;
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        layers = new List<Tilemap>();
        for (int i = 0; i < 7; i++)
        {
            if (!GameObject.Find(layerNames[i]))
            {
                var layer = Instantiate(prefab, transform);
                layer.gameObject.name = layerNames[i];
                layers.Add(layer.GetComponent<Tilemap>());
            }
            else
                layers.Add(GameObject.Find(layerNames[i]).GetComponent<Tilemap>());
        }
    }

    public void Initiate()
    {
        if (layers.Count > 0) Dispose();
        layers = new List<Tilemap>();
        for (int i = 0; i < 7; i++)
        {
            var layer = Instantiate(prefab, transform);
            layer.gameObject.name = layerNames[i];
            layers.Add(layer.GetComponent<Tilemap>());
            
        }
        RefreshLayers();
    }

    public void Dispose()
    {
        for (int i = 0; i < 7; i++)
        {
            var layer = layers[i];
            GameObject.Destroy(layer.gameObject);
            layers[i] = null;
        }
        layers.RemoveAll(x => x);
        layers.Clear();
    }

    public TileBase GetTile(int layer, Vector2 pos)
    {
        return layers[layer]?.GetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(layers[layer].transform.position.z)));
    }

    public void SetTile(int layer, Vector2 pos, string type)
    {
        TileBase tile = null;
        bool isLight = false;
        bool isGrey = true;
        if (type == "null") return;
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
            try
            {
                var obj = Instantiate(Resources.Load<GameObject>(string.Concat("Prefabs/Gimmicks/", type)), pos * 16, Quaternion.identity, layers[layer].transform).GetComponent<ObjectBehaviour>();
                obj.isLight = isLight;
                obj.isGrey = isGrey;
            }
            catch(System.ArgumentException) { Debug.LogError(string.Concat("Prefabs/Gimmicks/", type)); }
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
