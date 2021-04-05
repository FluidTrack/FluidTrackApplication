using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorOcurredWindowHandler : MonoBehaviour
{
    public static ErrorOcurredWindowHandler Instance;
    public GameObject SendToServerEffect;
    public Text msgText;
    public void ErrorMsg(string str) {
        msgText.text = str;
    }

    public void Awake() {
        Instance = this;
    }

    public void OkayButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        this.gameObject.SetActive(false);
    }

    public void BugReportClick() {
        SendToServerEffect.SetActive(true);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA1);
        DataHandler.ErrorLogs error = new DataHandler.ErrorLogs(msgText.text);
        StartCoroutine(DataHandler.CreateErrorlogs(error));
        this.gameObject.SetActive(false);
    }
}
