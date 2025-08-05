using UnityEngine;
using UnityEngine.UI;

//Handling audio and visual for thee selected element
public class ElementSelected : MonoBehaviour
{

    [Header("Sub Element")]
    [SerializeField] private bool invertSubElement = false;
    [SerializeField] private MaskableGraphic[] subElements;

    [Header("Color")]
    [SerializeField] private Color[] invertedColor;
    private Color[] originalColour;

    [Header("Audio")]
    [SerializeField] private AudioClip narratorClip;
    [SerializeField] private AudioSource narratorAudioPlayer;
    [SerializeField] private AudioSource ambienceAudioPlayer;
    private AudioClip selectedAudio;

    private void Awake()
    {
        //Play button selected sound
        selectedAudio = Resources.Load("Modern2") as AudioClip;
        
        //Get the inverted colour for switching
        if(invertSubElement)
        {
            originalColour = new Color[subElements.Length];
            for (int i = 0; i < subElements.Length; i++)
            {
                originalColour[i] = subElements[i].color;
            }
        }
    }


    public void OnSelection()
    {
        //Narrator button dub
        if (selectedAudio == null) Debug.Log(gameObject.name);
        ambienceAudioPlayer.PlayOneShot(selectedAudio);
        narratorAudioPlayer.clip = narratorClip;
        narratorAudioPlayer.Play();

        //Invert all sub elements colours,
        if(invertSubElement)
        {
            for (int i = 0; i < subElements.Length; i++)
            {
                subElements[i].color = invertedColor[i];
            }
        }

    }

    public void OnDeselection()
    {
        //Invert all sub elements colours back to normal
        if (invertSubElement)
        {
            for (int i = 0; i < subElements.Length; i++)
            {
                if (subElements[i] != null)
                {
                    subElements[i].color = originalColour[i];
                }
            }
        }
    }
}
