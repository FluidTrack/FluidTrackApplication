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

    public void OnEnable() {
        StartCoroutine(MongMongStep1());
    }

    private int rand = 0;
    private bool isStep1MongMongShow = false;

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
            yesterdayGardenLog = null;
            todayGardenLog = null;
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime,
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0)
                    todayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime - 1,
                    new TimeHandler.DateTimeStamp(DataHandler.Garden_logs.GardenLogs[i].timestamp)) == 0)
                    yesterdayGardenLog = DataHandler.Garden_logs.GardenLogs[i];
            }
            StartCoroutine(MongMongStep4());
        } else if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, DataHandler.lastJoin+1) == 0 || isStep1MongMongShow) {
            StartCoroutine(MongMongStep2());
        } else {
            // 몽몽이 버전 1
            AllMongMongOff();
            Debug.Log("Step1 MongMong-------------------");
            GloomyMong.SetActive(true);
            rand = Random.Range(0, 5);
            SetText(GloomyString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(GloomyVoice[rand]);
            float spendTime = GloomyVoice[rand].length;
            yield return new WaitForSeconds(spendTime + 1f);
            isStep1MongMongShow = true;
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
            Debug.Log("Step2 MongMong 어제 기록 있고 오늘 기록 없음-----------");
            rand = Random.Range(0, 5);
            SetText(EncourageString_2_2[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_2_2[rand]);
        } else {
            Debug.Log("Step2 MongMong 어제 기록 없음-------------------------");
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
            Debug.Log("Step3 MongMong 오늘 꽃 없고 어제 꽃 있음-----------");
            rand = Random.Range(0, 5);
            SetText(EncourageString_3_2[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_2[rand]);
        } else {
            Debug.Log("Step3 MongMong 오늘 꽃 없고 어제 꽃 없음-----------");
            rand = Random.Range(0, 5);
            SetText(EncourageString_3_1[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice_3_1[rand]);
        }
    }

    IEnumerator MongMongStep4() {
        yield return 0;
        AllMongMongOff();
        DefaultMong[Random.Range(0,2)].SetActive(true);
        bool todayCheck = ( todayGardenLog != null && todayGardenLog.flower >= 10 );

        bool flag = true;
        TimeHandler.DateTimeStamp lastTimestamp = null;
        TimeHandler.DateTimeStamp CurrentStamp = new TimeHandler.DateTimeStamp(TimeHandler.CurrentTime);
        TimeHandler.DateTimeStamp FlowerStamp = null;
        int TodayFlower = 0;

        FileStream fs = null; StreamReader sr = null;
        FileStream fs2 = null; StreamWriter sw = null;

        try {
            fs = new FileStream(DataHandler.dataPath + "/MainPageCongratulationDate", FileMode.Open);
            sr = new StreamReader(fs);
            lastTimestamp = new TimeHandler.DateTimeStamp(sr.ReadLine());
            sr.Close();  fs.Close();
        } catch(System.Exception e) {
            e.ToString(); lastTimestamp = CurrentStamp - 1;
            fs2 = new FileStream(DataHandler.dataPath + "/MainPageCongratulationDate", FileMode.OpenOrCreate);
            sw = new StreamWriter(fs2);
            sw.WriteLine(lastTimestamp);
            sw.Close(); fs2.Close(); 
        } finally {
            if (sw != null) sw.Close(); if (sr != null) sr.Close();
            if (fs != null) fs.Close(); if (fs2 != null) fs2.Close();
        }

        try {
            fs = new FileStream(DataHandler.dataPath + "/TodayFlower", FileMode.Open);
            sr = new StreamReader(fs);
            FlowerStamp = new TimeHandler.DateTimeStamp(sr.ReadLine());
            TodayFlower = int.Parse(sr.ReadLine());
            sr.Close(); fs.Close();
        } catch (System.Exception e) {
            e.ToString(); TodayFlower = 0; FlowerStamp = CurrentStamp - 1;
            fs2 = new FileStream(DataHandler.dataPath + "/TodayFlower", FileMode.OpenOrCreate);
            sw = new StreamWriter(fs2);
            sw.WriteLine(FlowerStamp.ToDateString());
            sw.WriteLine(TodayFlower.ToString());
            sw.Close(); fs2.Close();
        } finally {
            if (sw != null) sw.Close(); if (sr != null) sr.Close();
            if (fs != null) fs.Close(); if (fs2 != null) fs2.Close();
        }

        if (todayCheck && TimeHandler.DateTimeStamp.CmpDateTimeStamp(CurrentStamp,lastTimestamp) != 0) {
            Debug.Log("Step4 MongMong 오늘 꽃 10송이-----------");
            AllMongMongOff();
            CongratulationMong[Random.Range(0, 2)].SetActive(true);
            try {
                fs2 = new FileStream(DataHandler.dataPath + "/MainPageCongratulationDate", FileMode.OpenOrCreate);
                sw = new StreamWriter(fs2);
                TimeHandler.GetCurrentTime();
                sw.WriteLine(CurrentStamp.ToDateString());
                sw.Close(); fs2.Close();
            } catch(System.Exception e) { e.ToString(); } finally {
                if (sw != null) sw.Close(); if (fs2 != null) fs2.Close();
            }

            rand = Random.Range(0, 5);
            SetText(CongratulationString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(CongratulationVoice[rand]);
            float spendTime2 = CongratulationVoice[rand].length;
            yield return new WaitForSeconds(spendTime2 + 1f);
        } else if (todayGardenLog != null && TimeHandler.DateTimeStamp.CmpDateTimeStamp(CurrentStamp,FlowerStamp) == 0 &&
            TodayFlower < todayGardenLog.flower) {

            Debug.Log("Step4 MongMong 오늘 꽃 있고 어제 꽃 있음-----------");
            AllMongMongOff();
            ComplimentMong.SetActive(true);
            rand = Random.Range(0, 5);
            SetText(ComplimentString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(ComplimentVoice[rand]);
            float spendTime3 = ComplimentVoice[rand].length;
            TodayFlower = todayGardenLog.flower;
            yield return new WaitForSeconds(spendTime3 + 1f);
        }

        try {
            fs2 = new FileStream(DataHandler.dataPath + "/TodayFlower", FileMode.Open);
            sw = new StreamWriter(fs2);
            sw.WriteLine(CurrentStamp.ToDateString());
            sw.WriteLine(todayGardenLog.flower.ToString());
            sw.Close(); fs2.Close();
        } catch (System.Exception e) { e.ToString(); } finally {
            if (sw != null) sw.Close(); if (fs2 != null) fs2.Close();
        }

        if (Random.Range(0,2) == 0) {
            Debug.Log("Step4 MongMong 오늘 꽃 있고 어제 꽃 있음-----------");
            AllMongMongOff();
            DefaultMong[Random.Range(0, 2)].SetActive(true);
            rand = Random.Range(0, 5);
            SetText(DefaultString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(DefaultVoice[rand]);
        } else {
            Debug.Log("Step4 MongMong 오늘 꽃 있고 어제 꽃 있음-----------");
            AllMongMongOff();
            EncourageMong.SetActive(true);
            rand = Random.Range(0, 5);
            SetText(EncourageString[rand]);
            SoundHandler.Instance.MongMongSource.PlayOneShot(EncourageVoice[rand]);
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
