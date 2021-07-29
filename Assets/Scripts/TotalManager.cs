using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class TotalManager : MonoBehaviour
{
    public static TotalManager instance;
    public GreetingMongMong Greeting;
    public GameObject OpeningCanvas;
    public GameObject[] OtherCanvas;
    public ErrorOcurredWindowHandler ErrorHandler;
    public string TargetDateString = "";
    public Font[] fonts;
    public bool SkipOpening;
    public GameObject FlashEffect;
    public CANVAS currentCanvas;
    public bool isDebugMode = true;
    public bool isRegisterMode = false;
    public string targetName = "";

    public enum FONT_FAMILY {
        NAUM_GOTHIC,
        HY_YUBSU,
        TYPO_DABANGGU,
        D2_CODING,
    }

    public enum CANVAS {
        NULL, NAVI_BAR, FOOTER_BAR,
        WELCOME, WELCOME2, WELCOME3, WELCOME4, WELCOME5,
        HOME, LOG, FLOWER, TABLE, CALENDAR, BLIND_CON,
    }

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type) {
        if (logString.StartsWith("AndroidJavaException") || logString.StartsWith("Coroutine continue failure")) return;
            if( type == (LogType)4 || type == LogType.Error ) {
                SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
                ErrorHandler.gameObject.SetActive(true);
                ErrorHandler.ErrorMsg(logString + "\n\n" + stackTrace);
            }
#if !UNITY_EDITOR
            if( type == (LogType)4 || type == LogType.Error ) {
                string str = logString + "\n\n" + stackTrace;
                DataHandler.ErrorLogs error = new DataHandler.ErrorLogs(str);
                StartCoroutine(DataHandler.CreateErrorlogs(error));
            }
#endif
    }

    public void Awake() {
        instance = this;
        DataHandler.dataPath = Application.persistentDataPath;


        if (SkipOpening) {
            try {
                FileStream fs = new FileStream(DataHandler.dataPath + "/userData", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                DataHandler.User_id = int.Parse(sr.ReadLine());
                sr.Close();
                fs.Close();
            } catch (System.Exception e) {
                e.ToString();
                Debug.LogWarning("Cannot found userData\nDataHandler.User_id set 1 as default value.");
                DataHandler.User_id = 1;
            }
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

            StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
            StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
            StartCoroutine(DataHandler.ReadDrinkLogs(DataHandler.User_id));
            StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
            StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
            StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));

            Greeting.SayHello();
        }
    }

    public void Start() {
        if (NavigationHandler.Instance != null)
            NavigationHandler.Instance.ModeSetting(isDebugMode);
        if (!SkipOpening) {
            OpeningCanvas.SetActive(true);
            foreach (GameObject go in OtherCanvas) {
                try { go.SetActive(false); }
                catch (System.Exception e) { e.ToString(); }
            }
        }

        try {
            FileStream fs = new FileStream(DataHandler.dataPath + "/joinLog", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            TimeHandler.DateTimeStamp joinLogStamp = new TimeHandler.DateTimeStamp(sr.ReadLine());
            TimeHandler.GetCurrentTime();
            sr.Close(); fs.Close();
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(joinLogStamp, TimeHandler.CurrentTime) != 0) {
                FileStream fs2 = new FileStream(DataHandler.dataPath + "/yesterDayJoinLog", FileMode.OpenOrCreate);
                StreamWriter sw2 = new StreamWriter(fs2);
                sw2.WriteLine(joinLogStamp.ToString());
                sw2.Close(); fs2.Close();

                FileStream fs3 = new FileStream(DataHandler.dataPath + "/joinLog", FileMode.OpenOrCreate);
                StreamWriter sw3 = new StreamWriter(fs3);
                sw3.WriteLine(TimeHandler.CurrentTime.ToString());
                sw3.Close(); fs3.Close();
            }
        } catch (System.Exception e) {
            e.ToString();
            TimeHandler.GetCurrentTime();
            DataHandler.lastJoin = TimeHandler.CurrentTime;
            FileStream fs2 = new FileStream(DataHandler.dataPath + "/joinLog", FileMode.OpenOrCreate);
            StreamWriter sr2 = new StreamWriter(fs2);
            sr2.WriteLine(TimeHandler.CurrentTime.ToString());
            sr2.Close(); fs2.Close();

            FileStream fs3 = new FileStream(DataHandler.dataPath + "/yesterDayJoinLog", FileMode.OpenOrCreate);
            StreamWriter sr3 = new StreamWriter(fs3);
            sr3.WriteLine(TimeHandler.CurrentTime.ToString());
            sr3.Close(); fs3.Close();
        }

        try {
            FileStream fs = new FileStream(DataHandler.dataPath + "/yesterDayJoinLog", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            DataHandler.lastJoin = new TimeHandler.DateTimeStamp(sr.ReadLine());
            sr.Close(); sr.Close();
        } catch(System.Exception e) {
            e.ToString();
            TimeHandler.GetCurrentTime();
            DataHandler.lastJoin = TimeHandler.CurrentTime;
        }

        BLECheckCoroutine = StartCoroutine(BLE_Check());
    }

    public Coroutine BLECheckCoroutine;

    public void font_change(FONT_FAMILY ff,CANVAS can) {
        foreach (GameObject go in OtherCanvas)
            go.SetActive(true);

        Text[] allObjects = FindObjectsOfType<Text>();
        foreach (Text t in allObjects) {
            t.font = fonts[(int)ff];
        }

        for (int i = 0; i < OtherCanvas.Length; i++) {
            if(i != (int)can)
                OtherCanvas[i].SetActive(false);
        }
    }

    public void font_change(int ff, CANVAS can) {
        if (ff >= fonts.Length) return;
        foreach (GameObject go in OtherCanvas)
            go.SetActive(true);

        Text[] allObjects = FindObjectsOfType<Text>();
        foreach (Text t in allObjects) {
            t.font = fonts[(int)ff];
        }

        for (int i = 0; i < OtherCanvas.Length; i++) {
            if (i != (int)can)
                OtherCanvas[i].SetActive(false);
        }
    }

    public void BlindControl(bool value) {
        OtherCanvas[(int)CANVAS.BLIND_CON].SetActive(value);
    }

    public void Update() {
        int targetIndex = 0;
        for (int i = 8; i < 13; i++) {
            if (OtherCanvas[i] == null) continue;
            if (OtherCanvas[i].activeSelf)
                targetIndex = i;
        }
        if(currentCanvas != (CANVAS)targetIndex) {
            currentCanvas = ( targetIndex == 0 ) ? CANVAS.NULL : (CANVAS)targetIndex;
            DataHandler.User_isDataLoaded = false;
            StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
        }
    }

    //internal IEnumerator BLE_Check() {
    //    while (!OtherCanvas[(int)CANVAS.HOME].activeSelf)
    //        yield return 0;
    //    yield return new WaitForSeconds(0.2f);

    //    while (true) {
    //        if (BluetoothManager.GetInstance().isConnected == false) {
    //            if (DataHandler.User_moa_band_name != "") {
    //                instance.targetName = DataHandler.User_moa_band_name;
    //                BluetoothManager.GetInstance().OnConnectStart
    //                    (DataHandler.User_moa_band_name, "", "6e400001-b5a3-f393-e0a9-e50e24dcca9e",
    //                                                         "6e400002-b5a3-f393-e0a9-e50e24dcca9e",
    //                                                         "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
    //            }
    //            //while(!BluetoothManager.GetInstance().isConnected)
    //            yield return new WaitForSeconds(10f);
    //        }
    //        yield return new WaitForSeconds(2f);
    //    }
    //}

    internal void ResetCoroutine() {
        try {
            BluetoothLEHardwareInterface.BluetoothEnable(true);
        } catch(System.Exception e) { e.ToString(); }

        try {
            if (BLECheckCoroutine != null)
                StopCoroutine(BLECheckCoroutine);
        } catch(System.Exception e) { e.ToString(); }
        isReset = true;
        BLECheckCoroutine = StartCoroutine(BLE_Check2());
    }
    private bool isReset = false;
    private bool isFind = false;

    internal IEnumerator BLE_Check() {
        while (!OtherCanvas[(int)CANVAS.HOME].activeSelf)
            yield return 0;
        yield return new WaitForSeconds(0.2f);

        if (isReset) {
            yield return new WaitForSeconds(2f);
            isReset = false;
        }
        bool flag = true;
        try {
            BluetoothLEHardwareInterface.StopScan();
        } catch(System.Exception e) { e.ToString(); }
            yield return new WaitForSeconds(0.5f);

        while (DataHandler.User_moa_band_name == null)
            yield return 0;
        isFind = false;
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                if (name.Contains("Touch") && name == DataHandler.User_moa_band_name) {
                    Connection(name, address);
                    isFind = true;
                }
            }, null);
        }, (error) => {
            //BLECheckCoroutine = StartCoroutine(BLE_Check());
            flag = false;
        });

        StartCoroutine(checkingList());
        while (flag) yield return 0;
        yield return new WaitForSeconds(3f);
    }

    internal IEnumerator BLE_Check2() {
        yield return new WaitForSeconds(1f);
        isReset = false;
        bool flag = true;
        try {
            BluetoothLEHardwareInterface.StopScan();
        } catch (System.Exception e) { e.ToString(); }
        yield return new WaitForSeconds(0.5f);
        isFind = false;
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                if (name.Contains("Touch") && name == DataHandler.User_moa_band_name) {
                    Connection(name, address);
                    isFind = true;
                }
            }, null);
        }, (error) => {
            //BLECheckCoroutine = StartCoroutine(BLE_Check());
            flag = false;
        });

        StartCoroutine(checkingList());
        while (flag) yield return 0;
        yield return new WaitForSeconds(3f);
    }



    private int count = 0;
    public IEnumerator checkingList() {
        if(BLECheckCoroutine != null) {
            yield return new WaitForSeconds(5f);

            if (( ConnectingWindowHandler.Instance == null ||
                 !ConnectingWindowHandler.Instance.gameObject.activeSelf ) && isFind == false) {
                try {
                    BluetoothLEHardwareInterface.StopScan();
                } catch (System.Exception e) { e.ToString(); }

                yield return new WaitForSeconds(0.5f);

                BluetoothLEHardwareInterface.Initialize(true, false, () => {
                    BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                        if (name.Contains("Touch") && name == DataHandler.User_moa_band_name) {
                            Connection(name, address);
                            isFind = true;
                        }
                    }, null);
                }, (error) => {
                    Debug.LogError("BLE Error : " + error);
                    BluetoothLEHardwareInterface.Log("BLE Error: " + error);
                });
                StartCoroutine(checkingList());
            }
        }
        yield return 0;
    }


    public void Connection(string bandName,string address) {
        StopCoroutine(BLECheckCoroutine);
        BLECheckCoroutine = null;
        try {
            BluetoothLEHardwareInterface.StopScan();
        } catch(System.Exception e) { e.ToString(); }
        TotalManager.instance.targetName = DataHandler.User_moa_band_name;
        BluetoothManager BT = GameObject.Find("[SYSTEM] Total Manager").GetComponent<BluetoothManager>();
        BT.OnConnectStart(bandName, address,
            "6e400001-b5a3-f393-e0a9-e50e24dcca9e",
            "6e400002-b5a3-f393-e0a9-e50e24dcca9e",
            "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
    }

    public void OnApplicationQuit() {
    }



    public void Connect() {

    }

    public void Disconnect() {

    }
}
