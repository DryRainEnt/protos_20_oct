using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXObjectPool : MonoBehaviour
{
    public static FXObjectPool instance;

    Dictionary<string, List<GameObject>> pool;

    private void Awake()
    {
        instance = this;
        pool = new Dictionary<string, List<GameObject>>();
    }
    
    
    public GameObject GetOrCreate(string path)
    {
        GameObject fx = null;
        if (pool.ContainsKey(path))
            if (pool[path].Count > 0)
            {
                Debug.Log("Object Pooled from :" + path);
                fx = pool[path][0];
            }
            else
            {
                Debug.Log("Not enough Pool :" + path);
                fx = Instantiate(Resources.Load<GameObject>(path));
            }
        else
        {
            Debug.Log("Create New Object Pool :" + path);
            pool[path] = new List<GameObject>();
            fx = Instantiate(Resources.Load<GameObject>(path));
            pool[path].Add(fx);
        }
        var ps = fx.GetComponent<ParticleSystem>();
        if (ps) ps.Clear();
        var fxb = fx.GetComponent<FXBase>();
        if (fxb) fxb.Clear();
        fx.SetActive(false);
        return fx;
    }

    public GameObject Instantiate(string path, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        try
        {
            var obj = GetOrCreate(path);
            pool[path].Remove(obj);
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.transform.SetParent(parent);
            obj.SetActive(true);
            var ps = obj.GetComponent<ParticleSystem>();
            if (ps) ps.Play();
            var fxb = obj.GetComponent<FXBase>();
            if (fxb) fxb.Play();
            var fxo = obj.GetComponent<FXPooledObject>();
            if (fxo) fxo.Activate();
            return obj;
        }
        catch (System.Exception e) { Debug.LogError(e); }
        return null;
    }

    public GameObject Instantiate(string path, Vector3 pos)
    {
        var obj = GetOrCreate(path);
        pool[path].Remove(obj);
        obj.transform.position = pos;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(null);
        obj.SetActive(true);
        var ps = obj.GetComponent<ParticleSystem>();
        if (ps) ps.Play();
        var fxb = obj.GetComponent<FXBase>();
        if (fxb) fxb.Play();
        obj.GetComponent<FXPooledObject>()?.Activate();
        return obj;
    }
    
    public void Deactivate(FXPooledObject obj)
    {
        pool[obj.path].Add(obj.gameObject);
        var ps = obj.GetComponent<ParticleSystem>();
        if (ps) ps.Clear();
        var fxb = obj.GetComponent<FXBase>();
        if (fxb) fxb.Clear();
        obj.gameObject.SetActive(false);
    }
}
