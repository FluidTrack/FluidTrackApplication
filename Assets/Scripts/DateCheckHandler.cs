using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateCheckHandler : MonoBehaviour
{
    static public DateCheckHandler Instance;
    public FooterBarHandler Footer;
    public GameObject[] UIObjects;
    public LogBlocker Blocker;
    public LogCanvasHandler LogCanvas;
    private String currentStampString;
    private String stampString;

    public void Awake() {
        Instance = this;
    }

    public void OnEnable() {
        // Enable되면 Enable된 시간의 SplitString값을 저장함
        stampString = SplitString(new TimeHandler.DateTimeStamp(DateTime.Now));
        StartCoroutine(checkRoutine());
    }

    private IEnumerator checkRoutine() {
        while(true) {
            yield return new WaitForSeconds(1f);
            currentStampString = SplitString(new TimeHandler.DateTimeStamp(DateTime.Now));

            // 기존 시간의 SplitString 값과 현재 시간 SplitString값이 달라지면
            if(currentStampString != stampString) {
                // stampString값을 현재 값으로 바꿔주고, 정원가기 페이지로 이동
                stampString = SplitString(new TimeHandler.DateTimeStamp(DateTime.Now));
                MainPageMongMongHandler.InitComplete = false;
                if (Footer.gameObject.activeSelf == true) {
                    for (int i = 0; i < UIObjects.Length; i++)
                        UIObjects[i].SetActive(false);
                    Footer.FooterButtonClick(0);

                    ProtocolHandler.Instance.ReadGardenLogs();
                    if(LogCanvas.gameObject.activeSelf)
                        Blocker.OnSideClick();
                }
            }
        }
    }

    private string SplitString(TimeHandler.DateTimeStamp stamp) {
        // 날짜가 달라지면 string 결과가 달라짐
        string result = stamp.ToDateString();

        // 시,분,초 중 초의 10의 자리가 달라지달 때 마다 결과가 달라짐
        //result += stamp.Hours + ":" + stamp.Minutes + ":" + ( stamp.Seconds / 10 );


        return result;
    }
}
