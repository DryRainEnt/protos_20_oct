using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : ILoadableBehaviour
{
    public string stage;
    public string type;
    public string target;

    public bool isLight;
    public bool isGrey;

    public bool wait = false;

    public override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        yield return null;
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
