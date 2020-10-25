using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;
    RawImage FadeBlack;
    ImageAnimation Loading;
    Text Message;

    public bool OnFadeRoutine;
    Coroutine messageRoutine;


    private void Awake()
    {
        if (FadeController.instance)
            Destroy(instance.gameObject);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        FadeBlack = GetComponentInChildren<RawImage>();
        Loading = GetComponentInChildren<ImageAnimation>();
        Loading.gameObject.SetActive(false);

        Message = GetComponentInChildren<Text>();
        Message.text = "";
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
        Loading.gameObject.SetActive(true);
    }

    public void LoadingEnd()
    {
        Loading.gameObject.SetActive(false);
    }

    public void ErrorMessage(string text)
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }
        messageRoutine = StartCoroutine(ErrorMessageRoutine(text));
    }

    IEnumerator ErrorMessageRoutine(string text)
    {
        Message.text = text;
        yield return new WaitForSeconds(3);
        Message.text = "";
    }
}
