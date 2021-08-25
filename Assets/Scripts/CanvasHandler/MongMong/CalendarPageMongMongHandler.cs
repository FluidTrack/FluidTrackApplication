using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarPageMongMongHandler : MonoBehaviour
{
    public string[] Quotes;
    public AudioClip[] Voices;
    public GameObject[] Objects;
    public Text MongMongQuote;
    public GameObject Circle;

    public void OnEnable() {
        Circle.SetActive(false);
        Objects[0].SetActive(false);
        Objects[1].SetActive(false);
        Objects[Random.Range(0, Objects.Length)].SetActive(true);
        int rand = Random.Range(0, Voices.Length);
        SoundHandler.Instance.MongMongSource2.PlayOneShot(Voices[rand]);
        MongMongQuote.text = Quotes[rand];
    }
}
