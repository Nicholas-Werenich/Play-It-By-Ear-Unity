using UnityEngine;

public class LoadAudioManager : MonoBehaviour
{
    static bool audioManagerLoaded = false;
    [SerializeField] private GameObject audioManager;

    private void Awake()
    {
        if (!audioManagerLoaded)
        {
            Instantiate(audioManager);
            audioManagerLoaded = true;
        }
    }
}
