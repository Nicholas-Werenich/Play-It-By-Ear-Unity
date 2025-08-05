
using UnityEngine;

//Sound packet variable object
public class SoundPacket
{
    public float volume;
    public float delay;

    //1 = infront
    //0 = behind
    public float behindFactor;

    public Vector3 soundPosition;
    public AudioClip clip;
    
    public SoundPacket(float volume, float delay, Vector3 soundPosition, AudioClip clip, float behindFactor = 1)
    {
        this.volume = volume;
        this.delay = delay;
        this.soundPosition = soundPosition;
        this.clip = clip;
        this.behindFactor = behindFactor;
    }
}