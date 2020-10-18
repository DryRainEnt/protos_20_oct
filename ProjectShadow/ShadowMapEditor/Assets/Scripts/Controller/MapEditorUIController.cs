using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorUIController : MonoBehaviour
{
    public GameObject Basics;
    Coroutine OnBasicsRoutine;
    bool isBasicsOn = true;

    public GameObject Tilesets;
    public Coroutine OnTilesetsRoutine;
    bool isTilesetsOn = true;

    public void ToggleBasics()
    {
        if (OnBasicsRoutine != null) StopCoroutine(OnBasicsRoutine);
        OnBasicsRoutine = StartCoroutine(isBasicsOn ? OffBasics() : OnBasics());

    }

    public void ToggleTilesets()
    {
        if (OnTilesetsRoutine != null) StopCoroutine(OnTilesetsRoutine);
        OnTilesetsRoutine = StartCoroutine(isTilesetsOn ? OffTilesets() : OnTilesets());

    }

    IEnumerator OnBasics()
    {
        isBasicsOn = true;
        while (true)
        {
            Basics.transform.position += Vector3.right * 5f;
            if (Basics.transform.position.x >= 330f)
            {
                var temp = Basics.transform.position;
                temp.x = 330f;
                Basics.transform.position = temp;
                break;
            }
            yield return null;
        }
    }

    IEnumerator OffBasics()
    {
        isBasicsOn = false;
        while (true)
        {
            Basics.transform.position += Vector3.left * 5f;
            if (Basics.transform.position.x <= 155f)
            {
                var temp = Basics.transform.position;
                temp.x = 155f;
                Basics.transform.position = temp;
                break;
            }
            yield return null;
        }
    }

    IEnumerator OnTilesets()
    {
        isTilesetsOn = true;
        while (true)
        {
            Tilesets.transform.position += Vector3.left * 5f;
            if (Tilesets.transform.position.x <= Screen.width)
            {
                var temp = Tilesets.transform.position;
                temp.x = Screen.width;
                Tilesets.transform.position = temp;
                break;
            }
            yield return null;
        }
    }

    IEnumerator OffTilesets()
    {
        isTilesetsOn = false;
        while (true)
        {
            Tilesets.transform.position += Vector3.right * 5f;
            if (Tilesets.transform.position.x >= Screen.width + 200f)
            {
                var temp = Tilesets.transform.position;
                temp.x = Screen.width + 200f;
                Tilesets.transform.position = temp;
                break;
            }
            yield return null;
        }
    }

}
