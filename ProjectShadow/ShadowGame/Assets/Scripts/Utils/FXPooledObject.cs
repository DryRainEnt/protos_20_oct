using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPooledObject : MonoBehaviour
{
    public string path;
    public float time;
    LimitedLifetime life;
    
    public void Activate()
    {
        if (!life) life = gameObject.AddComponent<LimitedLifetime>();
        life.Initiate(time);
    }

    private void Update()
    {
        if (life.isDone)
        {
            life.isDone = false;
            Deactivate();
        }
    }

    public void Deactivate()
    {
        FXObjectPool.instance.Deactivate(this);
    }
}
