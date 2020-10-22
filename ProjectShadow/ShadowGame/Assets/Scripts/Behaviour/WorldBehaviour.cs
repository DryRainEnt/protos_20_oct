using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldBehaviour : MonoBehaviour
{
    public static WorldBehaviour instance;
    public static PlayerBehaviour player;

    public RawImage ScreenTint;

    public List<ObjectBehaviour> objectShiftPool;
    public List<ObjectBehaviour> objectGreyPool;

    public bool isLight;

    private Coroutine ScreenShakeRoutine;

    private void Awake()
    {
        instance = this;
        objectShiftPool = new List<ObjectBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isLight = true;
        RefreshObjects();
    }

    public void Shift()
    {
        isLight = !isLight;
        RefreshObjects();
        ScreenShake(6f, 0.15f, 0.9f);
        Distortion(player.transform);
    }

    public void Shift(GameObject target)
    {
        isLight = !isLight;
        RefreshObjects();
        ScreenShake(6f, 0.15f, 0.9f);
        Distortion(target.transform);
    }

    public void SetLight()
    {
        isLight = true;
        RefreshObjects();
    }

    public void SetShadow()
    {
        isLight = false;
        RefreshObjects();
    }
    
    public void RefreshObjects()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            RefreshObject(obj);
        }
    }

    public void RefreshObject(ObjectBehaviour obj)
    {
        if (obj.isGrey) return;
        obj.gameObject.SetActive(obj.isLight == isLight);
    }

    public void UnregistObject(ObjectBehaviour obj)
    {
        if (!obj.isGrey)
        {
            if (objectShiftPool.Contains(obj))
                objectShiftPool.Remove(obj);
        }
        else
        {
            if (objectGreyPool.Contains(obj))
                objectGreyPool.Remove(obj);
        }
    }

    public void RegistObject(ObjectBehaviour obj)
    {
        if (!obj.isGrey)
        {
            if (!objectShiftPool.Contains(obj))
                objectShiftPool.Add(obj);
        }
        else
        {
            if (!objectGreyPool.Contains(obj))
                objectGreyPool.Add(obj);
        }
        RefreshObject(obj);
    }

    public void Distortion(Transform t)
    {
        var path = player.isDead ? "Prefabs/FX/Distortion_sub" : "Prefabs/FX/Distortion";
        FXObjectPool.instance.Instantiate(path, player.isDead ? t.position + Vector3.back * 3f: t.position, t.rotation);
    }

    public void ItemUse(Transform t)
    {
        var path = "Prefabs/FX/Distortion_item";
        FXObjectPool.instance.Instantiate(path, t.position + Vector3.back * 3f, t.rotation);
    }

    public void ScreenShake(float power, float duration, float mult)
    {
        ScreenShakeRoutine = StartCoroutine(ScreenShaker(power, duration, mult));
    }

    private IEnumerator ScreenShaker(float power, float duration, float mult)
    {
        while (ScreenShakeRoutine != null)
        {
            yield return null;
        }
        var timer = 0f;
        var upper = true;
        var multiplier = 1f;
        var initPos = Camera.main.transform.localPosition;
        var wait = new WaitForSeconds(0.05f);
        while (timer < duration)
        {
            if (upper)
            {
                upper = false;
                Camera.main.transform.localPosition = initPos + Vector3.up * power * multiplier;
            }
            else
            {
                upper = true;
                Camera.main.transform.localPosition += initPos + Vector3.down * power * multiplier;
            }
            multiplier *= mult;
            timer += Time.deltaTime;
            yield return wait;
        }
        Camera.main.transform.localPosition = initPos;
        ScreenShakeRoutine = null;
    }
}
