using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Functions for common menu buttons
public class MenuButtons : MonoBehaviour
{
    [Header("Audio")]
    private AudioSource audioSource;

    //For entering and exiting menus
    [SerializeField] private AudioClip enterButton;
    [SerializeField] private AudioClip exitButton;

    private bool initialLoad = true;

    //Setting default raycast amount
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (!initialLoad)
            PlayerPrefs.SetInt("Raycast Amount", 120);
    }

    //Load main menu and reset variables
    public void MainMenu()
    {
        LevelComplete.jemsCollected = 0;
        audioSource.PlayOneShot(enterButton);

        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    //Load main menu and reset variables while playing animation
    public void MainMenuWithTransition()
    {
        LevelComplete.jemsCollected = 0;
        DisableButtonAfterUse();

        audioSource.PlayOneShot(enterButton);

        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, "Main Menu"));
    }

    //Load settings menu
    public void Settings()
    {
        LevelComplete.jemsCollected = 0;
        audioSource.PlayOneShot(enterButton);

        Time.timeScale = 1f;

        PlayerPrefs.SetString("Last Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Settings Menu");

    }

    //Load settings menu with animation
    public void SettingsWithTransition()
    {

        LevelComplete.jemsCollected = 0;
        DisableButtonAfterUse();

        audioSource.PlayOneShot(enterButton);

        PlayerPrefs.SetString("Last Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, "Settings Menu"));
    }

    //Quit game
    public void Quit()
    {

        audioSource.PlayOneShot(exitButton);

        Application.Quit();
    }

    //Go back to previous scene
    public void Back()
    {
        if(!LevelComplete.levelCompleted)
            Time.timeScale = 1f;
        audioSource.PlayOneShot(exitButton);

        FindFirstObjectByType<PauseMenu>().UnLoadMenu();
    }

    //Go back to previous scene with transition
    public void BackWithTransition()
    {
        if (!LevelComplete.levelCompleted)
            Time.timeScale = 1f;
        DisableButtonAfterUse();

        audioSource.PlayOneShot(exitButton);

        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, PlayerPrefs.GetString("Last Scene")));
    }

    //Load next level
    public void Continue()
    {

        Time.timeScale = 1.0f;
        audioSource.PlayOneShot(enterButton);

        PlayerPrefs.SetInt("Current Level", PlayerPrefs.GetInt("Current Level") + 1);
        SceneManager.LoadScene($"Level {PlayerPrefs.GetInt("Current Level")}");
    }

    //Load first level
    public void PlayWithTransition()
    {
        DisableButtonAfterUse();

        audioSource.PlayOneShot(enterButton);

        ThresholdController threshholdController = FindFirstObjectByType<ThresholdController>();
        PlayerPrefs.SetInt("Current Level", 1);
        StartCoroutine(threshholdController.TriggerExitAnimation(threshholdController.exitTime, "Level 1"));
    }

    //Disable button to stop spam calling
    private void DisableButtonAfterUse()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
    }
}
