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
    }

    public void ResetGame()
    {
        WorldBehaviour.player.lastDoor = WorldBehaviour.player.lastSave;
        WorldBehaviour.player.isDead = true;

        foreach (ObjectBehaviour obj in WorldBehaviour.instance.objectShiftPool)
        {
            if (obj.stage != StageController.instance.currentStage) continue;
            obj.gameObject.SetActive(true);
            var push = obj.GetComponent<PushableBehaviour>();
            if (push)
            {
                push.transform.position = push.initPos;
                push.sr.enabled = true;
                push.col.enabled = true;
            }
        }
        foreach (ObjectBehaviour obj in WorldBehaviour.instance.objectGreyPool)
        {
            if (obj.stage != StageController.instance.currentStage) continue;
            obj.gameObject.SetActive(true);
            var push = obj.GetComponent<PushableBehaviour>();
            if (push)
            {
                push.isDead = false;
                push.transform.position = push.initPos;
                push.sr.enabled = true;
                push.col.enabled = true;
            }
        }
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
