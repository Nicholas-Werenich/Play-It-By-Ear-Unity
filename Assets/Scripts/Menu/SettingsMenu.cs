using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;

//Functions for settings menu
public class SettingsMenu : MonoBehaviour
{

    [Header("Audio")]
    [SerializeField] private AudioSource narratorAudioPlayer;
    [SerializeField] private AudioSource ambienceAudioPlayer;

    [SerializeField] private AudioClip narratorToggleOn;
    [SerializeField] private AudioClip narratorToggleOff;

    [SerializeField] private AudioClip sliderIncrease;
    [SerializeField] private AudioClip sliderDecrease;

    [SerializeField] private AudioClip toggleOn;
    [SerializeField] private AudioClip toggleOff;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer mixerGroup;

    [Header("Objects")]

    private ToggleGroup audioQualityToggleGroup;
    [SerializeField] private List<Toggle> audioQualityToggles;


    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle narratorToggle;

    [SerializeField] private Slider narratorSlider;
    [SerializeField] private Slider gameplaySlider;
    [SerializeField] private Slider ambienceSlider;

    [SerializeField] private TMP_InputField narratorInput;
    [SerializeField] private TMP_InputField gameplayInput;
    [SerializeField] private TMP_InputField ambienceInput;

    //[SerializeField] private Toggle 

    private MenuAccessibility menuAccessibility;
    private int narratorIndex;
    static bool firstLoad = true;

    private void Awake()
    {
        menuAccessibility = FindFirstObjectByType<MenuAccessibility>();
        narratorIndex = menuAccessibility.UIElements.IndexOf(narratorSlider);

        if (firstLoad)
        {
            InitializeDefaults();
            firstLoad = false;
        }
        LoadSettings();
    }

    //Default values for settings
    private void InitializeDefaults()
    {
        PlayerPrefs.SetInt("Audio Quality Toggle", 0);
        PlayerPrefs.SetFloat("Narrator Volume", 80);
        PlayerPrefs.SetFloat("Gameplay Volume", 80);
        PlayerPrefs.SetFloat("Ambience Volume", 80);
        PlayerPrefs.SetInt("Fullscreen Toggle", 1);
        PlayerPrefs.SetInt("Narrator Toggle", 1);
        PlayerPrefs.Save();
    }

    //Load last saved settings
    private void LoadSettings()
    {
        int toggleIndex = PlayerPrefs.GetInt("Audio Quality Toggle", 0);
        if (toggleIndex >= 0 && toggleIndex < audioQualityToggles.Count)
        {
            audioQualityToggles[toggleIndex].isOn = true;
            ApplyAudioQuality(toggleIndex);
        }

        float narratorVol = PlayerPrefs.GetFloat("Narrator Volume", 80);
        float gameplayVol = PlayerPrefs.GetFloat("Gameplay Volume", 80);
        float ambienceVol = PlayerPrefs.GetFloat("Ambience Volume", 80);

        narratorSlider.value = narratorVol;
        gameplaySlider.value = gameplayVol;
        ambienceSlider.value = ambienceVol;

        SetNarratorVolume(narratorVol);
        SetGameplayVolume(gameplayVol);
        SetAmbienceVolume(ambienceVol);

        SetFullScreen(PlayerPrefs.GetInt("Fullscreen Toggle", 1) == 1);
        SetNarratorToggle(PlayerPrefs.GetInt("Narrator Toggle", 1) == 1);
    }

    public void SetNarratorVolume(float volume)
    {
        PlaySliderSound(volume, "Narrator Volume");
        mixerGroup.SetFloat("Narrator Volume", volume - 80);
        narratorInput.text = volume.ToString();
        narratorSlider.value = volume;
        PlayerPrefs.SetFloat("Narrator Volume", volume);
    }

    public void SetGameplayVolume(float volume)
    {
        PlaySliderSound(volume, "Gameplay Volume");
        gameplayInput.text = volume.ToString();
        mixerGroup.SetFloat("Gameplay Volume", volume - 80);
        gameplaySlider.value = volume;
        PlayerPrefs.SetFloat("Gameplay Volume", volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        PlaySliderSound(volume, "Ambience Volume");
        mixerGroup.SetFloat("Ambience Volume", volume - 80);
        ambienceInput.text = volume.ToString();
        ambienceSlider.value = volume;
        PlayerPrefs.SetFloat("Ambience Volume", volume);
    }

    private void PlaySliderSound(float newValue, string key)
    {
        float oldValue = PlayerPrefs.GetFloat(key, 80);
        ambienceAudioPlayer.PlayOneShot(newValue > oldValue ? sliderIncrease : sliderDecrease);
    }

    //Change amount of raycasts used in pings
    public void SetAudioQuality()
    {
        int selectedIndex = audioQualityToggles.FindIndex(t => t.isOn);
        if (selectedIndex != -1)
        {
            PlayerPrefs.SetInt("Audio Quality Toggle", selectedIndex);
            PlayerPrefs.Save();
            ApplyAudioQuality(selectedIndex);
        }
    }

    private void ApplyAudioQuality(int index)
    {
        Debug.Log("Applying audio quality settings for index: " + index);
        audioQualityToggles[index].isOn = true;

        int raycastAmount = 0;
        switch (index)
        {
            case 0:
                raycastAmount = 180; break;
            case 1:
                raycastAmount = 200; break;
            case 2:
                raycastAmount = 220; break;

        }

        PlayerPrefs.SetInt("Raycast Amount", raycastAmount);
        Debug.Log("Raycast amount: " + PlayerPrefs.GetInt("Raycast Amount"));
    }

    public void SetFullScreen(bool isFullscreen)
    {
        PlayToggleSound(isFullscreen);
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen Toggle", isFullscreen ? 1 : 0);
        fullScreenToggle.isOn = isFullscreen;
    }

    public void SetNarratorToggle(bool isNarratorEnabled)
    {
        PlayToggleSound(isNarratorEnabled);
        PlayerPrefs.SetInt("Narrator Toggle", isNarratorEnabled ? 1 : 0);
        narratorToggle.isOn = isNarratorEnabled; 

        narratorSlider.interactable = isNarratorEnabled;
        narratorInput.interactable = isNarratorEnabled;
        SetNarratorVolume(isNarratorEnabled ? PlayerPrefs.GetFloat("Narrator Volume", 80) : -80);
    }

    private void PlayToggleSound(bool isOn)
    {
        narratorAudioPlayer.clip = isOn ? narratorToggleOn : narratorToggleOff;
        narratorAudioPlayer.Play();
        ambienceAudioPlayer.PlayOneShot(isOn ? toggleOn : toggleOff);
    }
}

