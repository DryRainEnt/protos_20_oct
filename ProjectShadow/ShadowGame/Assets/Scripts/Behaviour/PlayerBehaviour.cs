using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<ItemBehaviour> items;

    public DoorBehaviour lastDoor;
    public DoorBehaviour lastSave;

    protected override void Awake()
    {
        base.Awake();
        WorldBehaviour.player = this;
        items = new List<ItemBehaviour>();
        
    }

    public override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        yield return null;
        actionDelay = 1.5f;
        actionStamp = 0f;
        anim.Play("LightRevive");
        PlaySFX("ReviveSFX");
    }

    public void ResetPosition()
    {
        if (lastDoor)
        {
            transform.position = lastDoor.transform.position;
        }
        else
        {
            transform.position = GameObject.Find("StartPoint").transform.position;
        }
    }

    public override void Update()
    {
        base.Update();
        transform.position = Constants.SetDepth(transform.position, -7f);
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            isDead = true;
    }
    
    protected override IEnumerator Dead()
    {
        SystemController.instance.deathCount++;
        PlaySFX("DeadSFX");
        actionDelay = 1.5f;
        actionStamp = 0f;
        anim.SetBool("Dead", true);
        anim.SetTrigger("Shift");
        WorldBehaviour.instance.ScreenTint.color = new Color(1f, 0f, 0f, 0.3f);
        yield return new WaitForSeconds(0.9f);
        WorldBehaviour.instance.ScreenTint.color = new Color(1f, 0f, 0f, 0f);
        ResetPosition();
        var ldo = lastDoor?.GetComponent<ObjectBehaviour>();
        if (ldo && !ldo.isGrey && ldo.isLight != WorldBehaviour.instance.isLight)
        {
            WorldBehaviour.instance.Shift();
            WorldBehaviour.player.isLight = !WorldBehaviour.player.isLight;
        }
        yield return new WaitForSeconds(0.6f);
        PlaySFX("ReviveSFX");
        anim.SetBool("Dead", false);
        onGround = true;
        anim.SetBool("onGround", true);
        anim.ResetTrigger("Shift");
        isDead = false;
        DeadRoutine = null;
    }

    public void GetItem(ItemBehaviour item)
    {
        items.Add(item);
        item.index = items.IndexOf(item);
    }

    public bool UseItem(ItemBehaviour item)
    {
        if (!item) return false;
        item.GetUsed();
        bool res = items.Remove(item);
        item.index = items.IndexOf(item);
        return res;
    }

    public bool UseKey()
    {
        var key = items.Find(x => x.type.Contains("Key") && !x.type.Contains("Clear"));
        return UseItem(key);
    }

    public bool UseClearKey()
    {
        var key = items.Find(x => x.type.Contains("ClearKey"));
        return UseItem(key);
    }
}
