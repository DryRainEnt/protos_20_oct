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

    public ItemBehaviour usedKey;
    public bool isOpen;

    Coroutine DoorOpenRoutine;
    Coroutine DoorEnterRoutine;

    // Start is called before the first frame update
    void Awake()
    {
        if (!isOpen)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            anim = GetComponentInChildren<Animator>();
            col = gameObject.GetComponent<BoxCollider2D>();
            interactable = GetComponent<InteractableBehaviour>();
            SFX = gameObject.AddComponent<AudioSource>();
            SFX.clip = Resources.Load<AudioClip>("Audio/SFX/" + (isClear ? "ClearDoorSFX" : "DoorSFX"));
            SFX.playOnAwake = false;
            SFX.loop = false;

            usedKey = null;
            isOpen = false;
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if (anim)
        {
            if (isOpen)
            {
                if (DoorOpenRoutine == null)
                    anim.SetBool("IsOpen", true);
            }
            else
            {
                anim.SetBool("IsOpen", false);
                anim.SetBool("OpenImmediate", false);
            }
            
        }
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
                usedKey = WorldBehaviour.player.UseClearKey();
                if (usedKey)
                {
                    DoorOpenRoutine = StartCoroutine(DoorOpen());
                }
            }
            else
            {
                usedKey = WorldBehaviour.player.UseKey();
                if (usedKey)
                    DoorOpenRoutine = StartCoroutine(DoorOpen());
            }
        }
    }

    public void DoorClose()
    {
        anim.SetBool("OpenImmediate", false);
        anim.SetBool("IsOpen", false);
        anim.Play(string.Format("{0}Closed", GetComponent<ObjectBehaviour>().type));
        isOpen = false;
    }

    IEnumerator DoorOpen()
    {
        SFX.Play();
        GetComponent<ObjectBehaviour>().wait = true;
        anim.SetBool("IsOpen", true);
        yield return new WaitForSeconds(openTime);
        isOpen = true;
        anim.SetBool("OpenImmediate", true);
        DoorOpenRoutine = null;
        interactable.faceInteract = false;
        GetComponent<ObjectBehaviour>().wait = false;
    }

    public void DoorEnterStart(CharacterBehaviour handler)
    {
        if (handler == WorldBehaviour.player)
        {

            var targetDoor = GetComponent<ObjectBehaviour>().target;
            
            if (!StageDoorPool.instance.stageDoorPool.ContainsKey(GetComponent<ObjectBehaviour>().target) || targetDoor == "" || targetDoor == "StageClear")
            {
                SystemController.instance.FinishGame();
                return;
            }

            var targetStage = StageDoorPool.instance.stageDoorPool[GetComponent<ObjectBehaviour>().target];
            
            if (!StageController.instance.stages[targetStage].activeInHierarchy)
                StageController.instance.ActivateStage(targetStage);

            WorldBehaviour.player.transform.position = GameObject.Find(targetDoor).transform.position + Vector3.up * 4;
            var des = GameObject.Find(targetDoor).GetComponent<DoorBehaviour>();
            if (des.interactable) des.interactable.faceInteract = false;
            des.isOpen = true;
            WorldBehaviour.player.lastDoor = des;
            if (des.anim)
            {
                des.anim.SetBool("IsOpen", true);
                des.anim.SetBool("OpenImmediate", true);
            }

            if (targetStage != StageController.instance.currentStage)
            {
                StageController.instance.SetCurrentStage(targetStage);
                WorldBehaviour.player.lastSave = des;
            }

        }
    }
    
}
