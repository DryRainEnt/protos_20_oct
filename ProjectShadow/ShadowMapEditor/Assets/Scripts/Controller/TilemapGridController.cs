using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TilemapGridController : MonoBehaviour
{
    public static TilemapGridController instance;

    public Material mat;

    public Toggle firstLayer;
    public Toggle autoGen;
    public InputField MapName;
    public InputField Width;
    public InputField Height;

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

    public Tile door;
    public Tile key;

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
        
        if (autoGen.isOn)
        {
            for (float i = TilemapBorder.instance.width / 16 * -0.5f - 2; i < TilemapBorder.instance.width / 16 * 0.5f + 2; i++)
                for (float j = TilemapBorder.instance.height / 16 * -0.5f - 2; j < TilemapBorder.instance.height / 16 * 0.5f + 2; j++)
                    if (i < TilemapBorder.instance.width / 16 * -0.5f + 1 || i > TilemapBorder.instance.width / 16 * 0.5f - 2
                        || j < TilemapBorder.instance.height / 16 * -0.5f + 1 || j > TilemapBorder.instance.height / 16 * 0.5f - 2)
                        SetTile(2, new Vector3(i, j), "GreyBlock");
        }

        firstLayer.isOn = true;
        TileCursor.instance.SetLayer(0);
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
        if (pos.x < TilemapBorder.instance.width / 16 * -0.5f - 2f || pos.x > TilemapBorder.instance.width / 16 * 0.5f + 2f) return;
        if (pos.y < TilemapBorder.instance.height / 16 * -0.5f - 2f || pos.y > TilemapBorder.instance.height / 16 * 0.5f + 2f) return;
        if (layer >= 10) layer -= 10;
        TileBase tile = null;
        bool eraser = false;
        switch (type)
        {
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

            case "Door": tile = door; break;
            case "Key": tile = key; break;

            case "null": eraser = true; break;
        }
        if (eraser)
        {
            layers[layer].SetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(layers[layer].transform.position.z)), null);
            return;
        }
        if (tile != null) layers[layer].SetTile(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(layers[layer].transform.position.z)), tile);
    }

    public void SetTilemapAlpha(int layer, bool alpha)
    {
        layers[layer].color = new Color(1f, 1f, 1f, alpha ? 1f : 0.5f);
        layers[layer].transform.localPosition = new Vector3(0, 0, alpha ? -1f : 0f);
    }
}
