using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPageMongMongHandler : MonoBehaviour
{
    public AudioClip[] DefaultVoice;
    public AudioClip[] CongratulationVoice;
    public AudioClip[] ComplimentVoice;
    public AudioClip[] GloomingVoice;

    public GameObject[] DefaultObject;
    public GameObject[] CongratulationObject;
    public GameObject ComplimentObject;
    public GameObject GloomingObject;

    public MissionCanvasHandler Mission;

    private int todayLog = -1;
    private int yesterdayLog = -1;

    public void OnEnable() {
        DefaultObject[0].SetActive(true);
        DefaultObject[1].SetActive(true);
        CongratulationObject[0].SetActive(false);
        CongratulationObject[1].SetActive(false);
        ComplimentObject.SetActive(false);
        GloomingObject.SetActive(false);
        DataHandler.User_isGardenDataLoaded = false;
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));
        StartCoroutine(FetchData());
        StartCoroutine(SayQuotes());
    }

    public void OnDisable() {
        flag = false;
        todayLog = -1;
        yesterdayLog = -1;
    }

    public bool flag = false;

    public IEnumerator SayQuotes() {
        while (!Mission.DrawDone)
            yield return 0;
        while (!flag) yield return 0;

        if(todayLog <= 0) {
            if(yesterdayLog >= 10 ||
                TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.TableCanvasTime,
                    new TimeHandler.DateTimeStamp(DataHandler.User_creation_date)) == 0) {
                DefaultMong();
            } else {
                GloomingMong();
            }
        } else {
            if(todayLog >= 10) {
                if(yesterdayLog >= 10) {
                    CongratulationMong();
                } else {
                    ComplimentMong();
                }
            } else {
                DefaultMong();
            }
        }
    }

    public void InitMongMong() {
        DefaultObject[0].SetActive(false);
        DefaultObject[1].SetActive(false);
        CongratulationObject[0].SetActive(false);
        CongratulationObject[1].SetActive(false);
        ComplimentObject.SetActive(false);
        GloomingObject.SetActive(false);
    }

    public void DefaultMong() {
        InitMongMong();
        DefaultObject[Random.Range(0,2)].SetActive(true);
        SoundHandler.Instance.MongMongSource.PlayOneShot(
            DefaultVoice[Random.Range(0, DefaultVoice.Length)]);
    }

    public void CongratulationMong() {
        InitMongMong();
        CongratulationObject[Random.Range(0, 2)].SetActive(true);
        SoundHandler.Instance.MongMongSource.PlayOneShot(
            CongratulationVoice[Random.Range(0, CongratulationVoice.Length)]);
    }

    public void ComplimentMong() {
        InitMongMong();
        ComplimentObject.SetActive(true);
        SoundHandler.Instance.MongMongSource.PlayOneShot(
            ComplimentVoice[Random.Range(0, ComplimentVoice.Length)]);
    }

    public void GloomingMong() {
        InitMongMong();
        GloomingObject.SetActive(true);
        SoundHandler.Instance.MongMongSource.PlayOneShot(
            GloomingVoice[Random.Range(0, GloomingVoice.Length)]);
    }

    public IEnumerator FetchData() {
        while (!DataHandler.User_isGardenDataLoaded)
            yield return 0;
        DataHandler.User_isGardenDataLoaded = false;

        TimeHandler.GetCurrentTime();
        TimeHandler.DateTimeStamp today = TimeHandler.CurrentTime;
        TimeHandler.DateTimeStamp yesterday = today - 1;

        DataHandler.GardenLog[] logs = DataHandler.Garden_logs.GardenLogs;
        int count = logs.Length;
        for(int i = 0; i < count; i++) {
            if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    yesterday, new TimeHandler.DateTimeStamp(logs[i].timestamp)) == 0)
                yesterdayLog = logs[i].flower;
            else if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                    today, new TimeHandler.DateTimeStamp(logs[i].timestamp)) == 0) {
                todayLog = logs[i].flower;
                break;
            }
        }
        flag = true;
    }
}
