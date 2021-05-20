using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerAlertPageMongMongHandler : MonoBehaviour
{
    public Text QuoteText;
    public string[] Quotes;
    public AudioClip[] Voices;

    public void OnEnable() {
        SoundHandler.Instance.MongMongSource.Stop();
        SoundHandler.Instance.MongMongSource2.Stop();
        int rand = Random.Range(0, Voices.Length);
        SoundHandler.Instance.MongMongSource.PlayOneShot(Voices[rand]);
        QuoteText.text = Quotes[rand];
    }

    public void StopVoice() {
        SoundHandler.Instance.MongMongSource.Stop();
        SoundHandler.Instance.MongMongSource2.Stop();
    }
}
