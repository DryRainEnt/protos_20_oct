using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class WorldBehaviour : MonoBehaviour
{
    public static WorldBehaviour instance;
    public static PlayerBehaviour player;

    public RawImage ScreenTint;

    public List<ObjectBehaviour> objectShiftPool;
    public List<ObjectBehaviour> objectGreyPool;

    public bool isLight;

    private Coroutine ScreenShakeRoutine;

    public Transform LightMask;
    public Transform ShadowMask;

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
        Shift(player.gameObject);
    }

    public void Shift(GameObject target)
    {
        StartCoroutine(MaskRoutine(isLight, 0.5f));
        isLight = !isLight;
        ScreenShake(6f, 0.15f, 0.9f);
        Distortion(target.transform);
        TurnOnObjects();
        MaskSetting();
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
    
    public void TurnOnAllObjects()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            obj.gameObject.SetActive(true);
        }
    }
    
    public void RefreshObjects()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            RefreshObject(obj);
        }
    }
    
    public void RefreshBackgrounds()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            if (obj.name.Contains("BackGrounds")) RefreshObject(obj);
        }
    }

    public void MaskSetting()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            var sr = obj.GetComponentInChildren<SpriteRenderer>();
            if (sr)
            {
                sr.maskInteraction = (obj.isLight == isLight ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.VisibleOutsideMask);
            }

            var tr = obj.GetComponent<TilemapRenderer>();
            if (tr)
            {
                tr.maskInteraction = (obj.isLight == isLight ? SpriteMaskInteraction.VisibleInsideMask : SpriteMaskInteraction.VisibleOutsideMask);
            }

        }
    }

    public void TurnOnObject(ObjectBehaviour obj)
    {
        obj.gameObject.SetActive(true);
    }

    public void TurnOffObject(ObjectBehaviour obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void TurnOnObjects()
    {
        foreach (ObjectBehaviour obj in objectShiftPool)
        {
            TurnOnObject(obj);
        }
    }

    public void RefreshObject(ObjectBehaviour obj)
    {
        if (obj.isGrey) return;
        if (obj.wait)
            StartCoroutine(WaitForRefresh(obj));
        else
            obj.gameObject.SetActive(obj.isLight == isLight);
    }

    IEnumerator WaitForRefresh(ObjectBehaviour obj)
    {
        var sr = obj.GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.enabled = false;

        while (obj.wait)
        {
            yield return null;
        }

        if (sr) sr.enabled = true;
        RefreshObject(obj);
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

    private IEnumerator MaskRoutine(bool light, float duration = 1f)
    {
        LightMask.position = Constants.SetDepth(player.transform.position, 5);
        ShadowMask.position = Constants.SetDepth(player.transform.position, 5);
        var timer = 0f;
        while (timer < duration)
        {
            var progress = 2 / (1 + Mathf.Pow(2.414f, 4 - 8 * timer / duration));
            LightMask.localScale = Vector3.one * progress;
            ShadowMask.localScale = Vector3.one * progress;
            timer += Time.deltaTime;
            yield return null;
        }
        RefreshObjects();
    }
}
