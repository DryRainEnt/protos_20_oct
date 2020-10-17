using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractTarget
{
    OnlyPlayer,
    OnlyEnemy,
    Everyone,
    None,
}

public class InteractableBehaviour : MonoBehaviour
{
    public CharacterBehaviour master;
    BoxCollider2D col;
    public InteractTarget target;
    public Vector2 size;

    GameObject fx;
    SpriteRenderer fxsr;
    public float fx_offset;

    public bool interactable;
    public float interactCall;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        if (!col) col = gameObject.AddComponent<BoxCollider2D>();
        else fx_offset = transform.localScale.y * col.size.y + 16;

        fx = Resources.Load<GameObject>("Prefabs/FX/FX_Interactable");
        fx = Instantiate(fx, transform.position + Vector3.up * fx_offset, Quaternion.identity, transform);
        fx.layer = 0;

        fxsr = fx.GetComponent<SpriteRenderer>();
        fxsr.enabled = false;

        size.x *= transform.localScale.x;
        size.y *= transform.localScale.y;
    }

    private void Update()
    {
        if (master)
        {
            if (!Constants.NearZero(master.interactObjectCall) && (master.interactObjectCall * (transform.position.x - master.transform.position.x) > 0f))
            {
                Interact(master.interactObjectCall);
                master.interactObjectCall = 0f;
            }
        }

        var collisions = Physics2D.OverlapBoxAll((Vector2)transform.position + col.offset, new Vector2(transform.localScale.x * col.size.x + 32, transform.localScale.y * col.size.y - 4), 0f);
        var dist = 99f;

        foreach (Collider2D collider in collisions)
        {
            var c = collider.GetComponent<CharacterBehaviour>();
            if (c)
            {
                var d = Vector2.Distance((Vector2)c.transform.position + Vector2.up * c.col.size.y / 2, (Vector2)transform.position + transform.localScale.y * col.offset);
                if (d < dist)
                {
                    dist = d;
                    if (!master || (master && master.interact == null && (master.sr.transform.localScale.x * (transform.position.x - master.transform.position.x) > 0f)))
                    {
                        master = c;
                        SetInteractable();
                    }
                }
            }
        }

        if (dist == 99f || (master.sr.transform.localScale.x * (transform.position.x - master.transform.position.x) < 0f))
        {
            master = null;
            SetNonInteractable();
        }
    }
    
    public void SetInteractable()
    {
        interactable = true;
        fxsr.enabled = true;
    }

    public void SetNonInteractable()
    {
        interactable = false;
        fxsr.enabled = false;
    }

    public void Interact(float call)
    {
        if (target == InteractTarget.None)
            return;
        else if (target == InteractTarget.OnlyPlayer && WorldBehaviour.player != master)
            return;
        else if (target == InteractTarget.OnlyEnemy && WorldBehaviour.player == master)
            return;

        interactCall = call;
    }

    public void StartInteract()
    {
        master.interact = this;
        master.sr.transform.localScale = new Vector3(master.transform.position.x - transform.position.x < 0 ? 1f : -1f, 1f, 1f);
    }

    public void EndInteract()
    {
        master.interact = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (col) Gizmos.DrawWireCube((Vector2)transform.position + transform.localScale.y * col.offset, new Vector2(transform.localScale.x * col.size.x + 32, transform.localScale.y * col.size.y - 4));
    }
}
