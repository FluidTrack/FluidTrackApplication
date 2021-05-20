using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageMongMongHandler : MonoBehaviour
{
    public GameObject[] DefaultMongMong;
    public GameObject   ComplimentMongMong;
    public GameObject[] CongratulationMongMong;

    public Text QuoteText;

    public string[] WaterQuotes;
    public string[] PeeQuotes;
    public string[] PooQuotes;

    public AudioClip[] WaterVoices;
    public AudioClip[] PeeVoices;
    public AudioClip[] PooVoices;

    public GameObject[] QuotesObjects;

    public void OnEnable() {
        foreach (GameObject go in QuotesObjects)
            go.SetActive(false);
        InitMong();
        DefaultMongMong[Random.Range(0, 2)].SetActive(true);
    }

    public void InitMong() {
        DefaultMongMong[0].SetActive(false);
        DefaultMongMong[1].SetActive(false);
        ComplimentMongMong.SetActive(false);
        CongratulationMongMong[0].SetActive(false);
        CongratulationMongMong[1].SetActive(false);
    }

    public void WaterDrop(int waterCount) {
        StartCoroutine(WaterDrop2(waterCount));
    }

    public void PeeDrop() {
        StartCoroutine(PeeDrop2());
    }

    public void PooDrop() {
        StartCoroutine(PooDrop2());
    }

    public IEnumerator WaterDrop2(int waterCount) {
        yield return new WaitForSeconds(0.8f);
        foreach (GameObject go in QuotesObjects)
            go.SetActive(true);
        InitMong();
        if (waterCount != 10)
            ComplimentMongMong.SetActive(true);
        else CongratulationMongMong[Random.Range(0, 2)].SetActive(true);
       
        int rand = Random.Range(0, WaterVoices.Length);
        QuoteText.text = WaterQuotes[rand];
        yield return new WaitForSeconds(0.2f);
        SoundHandler.Instance.MongMongSource.Stop();
        SoundHandler.Instance.MongMongSource2.Stop();
        SoundHandler.Instance.MongMongSource.PlayOneShot(WaterVoices[rand]);
    }

    public IEnumerator PeeDrop2() {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject go in QuotesObjects)
            go.SetActive(true);
        InitMong();
        ComplimentMongMong.SetActive(true);

        int rand = Random.Range(0, PeeVoices.Length);
        QuoteText.text = PeeQuotes[rand];
        yield return new WaitForSeconds(0.2f);
        SoundHandler.Instance.MongMongSource.Stop();
        SoundHandler.Instance.MongMongSource2.Stop();
        SoundHandler.Instance.MongMongSource.PlayOneShot(PeeVoices[rand]);
    }

    public IEnumerator PooDrop2() {
        yield return new WaitForSeconds(0.8f);
        foreach (GameObject go in QuotesObjects)
            go.SetActive(true);
        InitMong();
        ComplimentMongMong.SetActive(true);

        int rand = Random.Range(0, PooVoices.Length);
        QuoteText.text = PooQuotes[rand];
        yield return new WaitForSeconds(0.2f);
        SoundHandler.Instance.MongMongSource.Stop();
        SoundHandler.Instance.MongMongSource2.Stop();
        SoundHandler.Instance.MongMongSource.PlayOneShot(PooVoices[rand]);
    }
}
