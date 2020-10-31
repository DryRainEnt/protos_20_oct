using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    PlayerBehaviour master;
    public string type;
    CircleCollider2D range;
    AudioSource sound;
    AudioClip clip;
    private bool isConsumed = false;
    private bool isUsed = false;

    public bool IsUsed { get { return isUsed; } set { isUsed = value; } }
    public bool IsConsumed { get { return isConsumed; } set { isConsumed = value; } }
    public int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        type = GetComponent<ObjectBehaviour>().type;
        range = gameObject.AddComponent<CircleCollider2D>();
        clip = Resources.Load<AudioClip>("Audio/SFX/" + GetComponent<ObjectBehaviour>().type + "SFX");
        sound = gameObject.AddComponent<AudioSource>();
        sound.clip = clip;
        sound.playOnAwake = false;
    }

    public virtual void ConsumedBehaviour()
    {
        transform.position = Constants.SetDepth(transform.position + (master.transform.position + Vector3.up * master.col.size.y / 2 - transform.position) * 0.4f / (index + 1), -5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsed) return;
        if (isConsumed && master) ConsumedBehaviour();
        else
        {
            var collisions = Physics2D.OverlapCircleAll(transform.position, 20f);

            foreach (Collider2D collider in collisions)
            {
                var p = collider.GetComponent<PlayerBehaviour>();
                if (p)
                {
                    p.GetItem(this);
                    master = p;
                    isConsumed = true;
                    transform.localScale = Vector3.one * 0.5f;
                    transform.SetParent(null);
                    sound.Play();
                }
            }
        }
    }

    public void ItemReset()
    {
        master = null;
        IsConsumed = false;
        IsUsed = false;
        GetComponent<ObjectBehaviour>().ResetPosition();
    }

    public void GetUsed()
    {
        StartCoroutine(UsedRoutine());
    }
    
    IEnumerator UsedRoutine()
    {
        isUsed = true;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        WorldBehaviour.instance.ItemUse(transform);
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
