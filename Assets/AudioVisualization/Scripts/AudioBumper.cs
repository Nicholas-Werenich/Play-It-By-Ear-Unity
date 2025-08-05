using UnityEngine;

public class AudioBumper : MonoBehaviour {

    public AudioPeer audioPeer;
    public int frequencyBand = 0;
    public float multiplier = 0.2f;
    public Vector3 bumpDimensions = new Vector3(0, 1, 0);

    private Vector3 baseScale;

    private void Start() {
        baseScale = transform.localScale;
    }

    void Update() {
        if (audioPeer == null) {
            return;
        }
        transform.localScale = baseScale + bumpDimensions * audioPeer.GetFrequencyBand(frequencyBand) * multiplier;
        //transform.localPosition = new Vector2(transform.localPosition.x, -transform.localScale.y / 8f);
        float audioBand = audioPeer.GetFrequencyBand(frequencyBand);
    }
}
