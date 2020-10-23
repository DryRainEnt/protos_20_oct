using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehaviour : MonoBehaviour
{
    float upper;
    float lower;
    float left;
    float right;

    // Update is called once per frame
    void Update()
    {
        var pos = Constants.SetDepth(WorldBehaviour.player.transform.position, -10);
        if (upper - pos.y < 240f) pos = new Vector3(pos.x, upper - 240f, pos.z);
        if (pos.y - lower < 240f) pos = new Vector3(pos.x, lower + 240f, pos.z);
        if (pos.x - left < 240f) pos = new Vector3(left + 320f, pos.y, pos.z);
        if (right - pos.x < 240f) pos = new Vector3(right - 320f, pos.y, pos.z);

        if (upper - lower < 480) pos = new Vector3(pos.x, 0, pos.z);
        if (right - left < 640) pos = new Vector3(0, pos.y, pos.z);

        transform.position = pos;
    }
}
