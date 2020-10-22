using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    public GameObject DebugUI;
    public PlayerController player;

    public InputField moveSpeed;
    public InputField moveAccel;
    public InputField jumpPower;
    public InputField ladderSpeed;

    public InputField stageName;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            if (!StageController.instance.LoadStage(stageName.text))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DebugUI.SetActive(!DebugUI.activeInHierarchy);
            if (DebugUI.activeInHierarchy)
            {
                moveSpeed.text = player.moveSpeed.ToString();
                moveAccel.text = player.moveAccel.ToString();
                jumpPower.text = player.jumpPower.ToString();
                ladderSpeed.text = player.ladderSpeed.ToString();
            }
        }

        if (DebugUI.activeInHierarchy)
        {
            player.moveSpeed = float.Parse(moveSpeed.text);
            player.moveAccel = float.Parse(moveAccel.text);
            player.jumpPower = float.Parse(jumpPower.text);
            player.ladderSpeed = float.Parse(ladderSpeed.text);
        }
    }
}
