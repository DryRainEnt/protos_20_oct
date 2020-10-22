using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    PlayerBehaviour master;
    CircleCollider2D range;
    bool isConsumed = false;
    bool isUsed = false;

    // Start is called before the first frame update
    void Start()
    {
        range = gameObject.AddComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsed) return;
        if (isConsumed && master) transform.position = Constants.SetDepth(transform.position + (master.transform.position + Vector3.up * master.col.size.y / 2 - transform.position) * 0.2f, -5f);
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
                    transform.SetParent(master.transform);
                }
            }
        }
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
