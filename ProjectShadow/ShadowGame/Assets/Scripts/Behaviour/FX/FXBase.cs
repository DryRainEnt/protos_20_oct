using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXBase : MonoBehaviour
{
    protected float timer;
    public float duration;
    public float speed = 1f;
    protected Material mat;
    protected bool isOn;
    
    protected virtual void Awake()
    {
        mat = GetComponent<Renderer>().material;
        Clear();
    }

    public virtual void Clear()
    {
        timer = 0f;
        transform.localScale = Vector3.zero;
        isOn = false;
    }

    public virtual void Play()
    {
        isOn = true;
    }
}
