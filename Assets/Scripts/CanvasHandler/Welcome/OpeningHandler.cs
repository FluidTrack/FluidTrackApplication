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
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {}, null);
        }, (error) => {
            Debug.LogError("BLE Error : " + error);
            BluetoothLEHardwareInterface.Log("BLE Error: " + error);
        });
        ProgressBar.sizeDelta = new Vector2(230f * 0.2f,24f);
        StartCoroutine(CheckNetwork());
    }

    IEnumerator CheckNetwork() {
        yield return new WaitForSeconds(0.3f);
        //SoundHandler.Instance.Play_Music(0);
        yield return new WaitForSeconds(1.7f);
        ProgressLog.text = "네트워크 연결 확인 중";
        BluetoothLEHardwareInterface.StopScan();
        ProgressBar.sizeDelta = new Vector2(230f * 0.4f, 24f);
        UnityWebRequest request = new UnityWebRequest();

        using (request = UnityWebRequest.Get(DataHandler.ServerAddress + "read_users")) {
            yield return request.SendWebRequest();

            if (request.isNetworkError) {
                yield return new WaitForSeconds(2f);
                NetworkError.SetTrigger("active");
            } else {
                //Debug.Log(request.downloadHandler.text);
                yield return new WaitForSeconds(0.9f);
                ProgressLog.text = "이전 데이터 확인 중";
                ProgressBar.sizeDelta = new Vector2(230f * 0.7f, 24f);
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
            sr.Close(); fs.Close();
        } catch ( System.Exception e ) {
            e.ToString();
            flag = false;
        }
        yield return new WaitForSeconds(0.5f);
        if (flag) { 
            ProgressLog.text = "초기화 완료";
            ProgressBar.sizeDelta = new Vector2(230f, 24f);
            StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));


            while (true) {
                yield return 0;
                if(DataHandler.User_isDataLoaded) {
                    DataHandler.Garden_logs = new DataHandler.GardenLogsJson();
                    DataHandler.Water_logs = new DataHandler.WaterLogsJson();
                    DataHandler.Drink_logs = new DataHandler.DrinkLogsJson();
                    DataHandler.Poop_logs = new DataHandler.PoopLogsJson();
                    DataHandler.Pee_logs = new DataHandler.PeeLogsJson();
                    DataHandler.Garden_logs.GardenLogs = new DataHandler.GardenLog[0];
                    DataHandler.Water_logs.WaterLogs = new DataHandler.WaterLog[0];
                    DataHandler.Drink_logs.DrinkLogs = new DataHandler.DrinkLog[0];
                    DataHandler.Poop_logs.PoopLogs = new DataHandler.PoopLog[0];
                    DataHandler.Pee_logs.PeeLogs = new DataHandler.PeeLog[0];
                    DataHandler.CreateGardenIndex = new Queue<int> ();
                    DataHandler.CreateDrinkIndex = new Queue<int>();
                    DataHandler.CreateWaterIndex = new Queue<int>();
                    DataHandler.CreatePooIndex = new Queue<int>();
                    DataHandler.CreatePeeIndex = new Queue<int>();
                    StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
                    StartCoroutine(DataHandler.ReadDrinkLogs(DataHandler.User_id));
                    StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
                    StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
                    StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));

                    yield return new WaitForSeconds(0.5f);

                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FOOTER_BAR].SetActive(true);
                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].SetActive(true);
                    TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.NAVI_BAR].SetActive(true);
                    Instantiate(TotalManager.instance.FlashEffect);
                    DataHandler.User_isDataLoaded = false;
                    GreetingMongMong.Instance.SayHello();
                    break;
                }
            }
            this.gameObject.SetActive(false);

        } else {
            Debug.Log("초기화 실패");
            ProgressLog.text = "초기화 실패";
            DataHandler.Garden_logs = new DataHandler.GardenLogsJson();
            DataHandler.Water_logs = new DataHandler.WaterLogsJson();
            DataHandler.Drink_logs = new DataHandler.DrinkLogsJson();
            DataHandler.Poop_logs = new DataHandler.PoopLogsJson();
            DataHandler.Pee_logs = new DataHandler.PeeLogsJson();
            DataHandler.Garden_logs.GardenLogs = new DataHandler.GardenLog[0];
            DataHandler.Water_logs.WaterLogs = new DataHandler.WaterLog[0];
            DataHandler.Drink_logs.DrinkLogs = new DataHandler.DrinkLog[0];
            DataHandler.Poop_logs.PoopLogs = new DataHandler.PoopLog[0];
            DataHandler.Pee_logs.PeeLogs = new DataHandler.PeeLog[0];
            DataHandler.CreateGardenIndex = new Queue<int>();
            DataHandler.CreateDrinkIndex = new Queue<int>();
            DataHandler.CreateWaterIndex = new Queue<int>();
            DataHandler.CreatePooIndex = new Queue<int>();
            DataHandler.CreatePeeIndex = new Queue<int>();
            ProgressBar.sizeDelta = new Vector2(230f, 24f);
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME].SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

}
