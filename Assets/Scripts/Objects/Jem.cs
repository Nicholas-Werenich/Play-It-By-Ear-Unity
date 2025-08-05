using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//The collectible item players can hear
public class JemCollectable : MonoBehaviour
{

    [Header("Raycast Properties")]
    public int raycastAmount { get; private set; }

    [SerializeField] private float raycastTotalDistance;
    [SerializeField] private float bouncePenalty;

    [Header("References")]
    private PerformanceTestRaycast playerRaycastEcholocation;
    private CircleCollider2D circleCollider;
    private bool playerInRadius;
    private int playerLayer = 6;
    private int wallLayer = 8;
    private LayerMask raycastMask;
    private LineDrawer lineDrawer;


    [Header("Timer")]
    [SerializeField] private float soundEmitTimer;
    private float currentSoundEmitTimer;

    [Header("Audio")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip pickupSound;

    [Header("Visible Raycast")]
    private static Material _lineMaterial;
    private readonly List<(Vector3 start, Vector3 end, Color color)> lines = new();

    [SerializeField] private float raycastLineLifetime;

    private void Awake()
    {
        raycastAmount = PlayerPrefs.GetInt("Raycast Amount");

        raycastMask = LayerMask.GetMask("Player", "Wall");
        playerRaycastEcholocation = FindFirstObjectByType<PerformanceTestRaycast>();

        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = raycastTotalDistance * 4;

        currentSoundEmitTimer = soundEmitTimer;

        lineDrawer = FindFirstObjectByType<LineDrawer>();
    }

    private void Update()
    {
        currentSoundEmitTimer -= Time.deltaTime;

        if (currentSoundEmitTimer < 0 && playerInRadius)
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

    private float GetRaycastAngle()
    {
        return 360f / raycastAmount;
    }

    //Send out raycasts in circle, if contact then call RaycastContact()
    private void EmitRaycasts()
    {
        for (int i = 0; i < raycastAmount; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(360f - i * GetRaycastAngle() * Mathf.Deg2Rad), Mathf.Sin(360f - i * GetRaycastAngle() * Mathf.Deg2Rad)).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastTotalDistance, raycastMask);

            if (hit.collider != null)
            {
                DrawRaycastPath(hit, direction);

                //Divide by bounce penalty to simualte loss, minus one to stop infinite bouncing
                playerRaycastEcholocation.RaycastContact(hit, raycastTotalDistance - hit.distance, direction, idleSound);
            }
        }
    }

    //If plyaer contacts, collectJem
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == playerLayer) CollectJem();
    }

    //Add Jem to total
    private void CollectJem()
    {
        GameObject.Find("Player").GetComponent<AudioSource>().PlayOneShot(pickupSound);
        LevelComplete.jemsCollected++;
        Destroy(gameObject);
    }

    //Start emitting sound
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer) playerInRadius = true;
    }

    //Stop emitting sound
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer) playerInRadius = false;
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
                ? Color.green
                : UnityEngine.Random.ColorHSV() + Color.green
        );

        //activeLines.Add(new LineData(start, end, drawColor, raycastLineLifetime));
        lineDrawer.DrawRaycastPath(hit, direction, Color.green);
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
