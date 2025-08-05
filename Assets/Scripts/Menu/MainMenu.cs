using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("Scene Settings")]
    [SerializeField] private string nextScene;

    public void Settings()
    {
        PlayerPrefs.SetString("Last Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, "Settings Menu"));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, nextScene));
        
    }

}
