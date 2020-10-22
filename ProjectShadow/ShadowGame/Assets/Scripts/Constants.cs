using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static float gravity = -0.16f;

    public static float block_friction = 1f;

    public static float door_enter_time = 1f;

    public static bool NearZero(float value)
    {
        if (Mathf.Abs(value) < 0.01f)
            return true;
        return false;
    }

    public static float OverlapBoundAsPercent(Vector2 AA, Vector2 AB, Vector2 BA, Vector2 BB)
    {
        float area = Mathf.Abs((AA.x - AB.x) * (AA.y - AB.y));

        float overlap = Mathf.Abs(Mathf.Max(AA.x, BA.x) - Mathf.Min(AB.x, BB.x)) * Mathf.Abs(Mathf.Max(AA.y, BA.y) - Mathf.Min(AB.y, BB.y));
        
        return overlap / area;
    }

    public static Vector3 SetDepth (Vector3 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }
}
