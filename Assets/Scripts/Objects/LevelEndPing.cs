using System.Collections.Generic;
using UnityEngine;

//Ping to show player where end of level is
public class LevelEndPing : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;

    [Header("References")]
    [SerializeField] private GameObject levelEnd;
    private PerformanceTestRaycast echolocation;
    private KeyCode levelEndPingKey = KeyCode.LeftShift;

    [Header("Visible Raycast")]
    private static Material _lineMaterial;
    private readonly List<(Vector3 start, Vector3 end, Color color)> lines = new();

    [SerializeField] private float raycastLineLifetime;

    private int playerLayer = 6;

    private void Awake()
    {
        echolocation = GetComponent<PerformanceTestRaycast>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            LevelEndSound();
    }

    private void LevelEndSound()
    {
        Vector2 direction = (transform.position - levelEnd.transform.position).normalized;
        echolocation.ReceiveRaycast(echolocation.raycastTotalDistance/2f, direction, audioClip);
        Debug.Log("Recieving raycast");
        //Get the localized distance between the two to play sound at set volume, have narrator say distance between player and exit in meters (Record each individual digit)
    }

    void CreateLineMaterial()
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

    public void DrawRaycastPath(RaycastHit2D hit, Vector2 direction)
    {
        if (hit.collider == null) return;

        Vector3 start = hit.point;
        Vector3 end = hit.point + (direction.normalized * -hit.distance);

        Color drawColor = (
            ((1 << hit.collider.gameObject.layer) & playerLayer) != 0
                ? Color.yellow
                : UnityEngine.Random.ColorHSV() + Color.yellow
        );

        activeLines.Add(new LineData(start, end, drawColor, raycastLineLifetime));
    }


    void OnRenderObject()
    {
        if (activeLines.Count == 0) return;

        CreateLineMaterial();
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

    private readonly List<LineData> activeLines = new();
}
