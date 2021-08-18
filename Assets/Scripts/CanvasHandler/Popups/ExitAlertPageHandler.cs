using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAlertPageHandler : MonoBehaviour
{
    private bool isExit = false;
    public void EixtButton() {
        if(!isExit) {
            isExit = true;
            StartCoroutine(ExitProcess());
        }
    }

    IEnumerator ExitProcess() {
        FooterBarHandler.Instance.OnApplicationQuit();

        yield return new WaitForSeconds(0.3f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    public void CancelButton() {
        if(!isExit) {
            SoundHandler.Instance.MongMongSource.Stop();
            SoundHandler.Instance.MongMongSource2.Stop();
        }
    }
}
