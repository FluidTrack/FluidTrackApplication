using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingWindowHandler : MonoBehaviour
{
    static public ConnectingWindowHandler Instance;

    public void PlaySFX() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.SMALL_PING);
    }

    public void OnEnable() {
        StartCoroutine(AutoDelete());
        Instance = this;
    }

    public IEnumerator AutoDelete() {
        yield return new WaitForSeconds(2.5f);
        if(!TotalManager.instance.isRegisterMode) {
            MoabandStatusHandler.Instance.ConnectTimeRefresh();
            if(SettingWindowHandler.Instance != null)
                SettingWindowHandler.Instance.MoabandStatusText.text =
                    MoabandStatusHandler.Instance.MakingConnectTime();
        }
        this.gameObject.SetActive(false);
    }

    public void OnDisable() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CONNECT);
    }
}
