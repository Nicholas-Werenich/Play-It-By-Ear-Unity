using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

//Visuals for level complete menu
public class LevelComplete : MonoBehaviour
{
    static public int jemsCollected = 0;
    static public bool levelCompleted = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip jemCollected;

    [SerializeField] private AudioClip[] diamondAmountNarratorClips;


    private AudioSource audioPlayer;

    private Image jemVisual;

    [Header("Jem Amounts")]
    [SerializeField] private Sprite[] jemImageAmounts;

    [Header("Timing")]
    [SerializeField] private float timeBetweenJems;


    private void Awake()
    {
        levelCompleted = true;

        jemVisual = GetComponent<Image>();
        audioPlayer = GetComponentInParent<AudioSource>();

        audioPlayer.PlayOneShot(winSound);

        Time.timeScale = 0;
        StartCoroutine(GameWin());
    }

    //Slowly load jems in with sound effects for maximum joy
    private IEnumerator GameWin()
    {
        int jemsCollectedTemp = jemsCollected;
        jemsCollected = 0;
        for (int i = 0; i < jemsCollectedTemp; i++)
        {
            Debug.Log("Jem :" + i);
            yield return new WaitForSecondsRealtime(timeBetweenJems);

            jemVisual.sprite = jemImageAmounts[i];
            audioPlayer.PlayOneShot(jemCollected);
        }


        audioPlayer.clip = diamondAmountNarratorClips[jemsCollectedTemp];


        audioPlayer.Play();
        levelCompleted = false;
    }
}
