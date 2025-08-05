using UnityEngine;

//Audio Clip to be played on button interact
public class ButtonAudio : MonoBehaviour
{

    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = FindAnyObjectByType<AudioSource>();
    }
    public void OnClick()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
