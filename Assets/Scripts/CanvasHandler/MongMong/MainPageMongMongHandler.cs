using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainPageMongMongHandler : MonoBehaviour
{
    public GameObject LeftQuote;
    public GameObject RightQuote;


    public GameObject[] AllMong;
    public GameObject[] DefaultMong;
    public GameObject ComplimentMong;
    public GameObject[] CongratulationMong;
    public GameObject GloomyMong;
    public GameObject EncourageMong;

    public string[] DefaultString;
    public AudioClip[] DefaultVoice;

    public string[] ComplimentString;
    public AudioClip[] ComplimentVoice;

    public string[] CongratulationString;
    public AudioClip[] CongratulationVoice;

    public string[] GloomyString;
    public AudioClip[] GloomyVoice;

    public string[] EncourageString;
    public AudioClip[] EncourageVoice;

    public string[] EncourageString_2_1;
    public AudioClip[] EncourageVoice_2_1;

    public string[] EncourageString_2_2;
    public AudioClip[] EncourageVoice_2_2;

    public string[] EncourageString_3_1;
    public AudioClip[] EncourageVoice_3_1;

    public string[] EncourageString_3_2;
    public AudioClip[] EncourageVoice_3_2;

    private MongMongQuoteHandler LeftQuoteHandler;
    private MongMongQuoteHandler RightQuoteHandler;
    private bool isRight = false;

    public void Awake() {
        LeftQuoteHandler = LeftQuote.GetComponent<MongMongQuoteHandler>();
        RightQuoteHandler = RightQuote.GetComponent<MongMongQuoteHandler>();
    }

    public void SelectQuoteDirection(bool isRight) {
        this.isRight = isRight;
        LeftQuote.SetActive(!isRight);
        RightQuote.SetActive(isRight);
    }

    private bool isInit = false;

    Coroutine mongmongRoutine = null;
    Coroutine mongmongRoutine2 = null;
    public void OnEnable() {
        if(mongmongRoutine == null)
            mongmongRoutine = StartCoroutine(MongMongInit());
    }

    public void OnDisable() {
        if (mongmongRoutine != null) {
            StopCoroutine(mongmongRoutine);
            mongmongRoutine = null;
        }

        if (mongmongRoutine2 != null) {
            StopCoroutine(mongmongRoutine2);
            mongmongRoutine2 = null;
        }

        if (currentState == 6 || currentState == 7)
            currentState = 8;
        else if (currentState == 1)
            currentState = 3;

        SoundHandler.Instance.MongMongSource.Stop();
    }

    private int rand = 0;


    internal static DataHandler.GardenLog yesterdayGardenLog = null;
    internal static DataHandler.GardenLog todayGardenLog = null;

    internal static bool createdToday = false;
    internal static bool yesterdayJoin = false;
    internal static bool yesterdayLoged = false;
    internal static bool yesterdayFlower = false;
    internal static bool todayLoged = false;
    internal static bool todayFlower = false;
    internal static bool todayFlowerComplete = false;
    internal static bool InitComplete = false;
    internal int todayFlowerCount = -1;

    IEnumerator MongMongInit() {
        yield return new WaitForSeconds(0.2f);
        while (GreetingMongMong.Instance.GreetingMongMongObject.activeSelf) {
            LeftQuote.SetActive(false);
            RightQuote.SetActive(false);
            yield return 0;
        }
        SelectQuoteDirection(this.isRight);
        AllMongMongOff();
        TimeHandler.GetCurrentTime();

        createdToday = TimeHandler.DateTimeStamp.CmpDateTimeStamp( new TimeHandler.DateTimeStamp(
            DataHandler.User_creation_date), TimeHandler.CurrentTime) == 0;

        yesterdayJoin = TimeHandler.DateTimeStamp.CmpDateTimeStamp
            (TimeHandler.CurrentTime, DataHandler.lastJoin + 1) == 0;

        yesterdayGardenLog = null;
        todayGardenLog = null;
        for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime,
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0) {
                todayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                todayLoged = ( todayGardenLog.log_water > 0 || todayGardenLog.log_pee > 0 ||
                               todayGardenLog.log_poop > 0 || todayGardenLog.flower > 0);
                todayFlower = todayGardenLog.flower > 0;
                todayFlowerComplete = todayGardenLog.flower == 10;

                if(!InitComplete) todayFlowerCount = todayGardenLog.flower;
                if( todayFlowerCount > todayGardenLog.flower)
                    todayFlowerCount = todayGardenLog.flower;
            } else if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime - 1,
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0) {
                yesterdayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                yesterdayLoged = ( yesterdayGardenLog.log_water > 0 || yesterdayGardenLog.log_pee > 0 ||
                                   yesterdayGardenLog.log_poop > 0 || yesterdayGardenLog.flower > 0 );
                yesterdayFlower = yesterdayGardenLog.flower > 0;
            }
        }
        InitComplete = true;


        if (mongmongRoutine2 == null)
            mongmongRoutine2 = StartCoroutine(MongMongState());
        mongmongRoutine = null;
    }

    public static int currentState = 0;

    public enum STATE {
        GREETING = 0,
        ENCOURAGE_1 = 1,
        ENCOURAGE_2_1 = 2,
        ENCOURAGE_2_2 = 3,
        ENCOURAGE_3_1 = 4,
        ENCOURAGE_3_2 = 5,
        COMPLIMENT = 6,
        CONGRATULATION = 7,
        DEFAULT = 8
    };

    private IEnumerator MongMongState() {
        float spendTime = 0; bool flag = true;
        Debug.Log("MongMongState Start");

        while (true) {
            yield return 0;
            switch(currentState) {
            //=================================================================================================
            // CASE _ 0 : 인사 몽몽이 출현
            //=================================================================================================
            case (int)STATE.GREETING:
                if(InitComplete) {
                    if(createdToday) currentState = (int)STATE.DEFAULT;
                    else { 
                        if (!yesterdayJoin && !todayLoged)
                            currentState = (int)STATE.ENCOURAGE_1;

                        else if (yesterdayJoin && !yesterdayLoged && !todayLoged)
                            currentState = (int)STATE.ENCOURAGE_2_1;

                        else if (yesterdayJoin && yesterdayLoged && !yesterdayFlower && !todayLoged)
                            currentState = (int)STATE.ENCOURAGE_3_1;

                        else if (!(yesterdayJoin && yesterdayLoged && yesterdayFlower)
                                 && todayLoged && ! todayFlower)
                            currentState = (int)STATE.ENCOURAGE_3_2;

                        else if (( yesterdayJoin && yesterdayFlower ) || todayFlower)
                            currentState = (int)STATE.DEFAULT;
                    }
                }
            break;
            //=================================================================================================
            // CASE _ 1 : 격려 1 몽몽이 출현
            //=================================================================================================
            case (int)STATE.ENCOURAGE_1:
                AllMongMongOff();
                GloomyMong.SetActive(true);
                rand = Random.Range(0, 5);
                SetText(GloomyString[rand]);
                SoundHandler.Instance.MongMongSource.PlayOneShot(GloomyVoice[rand]);
                spendTime = GloomyVoice[rand].length;
                yield return new WaitForSeconds(spendTime + 1f);
                currentState = (int)STATE.ENCOURAGE_2_2;
                flag = true;
                break;
            //=================================================================================================
            // CASE _ 2 : 격려 2.1 몽몽이 출현
            //=================================================================================================
            case (int)STATE.ENCOURAGE_2_1:
                if(todayLoged && !todayFlower)      {  flag = true; currentState = (int)STATE.ENCOURAGE_3_2; }
                else if (todayLoged && todayFlower && todayGardenLog != null && todayGardenLog.flower >= 10)
                    {  flag = true; currentState = (int)STATE.CONGRATULATION; }
                else if (todayLoged && todayFlower)
                    {  flag = true; currentState = (int)STATE.COMPLIMENT; }

                else if(flag) {
                    AllMongMongOff();
                    EncourageMong.SetActive(true);
                    rand = Random.Range(0, 5);
                    SetText(EncourageString_2_1[rand]);
                    SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_2_1[rand]);
                    spendTime = EncourageVoice_2_1[rand].length;
                    yield return new WaitForSeconds(spendTime + 1f);
                    flag = false;
                }

            break;
            //=================================================================================================
            // CASE _ 3 : 격려 2.2 몽몽이 출현
            //=================================================================================================
            case (int)STATE.ENCOURAGE_2_2:
                if (todayLoged && !todayFlower) { flag = true; currentState = (int)STATE.ENCOURAGE_3_2; }
                else if (todayLoged && todayFlower && todayGardenLog != null && todayGardenLog.flower >= 10)
                    { flag = true; currentState = (int)STATE.CONGRATULATION; }
                else if (todayLoged && todayFlower)
                    { flag = true; currentState = (int)STATE.COMPLIMENT; }

                else if (flag) {
                    AllMongMongOff();
                    EncourageMong.SetActive(true);
                    rand = Random.Range(0, 5);
                    SetText(EncourageString_2_2[rand]);
                    SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_2_2[rand]);
                    spendTime = EncourageVoice_2_2[rand].length;
                    yield return new WaitForSeconds(spendTime + 1f);
                    flag = false;
                }

            break;
            //=================================================================================================
            // CASE _ 4 : 격려 3.1 몽몽이 출현
            //=================================================================================================
            case (int)STATE.ENCOURAGE_3_1:
                if (todayLoged && !todayFlower) { flag = true; currentState = (int)STATE.ENCOURAGE_3_2; }
                else if (todayLoged && todayFlower && todayGardenLog != null && todayGardenLog.flower >= 10)
                    { flag = true; currentState = (int)STATE.CONGRATULATION; }
                else if (todayLoged && todayFlower)
                    { flag = true; currentState = (int)STATE.COMPLIMENT; }

                else if (flag) {
                    AllMongMongOff();
                    EncourageMong.SetActive(true);
                    rand = Random.Range(0, 5);
                    SetText(EncourageString_3_1[rand]);
                    SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_1[rand]);
                    spendTime = EncourageVoice_3_1[rand].length;
                    yield return new WaitForSeconds(spendTime + 1f);
                    flag = false;
                }

            break;
            //=================================================================================================
            // CASE _ 5 : 격려 3.2 몽몽이 출현
            //=================================================================================================
            case (int)STATE.ENCOURAGE_3_2:
                if (todayFlower && todayGardenLog != null && todayGardenLog.flower >= 10)
                    { flag = true; currentState = (int)STATE.CONGRATULATION; }
                else if (todayFlower)
                    { flag = true; currentState = (int)STATE.COMPLIMENT; }

                else if (flag) {
                    AllMongMongOff();
                    EncourageMong.SetActive(true);
                    rand = Random.Range(0, 5);
                    SetText(EncourageString_3_2[rand]);
                    SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_2[rand]);
                    spendTime = EncourageVoice_3_2[rand].length;
                    yield return new WaitForSeconds(spendTime + 1f);
                    flag = false;
                }

            break;
            //=================================================================================================
            // CASE _ 6 : 칭찬 몽몽이 출현
            //=================================================================================================
            case (int)STATE.COMPLIMENT:
                AllMongMongOff();
                ComplimentMong.SetActive(true);
                rand = Random.Range(0, 5);
                SetText(ComplimentString[rand]);
                SoundHandler.Instance.MongMongSource.PlayOneShot(ComplimentVoice[rand]);
                spendTime = ComplimentVoice[rand].length;
                yield return new WaitForSeconds(spendTime + 1f);
                flag = true;
                if (todayGardenLog != null)
                    todayFlowerCount = todayGardenLog.flower;
                currentState = (int)STATE.DEFAULT;
            break;
            //=================================================================================================
            // CASE _ 7 : 축하 몽몽이 출현
            //=================================================================================================
            case (int)STATE.CONGRATULATION:
                AllMongMongOff();
                CongratulationMong[Random.Range(0, 2)].SetActive(true);
                rand = Random.Range(0, 5);
                SetText(CongratulationString[rand]);
                SoundHandler.Instance.MongMongSource.PlayOneShot(CongratulationVoice[rand]);
                spendTime = CongratulationVoice[rand].length;
                yield return new WaitForSeconds(spendTime + 1f);
                flag = true;
                if (todayGardenLog != null)
                    todayFlowerCount = todayGardenLog.flower;
                currentState = (int)STATE.DEFAULT;
            break;
            //=================================================================================================
            // CASE _ 8 : 기본/권장 몽몽이 출현
            //=================================================================================================
            case (int)STATE.DEFAULT:
                if (todayGardenLog != null && todayGardenLog.flower == 10 &&
                    todayGardenLog.flower != todayFlowerCount)
                    { flag = true; currentState = (int)STATE.CONGRATULATION; }
                else if (todayGardenLog != null && todayFlowerCount < todayGardenLog.flower)
                    { flag = true; currentState = (int)STATE.COMPLIMENT; }

                else if(flag) {
                    if (Random.Range(0, 2) == 0) {
                        Debug.Log("Step4 MongMong 오늘 꽃 있고 어제 꽃 있음-----------");
                        AllMongMongOff();
                        DefaultMong[Random.Range(0, 2)].SetActive(true);
                        rand = Random.Range(0, 5);
                        SetText(DefaultString[rand]);
                        SoundHandler.Instance.MongMongSource.PlayOneShot(DefaultVoice[rand]);
                        spendTime = DefaultVoice[rand].length;
                        yield return new WaitForSeconds(spendTime + 1f);
                    } else {
                        Debug.Log("Step4 MongMong 오늘 꽃 있고 어제 꽃 있음-----------");
                        AllMongMongOff();
                        EncourageMong.SetActive(true);
                        rand = Random.Range(0, 5);
                        SetText(EncourageString[rand]);
                        SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice[rand]);
                        spendTime = EncourageVoice[rand].length;
                        yield return new WaitForSeconds(spendTime + 1f);
                    }
                    flag = false;
                }

            break;
            }

        }
    }

    public void AllMongMongOff() {
        foreach (GameObject go in AllMong)
            go.SetActive(false);
    }

    public void SetText(string txt) {
        if (isRight) RightQuoteHandler.QuoteText.text = txt;
        else LeftQuoteHandler.QuoteText.text = txt;
    }
}
