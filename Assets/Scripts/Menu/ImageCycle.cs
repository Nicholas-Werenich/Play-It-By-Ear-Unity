using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Cycle between background images
//NOT USED
public class ImageCycle : MonoBehaviour
{

    [SerializeField] private List<Sprite> images = new List<Sprite>();
    [SerializeField] private float cycleTime;
    private float currentTime = 0;
    private Image image;
    private int index = 0;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        currentTime = cycleTime;
        NextImage();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        if(currentTime < 0)
        {
            currentTime = cycleTime;
            NextImage();
        }
    }

    private void NextImage()
    {
        Debug.Log(index);
        image.sprite = images[index];
        index++;
        if(index == images.Count)
            index = 0;
    }
}
