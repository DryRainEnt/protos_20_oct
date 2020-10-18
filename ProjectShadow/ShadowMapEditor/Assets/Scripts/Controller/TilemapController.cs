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

        if (Camera.main.orthographicSize > 20f || Input.mouseScrollDelta.y < 0f)
            Camera.main.orthographicSize += Input.mouseScrollDelta.y * -10f;

        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.position += Vector3.left * mouseDelta.x;
            Camera.main.transform.position += Vector3.down * mouseDelta.y;
        }

        mouseDelta = (Vector2)Input.mousePosition;
    }
}
