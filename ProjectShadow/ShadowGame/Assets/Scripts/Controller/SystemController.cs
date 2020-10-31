using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemController : MonoBehaviour
{
    public static SystemController instance;
    Coroutine ExclusiveRoutine;

    public float playTime;
    public List<string> stages;
    public int deathCount;

    private void Awake()
    {
        if (SystemController.instance)
            Destroy(instance.gameObject);
        instance = this;
        stages = new List<string>();

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    public void StartGame()
    {
        StageListController.instance.WriteStages();
        if (StageListController.instance)
            if (StageListController.instance.count == 0)
            {
                FadeController.instance.ErrorMessage("No map have been selected!");
                return;
            }
            
        if (ExclusiveRoutine == null)
            ExclusiveRoutine = StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        FadeController.instance.FadeOut(1f);
        while (FadeController.instance.OnFadeRoutine)
        {
            yield return null;
        }
        FadeController.instance.LoadingStart();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameStage");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => !DataController.instance.OnLoad);

        FadeController.instance.LoadingEnd();
        FadeController.instance.FadeIn(1f);
        ExclusiveRoutine = null;
        playTime = 0f;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameStage")
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
                ResetGame();
            playTime += Time.deltaTime;
        }
        if (SceneManager.GetActiveScene().name == "TitleScreen")
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                StageListController.instance.AddInitialStages();
                StageListController.instance.RefreshMapList();
            }
        }
    }

    public void ResetGame()
    {
        WorldBehaviour.player.lastDoor = WorldBehaviour.player.lastSave;
        WorldBehaviour.player.isDead = true;

        #region ResetObjects
        foreach (ObjectBehaviour obj in WorldBehaviour.instance.objectShiftPool)
        {
            var isOn = obj.gameObject.activeInHierarchy;
            if (obj.stage != StageController.instance.currentStage) continue;
            obj.gameObject.SetActive(true);
            var push = obj.GetComponent<PushableBehaviour>();
            if (push)
            {
                push.GetComponent<ObjectBehaviour>().ResetPosition();
                push.sr.enabled = true;
                push.col.enabled = true;
                WorldBehaviour.instance.ItemUse(push.transform);
            }
            var door = obj.GetComponent<DoorBehaviour>();
            if (door && obj.type != "StartPoint")
            {
                if (door.isOpen && !(door.isOpen && door.isClear && !door.usedKey))
                {
                    door.DoorClose();
                    WorldBehaviour.instance.ItemUse(door.transform);
                }
                if (door.usedKey != null)
                {
                    door.usedKey.gameObject.SetActive(true);
                    door.usedKey.IsUsed = false;
                    door.usedKey.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    if (door.usedKey.GetComponent<ObjectBehaviour>().stage == StageController.instance.currentStage)
                    {
                        door.usedKey.ItemReset();
                        door.usedKey.transform.SetParent(StageController.instance.stages[StageController.instance.currentStage].transform.GetChild(0).Find("Gimmicks").transform);
                        door.usedKey.GetComponent<ObjectBehaviour>().ResetPosition();
                        WorldBehaviour.instance.ItemUse(door.usedKey.transform);
                    }
                    else
                        WorldBehaviour.player.GetItem(door.usedKey);
                }
                door.usedKey = null;
            }
            var item = obj.GetComponent<ItemBehaviour>();
            if (item && isOn)
            {
                item.ItemReset();
                item.transform.SetParent(StageController.instance.stages[StageController.instance.currentStage].transform.GetChild(0).Find("Gimmicks").transform);
                item.GetComponent<ObjectBehaviour>().ResetPosition();
                WorldBehaviour.player.items.Remove(item);
                WorldBehaviour.instance.ItemUse(item.transform);
            }
            if (obj.isLight != WorldBehaviour.instance.isLight)
                obj.gameObject.SetActive(false);
        }
        foreach (ObjectBehaviour obj in WorldBehaviour.instance.objectGreyPool)
        {
            var isOn = obj.gameObject.activeInHierarchy;
            if (obj.stage != StageController.instance.currentStage) continue;
            obj.gameObject.SetActive(true);
            var push = obj.GetComponent<PushableBehaviour>();
            if (push)
            {
                push.GetComponent<ObjectBehaviour>().ResetPosition();
                push.sr.enabled = true;
                push.col.enabled = true;
                WorldBehaviour.instance.ItemUse(push.transform);
            }
            var door = obj.GetComponent<DoorBehaviour>();
            if (door && obj.type != "StartPoint")
            {
                if (door.isOpen && !(door.isOpen && door.isClear && !door.usedKey))
                {
                    door.DoorClose();
                    WorldBehaviour.instance.ItemUse(door.transform);
                }
                if (door.usedKey != null)
                {
                    door.usedKey.gameObject.SetActive(true);
                    door.usedKey.IsUsed = false;
                    door.usedKey.GetComponentInChildren<SpriteRenderer>().enabled = true;
                    if (door.usedKey.GetComponent<ObjectBehaviour>().stage == StageController.instance.currentStage)
                    {
                        door.usedKey.ItemReset();
                        door.usedKey.transform.SetParent(StageController.instance.stages[StageController.instance.currentStage].transform.GetChild(0).Find("Gimmicks").transform);
                        door.usedKey.GetComponent<ObjectBehaviour>().ResetPosition();
                        WorldBehaviour.instance.ItemUse(door.usedKey.transform);
                    }
                    else
                        WorldBehaviour.player.GetItem(door.usedKey);
                    door.usedKey = null;
                }
            }
            var item = obj.GetComponent<ItemBehaviour>();
            if (item && isOn)
            {
                item.ItemReset();
                item.transform.SetParent(StageController.instance.stages[StageController.instance.currentStage].transform.GetChild(0).Find("Gimmicks").transform);
                item.GetComponent<ObjectBehaviour>().ResetPosition();
                WorldBehaviour.player.items.Remove(item);
                WorldBehaviour.instance.ItemUse(item.transform);
            }
            else if (item)
            {
                item.gameObject.SetActive(false);
            }
        }
        #endregion
        
    }

    public void FinishGame()
    {
        if (ExclusiveRoutine == null)
            ExclusiveRoutine = StartCoroutine(FinishGameRoutine());
    }

    IEnumerator FinishGameRoutine()
    {
        FadeController.instance.FadeOut(1f);
        while (FadeController.instance.OnFadeRoutine)
        {
            yield return null;
        }
        FadeController.instance.LoadingStart();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Result");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.LoadingEnd();
        FadeController.instance.FadeIn(1f);
        ExclusiveRoutine = null;
    }

    public void ExitGame()
    {
        if (ExclusiveRoutine == null)
            ExclusiveRoutine = StartCoroutine(ExitGameRoutine());
    }

    IEnumerator ExitGameRoutine()
    {
        FadeController.instance.FadeOut(1f);
        while (FadeController.instance.OnFadeRoutine)
        {
            yield return null;
        }
        FadeController.instance.LoadingStart();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TitleScreen");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.LoadingEnd();
        FadeController.instance.FadeIn(1f);
        ExclusiveRoutine = null;
    }
}
