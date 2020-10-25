using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;
    RawImage FadeBlack;
    Animator Loading;

    public bool OnFadeRoutine;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        FadeBlack = GetComponentInChildren<RawImage>();
        Loading = GetComponentInChildren<Animator>();
        Loading.gameObject.SetActive(false);
    }
    
    public void FadeOut(float duration)
    {
        if (!OnFadeRoutine)
        {
            OnFadeRoutine = true;
            StartCoroutine(FadeOutRoutine(duration));
        }
    }

    IEnumerator FadeOutRoutine(float duration)
    {
        OnFadeRoutine = true;
        float timer = 0;
        while (timer < duration)
        {
            FadeBlack.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        OnFadeRoutine = false;
    }

    public void FadeIn(float duration)
    {
        if (!OnFadeRoutine)
        {
            OnFadeRoutine = true;
            StartCoroutine(FadeInRoutine(duration));
        }
    }

    IEnumerator FadeInRoutine(float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            FadeBlack.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        OnFadeRoutine = false;
    }

    public void LoadingStart()
    {
        Loading.gameObject.SetActive(false);
        Loading.SetTrigger("Reset");
    }

    public void LoadingEnd()
    {
        Loading.ResetTrigger("Reset");
        Loading.gameObject.SetActive(false);
    }
}
