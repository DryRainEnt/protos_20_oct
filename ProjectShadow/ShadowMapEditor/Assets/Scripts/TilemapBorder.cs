using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapBorder : MonoBehaviour
{
    public static TilemapBorder instance;
    public int width = 0;
    public int height = 0;
    static Material lineMaterial;

    LineRenderer lineRenderer;

    private void Start()
    {
        instance = this;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 3f;
        lineRenderer.endWidth = 3f;
        lineRenderer.positionCount = 5;
    }

    void Update()
    {
        lineRenderer.SetPositions(
            new Vector3[] {
                new Vector3(-width / 2, -height / 2, -3),
                new Vector3(width / 2, -height / 2, -3),
                new Vector3(width / 2, height / 2, -3),
                new Vector3(-width / 2, height / 2, -3),
                new Vector3(-width / 2, -height / 2, -3)
            }
            );
    }
}
