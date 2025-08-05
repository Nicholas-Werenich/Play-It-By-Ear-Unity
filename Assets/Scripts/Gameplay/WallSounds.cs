using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

//Sounds the player makes when running into walls
public class WallSounds : MonoBehaviour
{

    [Header("Wall Check")]
    [SerializeField] private float playerSize;
    [SerializeField] private float raycastDistance;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> wallImpactSounds;
    [SerializeField] private AudioClip wallScraping;

    private bool touchingWall = false;
    private Vector2 currentWall;

    public LayerMask wallLayer;

    private bool hitUp = false;
    private bool hitDown = false;
    private bool hitLeft = false;
    private bool hitRight = false;
    //private int wallLayer = 8;

    private void Awake()
    {
        audioSource.clip = wallScraping;
    }

    void Update()
    {
        CheckWallCollision();
    }

    //Check collisions for each direction to allow touching 2 walls at once
    void CheckWallCollision()
    {
        bool newHitUp = RaycastDirection(Vector2.up);
        bool newHitDown = RaycastDirection(Vector2.down);
        bool newHitLeft = RaycastDirection(Vector2.left);
        bool newHitRight = RaycastDirection(Vector2.right);

        if (newHitUp && !hitUp) OnNewWallHit();
        if (newHitDown && !hitDown) OnNewWallHit();
        if (newHitLeft && !hitLeft) OnNewWallHit();
        if (newHitRight && !hitRight) OnNewWallHit();

        hitUp = newHitUp;
        hitDown = newHitDown;
        hitLeft = newHitLeft;
        hitRight = newHitRight;
    }

    //Check collisions with raycasts instead of colliders for accuracy
    private bool RaycastDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, playerSize/2f + raycastDistance, wallLayer);
        return hit.collider != null;
    }

    private void OnNewWallHit()
    {
        audioSource.PlayOneShot(wallImpactSounds[Random.Range(0,wallImpactSounds.Count)]);
    }
}
