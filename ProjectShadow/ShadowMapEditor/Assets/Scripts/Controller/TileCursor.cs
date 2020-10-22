using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileCursor : MonoBehaviour
{
    public static TileCursor instance;
    SpriteRenderer sr;
    public string Tilename = "null";
    public int targetLayer;
    bool tileControl = true;

    public InputField tileName;
    public InputField tileTarget;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        var wantedPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        transform.position = wantedPos;

        var pos = transform.position;
        if (!Input.GetKey(KeyCode.LeftShift)) pos = new Vector2(Mathf.FloorToInt(pos.x / 16), Mathf.FloorToInt(pos.y / 16));
        else pos = new Vector2(pos.x / 16, pos.y / 16);
        sr.transform.position = new Vector3(pos.x * 16 + 8, pos.y * 16 + 8, -5);
        
        if (Input.GetMouseButtonDown(0) && IsPointerOverUIElement())
        {
            tileControl = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            tileControl = true;
        }

        if (Input.GetMouseButton(0) && tileControl)
        {
            DataController.instance.Add(targetLayer, pos, Tilename, tileName.text, tileTarget.text);
        }

        if (Input.GetMouseButton(1) && tileControl)
        {
            DataController.instance.Add(targetLayer, pos, "null");
        }

        if (Input.GetMouseButtonDown(2))
        {
            sr.enabled = false;
            Cursor.visible = false;
        }
        if (Input.GetMouseButtonUp(2))
        {
            sr.enabled = true;
            Cursor.visible = true;
        }
    }

    public void SetLayer(int layer)
    {
        targetLayer = layer;
        for (int i = 0; i < 7; i++)
            TilemapGridController.instance.SetTilemapAlpha(i, false);
        TilemapGridController.instance.SetTilemapAlpha(targetLayer, true);
    }
    
    public void SetIcon()
    {
        Tilename = EventSystem.current.currentSelectedGameObject.name;

        var button = GameObject.Find(Tilename);
        sr.sprite = button.GetComponentInChildren<Image>().sprite;
    }
    
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResults)
    {
        for (int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    ///Gets all event system ray cast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
