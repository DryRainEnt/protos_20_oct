using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : ILoadableBehaviour
{
    public static SFXController instance;

    public AudioClip LightBGM;
    public AudioClip ShadowBGM;
    
    public AudioSource SFX;
    public AudioSource lightAudio;
    public AudioSource shadowAudio;

    private void Awake()
    {
        instance = this;

        SFX = gameObject.AddComponent<AudioSource>();
        SFX.volume = 0.7f;
        SFX.playOnAwake = false;
        SFX.loop = false;

        lightAudio = gameObject.AddComponent<AudioSource>();
        lightAudio.clip = LightBGM;
        lightAudio.playOnAwake = false;
        lightAudio.loop = true;

        shadowAudio = gameObject.AddComponent<AudioSource>();
        shadowAudio.clip = ShadowBGM;
        shadowAudio.playOnAwake = false;
        shadowAudio.loop = true;
    }

    // Start is called before the first frame update
    public override IEnumerator Start()
    {
        yield return StartCoroutine(base.Start());
        yield return null;
        lightAudio.Play();
        shadowAudio.Play();
        OnShift(0, false);
    }

    public void PlaySFX(string name)
    {
        SFX.clip = Resources.Load<AudioClip>("Audio/SFX/" + name);
        SFX.Play();
    }

    public void OnShift(float duration, bool playSFX = true)
    {
        if (playSFX) PlaySFX("ShiftSFX");
        if (WorldBehaviour.instance.isLight)
        {
            StartCoroutine(ShiftSFXRoutine(shadowAudio, lightAudio, duration));
        }
        else
        {
            StartCoroutine(ShiftSFXRoutine(lightAudio, shadowAudio, duration));
        }
    }
    
    IEnumerator ShiftSFXRoutine(AudioSource from, AudioSource to, float duration)
    {
        float timer = 0;

        while (timer < duration)
        {
            from.volume = Mathf.Lerp(1, 0, timer / duration);
            to.volume = Mathf.Lerp(0, 1, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        from.volume = 0.0f;
        to.volume = 1.0f;
    }

}
