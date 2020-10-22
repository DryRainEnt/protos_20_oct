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
   public override void Start()
    {
        base.Start();

        interactable = GetComponent<InteractableBehaviour>();
        initDist = 0f;
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

        if (master != null)
        {
            hspeed = master.hsum * master.PushSlow;
        }

        base.Update();

        if (master && (master != interactable.master || !master.onGround))
        {
            PushEnd();
        }

        if ((master && (blocked[0] || blocked[1] || blocked[2])) || !master)
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

                if (!Constants.NearZero(hsum))
                    transform.position = new Vector3(master.transform.position.x + (master.transform.position.x - transform.position.x < 0f ? 1f : -1f) * (initDist), master.transform.position.y);
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
        hspeed = 0f;
    }
}
