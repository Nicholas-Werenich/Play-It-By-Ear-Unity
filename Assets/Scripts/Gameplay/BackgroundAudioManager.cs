using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//Constant background music
public class BackgroundAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private List<AudioClip> backgroundNoises;
    //private string[] musicScenes = { "Main Menu", "Pause Menu", "Level Complete", "Settings Menu" };

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        foreach(AudioClip clip in backgroundNoises)
        {
            AudioSource currentSource = gameObject.AddComponent<AudioSource>();
            currentSource.outputAudioMixerGroup = audioMixer;
            currentSource.clip = clip;
            currentSource.loop = true;
            currentSource.Play();
        }
    }
}
