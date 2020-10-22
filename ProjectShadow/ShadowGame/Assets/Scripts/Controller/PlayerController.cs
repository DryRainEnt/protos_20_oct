using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    none,
    shift,
    ladder,
    interact
}

public class PlayerController : MonoBehaviour
{
    private CharacterBehaviour behaviour;

    private bool isMovable = true;

    private ActionType actionType;
    private bool onShift = false;

    public float jumpPower = 3f;

    public float moveAccel = 0.3f;
    public float moveSpeed = 3f;

    public float ladderSpeed = 1f;

    public KeyCode rightKey = KeyCode.D;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;

    public KeyCode interactKey = KeyCode.E;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode shiftKey = KeyCode.LeftShift;

    // Start is called before the first frame update
    void Start()
    {
        behaviour = GetComponent<CharacterBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        behaviour.interactObjectCall = 0f;
        behaviour.ladderDropCall = 0f;

        if (behaviour.actionStamp < 0 && behaviour.onGround && behaviour.landingStamp < 0)
        {
            if (actionType == ActionType.shift)
            {
                gameObject.layer = behaviour.isLight ? LayerMask.NameToLayer("LightCharacter") : LayerMask.NameToLayer("ShadowCharacter");
                actionType = ActionType.none;
            }
            if (Input.GetKeyDown(shiftKey))
            {
                behaviour.actionDelay = 0.5f;
                behaviour.actionStamp = 0f;
                actionType = ActionType.shift;
                behaviour.anim.SetTrigger("Shift");
                onShift = true;
                
                if (behaviour.onPush)
                    behaviour.interactObjectCall = behaviour.sr.transform.localScale.x;
            }
        }

        if (behaviour.actionStamp > 0.3f && actionType == ActionType.shift && onShift)
        {
            WorldBehaviour.instance.Shift();
            behaviour.anim.ResetTrigger("Shift");
            Camera.main.backgroundColor = behaviour.isLight ? Color.white : Color.black;
            onShift = false;
        }

        #region MovementControl
        if (behaviour.actionStamp > 0f)
        {
            behaviour.hspeed = 0f;
            behaviour.vspeed = 0f;
            isMovable = false;
        }
        else
            isMovable = true;
        
        if (!isMovable || actionType == ActionType.shift)
            return;

        var hInput = 0;
        if (Input.GetKey(rightKey))
        {
            hInput += 1;
        }
        if (Input.GetKey(leftKey))
        {
            hInput -= 1;
        }

        if (Input.GetKey(jumpKey) && behaviour.onGround && behaviour.landingStamp < 0 && actionType == ActionType.none)
        {
            behaviour.vspeed += jumpPower;
            behaviour.anim.SetTrigger("Jump");
            if (behaviour.onPush)
                behaviour.interactObjectCall = behaviour.sr.transform.localScale.x;
        }

        if (Mathf.Abs(behaviour.hspeed) <= moveSpeed)
            behaviour.hspeed += moveAccel * hInput;
        else if (behaviour.hspeed < 0)
            behaviour.hspeed = -1 * moveSpeed;
        else
            behaviour.hspeed = moveSpeed;

        if (hInput == 0)
        {
            if (Mathf.Abs(behaviour.hspeed) > moveAccel)
                behaviour.hspeed -= Mathf.Sign(behaviour.hspeed) * moveAccel;
            else
                behaviour.hspeed = 0f;
        }
        #endregion

        #region LadderControl
        behaviour.ladderDropCall = 0f;
        var vInput = 0;
        if (Input.GetKey(upKey))
        {
            vInput += 1;
        }
        if (Input.GetKey(downKey))
        {
            vInput -= 1;
        }

        if (behaviour.onLadder)
            behaviour.vspeed = vInput * ladderSpeed;
        else
        {
            if (behaviour.onGround)
                actionType = ActionType.none;
            if (vInput == 0)
                behaviour.ladderGrabCall = 0;
        }

        if (Input.GetKeyDown(upKey) || Input.GetKeyDown(downKey))
            behaviour.ladderGrabCall = vInput;

        if (Input.GetKeyDown(leftKey) || Input.GetKeyDown(rightKey))
            behaviour.ladderDropCall = hInput;
        
        #endregion

        if (Input.GetKeyDown(interactKey))
        {
            behaviour.interactObjectCall = behaviour.sr.transform.localScale.x;
        }
    }
}
