using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MovableBehaviour
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
    }
}
