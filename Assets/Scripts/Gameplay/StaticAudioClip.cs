using UnityEngine;

public class StaticAudioClip : MonoBehaviour
{
    static public AudioClip audioClip { get; private set; }

    private void Awake()
    {
        audioClip = Resources.Load("Modern2") as AudioClip;
    }
}
