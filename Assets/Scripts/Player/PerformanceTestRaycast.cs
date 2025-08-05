using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

//Emmit soundwaves in form of raycasts that ricochet and come back to player, collecting info on surroundings
public class PerformanceTestRaycast : MonoBehaviour
{
    [Header("Raycast Properties")]
    private float raycastAmount;
    [SerializeField] public float raycastTotalDistance;
    [SerializeField] private float bouncePenalty;
    //[SerializeField] private int bounceLimit;
    [Header("Player")]
    private int playerHit = 0;
    private float playerSize = 1;


    [Header("References")]
    private LayerMask playerLayer = 6;
    public LayerMask wallLayer;
    private LineDrawer lineDrawer;

    [Header("Audio")]
    [SerializeField] private float delay;
    [SerializeField] private float spatialExaggeration;
    [SerializeField] private float reverb;
    [SerializeField] private float behindEffectMinValue;
    [SerializeField] private float behindPitch;
    [SerializeField] private float behindLowCutOff;

    [SerializeField] private AudioMixerGroup gameplayAudio;
    [SerializeField] private AudioClip pingNoise;

    [Header("Input")]
    private bool spawnToggle = false;

    [Header("Timing")]
    [SerializeField] private float soundInterval;
    private float currentSoundInterval;

    [Header("Visible Raycast")]
    private static Material _lineMaterial;
    private readonly List<(Vector3 start, Vector3 end, Color color)> lines = new();

    [SerializeField] private float raycastLineLifetime;


    private List<SoundPacket> soundPackets = new List<SoundPacket>();

    private void Awake()
    {
        raycastAmount = PlayerPrefs.GetInt("Raycast Amount");
        lineDrawer = FindFirstObjectByType<LineDrawer>();
    }

    /* Controller Input and Vibration
     * Microphone input 
     * Friction rubbing sound when walking along wall
     * Different sound modifiers when hitting different materials\
     */

    private void Update()
    {
        /*
         * Call for every microphone sound package period
         * EmitRaycasts();
        */

        if(Input.GetMouseButtonDown(0))
        {
            playerHit = 0;
            EmitRaycasts();
            Debug.Log("Player hit: " + playerHit);
        }

        EmitSoundToggle();
    }

    private void FixedUpdate()
    {
        DelayCountDown();
    }

