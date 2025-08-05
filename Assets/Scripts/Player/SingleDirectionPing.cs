using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//Send a single soundwave in a direction
public class SingleDirectionPing : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;

    private PerformanceTestRaycast echolocation;
    private bool isUsed = false;

    private MouseButton buttonKey = MouseButton.Right;
   
    private void Awake()
    {
        echolocation = GetComponent<PerformanceTestRaycast>();
    }

    public void OnInteract(InputValue inputValue)
    {
        Debug.Log("KEY PRESSED");
    }

    private void Update()
    {
        if (Input.GetMouseButton(((int)buttonKey)))
        {
            if(PingDirection() != Vector2.zero)
            {
                if(!isUsed)
                    echolocation.SingleRaycast(PingDirection(), audioClip);
                isUsed = true;
            }
            else
                isUsed = false;
        }
    }

    //Direction of raycast
    private Vector2 PingDirection()
    {
        if (Input.GetAxis("Horizontal") > 0)
            return Vector2.right;
        else if (Input.GetAxis("Horizontal") < 0)
            return Vector2.left;
        else if (Input.GetAxis("Vertical") > 0)
            return Vector2.up;
        else if (Input.GetAxis("Vertical") < 0)
            return Vector2.down;

        return Vector2.zero;
    }

}
