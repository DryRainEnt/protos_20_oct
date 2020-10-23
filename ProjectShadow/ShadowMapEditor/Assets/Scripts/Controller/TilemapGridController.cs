using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TilemapGridController : MonoBehaviour
{
    public static TilemapGridController instance;

    public Material mat;

    public List<Toggle> layerToggles;
    public InputField MapName;
    public InputField Width;
    public InputField Height;

    public Tile startPoint;

    public RuleTile lightBlock;
    public RuleTile shadowBlock;
    public RuleTile greyBlock;
    public Tile lightLadder;
    public Tile shadowLadder;
    public Tile greyLadder;

    public Tile lightBox;
    public Tile shadowBox;
    public Tile greyBox;

    public Tile lightBigBox;
    public Tile shadowBigBox;
    public Tile greyBigBox;

    public Tile greyDoor;
    public Tile lightDoor;
    public Tile shadowDoor;
    public Tile key;

    public Tile clearDoor;
    public Tile clearKey;

    public Tile energy;
    public Tile lightTrap;
    public Tile shadowTrap;
    public Tile greyTrap;

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
            var layer = Instantiate(prefab, transform);
            layer.gameObject.name = layerNames[i];
            layers.Add(layer.GetComponent<Tilemap>());
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

        layerToggles[0].isOn = true;
        TileCursor.instance.SetLayer(0);
    }

    public void GenerateBorder()
    {
        for (float i = TilemapBorder.instance.width / 16 * -0.5f - 2; i < TilemapBorder.instance.width / 16 * 0.5f + 2; i++)
            for (float j = TilemapBorder.instance.height / 16 * -0.5f - 2; j < TilemapBorder.instance.height / 16 * 0.5f + 2; j++)
                if (i < TilemapBorder.instance.width / 16 * -0.5f + 1 || i > TilemapBorder.instance.width / 16 * 0.5f - 2
                    || j < TilemapBorder.instance.height / 16 * -0.5f + 1 || j > TilemapBorder.instance.height / 16 * 0.5f - 2)
                {
                    DataController.instance.Add(2, new Vector2(i, j), "GreyBlock");
                }
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
        return layers[layer]?.GetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), -1));
    }

    public void SetTile(int layer, Vector2 pos, string type)
    {
        if (type != "null")
        {
            if (pos.x < TilemapBorder.instance.width / 16 * -0.5f - 2f || pos.x >= TilemapBorder.instance.width / 16 * 0.5f + 2f) return;
            if (pos.y < TilemapBorder.instance.height / 16 * -0.5f - 2f || pos.y >= TilemapBorder.instance.height / 16 * 0.5f + 2f) return;
        }
        if (layer >= 10) layer -= 10;
        TileBase tile = null;
        bool eraser = false;
        switch (type)
        {
            case "StartPoint": tile = startPoint; break;

            case "LightBlock": tile = lightBlock; break;
            case "ShadowBlock": tile = shadowBlock; break;
            case "GreyBlock": tile = greyBlock; break;
            case "LightLadder": tile = lightLadder; break;
            case "ShadowLadder": tile = shadowLadder; break;
            case "GreyLadder": tile = greyLadder; break;

            case "LightBox": tile = lightBox; break;
            case "ShadowBox": tile = shadowBox; break;
            case "GreyBox": tile = greyBox; break;
            case "LightBigBox": tile = lightBigBox; break;
            case "ShadowBigBox": tile = shadowBigBox; break;
            case "GreyBigBox": tile = greyBigBox; break;

            case "GreyDoor": tile = greyDoor; break;
            case "LightDoor": tile = lightDoor; break;
            case "ShadowDoor": tile = shadowDoor; break;
            case "Key": tile = key; break;

            case "ClearDoor": tile = clearDoor; break;
            case "ClearKey": tile = clearKey; break;

            case "EnergyCan": tile = energy; break;
            case "LightTrap": tile = lightTrap; break;
            case "ShadowTrap": tile = shadowTrap; break;
            case "GreyTrap": tile = greyTrap; break;
                
            case "null": eraser = true; break;
        }
        var tilepos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), -1);
        Debug.Log(string.Format("{0} >> [ {1}, {2}, {3} ] >> {4}", layer, tilepos.x, tilepos.y, tilepos.z, type));
        if (eraser)
        {
            layers[layer].SetTile(tilepos, null);
            return;
        }
        if (tile != null) layers[layer].SetTile(tilepos, tile);

        layers[layer].RefreshTile(tilepos);
    }

    public void SetTilemapAlpha(int layer, bool alpha)
    {
        layers[layer].color = new Color(1f, 1f, 1f, alpha ? 1f : 0.5f);
        layers[layer].transform.localPosition = new Vector3(0, 0, alpha ? -1f : 0f);
    }
}
