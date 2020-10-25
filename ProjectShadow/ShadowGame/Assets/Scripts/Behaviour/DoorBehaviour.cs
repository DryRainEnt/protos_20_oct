using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    CharacterBehaviour master;
    InteractableBehaviour interactable;
    AudioSource SFX;

    public SpriteRenderer sr;
    public Animator anim;
    public BoxCollider2D col;

    public bool isClear;
    public string targetStageName;

    public float openTime;

    public bool isOpen;

    Coroutine DoorOpenRoutine;
    Coroutine DoorEnterRoutine;

    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        col = gameObject.GetComponent<BoxCollider2D>();
        interactable = GetComponent<InteractableBehaviour>();
        SFX = gameObject.AddComponent<AudioSource>();
        SFX.clip = Resources.Load<AudioClip>("Audio/SFX/" + (isClear ? "ClearDoorSFX" : "DoorSFX"));
        SFX.playOnAwake = false;
        SFX.loop = false;

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
            else if (DoorOpenRoutine == null)
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
        {
            if (isClear)
            {
                if (WorldBehaviour.player.UseClearKey())
                    DoorOpenRoutine = StartCoroutine(DoorOpen());
            }
            else
            {
                if (WorldBehaviour.player.UseKey())
                    DoorOpenRoutine = StartCoroutine(DoorOpen());
            }
        }
    }

    IEnumerator DoorOpen()
    {
        SFX.Play();
        GetComponent<ObjectBehaviour>().wait = true;
        anim.SetBool("IsOpen", true);
        yield return new WaitForSeconds(openTime);
        isOpen = true;
        DoorOpenRoutine = null;
        interactable.faceInteract = false;
        GetComponent<ObjectBehaviour>().wait = false;
    }

    public void DoorEnterStart(CharacterBehaviour handler)
    {
        if (handler == WorldBehaviour.player)
        {
            var targetStage = StageDoorPool.instance.stageDoorPool[GetComponent<ObjectBehaviour>().target];
            var targetDoor = GetComponent<ObjectBehaviour>().target;

            if (!StageController.instance.stages[targetStage].activeInHierarchy)
                StageController.instance.ActivateStage(targetStage);

            WorldBehaviour.player.transform.position = GameObject.Find(targetDoor).transform.position + Vector3.up * 4;
            var des = GameObject.Find(targetDoor).GetComponent<DoorBehaviour>();
            des.isOpen = true;
            if (des.interactable) des.interactable.faceInteract = false;
            WorldBehaviour.player.lastDoor = des;
            if (des.anim) des.anim.SetTrigger("OpenImmediate");

            if (targetStage != StageController.instance.currentStage)
            {
                StageController.instance.SetCurrentStage(targetStage);
            }

        }
    }
    
}
