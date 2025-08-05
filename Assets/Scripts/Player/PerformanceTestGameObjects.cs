using Unity.VisualScripting;
using UnityEngine;

//Spawn sound objects to act as sound packets in realtime
//NOT USED
public class PerformanceTestGameObjects : MonoBehaviour
{
    [Header("Sound Object")]
    [SerializeField] private GameObject soundObject;
    [SerializeField] private int objectAmount;
    [SerializeField] private float objectSpeed;

    [Header("Player")]
    [SerializeField] private float playerSize = 1f;

    [Header("Audio")]
    [SerializeField] private float volumeSensitivity;
    private AudioSource audioSource;


    private bool spawnToggle = false;
    [SerializeField] private float soundInterval;
    private float currentSoundInterval;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            EmitSoundObjects();

        SpawnToggle();
    }


    //Toggle emitting at intervals
    private void SpawnToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !spawnToggle)
            spawnToggle = true;
        else if (Input.GetKeyDown(KeyCode.Space) && spawnToggle)
            spawnToggle = false;

            currentSoundInterval -= Time.deltaTime;

        if (spawnToggle && currentSoundInterval < 0)
        {
            currentSoundInterval = soundInterval;
            EmitSoundObjects();
        }
    }


    private float ObjectAngle()
    {
        return 360f / objectAmount;
    }

    //Positon for spawn (radius around player to stop collison inside of player)
    private Vector2 SoundObjectPosiion(int order)
    {
        Vector2 positon;
        positon.x = transform.position.x + playerSize * Mathf.Sin(order * ObjectAngle() * Mathf.Deg2Rad);
        positon.y = transform.position.y + playerSize * Mathf.Cos(order * ObjectAngle() * Mathf.Deg2Rad);
        return positon;
    }

    //Emmit objects in circle with a force and soundclip
    private void EmitSoundObjects()
    {
        for(int i = 0; i < objectAmount; i++)
        {
            GameObject currentSoundObject = Instantiate(soundObject, SoundObjectPosiion(i), Quaternion.Euler(0,0,360 - (i * ObjectAngle())));
            Debug.Log(360 - (i * ObjectAngle()));
            currentSoundObject.GetComponent<Rigidbody2D>().AddForce((currentSoundObject.transform.position - transform.position) *  objectSpeed);
            currentSoundObject.GetComponent<Lifetime>().clip = audioSource.clip;
            currentSoundObject.GetComponent<Lifetime>().volumeSensitivity = volumeSensitivity;
        }
    }
}
