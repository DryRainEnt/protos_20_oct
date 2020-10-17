using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : CharacterBehaviour
{
    private void Awake()
    {
        WorldBehaviour.player = this;
    }

    public override void Update()
    {
        base.Update();
    }
    
    protected override IEnumerator Dead()
    {
        actionDelay = 0.5f;
        actionStamp = 0f;
        WorldBehaviour.instance.ScreenTint.color = new Color(1f, 0f, 0f, 0.3f);
        Camera.main.backgroundColor = Color.red;
        yield return new WaitForSeconds(0.3f);
        WorldBehaviour.instance.ScreenTint.color = new Color(1f, 0f, 0f, 0f);
        WorldBehaviour.instance.Shift();
        isDead = false;
        DeadRoutine = null;
    }
}
