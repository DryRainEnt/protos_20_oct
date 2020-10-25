using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemController : MonoBehaviour
{
    Coroutine ExclusiveRoutine;

    // Start is called before the first frame update
    public void StartGame()
    {
        StageListController.instance.WriteStages();
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

        FadeController.instance.FadeIn(1f);
        ExclusiveRoutine = null;
    }
}
