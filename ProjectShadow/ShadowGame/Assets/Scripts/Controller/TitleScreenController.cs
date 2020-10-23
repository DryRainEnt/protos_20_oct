using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour
{
    public GameObject menuGroup;
    public Button start;
    public RawImage banner;

    bool endOpening = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OpeningRoutineHolder());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
            endOpening = true;
    }

    IEnumerator OpeningRoutineHolder()
    {
        var routine = StartCoroutine(OpeningRoutine());
        yield return routine;
        Debug.Log("Routine End");
        endOpening = true;
        banner.transform.position = new Vector3(banner.transform.position.x, -240f);
        menuGroup.SetActive(true);
        start.Select();
    }

    IEnumerator OpeningRoutine()
    {
        Debug.Log("Routine Start");
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;
            if (endOpening) yield break;
            yield return null;
        }

        if (endOpening) yield break;

        while (timer < 4)
        {
            var progress = 1 / (1 + Mathf.Pow(2.414f, 6 - 6 * (timer - 1)));
            banner.transform.position = new Vector3(banner.transform.position.x, 240f - progress * 480f);
            timer += Time.deltaTime;
            if (endOpening) yield break;
            yield return null;
        }

        if (endOpening) yield break;

        while (timer < 2)
        {
            timer += Time.deltaTime;
            if (endOpening) yield break;
            yield return null;
        }

    }
}
