using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("Visible Raycast")]
    private static Material _lineMaterial;
    private readonly List<LineData> activeLines = new();

    [SerializeField] private float raycastLineLifetime = 1f;
    private int playerLayer = 6;

    private void Awake()
    {
        CreateLineMaterial();
    }

    private void Update()
    {
        for (int i = activeLines.Count - 1; i >= 0; i--)
        {
            activeLines[i].remainingTime -= Time.deltaTime;
            if (activeLines[i].remainingTime <= 0)
            {
                activeLines.RemoveAt(i);
            }
        }
    }

    private void CreateLineMaterial()
    {
        if (_lineMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void DrawRaycastPath(RaycastHit2D hit, Vector2 direction, Color? rayColor = null)
    {
        if (hit.collider == null) return;

        Vector3 start = hit.point;
        Vector3 end = hit.point + (direction.normalized * -hit.distance);

        Color drawColor = rayColor ?? (
            hit.collider.gameObject.layer == playerLayer
                ? Color.blue
                : Color.red
        );

        activeLines.Add(new LineData(start, end, drawColor, raycastLineLifetime));
    }

    private void OnRenderObject()
    {
        if (activeLines.Count == 0) return;

        _lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        foreach (var line in activeLines)
        {
            GL.Color(line.color);
            GL.Vertex(line.start);
            GL.Vertex(line.end);
        }

        GL.End();
        GL.PopMatrix();
    }

    private class LineData
    {
        public Vector3 start, end;
        public Color color;
        public float remainingTime;

        public LineData(Vector3 start, Vector3 end, Color color, float duration)
        {
            this.start = start;
            this.end = end;
            this.color = color;
            this.remainingTime = duration;
        }
    }
}
