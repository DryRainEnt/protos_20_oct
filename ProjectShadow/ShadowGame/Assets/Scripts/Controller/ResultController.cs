using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    public Text time;
    public Text stageCount;
    public Text stages;
    public Text deaths;

    public float _time;
    public int _stageCount;
    public string _stages;
    public int _deaths;
    
    // Start is called before the first frame update
    void Awake()
    {
        _time = 0f ;
        _stageCount = 0;
        _stages = "";
        _deaths = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeController.instance.OnFadeRoutine) return;

        time.text = string.Format("PlayTime:\n{0}:{1}.{2}", Mathf.FloorToInt(_time / 60), Mathf.FloorToInt(_time % 60), Mathf.FloorToInt((_time - Mathf.FloorToInt(_time)) * 100));
        stageCount.text = string.Format("{0}\nStages", _stageCount);
        stages.text = _stages;
        deaths.text = string.Format("Death: {0}", _deaths);

        if (_time < SystemController.instance.playTime)
            _time++;
        else
            _time = SystemController.instance.playTime;

        if (_stageCount < SystemController.instance.stages.Count)
        {
            _stageCount++;

            _stages = "";
            for (int i = 0; i < SystemController.instance.stages.Count; i++)
            {
                _stages += SystemController.instance.stages[i] + "\n";
            }
            stages.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30 * _stageCount);
        }

        if (_deaths < SystemController.instance.deathCount)
            _deaths++;

    }

    public void PlayAgain()
    {
        SystemController.instance.StartGame();
    }

    public void ResetGame()
    {
        SystemController.instance.ExitGame();
    }
}
