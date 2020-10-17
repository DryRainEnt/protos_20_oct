using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distortion : FXBase
{
    new protected float duration = 1f;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Renderer>().material = new Material(mat);
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn) return;

        mat.SetFloat("_BumpAmt", Mathf.Max(0, Mathf.Lerp(-10f, 10f, 1f / (timer + 1f))));
        transform.localScale = Vector3.one * Mathf.Max(0, Mathf.Lerp(960f, 16f, 1f / (timer + 1f)));
        timer += Time.deltaTime;
    }
}
