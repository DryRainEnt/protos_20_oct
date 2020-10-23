using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapController : MonoBehaviour
{
    Vector2 mouseDelta;
    // Update is called once per frame
    void Update()
    {
        mouseDelta = (Vector2)Input.mousePosition - mouseDelta;

        if (!Input.GetKey(KeyCode.LeftShift))
            if (Camera.main.orthographicSize > 20f || Input.mouseScrollDelta.y < 0f)
                Camera.main.orthographicSize += Input.mouseScrollDelta.y * -10f;

        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.position += Vector3.left * mouseDelta.x;
            Camera.main.transform.position += Vector3.down * mouseDelta.y;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            var pos = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(0, 0, pos.z);
        }

        mouseDelta = (Vector2)Input.mousePosition;
    }
}
