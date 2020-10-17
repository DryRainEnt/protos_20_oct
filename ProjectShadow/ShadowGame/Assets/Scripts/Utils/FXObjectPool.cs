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
    }

    // Start is called before the first frame update
    void Start()
    {
        pool = new Dictionary<string, List<GameObject>>();
    }
    
    public GameObject GetOrCreate(string path)
    {
        GameObject fx = null;
        if (pool.ContainsKey(path))
            if (pool[path].Count > 0)
            {
                fx = pool[path][0];
            }
            else
            {
                fx = Instantiate(Resources.Load<GameObject>(path));
            }
        else
        {
            pool[path] = new List<GameObject>();
            fx = Instantiate(Resources.Load<GameObject>(path));
            pool[path].Add(fx);
        }
        var ps = fx.GetComponent<ParticleSystem>();
        if (ps) ps.Clear();
        var fxb = fx.GetComponent<FXBase>();
        if (fxb) fxb.Clear();
        fx.SetActive(false);
        Debug.Log(fx.name);
        return fx;
    }

    public GameObject Instantiate(string path, Vector3 pos, Quaternion rot, Transform parent = null)
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
        obj.GetComponent<FXPooledObject>()?.Activate();
        return obj;
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
