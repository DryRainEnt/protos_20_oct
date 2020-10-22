using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : CharacterBehaviour
{
    public List<ItemBehaviour> items;

    public DoorBehaviour lastDoor;

    private void Awake()
    {
        WorldBehaviour.player = this;
        items = new List<ItemBehaviour>();

    }

    public void ResetPosition()
    {
        lastDoor = GameObject.Find("StartPoint").GetComponent<DoorBehaviour>();
        if (lastDoor)
        {
            transform.position = lastDoor.transform.position;

            if (WorldBehaviour.player.isLight != WorldBehaviour.instance.isLight) WorldBehaviour.instance.Shift();
            else WorldBehaviour.instance.Distortion(transform);
        }
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

    public void GetItem(ItemBehaviour item)
    {
        items.Add(item);
    }

    public bool UseItem(ItemBehaviour item)
    {
        if (!item) return false;
        item.GetUsed();
        return items.Remove(item);
    }

    public bool UseKey()
    {
        var key = items.Find(x => x.name.Contains("Key"));
        return UseItem(key);
    }
}
