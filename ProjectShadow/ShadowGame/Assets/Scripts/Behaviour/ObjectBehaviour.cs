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

    Vector3 initPos;
    Vector3 initScale;

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
        initPos = transform.position;
        initScale = transform.localScale;
    }

    public void ResetPosition()
    {
        transform.position = initPos;
        transform.localScale = initScale;
    }

    private void OnDestroy()
    {
        WorldBehaviour.instance.UnregistObject(this);
    }
}
