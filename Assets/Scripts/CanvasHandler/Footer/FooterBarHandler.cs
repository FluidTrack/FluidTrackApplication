using System.Collections;
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

    public enum FOOTER_BTN {
        HOME, LOG, TABLE, CALENDAR,
    };

    public void Awake() {
        Instance = this;
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
        startLog_new = System.DateTime.Now;
    }

    public void FooterButtonClick(int index) {
        DataHandler.User_isDataLoaded = false;
        DataHandler.User_isWaterDataLoaded = false;
        DataHandler.User_isDrinkDataLoaded = false;
        DataHandler.User_isPeeDataLoaded = false;
        DataHandler.User_isPooDataLoaded = false;
        DataHandler.User_isGardenDataLoaded = false;

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

        endLog = System.DateTime.Now;
        startLog_old = startLog_new;
        startLog_new = System.DateTime.Now;
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
    }

    public void OnDisable() {
        endLog = System.DateTime.Now;
        startLog_old = startLog_new;
        startLog_new = System.DateTime.Now;
        long elapsedTicks = startLog_old.Ticks - endLog.Ticks;
        System.TimeSpan elapsedSpan = new System.TimeSpan(elapsedTicks);
        screenLog.start_time = ( new TimeHandler.DateTimeStamp(startLog_old) ).ToString();
        screenLog.end_time = ( new TimeHandler.DateTimeStamp(endLog) ).ToString();
        screenLog.second = (uint)elapsedSpan.TotalSeconds;
        screenLog.id = DataHandler.User_id;
        StartCoroutine(DataHandler.CreateScreenlogs(screenLog));
    }
}