    //Constantly emmit raycasts at intervals
    private void EmitSoundToggle()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !spawnToggle)
            spawnToggle = true;
        else if (Input.GetKeyDown(KeyCode.Space) && spawnToggle)
            spawnToggle = false;

        currentSoundInterval -= Time.deltaTime;

        if (spawnToggle && currentSoundInterval < 0)
        {
            currentSoundInterval = soundInterval;
            EmitRaycasts();
        }
    }

    //The angle the raycast should be emmitted based on the current amount of them
    private float GetRaycastAngle()
    {
        return 360f / raycastAmount; 
    }

    //Send out raycasts in circle, if contact then call RaycastContact()
    private void EmitRaycasts()
    {
        for(int i = 0; i < raycastAmount; i++)
        {
            //Get direction of current emmition based on place in cycle and total amount of raycasts
            Vector2 direction = new Vector2(Mathf.Cos(360f - i * GetRaycastAngle() * Mathf.Deg2Rad), Mathf.Sin(360f - i * GetRaycastAngle() * Mathf.Deg2Rad)).normalized;

            //Send out raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastTotalDistance, wallLayer);

            //If the raycast manages to hit a wall
            if (hit.collider != null){
               
                //Draw raycast
                DrawRaycastPath(hit, direction);

                //Divide by bounce penalty to simualte loss, minus one to stop infinite bouncing
                RaycastContact(hit, (raycastTotalDistance - hit.distance) / bouncePenalty -1, direction, pingNoise);

            }
        }
    }

    //Emmit a singular raycast in one direction
    public void SingleRaycast(Vector2 direction, AudioClip audioClip)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastTotalDistance, wallLayer);

        if (hit.collider != null)
        {
            DrawRaycastPath(hit, direction, Color.gray);

            RaycastContact(hit, (raycastTotalDistance - hit.distance) / bouncePenalty - 1, direction, audioClip);
        }
    }

    //Recursively check raycast bounce. If contact will wall or player, then either create new raycast in reflection direction and then recall this RaycastContact() or call ReceiveRaycast() if hit player.
    //Otherwise raycast hits nothing and soundwave dies out
    public void RaycastContact(RaycastHit2D ray, float remainingDistance, Vector2 direction, AudioClip audioClip = null)
    {
        //if theres no more distance, die out
        if (remainingDistance <= 0)
            return;
        
        //Raycast hit wall, reflect
        if (ray.collider.gameObject.layer == 8)
        {
            Vector2 newDirection = Vector2.Reflect(direction, ray.normal).normalized;
            RaycastHit2D hit = Physics2D.Raycast(ray.point, newDirection, remainingDistance);

            //If new raycast hits wall, simulate again
            if (hit.collider != null)
            {
                DrawRaycastPath(hit, newDirection);
                RaycastContact(hit, (remainingDistance - hit.distance) / bouncePenalty -1, newDirection, audioClip);
            }
        }
        //Raycast hit player, start countdown for sound
        else if(ray.collider.gameObject.layer == playerLayer)
        {
            ReceiveRaycast(remainingDistance, ray.point, audioClip);
        }
    }

    //Store raycast with remaining distance mapped to volume and where it hit the player
    public void ReceiveRaycast(float remainingDistance, Vector3 hitPosition, AudioClip audioClip = null)
    {
        if (audioClip == null)
            audioClip = pingNoise;

        playerHit++;
        soundPackets.Add(new SoundPacket(MapVolume(remainingDistance), remainingDistance * bouncePenalty, hitPosition - transform.position, audioClip));
        
    }

    //Add one to each raycastDistance value in list of soundPackets, if the raycasts distance = max raycast distance then call PlaySound()
    private void DelayCountDown()
    {
        for(int i = 0; i < soundPackets.Count; i++)
        {
            soundPackets[i].delay += delay;

            if (soundPackets[i].delay >= raycastTotalDistance)
            {
                PlaySound(BehindFilter(soundPackets[i]));
                soundPackets.Remove(soundPackets[i]);
            }
        }
    }

    //Play sound at local position where ray hit player, at volume relevant to distance traveled
    private void PlaySound(SoundPacket soundPacket)
    {

        GameObject audioObject = new GameObject("AudioSource: " + soundPacket.volume);
        audioObject.transform.SetParent(transform);
        audioObject.transform.localPosition = soundPacket.soundPosition * spatialExaggeration;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1;
        audioSource.clip = soundPacket.clip;
        audioSource.volume = soundPacket.volume;
        audioSource.reverbZoneMix = reverb + (1f - soundPacket.behindFactor) * behindEffectMinValue;
        audioSource.pitch = Mathf.Pow(soundPacket.behindFactor, behindPitch);
        audioSource.outputAudioMixerGroup = gameplayAudio;
        audioSource.spatialize = true;

        AudioLowPassFilter lowPassFilter = audioObject.AddComponent<AudioLowPassFilter>();
        lowPassFilter.cutoffFrequency = 5000 * Mathf.Pow(soundPacket.behindFactor, behindLowCutOff);

        audioSource.Play();

        Destroy(audioObject, audioSource.clip.length);
    }

    //Filter to simulate hearing something behind you
    private SoundPacket BehindFilter(SoundPacket soundPacket)
    {
        if(soundPacket.soundPosition.y < 0)
        {
            //Change behind filter depending on how behind it is to player. Ex. Directly behind is the most muffled
            soundPacket.volume *= behindEffectMinValue + (Mathf.Abs(soundPacket.soundPosition.x) / (playerSize / 2)) * (1f - behindEffectMinValue);
            soundPacket.behindFactor = Mathf.Clamp(Mathf.Abs(soundPacket.soundPosition.x) + (playerSize/2f) + (1f - behindEffectMinValue),0.5f,1f);
        }
        return soundPacket;
    }

    //Map distance to resonable volume)
    private float MapVolume(float distance)
    {
        return Mathf.Abs(distance / (raycastTotalDistance / bouncePenalty));
    }

    //Draw raycast
    public void DrawRaycastPath(RaycastHit2D hit, Vector2 direction, Color? rayColor = null)
    {
        lineDrawer.DrawRaycastPath(hit, direction, rayColor);
    }

}
