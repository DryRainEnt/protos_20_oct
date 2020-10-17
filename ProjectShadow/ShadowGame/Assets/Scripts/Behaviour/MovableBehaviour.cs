using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBehaviour : MonoBehaviour
{
    public SpriteRenderer sr;
    public Animator anim;
    public BoxCollider2D col;

    public bool isGravity;

    public bool isLight;

    public float hspeed;
    public float vspeed;

    public float hsum;
    public float vsum;

    public bool onGround;
    public bool onLadder;
    public bool onPush;
    public float PushSlow = 0.5f;

    public float ladderGrabCall;
    public float ladderDropCall;
    public bool ladderGrabbable;

    public float actionDelay = 0f;
    public float actionStamp = -1f;

    public float landingDelay = 0.25f;
    public float landingStamp = -1f;

    public float interactObjectCall;
    public InteractableBehaviour interact;

    public bool isDead = false;
    protected Coroutine DeadRoutine;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<BoxCollider2D>();

        hspeed = 0f;
        vspeed = 0f;

        isDead = false;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isDead)
        {
            if (DeadRoutine == null) DeadRoutine = OnDead();
            return;
        }

        isLight = WorldBehaviour.instance.isLight;

        if (actionStamp >= 0 && actionStamp < actionDelay)
        {
            actionStamp += Time.deltaTime;
        }
        else
        {
            actionDelay = 0f;
            actionStamp = -1f;
        }

        if (actionStamp >= 0)
        {
            hspeed = 0f;
            vspeed = 0f;
            return;
        }

        if (Constants.NearZero(hspeed))
            hspeed = 0f;

        if (landingStamp >= 0 && landingStamp < landingDelay)
        {
            landingStamp += Time.deltaTime;
            hspeed = hspeed * 0.8f;
        }
        else
            landingStamp = -1f;

        #region MovementBehaviour

        /// vertical movement check
        /// 
        transform.position += Vector3.up * vspeed;

        if (isGravity)
            vspeed += Constants.gravity;

        col.enabled = false;

        RaycastHit2D vhit =
            isLight ? Physics2D.Raycast(transform.position, Vector2.up, vspeed <= 0 ? vspeed : col.size.y + vspeed, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("GreyBlock")))
            : Physics2D.Raycast(transform.position, Vector2.up, vspeed <= 0 ? vspeed : col.size.y + vspeed, 1 << LayerMask.NameToLayer("ShadowBlock") | (1 << LayerMask.NameToLayer("GreyBlock")));

        RaycastHit2D ghit =
            isLight ? Physics2D.Raycast(transform.position, Vector2.up, vspeed <= 0 ? vspeed : 0, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("LightLadder")) | (1 << LayerMask.NameToLayer("GreyBlock")) | (1 << LayerMask.NameToLayer("GreyLadder")))
            : Physics2D.Raycast(transform.position, Vector2.up, vspeed <= 0 ? vspeed : 0, 1 << LayerMask.NameToLayer("ShadowBlock") | 1 << LayerMask.NameToLayer("ShadowLadder") | (1 << LayerMask.NameToLayer("GreyBlock")) | (1 << LayerMask.NameToLayer("GreyLadder")));

        RaycastHit2D uhit =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, vspeed <= 0 ? 1 : 0, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("LightLadder")) | (1 << LayerMask.NameToLayer("GreyBlock")) | (1 << LayerMask.NameToLayer("GreyLadder")))
            : Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, vspeed <= 0 ? 1 : 0, 1 << LayerMask.NameToLayer("ShadowBlock") | 1 << LayerMask.NameToLayer("ShadowLadder") | (1 << LayerMask.NameToLayer("GreyBlock")) | (1 << LayerMask.NameToLayer("GreyLadder")));

        if (ghit && !uhit)
        {
            if (vspeed < 0)
            {
                if (!onGround)
                {
                    landingStamp = 0f;
                    transform.position = ghit.point;
                }
                anim?.ResetTrigger("Jump");
                onGround = true;
            }
            vspeed = 0f;
        }

        if (vhit)
        {
            if (vspeed <= 0)
            {
                if (!onGround)
                {
                    landingStamp = 0f;
                }
                anim?.ResetTrigger("Jump");
                onGround = true;
                transform.position = vhit.point;
            }
            else
            {
                transform.position = vhit.point + Vector2.down * col.size.y;
            }
            vspeed = 0f;
        }
        else
        {
            if (!Constants.NearZero(vspeed))
                onGround = false;
        }

        /// horizontal movement check
        /// 
        var h_range = transform.localScale.x * (hspeed < 0 ? col.size.x / 2 * -1 : col.size.x / 2);
        RaycastHit2D hhitl =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up * 3, Vector2.right, h_range, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("GreyBlock")))
            : Physics2D.Raycast(transform.position + Vector3.up * 3, Vector2.right, h_range, 1 << LayerMask.NameToLayer("ShadowBlock") | (1 << LayerMask.NameToLayer("GreyBlock")));

        RaycastHit2D hhitm =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up * col.size.y / 2, Vector2.right, h_range, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("GreyBlock")))
            : Physics2D.Raycast(transform.position + Vector3.up * col.size.y / 2, Vector2.right, h_range, 1 << LayerMask.NameToLayer("ShadowBlock") | (1 << LayerMask.NameToLayer("GreyBlock")));

        RaycastHit2D hhitu =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up * (col.size.y - 3), Vector2.right, h_range, (1 << LayerMask.NameToLayer("LightBlock")) | (1 << LayerMask.NameToLayer("GreyBlock")))
            : Physics2D.Raycast(transform.position + Vector3.up * (col.size.y - 3), Vector2.right, h_range, 1 << LayerMask.NameToLayer("ShadowBlock") | (1 << LayerMask.NameToLayer("GreyBlock")));

        if (!onLadder)
        {
            if (hhitl || hhitm || hhitu)
            {
                if (Constants.NearZero(vspeed) && Constants.NearZero(hspeed))
                {
                    if (hhitl && Constants.NearZero(hhitl.point.x - transform.position.x))
                    {
                        CheckOverlap();
                    }
                    else if (hhitm && Constants.NearZero(hhitm.point.x - transform.position.x))
                    {
                        CheckOverlap();
                    }
                    else if (hhitu && Constants.NearZero(hhitu.point.x - transform.position.x))
                    {
                        CheckOverlap();
                    }
                }

                if (!Constants.NearZero(hspeed))
                {
                    if (hhitl && !(onPush && hhitm.collider.gameObject.GetComponent<PushableBehaviour>()))
                    {
                        transform.position = new Vector3(hhitl.point.x + transform.localScale.x * col.size.x / 2 * Mathf.Sign(-hspeed), transform.position.y);
                    }
                    else if (hhitm && !(onPush && hhitm.collider.gameObject.GetComponent<PushableBehaviour>()))
                    {
                        transform.position = new Vector3(hhitm.point.x + transform.localScale.x * col.size.x / 2 * Mathf.Sign(-hspeed), transform.position.y);

                    }
                    else if (hhitu && !(onPush && hhitm.collider.gameObject.GetComponent<PushableBehaviour>()))
                    {
                        transform.position = new Vector3(hhitu.point.x + transform.localScale.x * col.size.x / 2 * Mathf.Sign(-hspeed), transform.position.y);
                    }
                }

                hspeed = 0f;
            }
            else
            {
                if (onPush) PushSlow = 1 / Mathf.Lerp(6, 3, 1 / (interact.size.x * interact.size.y));
            }
        }

        hsum = hspeed;
        transform.position += Vector3.right * hsum * (onPush ? PushSlow : 1);

        #endregion

        col.enabled = true;

        #region LadderBehaviour

        RaycastHit2D lhit =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, col.size.y + vspeed, (1 << LayerMask.NameToLayer("LightLadder")) | (1 << LayerMask.NameToLayer("GreyLadder")))
            : Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, col.size.y + vspeed, 1 << LayerMask.NameToLayer("ShadowLadder") | (1 << LayerMask.NameToLayer("GreyLadder")));

        RaycastHit2D luhit =
            isLight ? Physics2D.Raycast(transform.position + Vector3.up * col.size.y / 2, Vector2.up, 1, (1 << LayerMask.NameToLayer("LightLadder")) | (1 << LayerMask.NameToLayer("GreyLadder")))
            : Physics2D.Raycast(transform.position + Vector3.up * col.size.y / 2, Vector2.up, 1, 1 << LayerMask.NameToLayer("ShadowLadder") | (1 << LayerMask.NameToLayer("GreyLadder")));

        RaycastHit2D lvhit =
            isLight ? Physics2D.Raycast(transform.position, Vector2.down, 1, (1 << LayerMask.NameToLayer("LightLadder")) | (1 << LayerMask.NameToLayer("GreyLadder")))
            : Physics2D.Raycast(transform.position, Vector2.down, 1, 1 << LayerMask.NameToLayer("ShadowLadder") | (1 << LayerMask.NameToLayer("GreyLadder")));

        if (lhit || lvhit)
            ladderGrabbable = true;
        else
            ladderGrabbable = false;

        if (!onLadder)
        {
            if (!Constants.NearZero(ladderGrabCall))
            {
                if (lhit && ladderGrabCall > 0)
                {
                    onGround = false;
                    onLadder = true;
                    transform.position = new Vector3(lhit.point.x + (lhit.point.x < 0 ? -8 - lhit.point.x % 16 : 8 - lhit.point.x % 16), transform.position.y);
                    anim?.SetTrigger("grabLadder");
                }
                else if (lvhit && ladderGrabCall < 0 && (ghit && !uhit))
                {
                    onGround = false;
                    onLadder = true;
                    transform.position = new Vector3(lvhit.point.x + (lvhit.point.x < 0 ? -8 - lvhit.point.x % 16 : 8 - lvhit.point.x % 16), transform.position.y - col.size.y / 2 - 2);
                    anim?.SetTrigger("grabLadder");
                }
                else if (lvhit && ladderGrabCall < 0 && !onGround)
                {
                    onGround = false;
                    onLadder = true;
                    transform.position = new Vector3(lvhit.point.x + (lvhit.point.x < 0 ? -8 - lvhit.point.x % 16 : 8 - lvhit.point.x % 16), transform.position.y);
                    anim?.SetTrigger("grabLadder");
                }
            }
        }
        else
        {
            if (!Constants.NearZero(ladderDropCall) && !(hhitl || hhitm || hhitu))
            {
                onLadder = false;
                transform.position += Mathf.Sign(ladderDropCall) * Vector3.right * 12;
            }
            if (onGround)
            {
                landingStamp = 0f;
                anim?.ResetTrigger("Jump");
                onLadder = false;
                vspeed = 0f;
            }
            if (!lhit && !lvhit)
                onLadder = false;
            if (vspeed > 0 && !luhit)
            {
                onLadder = false;
                transform.position += Vector3.up * col.size.y / 2;
            }
        }

        #endregion
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -3f);
        vsum = vspeed;
        hsum = hspeed;
    }

    private void LateUpdate()
    {
        if (!Constants.NearZero(hspeed) && !onPush)
        {
            if (hspeed > 0)
                sr.transform.localScale = new Vector3(1, 1, 1);
            else
                sr.transform.localScale = new Vector3(-1, 1, 1);
        }

        anim?.SetBool("isLight", isLight);
        anim?.SetBool("onGround", onGround);
        anim?.SetBool("onLadder", onLadder);
        anim?.SetFloat("onPush", onPush ? (sr.transform.localScale.x * hspeed < 0 ? -1f : 1f) * Mathf.Abs(hspeed * 0.3f) : 100f);
        anim?.SetFloat("hspeed", hsum);
        anim?.SetFloat("vspeed", vsum);
        anim?.SetBool("isMoving", !Constants.NearZero(hspeed));
    }

    private void CheckOverlap()
    {
        if (DeadRoutine == null)
            DeadRoutine = StartCoroutine(OverlapChecker());
    }

    IEnumerator OverlapChecker()
    {
        var count = 0;
        while (count < 10)
        {
            var overlap = Physics2D.OverlapBoxAll(transform.position + Vector3.up * col.size.y / 2, col.size - new Vector2(2, 2), 0, isLight ? 1 << LayerMask.NameToLayer("LightBlock") : 1 << LayerMask.NameToLayer("ShadowBlock"));
            if (overlap.Length > 0)
            {
                bool pushCheck = false;
                //Debug.LogError(overlap);
                foreach (Collider2D c2 in overlap)
                {
                    var push = c2.GetComponent<PushableBehaviour>();
                    if (push)
                    {
                        push.hspeed = Mathf.Sign(c2.transform.position.x - transform.position.x);
                        pushCheck = true;
                    }
                }
                if (!pushCheck) { isDead = true; break; }
            }
            else { isDead = false;}
            count++;
            yield return null;
        }
        DeadRoutine = null;
    }

    protected Coroutine OnDead()
    {
        return StartCoroutine(Dead());
    }

    protected virtual IEnumerator Dead()
    {
        var fx = FXObjectPool.instance.Instantiate("Prefabs/FX/FX_Emission", (Vector3)transform.position + (Vector3)col.offset * transform.localScale.y + Vector3.forward * -5f, Quaternion.identity);
        sr.enabled = false;
        col.enabled = false;

        var o = GetComponent<ObjectBehaviour>();
        var ps = fx.GetComponent<ParticleSystem>().main;
        ps.startColor = o.isGrey ? Color.grey : (o.isLight ? Color.white : Color.black);
        
        yield return new WaitForSeconds(1);

        DeadRoutine = null;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * vspeed);
        Gizmos.DrawLine(transform.position + Vector3.up * 16, transform.position + Vector3.up * 16 + Vector3.right * hspeed);
    }
}
