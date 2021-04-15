using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingWindowHandler : MonoBehaviour
{
    public void PlaySFX() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.SMALL_PING);
    }

    public void OnEnable() {
        StartCoroutine(AutoDelete());
    }

    public IEnumerator AutoDelete() {
        yield return new WaitForSeconds(2.5f);
        this.gameObject.SetActive(false);
        MoabandStatusHandler.Instance.ConnectTimeRefresh();
        SettingWindowHandler.Instance.MoabandStatusText.text =
            MoabandStatusHandler.Instance.MakingConnectTime();
    }

    public void OnDisable() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CONNECT);
    }
}
