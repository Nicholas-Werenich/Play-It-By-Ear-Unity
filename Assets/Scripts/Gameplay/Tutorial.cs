using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Play sounds during the tutorial
public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<AudioClip> tutorialAudio;
    [SerializeField] private List<GameObject> tutorialCheckPoints;
    private Dictionary<GameObject,AudioClip> tutorialPoints;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();   

        tutorialPoints = new Dictionary<GameObject,AudioClip>();
        for (int i = 0; i < tutorialCheckPoints.Count; i++)
        {
            Debug.Log(i);
            tutorialPoints.Add(tutorialCheckPoints[i],tutorialAudio[i]);
        }
    }

    //Check player collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tutorialPoints.ContainsKey(collision.gameObject))
        {
            TutorialPoint(collision.gameObject);
        }
    }

    //Play tutorial clip
    private void TutorialPoint(GameObject tutorialCheckPoint)
    {
        audioSource.clip = tutorialPoints[tutorialCheckPoint];
        audioSource.Play();
        Destroy(tutorialCheckPoint);
    }
}

