using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilemapBorder : MonoBehaviour
{
    public static TilemapBorder instance;
    public int width = 0;
    public int height = 0;

    public InputField Width;
    public InputField Height;

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
        int.TryParse(Width.text, out width);
        int.TryParse(Height.text, out height);

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
