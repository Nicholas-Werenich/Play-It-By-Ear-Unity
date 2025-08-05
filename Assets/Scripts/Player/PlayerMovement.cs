using UnityEngine;
using UnityEngine.SceneManagement;

//Moving player in 2D space
public class PlayerMovement : MonoBehaviour
{

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [SerializeField] private float movementSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
    }
    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Movement()
    {
        rb.AddForce(new Vector2 (horizontalInput, verticalInput).normalized * movementSpeed, ForceMode2D.Force);
    }
}
