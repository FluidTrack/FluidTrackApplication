using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPageMongMongHandler : MonoBehaviour
{
    public string[] MongMongQuotes_Poo;
    public AudioClip[] MongMongVoice_Poo;

    public string[] MongMongQuotes_Log;
    public AudioClip[] MongMongVoice_Log;

    public Text MongMongQuote;
    public GameObject[] MongMongObject;

    public void OnEnable() {
        int randomeMong = Random.Range(0, 2);
        MongMongObject[( randomeMong == 0 ) ? 0 : 1].SetActive(true);
        MongMongObject[( randomeMong == 0 ) ? 1 : 0].SetActive(false);
        if(LogCanvasHandler.Instance.isLogMongMong_Poo) {
            int randomSeed = Random.Range(0, MongMongQuotes_Poo.Length);
            MongMongQuote.text = MongMongQuotes_Poo[randomSeed];
            SoundHandler.Instance.MongMongSource.
                PlayOneShot(MongMongVoice_Poo[randomSeed]);
        } else {
            int randomSeed = Random.Range(0, MongMongQuotes_Log.Length);
            MongMongQuote.text = MongMongQuotes_Log[randomSeed];
            SoundHandler.Instance.MongMongSource.
                PlayOneShot(MongMongVoice_Log[randomSeed]);
        }
    }
}
