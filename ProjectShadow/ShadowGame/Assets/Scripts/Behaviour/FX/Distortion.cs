using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distortion : FXBase
{
    public float maxScale;
    public float minScale;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Renderer>().material = new Material(mat);
        GetComponent<Renderer>().sortingOrder = 10;
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn) return;
        mat.SetFloat("_BumpAmt", Mathf.Max(0, Mathf.Lerp(-10f, 10f, 1f / (timer * speed + 1f))));
        transform.localScale = Vector3.one * Mathf.Max(0, Mathf.Lerp(maxScale, minScale, 1f / (timer + 1f)));
        timer += Time.deltaTime;
    }
}
