using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertHandler : MonoBehaviour
{
    public PopUpHandler [] Popup;

    public AudioClip LOW_BAT;
    public AudioClip CHARGE_BAT;
    public AudioClip BAT_INFO;
    public AudioClip CON;
    public AudioClip ALERT;
    public AudioClip ERROR;
    public AudioClip DISCON;
    public AudioClip MSG;

    public enum POPUPS {
        LOW_BAT,
        CHARGE_BAT,
        BAT_INFO,
        CON,
        ALERT,
        ERROR,
        DISCON,
        MSG,
    };

    public bool isPopped = false;

    IEnumerator popCheck() {
        isPopped = true;
        yield return new WaitForSeconds(3f);
        isPopped = false;
    }

    public static AlertHandler GetInstance() {
        GameObject go = GameObject.Find("MainCanvas");
        return go.GetComponent<AlertHandler>();
    }

    public void Pop_LowBat(int bat) {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(LOW_BAT);
        Popup[0].SubText.text = "모아밴드의 배터리 잔량이 얼마 남지 않았습니다.\n" + 
            "배터리 잔량 : " + bat + "%";
        Popup[0].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_ChargeBat(int percent) {
        if (isPopped) return;
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(CHARGE_BAT);

        Popup[1].SubText.text = "모아밴드 배터리 잔량 : " + percent + "%";
        Popup[1].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_BatInfo(int percent) {
        if (isPopped) return;
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(BAT_INFO);

        Popup[2].SubText.text = "모아밴드 배터리 잔량 : " + percent + "%";
        Popup[2].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_Con() {
        if (isPopped) return;
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(CON);

        Popup[3].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_Alert(string alert) {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(ALERT);

        Popup[4].SubText.text = alert;
        Popup[4].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_Error(string error) {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(ERROR);

        Popup[5].SubText.text = error;
        Popup[5].anim.SetTrigger("Pop");
        StartCoroutine(popCheck());
    }

    public void Pop_Discon(string name) {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(DISCON);

        if (isPopped) return;
        Popup[6].anim.SetTrigger("Pop");
        Popup[6].SubText.text = name + " 모아밴드와\n연결이 끊어졌습니다.";
        StartCoroutine(popCheck());
    }

    public void Pop_Msg(string str) {
        Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(MSG);

        Popup[7].anim.SetTrigger("Pop");
        Popup[7].SubText.text = str;
        StartCoroutine(popCheck());
    }
}
