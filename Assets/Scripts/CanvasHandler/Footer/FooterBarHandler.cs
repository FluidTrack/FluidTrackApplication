﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBarHandler : MonoBehaviour
{
    public static FooterBarHandler Instance;
    public Image[] Buttons;
    public Image[] ButtonImage;
    public Text[] ButtonText;
    public Color ActiveContentColor;
    public Color InactiveContentColor;
    public Color Active;
    public Color Inactive;
    public FOOTER_BTN currentPage;
    public GameObject Reconnect;
    private bool isConnected = false;

    public enum FOOTER_BTN {
        HOME, LOG, TABLE, CALENDAR,
    };

    public void Awake() {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    DataHandler.ScreenLog screenLog;
    System.DateTime startLog_new;
    System.DateTime startLog_old;
    System.DateTime endLog;

    public void Start() {
        GameObject[] pages = {
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR],
        };

        for (int i = 0; i < pages.Length; i++) {
            try {
                if (pages[i].activeSelf) {
                    currentPage = (FOOTER_BTN)i;
                    screenLog.screen_num = i;
                    Buttons[i].color = Active;
                    ButtonText[i].color = ActiveContentColor;
                    ButtonImage[i].color = ActiveContentColor;
                    break;
                }
            } catch(System.Exception e) { e.ToString(); }
        }
        screenLog = new DataHandler.ScreenLog();

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].activeSelf)
            screenLog.screen_num = 0;

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf)
            screenLog.screen_num = 1;

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE].activeSelf)
            screenLog.screen_num = 2;

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR].activeSelf)
            screenLog.screen_num = 3;

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf)
            screenLog.screen_num = 4;

        startLog_new = System.DateTime.Now;
    }

    public void FooterButtonClick(int index) {
        DataHandler.User_isDataLoaded = false;
        DataHandler.User_isWaterDataLoaded = false;
        DataHandler.User_isDrinkDataLoaded = false;
        DataHandler.User_isPeeDataLoaded = false;
        DataHandler.User_isPooDataLoaded = false;
        DataHandler.User_isGardenDataLoaded = false;
        SoundHandler.Instance.StopMongMong();



        endLog = System.DateTime.Now;
        startLog_old = startLog_new;
        long elapsedTicks = endLog.Ticks - startLog_old.Ticks;
        System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
        screenLog.start_time = (new TimeHandler.DateTimeStamp(startLog_old)).ToString();
        screenLog.end_time = (new TimeHandler.DateTimeStamp(endLog)).ToString();

        screenLog.second = (uint) elapsedSpan.TotalSeconds;
        screenLog.id = DataHandler.User_id;
        StartCoroutine(DataHandler.CreateScreenlogs(screenLog));

        if (currentPage == (FOOTER_BTN)index)
            return;
    
        GameObject[] pages = {
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR]
        };

        if(index > (int)currentPage) {
            pages[(int)currentPage].GetComponent<Animator>().SetTrigger("OnDisableMirrored");
            pages[index].SetActive(true);
            pages[index].GetComponent<Animator>().SetTrigger("OnEnableMirrored");
        } else {
            pages[(int)currentPage].GetComponent<Animator>().SetTrigger("OnDisable");
            pages[index].SetActive(true);
        }

        for(int i = 0; i < Buttons.Length; i ++) {
            if(i == index) {
                Buttons[i].color = Active;
                ButtonText[i].color = ActiveContentColor;
                ButtonImage[i].color = ActiveContentColor;
            } else {
                Buttons[i].color = Inactive;
                ButtonText[i].color = InactiveContentColor;
                ButtonImage[i].color = InactiveContentColor;
            }
        }

        currentPage = (FOOTER_BTN)index;
        startLog_new = System.DateTime.Now;
        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].activeSelf) {
            Animator tempAnim = TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].GetComponent<Animator>();
            if (!tempAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !tempAnim.GetCurrentAnimatorStateInfo(0).IsName("OnEnable")) return;

            screenLog.screen_num = 0;
        }

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf) {
            Animator tempAnim = TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].GetComponent<Animator>();
            if (!tempAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !tempAnim.GetCurrentAnimatorStateInfo(0).IsName("OnEnable")) return;
            screenLog.screen_num = 1;

        }

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE].activeSelf) {
            Animator tempAnim = TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE].GetComponent<Animator>();
            if (!tempAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !tempAnim.GetCurrentAnimatorStateInfo(0).IsName("OnEnable")) return;
            screenLog.screen_num = 2;

        }

        if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR].activeSelf) {
            Animator tempAnim = TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR].GetComponent<Animator>();
            if (!tempAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !tempAnim.GetCurrentAnimatorStateInfo(0).IsName("OnEnable")) return;
            screenLog.screen_num = 3;
        }
    }
    private bool bPause = false;
    private void OnApplicationPause(bool pause) {
        if (pause) {
            if(Reconnect.activeSelf)
                Reconnect.GetComponent<MoabandReconnection>().CancelButtonClick();

            try {
                try {
                    BluetoothLEHardwareInterface.StopScan();
                } catch (System.Exception e) { e.ToString(); }
                BluetoothLEHardwareInterface.DisconnectAll();
            } catch (System.Exception e) { Debug.LogError(e.ToString()); }

            bPause = true;
            // 일시정지
            endLog = System.DateTime.Now;
            startLog_old = startLog_new;
            long elapsedTicks = endLog.Ticks - startLog_old.Ticks;
            System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
            screenLog.start_time = ( new TimeHandler.DateTimeStamp(startLog_old) ).ToString();
            screenLog.end_time = ( new TimeHandler.DateTimeStamp(endLog) ).ToString();

            screenLog.second = (uint)elapsedSpan.TotalSeconds;
            screenLog.id = DataHandler.User_id;
            StartCoroutine(DataHandler.CreateScreenlogs(screenLog));
        } else {
            if (bPause) {
                bPause = false;
                // 일시정지후 돌아옴
                // StartCoroutine(ReconnectButtonLoop());
                

                screenLog = new DataHandler.ScreenLog();

                if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].activeSelf)
                    screenLog.screen_num = 0;
                if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf)
                    screenLog.screen_num = 1;
                if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE].activeSelf)
                    screenLog.screen_num = 2;
                if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR].activeSelf)
                    screenLog.screen_num = 3;
                if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf)
                    screenLog.screen_num = 4;

                startLog_new = System.DateTime.Now;
            }
        }
    }

    IEnumerator ReconnectButtonLoop() {
        yield return new WaitForSeconds(2f);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        try {
            BluetoothLEHardwareInterface.StopScan();
        } catch (System.Exception e) { e.ToString(); }

        if (!BluetoothManager.GetInstance().isReconnectEnable) {
            if (TotalManager.instance.BLECheckCoroutine != null)
                StopCoroutine(TotalManager.instance.BLECheckCoroutine);
            TotalManager.instance.BLECheckCoroutine = null;
            isConnected = BluetoothManager.GetInstance().isConnected;
            BluetoothManager.GetInstance().isReconnectEnable = false;
            BluetoothManager.GetInstance().AutoConnect = false;
            try {
                BluetoothLEHardwareInterface.StopScan();
            } catch (System.Exception e) { e.ToString(); }
        }

        Reconnect.SetActive(true);
        yield return 0;
    }

    private bool FlowerPageFlag = false;
    public void Update() {
        if(!FlowerPageFlag) {
            if (TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf) {
                FlowerPageFlag = true;
                endLog = System.DateTime.Now;
                startLog_old = startLog_new;
                long elapsedTicks = endLog.Ticks - startLog_old.Ticks;
                System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
                screenLog.start_time = ( new TimeHandler.DateTimeStamp(startLog_old) ).ToString();
                screenLog.end_time = ( new TimeHandler.DateTimeStamp(endLog) ).ToString();

                screenLog.second = (uint)elapsedSpan.TotalSeconds;
                screenLog.id = DataHandler.User_id;
                StartCoroutine(DataHandler.CreateScreenlogs(screenLog));
                screenLog.screen_num = 4;
                startLog_new = System.DateTime.Now;
            }
        } else if (!TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf) {
                FlowerPageFlag = false;
                endLog = System.DateTime.Now;
                startLog_old = startLog_new;
                long elapsedTicks = endLog.Ticks - startLog_old.Ticks;
                System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
                screenLog.start_time = ( new TimeHandler.DateTimeStamp(startLog_old) ).ToString();
                screenLog.end_time = ( new TimeHandler.DateTimeStamp(endLog) ).ToString();

                screenLog.second = (uint)elapsedSpan.TotalSeconds;
                screenLog.id = DataHandler.User_id;
                StartCoroutine(DataHandler.CreateScreenlogs(screenLog));
                screenLog.screen_num = 1;
                startLog_new = System.DateTime.Now;
            }
    }

    public void OnApplicationQuit() {
        endLog = System.DateTime.Now;
        startLog_old = startLog_new;
        long elapsedTicks = startLog_old.Ticks - endLog.Ticks;
        System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
        screenLog.start_time = ( new TimeHandler.DateTimeStamp(startLog_old) ).ToString();
        screenLog.end_time = ( new TimeHandler.DateTimeStamp(endLog) ).ToString();
        screenLog.second = (uint)elapsedSpan.TotalSeconds;
        Debug.Log(screenLog.second);
        screenLog.id = DataHandler.User_id;
        //StartCoroutine(DataHandler.CreateScreenlogs(screenLog));
        startLog_new = System.DateTime.Now;
    }
}
