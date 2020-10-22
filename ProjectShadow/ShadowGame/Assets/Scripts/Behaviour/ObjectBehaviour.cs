using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    public string target;

    public bool isLight;
    public bool isGrey;

    private void Start()
    {
        if (!isGrey)
        {
            WorldBehaviour.instance.objectShiftPool.Add(this);
            WorldBehaviour.instance.RefreshObject(this);
        }
        else
        {
            transform.localPosition += Vector3.back;
            WorldBehaviour.instance.objectGreyPool.Add(this);
        }
    }

    private void OnDestroy()
    {
        WorldBehaviour.instance.UnregistObject(this);
    }
}
