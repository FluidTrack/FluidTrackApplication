using System.Collections;
using System.Collections.Generic;
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

    public void OnEnable() {
        StartCoroutine(MongMongStep1());
    }

    private int rand = 0;

    IEnumerator MongMongStep1() {
        yield return new WaitForSeconds(0.2f);
        while (GreetingMongMong.Instance.GreetingMongMongObject.activeSelf) {
            LeftQuote.SetActive(false);
            RightQuote.SetActive(false);
            yield return 0;
        }
        SelectQuoteDirection(this.isRight);

        TimeHandler.GetCurrentTime();

        if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(new TimeHandler.DateTimeStamp(DataHandler.User_creation_date),
            TimeHandler.CurrentTime) == 0) {
            StartCoroutine(MongMongStep4());
        } else if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, DataHandler.lastJoin) == 0) {
            StartCoroutine(MongMongStep2());
        } else if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, DataHandler.lastJoin+1) == 0) {
            StartCoroutine(MongMongStep4());
        } else {
            // 몽몽이 버전 1
            AllMongMongOff();
            GloomyMong.SetActive(true);
            rand = Random.Range(0, 5);
            SetText(GloomyString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(GloomyVoice[rand]);
            float spendTime = GloomyVoice[rand].length;
            yield return new WaitForSeconds(spendTime + 1f);
            StartCoroutine(MongMongStep2());
        }

    }


    private DataHandler.GardenLog yesterdayGardenLog = null;
    private DataHandler.GardenLog todayGardenLog = null;

    IEnumerator MongMongStep2() {
        yield return 0;
        AllMongMongOff();
        EncourageMong.SetActive(true);

        yesterdayGardenLog = null;
        todayGardenLog = null;
        for(int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime,
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0)
                todayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime - 1,
                new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0)
                yesterdayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
        }

        bool todayCheck = ( todayGardenLog != null &&
            ( todayGardenLog.log_water > 0 || todayGardenLog.log_pee > 0 || todayGardenLog.log_poop > 0 ||
              todayGardenLog.flower > 0) );
        bool yesterdayCheck = ( yesterdayGardenLog != null &&
            ( yesterdayGardenLog.log_water > 0 || yesterdayGardenLog.log_pee > 0 || yesterdayGardenLog.log_poop > 0 ||
              yesterdayGardenLog.flower > 0) );


        if (todayCheck == true && yesterdayCheck == true) {
            StartCoroutine(MongMongStep3());
        } else if (todayCheck == false && yesterdayCheck == true) {
            rand = Random.Range(0, 5);
            SetText(EncourageString_2_2[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_2_2[rand]);
        } else {
            rand = Random.Range(0, 5);
            SetText(EncourageString_2_1[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_2_1[rand]);
        }
    }

    IEnumerator MongMongStep3() {
        yield return 0;
        bool todayCheck = ( todayGardenLog != null && todayGardenLog.flower > 0 );
        bool yesterdayCheck = ( yesterdayGardenLog != null && yesterdayGardenLog.flower > 0 );

        if (todayCheck == true && yesterdayCheck == true) {
            StartCoroutine(MongMongStep4());
        } else if (todayCheck == false && yesterdayCheck == true) {
            rand = Random.Range(0, 5);
            SetText(EncourageString_3_2[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_2[rand]);
        } else {
            rand = Random.Range(0, 5);
            SetText(EncourageString_3_1[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_1[rand]);
        }
    }

    IEnumerator MongMongStep4() {
        yield return 0;
        AllMongMongOff();
        DefaultMong[Random.Range(0,2)].SetActive(true);
        bool todayCheck = ( todayGardenLog != null && todayGardenLog.flower == 10 );

        if (todayCheck) {
            AllMongMongOff();
            CongratulationMong[Random.Range(0, 2)].SetActive(true);
            rand = Random.Range(0, 5);
            SetText(CongratulationString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(CongratulationVoice[rand]);
        } else {
            if(Random.Range(0,2) == 0) {
                AllMongMongOff();
                DefaultMong[Random.Range(0, 2)].SetActive(true);
                rand = Random.Range(0, 5);
                SetText(DefaultString[rand]);
                SoundHandler.Instance.MongMongSource.PlayOneShot(DefaultVoice[rand]);
            } else {
                AllMongMongOff();
                EncourageMong.SetActive(true);
                rand = Random.Range(0, 5);
                SetText(EncourageString[rand]);
                SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice[rand]);
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
