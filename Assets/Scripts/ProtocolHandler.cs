using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class ProtocolHandler : MonoBehaviour {
    public static ProtocolHandler Instance;
    public BluetoothManager BT;
    public TotalManager Main;
    public Text DebugText;
    public GameObject LogCircle;
    public GameObject FlowerCircle;

    public GameObject SendingServerObject;
    public SendingServerWindowHandler SendingServer;

    DataHandler.GardenLog[] array;
    DataHandler.PoopLog[] array1;
    DataHandler.WaterLog[] array2;
    DataHandler.PeeLog[] array3;

    DataHandler.PoopLog log1;
    DataHandler.WaterLog log2;
    DataHandler.PeeLog log3;

    private Coroutine checkHistoryRoutine = null;

    public void Awake() {
        Instance = this;
        WaterQueue = new Queue<DataHandler.WaterLog>();
        PooQueue = new Queue<DataHandler.PoopLog>();
        PeeQueue = new Queue<DataHandler.PeeLog>();
        GardenList = new List<DataHandler.GardenLog>();
        StartCoroutine(UserCheck());
    }

    IEnumerator UserCheck() {
        while (true) {
            if (DataHandler.User_name != null && DataHandler.User_name != "")
                break;
            yield return 0;
        }
        ReadGardenLogs();
    }

    public void ReadGardenLogs() {
        bool ThereAreTodos = false;
        bool ThereAreTodosToday = false;
        bool ThereIsUnknown = false;

        if (DataHandler.Poop_logs != null && DataHandler.Poop_logs.PoopLogs != null && DataHandler.Poop_logs.PoopLogs.Length > 0)
            foreach (DataHandler.PoopLog log in DataHandler.Poop_logs.PoopLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.CurrentTime) == 0) {
                    if (log.type == 8) {
                        ThereIsUnknown = true;
                        break;
                    }
                }
            }

        if (DataHandler.Garden_logs != null && DataHandler.Garden_logs.GardenLogs != null && DataHandler.Garden_logs.GardenLogs.Length > 0) {
            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.LogCanvasTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 )) 
                        ThereAreTodos = true;
                }
            }

            foreach (DataHandler.GardenLog log in DataHandler.Garden_logs.GardenLogs) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(log.timestamp), TimeHandler.CurrentTime) == 0) {
                    if (( log.log_water > 0 && log.flower < 10 ) || ( log.log_pee > 0 && log.item_0 == 0 ) || ( log.log_poop > 0 && log.item_1 == 0 ))
                        ThereAreTodosToday = true;
                }
            }
        }

        LogCircle.SetActive(ThereAreTodosToday || ThereIsUnknown);
        FlowerCircle.SetActive(ThereAreTodos);
    }


    public IEnumerator ReadGardenLogsRoutine() {
        yield return 0;
        ReadGardenLogs();
    }

    List<DataHandler.GardenLog> GardenList;

    public void ParsingBytes(byte[] bytes) {
        int length = bytes.Length;
        byte cmd = bytes[0];

        //DebugText.text += "1 ▶";
        //for(int i = 0; i < length; i++) {
        //    DebugText.text += bytes[i].ToString();
        //    if (i != length - 1) DebugText.text += "-";
        //}
        //DebugText.text += "\n";

        if (cmd == 0x03) {
            //==============================================================================
            // 타임존 변경 Confirm
            //==============================================================================
            Debug.Log("모아밴드의 시간이 성공적으로 변경됨.");
        } else if (cmd == 0x07) {
            //==============================================================================
            // 배터리 확인
            //==============================================================================
            if (MoabandStatusHandler.Instance != null &&
                MoabandStatusHandler.Instance.gameObject.activeSelf)
                MoabandStatusHandler.Instance.value = (int)bytes[2];
            BluetoothManager.GetInstance().ConnectFlag = true;
        } else if (cmd == 0x12) {
            //if (bytes[3] == 0)
            //AlertHandler.GetInstance().Pop_LowBat((int)bytes[2]);
        } else if (cmd == 0x24) {
            //==============================================================================
            // Button Input
            //==============================================================================
            if (!TotalManager.instance.isRegisterMode) {
                string stamp = GetCurrentTimeStamp();
                if (bytes[6] == 3) {
                    log1 = new DataHandler.PoopLog();
                    log1.id = DataHandler.User_id;
                    log1.auto = 1;
                    log1.type = 8;
                    log1.timestamp = stamp;

                    array1 = new DataHandler.PoopLog[DataHandler.Poop_logs.PoopLogs.Length + 1];
                    DataHandler.CreatePooIndex.Enqueue(array1.Length - 1);
                    for (int i = 0; i < DataHandler.Poop_logs.PoopLogs.Length; i++)
                        array1[i] = DataHandler.Poop_logs.PoopLogs[i];
                    array1[array1.Length - 1] = log1;
                    DataHandler.Poop_logs.PoopLogs = array1;
                    StartCoroutine(DataHandler.CreatePooplogs(log1));
                } else if (bytes[6] == 1) {
                    log2 = new DataHandler.WaterLog();
                    log2.id = DataHandler.User_id;
                    log2.auto = 1;
                    log2.timestamp = stamp;
                    log2.type = 0;
                    array2 = new DataHandler.WaterLog[DataHandler.Water_logs.WaterLogs.Length + 1];
                    DataHandler.CreateWaterIndex.Enqueue(array2.Length - 1);
                    for (int i = 0; i < DataHandler.Water_logs.WaterLogs.Length; i++)
                        array2[i] = DataHandler.Water_logs.WaterLogs[i];
                    array2[array2.Length - 1] = log2;
                    DataHandler.Water_logs.WaterLogs = array2;
                    StartCoroutine(DataHandler.CreateWaterlogs(log2));
                } else if (bytes[6] == 2) {
                    log3 = new DataHandler.PeeLog();
                    log3.id = DataHandler.User_id;
                    log3.auto = 1;
                    log3.timestamp = stamp;
                    array3 = new DataHandler.PeeLog[DataHandler.Pee_logs.PeeLogs.Length + 1];
                    DataHandler.CreatePeeIndex.Enqueue(array3.Length - 1);
                    for (int i = 0; i < DataHandler.Pee_logs.PeeLogs.Length; i++)
                        array3[i] = DataHandler.Pee_logs.PeeLogs[i];
                    array3[array3.Length - 1] = log3;
                    DataHandler.Pee_logs.PeeLogs = array3;
                    StartCoroutine(DataHandler.CreatePeelogs(log3));
                }

                DataHandler.GardenLog targetGardenLog = null;
                int targetIndex = -1;
                for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                    if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                        new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp),
                        new TimeHandler.DateTimeStamp(stamp)) == 0) {
                        targetIndex = i;
                        targetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                        break;
                    }
                }

                if (targetGardenLog == null) {
                    targetGardenLog = new DataHandler.GardenLog();
                    targetGardenLog.id = DataHandler.User_id;
                    targetGardenLog.timestamp = stamp;
                    targetGardenLog.flower = 0;
                    if (bytes[6] == 1) targetGardenLog.log_water = 1;
                    else if (bytes[6] == 2) targetGardenLog.log_pee = 1;
                    else if (bytes[6] == 3) targetGardenLog.log_poop = 1;
                    array = new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
                    DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
                    array[array.Length - 1] = targetGardenLog;
                    for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                        array[i] = DataHandler.Garden_logs.GardenLogs[i];
                    DataHandler.Garden_logs.GardenLogs = array;
                    StartCoroutine(DataHandler.CreateGardenlogs(targetGardenLog));
                } else {
                    if (bytes[6] == 1) DataHandler.Garden_logs.GardenLogs[targetIndex].log_water += 1;
                    else if (bytes[6] == 2) DataHandler.Garden_logs.GardenLogs[targetIndex].log_pee += 1;
                    else if (bytes[6] == 3) DataHandler.Garden_logs.GardenLogs[targetIndex].log_poop += 1;
                    StartCoroutine(DataHandler.UpdateGardenLogs(DataHandler.Garden_logs.GardenLogs[targetIndex]));
                }
                MainPageMongMongHandler.todayGardenLog = targetGardenLog;
                MainPageMongMongHandler.todayLoged = true;
                SoundHandler.Instance.Play_SFX(SoundHandler.SFX.DATA2);
                StartCoroutine(ReadGardenLogsRoutine());
                if (TotalManager.instance.currentCanvas == TotalManager.CANVAS.HOME) {
                    HomeHandler.Instance.Redrawmap2();
                    MainPageHeaderHandler.Instance.DataReload();
                } else if (TotalManager.instance.currentCanvas == TotalManager.CANVAS.LOG) {
                    LogCanvasHandler.Instance.BLE_Redraw();
                }
            } else {
                if (RegisterBandHandler.Instance != null &&
                    RegisterBandHandler.Instance.gameObject.activeSelf) {
                    RegisterBandHandler register = RegisterBandHandler.Instance;
                    switch (bytes[6]) {
                        case 3: register.PooButtonClick(); break;
                        case 1: register.WaterButtonClick(); break;
                        case 2: register.PeeButtonClick(); break;
                    }
                }
            }
        } else if (cmd == 0x27) {
            //==============================================================================
            // History
            //==============================================================================
            string stamp = GetCurrentTimeStamp();
            if (!TotalManager.instance.isRegisterMode) {
                int Length = 0;
                if ((int)bytes[1] == 15) Length = 3;
                else if ((int)bytes[1] == 10) Length = 2;
                else if ((int)bytes[1] == 5) Length = 1;
                if (Length == 0) return;

                if (checkHistoryRoutine != null) {
                    StopCoroutine(checkHistoryRoutine);
                    checkHistoryRoutine = null;
                }
                for (int i = 0; i < Length; i++) {
                    string historyStamp = MakeTimeStamp(bytes[2 + 5 * i], bytes[3 + 5 * i], bytes[4 + 5 * i], bytes[5 + 5 * i]);
                    if (bytes[6 + 5 * i] == 3) {
                        DataHandler.PoopLog log1 = new DataHandler.PoopLog();
                        log1.id = DataHandler.User_id;
                        log1.auto = 1;
                        log1.timestamp = historyStamp;
                        log1.type = 8;
                        PooQueue.Enqueue(log1);
                    } else if (bytes[6 + 5 * i] == 1) {
                        DataHandler.WaterLog log2 = new DataHandler.WaterLog();
                        log2.id = DataHandler.User_id;
                        log2.auto = 1;
                        log2.timestamp = historyStamp;
                        log2.type = 0;
                        WaterQueue.Enqueue(log2);
                    } else if (bytes[6 + 5 * i] == 2) {
                        DataHandler.PeeLog log3 = new DataHandler.PeeLog();
                        log3.id = DataHandler.User_id;
                        log3.auto = 1;
                        log3.timestamp = historyStamp;
                        PeeQueue.Enqueue(log3);
                    }

                    DataHandler.GardenLog targetGardenLog = null;
                    int targetIndex = -1;
                    for (int j = 0; j < DataHandler.Garden_logs.GardenLogs.Length; j++) {
                        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                            new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[j].timestamp),
                            new TimeHandler.DateTimeStamp(historyStamp)) == 0) {
                            targetIndex = j;
                            targetGardenLog = DataHandler.Garden_logs.GardenLogs[j];
                            break;
                        }
                    }

                    if (targetGardenLog == null) {
                        targetGardenLog = new DataHandler.GardenLog();
                        targetGardenLog.id = DataHandler.User_id;
                        targetGardenLog.timestamp = historyStamp;
                        targetGardenLog.flower = 0;
                        targetGardenLog.log_water = 0; targetGardenLog.log_pee = 0; targetGardenLog.log_poop = 0;
                        if (bytes[6 + 5 * i] == 1) targetGardenLog.log_water = 1;
                        else if (bytes[6 + 5 * i] == 2) targetGardenLog.log_pee = 1;
                        else if (bytes[6 + 5 * i] == 3) targetGardenLog.log_poop = 1;
                        array = new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
                        DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
                        array[array.Length - 1] = targetGardenLog;
                        for (int j = 0; j < DataHandler.Garden_logs.GardenLogs.Length; j++)
                            array[j] = DataHandler.Garden_logs.GardenLogs[j];
                        DataHandler.Garden_logs.GardenLogs = array;
                        StartCoroutine(DataHandler.CreateGardenlogs(targetGardenLog));
                    } else {
                        if (bytes[6 + 5 * i] == 1) DataHandler.Garden_logs.GardenLogs[targetIndex].log_water += 1;
                        else if (bytes[6 + 5 * i] == 2) DataHandler.Garden_logs.GardenLogs[targetIndex].log_pee += 1;
                        else if (bytes[6 + 5 * i] == 3) DataHandler.Garden_logs.GardenLogs[targetIndex].log_poop += 1;

                        DataHandler.GardenLog target = DataHandler.Garden_logs.GardenLogs[targetIndex];
                        bool flag = false;
                        for (int k = 0; k < GardenList.Count; k++) {
                            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                                new TimeHandler.DateTimeStamp(GardenList[k].timestamp),
                                new TimeHandler.DateTimeStamp(target.timestamp)) == 0) {
                                GardenList[k] = target; flag = true;
                                break;
                            }
                        }
                        if (!flag) GardenList.Add(target);
                    }
                }
                checkHistoryRoutine = StartCoroutine(CheckHistory(1f));
            }
        }
    }

    private Queue<DataHandler.WaterLog> WaterQueue;
    private Queue<DataHandler.PoopLog> PooQueue;
    private Queue<DataHandler.PeeLog> PeeQueue;


    private IEnumerator CheckHistory(float s, bool isShow = true) {
        SendingServerObject.SetActive(isShow);
        int totalDataNum = WaterQueue.Count + PeeQueue.Count + PooQueue.Count;
        int currentNum = 0;
        if(totalDataNum > 0) {
            SendingServer.DrawBar(totalDataNum, currentNum);
            yield return new WaitForSeconds(s);
            while (WaterQueue.Count > 0) {
                array2 = new DataHandler.WaterLog[DataHandler.Water_logs.WaterLogs.Length + 1];
                DataHandler.CreateWaterIndex.Enqueue(array2.Length - 1);
                for (int j = 0; j < DataHandler.Water_logs.WaterLogs.Length; j++)
                    array2[j] = DataHandler.Water_logs.WaterLogs[j];
                array2[array2.Length - 1] = WaterQueue.Dequeue();
                DataHandler.Water_logs.WaterLogs = array2;
                StartCoroutine(DataHandler.CreateWaterlogs(array2[array2.Length - 1]));
                yield return new WaitForSeconds(0.1f);
                currentNum++;
                SendingServer.DrawBar(totalDataNum, currentNum);
            }

            while (PeeQueue.Count > 0) {
                array3 = new DataHandler.PeeLog[DataHandler.Pee_logs.PeeLogs.Length + 1];
                DataHandler.CreatePeeIndex.Enqueue(array3.Length - 1);
                for (int j = 0; j < DataHandler.Pee_logs.PeeLogs.Length; j++)
                    array3[j] = DataHandler.Pee_logs.PeeLogs[j];
                array3[array3.Length - 1] = PeeQueue.Dequeue();
                DataHandler.Pee_logs.PeeLogs = array3;
                StartCoroutine(DataHandler.CreatePeelogs(array3[array3.Length - 1]));
                yield return new WaitForSeconds(0.1f);
                currentNum++;
                SendingServer.DrawBar(totalDataNum, currentNum);
            }

            while (PooQueue.Count > 0) {
                array1 = new DataHandler.PoopLog[DataHandler.Poop_logs.PoopLogs.Length + 1];
                DataHandler.CreatePooIndex.Enqueue(array1.Length - 1);
                for (int j = 0; j < DataHandler.Poop_logs.PoopLogs.Length; j++)
                    array1[j] = DataHandler.Poop_logs.PoopLogs[j];
                array1[array1.Length - 1] = PooQueue.Dequeue();
                DataHandler.Poop_logs.PoopLogs = array1;
                StartCoroutine(DataHandler.CreatePooplogs(array1[array1.Length - 1]));
                yield return new WaitForSeconds(0.1f);
                currentNum++;
                SendingServer.DrawBar(totalDataNum, currentNum);
            }

            for (int i = 0; i < GardenList.Count; i++) {
                StartCoroutine(DataHandler.UpdateGardenLogs(GardenList[i]));
                yield return new WaitForSeconds(0.1f);
            }
            GardenList.Clear();

            StartCoroutine(ReadGardenLogsRoutine());
            if (TotalManager.instance.currentCanvas == TotalManager.CANVAS.HOME) {
                HomeHandler.Instance.Redrawmap2();
                MainPageHeaderHandler.Instance.DataReload();
            } else if (TotalManager.instance.currentCanvas == TotalManager.CANVAS.LOG) {
                LogCanvasHandler.Instance.BLE_Redraw();
            }
        }

        DataHandler.MoabandLog band_log = new DataHandler.MoabandLog();
        band_log.id = DataHandler.User_id;
        band_log.log_count = totalDataNum;
        band_log.timestamp = TimeHandler.CurrentTime.ToString();
        try { band_log.battery_moaband = MoabandStatusHandler.Instance.value; }
        catch (System.Exception e) { e.ToString(); band_log.battery_moaband = 100; }
        try { band_log.battery_tablet = DeviceBatteryIconHandler.Instance.currentValue; }
        catch (System.Exception e) { e.ToString(); band_log.battery_tablet = 100; }
        band_log.auto = 1;
        StartCoroutine(DataHandler.CreateMoabandlogs(band_log));

        yield return new WaitForSeconds(1f);
        SendingServerObject.SetActive(false);
        checkHistoryRoutine = null;
    }

    static public byte[] SetTimerToCurrent() {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[10];
        bytes[0] = 0x03;
        bytes[1] = 8;
        bytes[2] = (byte)now.Year;
        bytes[3] = (byte)( now.Month );
        bytes[4] = (byte)now.Day;
        bytes[5] = (byte)now.Hour;
        bytes[6] = (byte)now.Minute;
        bytes[7] = (byte)now.Second;
        bytes[8] = 0;
        bytes[9] = 0;
        return bytes;
    }

    static public byte[] SetTimer(int year, int month, int day, int hour, int min, int sec) {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[10];
        bytes[0] = 0x03;
        bytes[1] = 8;
        bytes[2] = (byte)year;
        bytes[3] = (byte)month;
        bytes[4] = (byte)day;
        bytes[5] = (byte)hour;
        bytes[6] = (byte)min;
        bytes[7] = (byte)sec;
        bytes[8] = 0;
        bytes[9] = 0;
        return bytes;
    }

    static public byte[] QueryBattery() {
        DateTime now = DateTime.Now;
        byte[] bytes = new byte[2];
        bytes[0] = 0x07;
        bytes[1] = 0;
        return bytes;
    }

