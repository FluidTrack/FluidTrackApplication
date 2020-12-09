using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public class OpeningHandler : MonoBehaviour
{
    public RectTransform ProgressBar;
    public Text ProgressLog;
    public Animator NetworkError;

    void Start() {
        ProgressLog.text = "어플리케이션 초기화 중";
        ProgressBar.sizeDelta = new Vector2(80f,58.3f);
        StartCoroutine(CheckNetwork());
    }

    IEnumerator CheckNetwork() {
        yield return new WaitForSeconds(0.5f);
        ProgressLog.text = "네트워크 연결 확인 중";
        ProgressBar.sizeDelta = new Vector2(500f, 58.3f);
        UnityWebRequest request = new UnityWebRequest();

        using (request = UnityWebRequest.Get(DataHandler.ServerAddress + "read_users")) {
            yield return request.SendWebRequest();

            if (request.isNetworkError) {
                yield return new WaitForSeconds(2f);
                NetworkError.SetTrigger("active");
            } else {
                Debug.Log(request.downloadHandler.text);
                yield return new WaitForSeconds(0.9f);
                ProgressLog.text = "이전 데이터 확인 중";
                ProgressBar.sizeDelta = new Vector2(1200f, 58.3f);
                StartCoroutine(CheckUser());
            }
        }
    }

    public void QuitApplication() {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator CheckUser() {
        bool flag = true;
        yield return 0;
        try {
            FileStream fs = new FileStream(DataHandler.dataPath + "/userData", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            DataHandler.User_id = int.Parse(sr.ReadLine());
        } catch ( System.Exception e ) {
            Debug.Log(e.ToString());
            flag = false;
        }
        yield return new WaitForSeconds(0.5f);

        if (flag) { 
            ProgressLog.text = "초기화 완료";
            ProgressBar.sizeDelta = new Vector2(1800f, 58.3f);
            StartCoroutine(DataHandler.read_users(DataHandler.User_id));

            while(true) {
                yield return 0;
                if(DataHandler.User_isDataLoaded) {
                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FOOTER_BAR].SetActive(true);
                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].SetActive(true);
                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.NAVI_BAR].SetActive(true);
                    Instantiate(TotalManager.instance.FlashEffect);
                    break;
                }
            }
            this.gameObject.SetActive(false);

        } else {
            ProgressLog.text = "초기화 실패";
            ProgressBar.sizeDelta = new Vector2(1800f, 58.3f);
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME].SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

}
