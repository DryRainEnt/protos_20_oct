using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBehaviour : MovableBehaviour
{
    CharacterBehaviour master;
    InteractableBehaviour interactable;

    public bool isPushable;
    float initDist;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<BoxCollider2D>();
        interactable = GetComponent<InteractableBehaviour>();

        hspeed = 0f;
        vspeed = 0f;
        initDist = 0f;

        isDead = false;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (!Constants.NearZero(interactable.interactCall))
        {
            if (master)
            {
                interactable.EndInteract();
                PushEnd();
            }
            else
            {
                interactable.StartInteract();
                PushStart(interactable.master);
            }
            interactable.interactCall = 0f;
        }

        if (master != null && !Constants.NearZero(master.hsum))
        {
            hspeed = master.hsum * master.PushSlow;
        }
        else
            hspeed = 0f;

        base.Update();

        if (master && (master != interactable.master || !master.onGround))
        {
            PushEnd();
        }

        if (master && Constants.NearZero(master.hsum))
        {
            isPushable = false;
        }
        else
        {
            isPushable = true;
        }

        if (master)
        {
            if (initDist < transform.localScale.x * col.size.x / 2 + master.col.size.x)
            {
                if (!Constants.NearZero(master.hspeed) && master.hspeed * (transform.position.x - master.transform.position.x) > 0f)
                    initDist = Mathf.Min(Mathf.Abs(master.transform.position.x - transform.position.x) + Mathf.Abs(master.hspeed), (transform.localScale.x * col.size.x / 2 + master.col.size.x + 1f));

                if (!Constants.NearZero(hsum)) master.transform.position = new Vector3(transform.position.x + (master.transform.position.x - transform.position.x < 0f ? -1f : 1f) * (initDist), master.transform.position.y);
                else transform.position = new Vector3(master.transform.position.x + (master.transform.position.x - transform.position.x < 0f ? 1f : -1f) * (initDist), master.transform.position.y);
            }
        }
    }

    private void LateUpdate()
    {
        
    }

    public void PushStart(CharacterBehaviour handler)
    {
        master = handler;
        master.onPush = true;
        initDist = Mathf.Min(Mathf.Abs(master.transform.position.x - transform.position.x), (transform.localScale.x * col.size.x / 2 + master.col.size.x));
        transform.position = new Vector3(master.transform.position.x + (master.transform.position.x - transform.position.x < 0f ? 1f : -1f) * initDist, transform.position.y);
    }

    public void PushEnd()
    {
        master.onPush = false;
        master = null;
        initDist = 0f;
        GetComponent<MovableBehaviour>().hspeed = 0f;
    }
}
