using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

//Control threshold value of menu material
public class ThresholdController : MonoBehaviour
{

    [Header("Threshold Values")]
    [SerializeField] private float minThreshold;
    [SerializeField] private float maxThreshold;

    private float currentThreshold = 1;
    private float elapsedTime = 0;

    private float oldThreshold;
    private float targetValue;

    private bool reachedValue = true;
    private bool startAnimationEnded = false;
    private bool exitAnimation = false;
    private bool darknessSceneAdjust = false;

    [Header("Timing")]
    [SerializeField] private float enterTime;
    [SerializeField] public float exitTime;
    [SerializeField] private float idleTime;

    [Header("Material Settings")]
    private float darkness;
    private float currentDarkness;

    private float smoothness;
    private float currentSmoothness;
    

    private Material material;

    private void Awake()
    {
        material = GetComponent<Image>().material;

        darkness = 0.32f;
        smoothness = 0.28f;

        material.SetFloat("_Lightness", 1);
        material.SetFloat("_Threshold", 1);
        material.SetFloat("_Darkness", 0.001f);
        material.SetFloat("_Smoothness", 0);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (!startAnimationEnded)
        {
            EnterAnimation();
            return;
        }

        if (exitAnimation)
        {
            ExitAnimation();
            return;
        }

        if (reachedValue)
        {
            reachedValue = false;
            elapsedTime = 0;
            oldThreshold = currentThreshold;
            targetValue = targetValue == maxThreshold ? minThreshold : maxThreshold;
        }
        IdleAnimation();
    }

    //Animation for entering scene
    private void EnterAnimation()
    {
        float lerpingValue = elapsedTime / enterTime;
        currentThreshold = Mathf.SmoothStep(1, minThreshold, lerpingValue);
        material.SetFloat("_Threshold", currentThreshold);

        currentDarkness = Mathf.SmoothStep(0.001f, 1, lerpingValue);
        material.SetFloat("_Darkness", currentDarkness);

        currentSmoothness = Mathf.SmoothStep(0,smoothness, lerpingValue);
        material.SetFloat("_Smoothness", currentSmoothness);

        if (elapsedTime >= enterTime)
        {
            elapsedTime = 0;
            startAnimationEnded = true;
            material.SetFloat("_Lightness", 0);
        }
    }
    
    //Pulsing idle animation
    private void IdleAnimation()
    {
        float lerpingValue = elapsedTime / idleTime;
        currentThreshold = Mathf.Lerp(oldThreshold, targetValue, lerpingValue);
        material.SetFloat("_Threshold", currentThreshold);
 
        if (elapsedTime >= idleTime)
        {
            reachedValue = true;
        }

        if (!darknessSceneAdjust)
        {
            currentDarkness = Mathf.SmoothStep(1, darkness, lerpingValue * 2);
            material.SetFloat("_Darkness", currentDarkness);

            if(elapsedTime >= idleTime/2)
                darknessSceneAdjust = true;
        }
    }

    //Changing scene setup
    public IEnumerator TriggerExitAnimation(float time, string nextScene)
    {
        exitAnimation = true;
        elapsedTime = 0;
        yield return new WaitForSeconds(time - 0.1f);
        SceneManager.LoadScene(nextScene);
    }

    //Exit animation
    private void ExitAnimation()
    {
        float lerpingValue = elapsedTime / exitTime;
        currentThreshold = Mathf.SmoothStep(oldThreshold, 1, lerpingValue);
        material.SetFloat("_Threshold", currentThreshold);

        currentDarkness = Mathf.SmoothStep(darkness, 0.0001f, lerpingValue * 2);
        material.SetFloat("_Darkness", currentDarkness);

        currentSmoothness = Mathf.SmoothStep(smoothness, 0, lerpingValue * 2);
        material.SetFloat("_Smoothness", currentSmoothness);

        if (elapsedTime >= exitTime)
        {
            exitAnimation = false;
        }
    }

    //Reset on app quit
    private void OnApplicationQuit()
    {
        material.SetFloat("_Darkness", darkness);
        material.SetFloat("_Smoothness", smoothness);
    }

}
