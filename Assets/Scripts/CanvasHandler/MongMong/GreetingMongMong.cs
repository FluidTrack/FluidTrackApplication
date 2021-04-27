using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreetingMongMong : MonoBehaviour
{
    public GameObject GreetingMongMongObject;
    public Text GreetingText;
    public Animator Anim;
    public string[] greetingQuote = new string[3];
    public AudioClip[] greetingVoice = new AudioClip[3];
    public AudioClip SFX;
    public bool isGreeting = true;

    private bool isPlayedVoice = false;
    private int rand = 0;
    public void Start() {
        if (isGreeting) {
            GreetingMongMongObject.SetActive(true);
            rand = Random.Range(0, 3);
            GreetingText.text = greetingQuote[rand];
            TouchAndMouseManager.Instance.isTouchEnable = false;
            StartCoroutine(PlaySFX());
        } else this.enabled = false;
    }

    IEnumerator PlaySFX() {
        yield return new WaitForSeconds(1.4f);
        SoundHandler.Instance.MongMongSource2.PlayOneShot(SFX);
    }

    public void Update() {
        if (isPlayedVoice) return;
        if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Greeting_Idle")) {
            SoundHandler.Instance.MongMongSource.PlayOneShot(greetingVoice[rand]);
            isPlayedVoice = true;
        }
    }

    public void OkayButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        GreetingMongMongObject.SetActive(false);
        TouchAndMouseManager.Instance.isTouchEnable = true;
        this.enabled = false;
    }
}