public string GetCurrentTimeStamp() {
    DateTime now = DateTime.Now;
    string result = now.Year + "-";
    result += "" + ( now.Month / 10 ) + ( now.Month % 10 ) + "-";
    result += "" + ( now.Day / 10 ) + ( now.Day % 10 ) + " ";
    result += "" + ( now.Hour / 10 ) + ( now.Hour % 10) + ":";
    result += "" + ( now.Minute / 10 ) + ( now.Minute % 10 ) + ":";
    result += "" + ( now.Second / 10 ) + ( now.Second % 10 ) + "";
    return result;
}

    public string MakeTimeStamp(byte day, byte hour, byte min, byte sec) {
        int day_int = ( day / 16 ) * 10 + day % 16;
        int hour_int = ( hour / 16 ) * 10 + hour % 16;
        int min_int = ( min / 16 ) * 10 + min % 16;
        int sec_int = ( sec / 16 ) * 10 + sec % 16;
        DateTime now = DateTime.Now;

        string tempResult = MakeTimeStamp(now.Year, ( now.Month ),
            day_int, hour_int, min_int, sec_int);

        if (TimeHandler.DateTimeStamp.isLeafYear(now.Year)) {
            int[] arr = TimeHandler.DateTimeStamp.LeafYearDaysList;
            if (day_int > arr[now.Month - 1]) {
                return MakeTimeStamp(now.Year, ( now.Month - 1 ),
                          day_int, hour_int, min_int, sec_int);
            }
        } else {
            int[] arr = TimeHandler.DateTimeStamp.NormalYearDaysList;
            if (day_int > arr[now.Month - 1]) {
                return MakeTimeStamp(now.Year, ( now.Month - 1 ),
                          day_int, hour_int, min_int, sec_int);
            }
        }

        TimeHandler.DateTimeStamp tempStamp =
            new TimeHandler.DateTimeStamp(tempResult);

        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp
            (tempStamp, TimeHandler.CurrentTime) == 1)
            return MakeTimeStamp(now.Year, ( now.Month - 1 ),
                          day_int, hour_int, min_int, sec_int);
        else return tempResult;
    }

    public string MakeTimeStamp(int day, int hour, int min, int sec) {
        DateTime now = DateTime.Now;
        return MakeTimeStamp(now.Year, ( now.Month ), day, hour, min, sec);
    }

    public string MakeTimeStamp(int year, int month, int day, int hour, int min, int sec) {
        return year + "-" + (month/10) + (month%10) + "-" + (day/10) + (day%10) + " " + 
               (hour/10) + (hour%10) + ":" + (min/10) + (min%10) + ":" + (sec/10) + (sec%10);
    }

    static public byte[] GetHistory() {
        byte[] bytes = new byte[2];
        bytes[0] = 0x27;
        bytes[1] = 0;
        if(Instance != null) {
            if (Instance.checkHistoryRoutine != null) {
                Instance.StopCoroutine(Instance.checkHistoryRoutine);
                Instance.checkHistoryRoutine =
                    Instance.StartCoroutine(Instance.CheckHistory(1f,false));
            }
        }
        return bytes;
    }

    static public byte[] GetRedLEDOn() {
        byte[] bytes = new byte[3];
        bytes[0] = 0x44;
        bytes[1] = 1;
        bytes[2] = 1;
        return bytes;
    }

    static public byte[] GetGreenLEDOn() {
        byte[] bytes = new byte[3];
        bytes[0] = 0x44;
        bytes[1] = 1;
        bytes[2] = 2;
        return bytes;
    }

    static public byte[] GetBlueLEDOn() {
        byte[] bytes = new byte[3];
        bytes[0] = 0x44;
        bytes[1] = 1;
        bytes[2] = 3;
        return bytes;
    }

    static public byte[] GetVibrateOn() {
        byte[] bytes = new byte[2];
        bytes[0] = 0x45;
        bytes[1] = 0;
        return bytes;
    }
}