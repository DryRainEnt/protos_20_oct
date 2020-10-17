using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
    GameObject target;
    float duration;
    public bool isDone;

    private void Awake()
    {
        target = gameObject;
    }

    public void Initiate(float t)
    {
        duration = t;
        isDone = false;
        StartCoroutine(Deactivate());
    }
    
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(duration);
        isDone = true;
    }
}
