using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


//Sound made by the end of the level object to notify player of exit
public class FinishLine : MonoBehaviour
{
    [Header("Audio")]
    private int raycastAmount;
    [SerializeField] private float raycastTotalDistance;

    [Header("Sound")]
    [SerializeField] private AudioClip exitSound;
    [SerializeField] private AudioClip winSound;

    [Header("Timer")]
    [SerializeField] private float soundEmitTimer;
    private float currentSoundEmitTimer;

    [Header("References")]
    private int playerLayer = 6;
    private GameObject player;
    private PerformanceTestRaycast playerRaycast;
    private AudioSource audioSource;
    private Vector3 bounds;
    [SerializeField] private bool rotate = false;
    [SerializeField] private bool flip = false;
    private int signChange = 1;
    private LineDrawer lineDrawer;

    [Header("Visible Raycast")]
    private static Material _lineMaterial;
    private readonly List<(Vector3 start, Vector3 end, Color color)> lines = new();

    [SerializeField] private float raycastLineLifetime;


    private void Awake()
    {
        //Scale amount of raycasts based on settings and size
        raycastAmount = (int)(PlayerPrefs.GetInt("Raycast Amount")/10 * Mathf.Max(transform.localScale.x, transform.localScale.y));

        player = GameObject.Find("Player");
        audioSource = player.GetComponent<AudioSource>();
        playerRaycast = player.GetComponent<PerformanceTestRaycast>();
        bounds = GetComponent<SpriteRenderer>().bounds.size;

        lineDrawer = FindFirstObjectByType<LineDrawer>();
    }

    private void Update()
    {
        signChange = flip ? -1 : 1;

        currentSoundEmitTimer -= Time.deltaTime;

        if (currentSoundEmitTimer < 0)
        {
            currentSoundEmitTimer = soundEmitTimer;
            EmitRaycasts();
        }

        for (int i = activeLines.Count - 1; i >= 0; i--)
        {
            activeLines[i].remainingTime -= Time.deltaTime;
            if (activeLines[i].remainingTime <= 0)
            {
                activeLines.RemoveAt(i);
            }
        }
    }

    private Vector2 RaycastStartPosition(int i)
    {
        if (!rotate)
        {
            //Horizontal casting
            float yOffset = (bounds.y / raycastAmount * i) - (bounds.y / 2f);
            return (Vector2)transform.position + new Vector2(signChange * (bounds.x / 2f + 0.1f), yOffset);
        }
        else
        {
            //Vertical casting
            float xOffset = (bounds.x / raycastAmount * i) - (bounds.x / 2f);
            return (Vector2)transform.position + new Vector2(xOffset, signChange * (bounds.y / 2f + 0.1f));
        }
    }

    //Emit raycasts in parralel line
    private void EmitRaycasts()
    {
        Vector2 direction = rotate ? Vector2.up * signChange : Vector2.right * signChange;

        for (int i = 0; i < raycastAmount; i++)
        {
            Vector2 startPosition = RaycastStartPosition(i);
            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, raycastTotalDistance);

            if (hit.collider != null)
            {
                DrawRaycastPath(hit, direction);

                if (hit.collider.gameObject.layer == playerLayer)
                {
                    float adjustedDistance = raycastTotalDistance - Vector2.Distance(hit.point - direction * hit.distance, player.transform.position);
                    playerRaycast.ReceiveRaycast(adjustedDistance, hit.point, exitSound);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            OnFinish();
        }
    }

    //Level complete menu
    private void OnFinish()
    {
        audioSource.PlayOneShot(winSound);
        SceneManager.LoadScene("Level Complete", LoadSceneMode.Additive);
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
        lineDrawer.DrawRaycastPath(hit, direction, Color.yellow);
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
