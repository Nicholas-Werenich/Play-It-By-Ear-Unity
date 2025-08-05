using UnityEngine;

//Sound object script to hold variables and decay over time
public class Lifetime : MonoBehaviour
{
    [Header("Time Properties")]
    [SerializeField] public float lifeTime;
    [SerializeField] public float dyingSpeed;
    [SerializeField] private float bouncePenalty;

    private float remainingLifeTime;

    public AudioClip clip;
    public float volumeSensitivity = 1;
    //6 layermask is player
    private int playerLayer = 6;


    public void Awake()
    {
        remainingLifeTime = lifeTime;
    }
    public void Update()
    {
        //Die out overtime
        remainingLifeTime -= Time.deltaTime * dyingSpeed;

        if(remainingLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    //Check hit for player or penalty
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
            HitPlayer();
        remainingLifeTime -= bouncePenalty;  
    }

    //Play sound at position of player hit
    private void HitPlayer()
    {
        float volume = Map(volumeSensitivity * remainingLifeTime, 0, (lifeTime - bouncePenalty) * volumeSensitivity, 0.1f, 1);
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);

        Destroy(gameObject);
    }

    //Bad mapping function for volume
    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return Mathf.Abs((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }
}
