using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    CharacterBehaviour master;
    InteractableBehaviour interactable;

    public SpriteRenderer sr;
    public Animator anim;
    public BoxCollider2D col;

    public string targetStageName;

    public float openTime;

    public bool isOpen;

    Coroutine DoorOpenRoutine;
    Coroutine DoorEnterRoutine;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        col = gameObject.GetComponent<BoxCollider2D>();
        interactable = GetComponent<InteractableBehaviour>();

        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && !Constants.NearZero(interactable.interactCall))
        {
            if (isOpen)
            {
                interactable.StartInteract();
                DoorEnterStart(interactable.master);
                interactable.EndInteract();
            }
            else
            {
                interactable.StartInteract();
                DoorOpenStart(interactable.master);
                interactable.EndInteract();
            }
            interactable.interactCall = 0f;
        }
    }

    public void DoorOpenStart(CharacterBehaviour handler)
    {
        if (handler == WorldBehaviour.player)
            if (WorldBehaviour.player.UseKey())
                DoorOpenRoutine = StartCoroutine(DoorOpen());
    }

    IEnumerator DoorOpen()
    {
        anim.SetBool("IsOpen", true);
        yield return new WaitForSeconds(openTime);
        isOpen = true;
        DoorOpenRoutine = null;
        interactable.faceInteract = false;
    }

    public void DoorEnterStart(CharacterBehaviour handler)
    {
        if (handler == WorldBehaviour.player)
            DoorEnterRoutine = StartCoroutine(DoorEnter());
    }

    IEnumerator DoorEnter()
    {
        yield return new WaitForSeconds(Constants.door_enter_time);
        isOpen = true;
        DoorEnterRoutine = null;
    }

}
