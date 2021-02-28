using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHandler : MonoBehaviour
{
  public int MyUserId;
  public string MyCreationDate;
  public string MyRealCreationDate;
  public static string MyCurrentDate;
  public DataHandler.PeeLog[] MyPeeLogs;
  public DataHandler.WaterLog[] MyWaterLogs;
  public DataHandler.PoopLog[] MyPoopLogs;

  public string[] MyDates = new string[28];
  public Image[] PeeGauges = new Image[28];
  public Image[] WaterGauges = new Image[28];
  public Image[] PoopGauges = new Image[28];
  public Text[] PeeGaugeTexts = new Text[28];
  public Text[] WaterGaugeTexts = new Text[28];
  public Sprite[] AfterPeeGauges = new Sprite[5];
  public Sprite[] AfterWaterGauges = new Sprite[6];
  public Sprite[] AfterPoopGauges = new Sprite[2];
  public int[] PeeBoundary = new int[4];
  public int[] WaterBoundary = new int[5];
  public Text[] LeftLabelDates = new Text[7];
  public Text[] RightLabelDates = new Text[7];
  public Text[] WeekTexts = new Text[4];
  public String[] WeekTextsList = {"1주", "2주", "3주", "4주", "5주", "6주", "7주", "8주"};
  public int WeekMode = 0;


  public int CountPee(DataHandler.PeeLog[] logs, string Date) {
    int Count = 0;
    int Current = 0;
    // if (logs[Current].timestamp.Substring(0, 10) > Date) {
    //   return 0;
    // }
    TimeHandler.DateTimeStamp CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    while (CurrentLogTime.ToDateString() != Date) {
    // 또는
    // string OnlyDateNoTime[] = logs[k].timestamp.Split(new string[] {" "}, StringSplitOptions.None);
      Current++;
      if (Current == logs.Length) {
        return 0;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }
    while (CurrentLogTime.ToDateString() == Date) {
      Count++;
      Current++;
      if (Current == logs.Length) {
        break;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }
    return Count;

  }

  public int CountWater(DataHandler.WaterLog[] logs, string Date) {
    int Count = 0;
    int Current = 0;

    TimeHandler.DateTimeStamp CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    while (CurrentLogTime.ToDateString() != Date) {
    //while (logs[Current].timestamp.Substring(0, 10) != Date) {
      Current++;
      if (Current == logs.Length) {
        return 0;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }
    while (CurrentLogTime.ToDateString() == Date) {
    //while (logs[Current].timestamp.Substring(0, 10) == Date) {
      Count++;
      Current++;
      if (Current == logs.Length) {
        break;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }
    return Count;
  }

  public bool FindGoodPooDay (DataHandler.PoopLog[] logs, string Date) {
    int Current = 0;
    TimeHandler.DateTimeStamp CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);

    while (CurrentLogTime.ToDateString() != Date) {
      Current++;
      if (Current == logs.Length) {
        return false;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }
    while (CurrentLogTime.ToDateString() == Date) {
      if ((logs[Current].type == 3)||(logs[Current].type == 4)) {
        return true;
      }
      Current++;
      if (Current == logs.Length) {
        return false;
      }
      CurrentLogTime = new TimeHandler.DateTimeStamp(logs[Current].timestamp);
    }

    return false;

  }

  public void ChangePeeGaugeText(DataHandler.PeeLog[] logs, Text[] texts) {
    for (int i = 0; i < texts.Length; i++) {
      texts[i].text = CountPee(logs, MyDates[i]).ToString();
    }
  }

  public void ChangeWaterGaugeText(DataHandler.WaterLog[] logs, Text[] texts) {
    for (int i = 0; i < texts.Length; i++) {
      texts[i].text = CountWater(logs, MyDates[i]).ToString();
    }
  }

  public void ChangePeeGaugeImage (DataHandler.PeeLog[] logs, Image[] beforeImages, Sprite[] afterImages, int[] boundary) {
    for (int i = 0; i < beforeImages.Length; i++) {
      if (CountPee(logs, MyDates[i]) <= boundary[0]) {
        beforeImages[i].sprite = afterImages[0];
      }
      else if (CountPee(logs, MyDates[i]) <= boundary[1]){
        beforeImages[i].sprite = afterImages[1];
      }
      else if (CountPee(logs, MyDates[i]) <= boundary[2]){
        beforeImages[i].sprite = afterImages[2];
      }
      else if (CountPee(logs, MyDates[i]) <= boundary[3]){
        beforeImages[i].sprite = afterImages[3];
      }
      else {
        beforeImages[i].sprite = afterImages[4];
      }
    }
  }

  public void ChangeWaterGaugeImage (DataHandler.WaterLog[] logs, Image[] beforeImages, Sprite[] afterImages, int[] boundary) {
    for (int i = 0; i < beforeImages.Length; i++) {
      if (CountWater(logs, MyDates[i]) <= boundary[0]) {
        beforeImages[i].sprite = afterImages[0];
      }
      else if (CountWater(logs, MyDates[i]) <= boundary[1]){
        beforeImages[i].sprite = afterImages[1];
      }
      else if (CountWater(logs, MyDates[i]) <= boundary[2]){
        beforeImages[i].sprite = afterImages[2];
      }
      else if (CountWater(logs, MyDates[i]) <= boundary[3]){
        beforeImages[i].sprite = afterImages[3];
      }
      else if (CountWater(logs, MyDates[i]) <= boundary[4]){
        beforeImages[i].sprite = afterImages[4];
      }
      else {
        beforeImages[i].sprite = afterImages[5];
      }
    }
  }

  public void ChangePoopGaugeImage (DataHandler.PoopLog[] logs, Image[] beforeImages, Sprite[] afterImages) {
    for (int i = 0; i < beforeImages.Length; i++) {
      if (FindGoodPooDay(logs, MyDates[i])) {
        beforeImages[i].sprite = afterImages[0];
      }
      else {
        beforeImages[i].sprite = afterImages[1];
      }
    }
  }

  public void FillDateFromCreation (string CreatingDate, string[] dates) {
    TimeHandler.DateTimeStamp CreatingDateStamp = new TimeHandler.DateTimeStamp(CreatingDate);
    for (int i = 0; i < dates.Length; i++) {
      dates[i] = (CreatingDateStamp + i).ToDateString();
    }
  }

  public void FillWeekText (Text[] WeekTexts, int WeekMode) {
    for (int i = 0; i < 4; i++) {
      WeekTexts[i].text = WeekTextsList[i + WeekMode];
    }
  }

  public void SetDateLabel (string CreatingDate, Text[] right, Text[] left) {
    TimeHandler.DateTimeStamp CreatingDateStamp = new TimeHandler.DateTimeStamp(CreatingDate);
    int crt = CreatingDateStamp.Date;
    right[0].text = TimeHandler.DateTimeStamp.DateList[crt];
    left[0].text = TimeHandler.DateTimeStamp.DateList[crt];
    for (int i = 1; i < 7; i++) {
      crt++;
      right[i].text = TimeHandler.DateTimeStamp.DateList[crt % 7];
      left[i].text = TimeHandler.DateTimeStamp.DateList[crt % 7];
    }
  }

  public void OnWeekChangeButton_UpClicked() {
    Debug.Log(MyRealCreationDate);
    TimeHandler.DateTimeStamp mrcdts2 = new TimeHandler.DateTimeStamp(MyRealCreationDate);
    string MyRealCreationDate2 = mrcdts2.ToString();
    if (MyCreationDate == MyRealCreationDate2) {
      Debug.Log("wrong");
    }
    else {
      WeekMode--;
      for (int i = 0; i < 4; i++) {
        WeekTexts[i].text = WeekTextsList[i + WeekMode];
      }
      TimeHandler.DateTimeStamp mcdts = new TimeHandler.DateTimeStamp(MyCreationDate);
      mcdts = mcdts - 7;
      MyCreationDate = mcdts.ToString();

      //test
      FillDateFromCreation(MyCreationDate, MyDates);
      ChangePeeGaugeText(MyPeeLogs, PeeGaugeTexts);
      ChangeWaterGaugeText(MyWaterLogs, WaterGaugeTexts);
      ChangePeeGaugeImage(MyPeeLogs, PeeGauges, AfterPeeGauges, PeeBoundary);
      ChangeWaterGaugeImage(MyWaterLogs, WaterGauges, AfterWaterGauges, WaterBoundary);
      ChangePoopGaugeImage(MyPoopLogs, PoopGauges, AfterPoopGauges);
    }
    Debug.Log(MyCreationDate + "&" + MyRealCreationDate);
  }

  public void OnWeekChangeButton_DownClicked() {
    TimeHandler.DateTimeStamp mrcdts = new TimeHandler.DateTimeStamp(MyRealCreationDate);
    mrcdts = mrcdts + 28;
    string MyRealCreationDate2 = mrcdts.ToString();
    if (MyCreationDate == MyRealCreationDate2) {

    }
    else {
      WeekMode++;
      for (int i = 0; i < 4; i++) {
        WeekTexts[i].text = WeekTextsList[i + WeekMode];
      }
      TimeHandler.DateTimeStamp mcdts = new TimeHandler.DateTimeStamp(MyCreationDate);
      mcdts = mcdts + 7;
      MyCreationDate = mcdts.ToString();

      //test
      FillDateFromCreation(MyCreationDate, MyDates);
      ChangePeeGaugeText(MyPeeLogs, PeeGaugeTexts);
      ChangeWaterGaugeText(MyWaterLogs, WaterGaugeTexts);
      ChangePeeGaugeImage(MyPeeLogs, PeeGauges, AfterPeeGauges, PeeBoundary);
      ChangeWaterGaugeImage(MyWaterLogs, WaterGauges, AfterWaterGauges, WaterBoundary);
      ChangePoopGaugeImage(MyPoopLogs, PoopGauges, AfterPoopGauges);
    }
    Debug.Log(MyCreationDate + "&" + MyRealCreationDate);
  }



  IEnumerator CheckLoadAndVisualize() {
    while (!DataHandler.User_isDataLoaded) {
        yield return 0;
    }
    DataHandler.User_isDataLoaded = false;

    while (!DataHandler.User_isPeeDataLoaded) {
        yield return 0;
    }
    DataHandler.User_isPeeDataLoaded = false;

    while (!DataHandler.User_isWaterDataLoaded) {
      yield return 0;
    }
    DataHandler.User_isWaterDataLoaded = false;

    while (!DataHandler.User_isPooDataLoaded) {
      yield return 0;
    }
    DataHandler.User_isPooDataLoaded = false;

    MyUserId = DataHandler.User_id;
    MyCreationDate = DataHandler.User_creation_date;
    MyRealCreationDate = MyCreationDate;
    MyPeeLogs = DataHandler.Pee_logs.PeeLogs;
    MyWaterLogs = DataHandler.Water_logs.WaterLogs;
    MyPoopLogs = DataHandler.Poop_logs.PoopLogs;

    FillDateFromCreation(MyCreationDate, MyDates);
    FillWeekText(WeekTexts, WeekMode);
    SetDateLabel(MyCreationDate, RightLabelDates, LeftLabelDates);

    ChangePeeGaugeText(MyPeeLogs, PeeGaugeTexts);
    ChangeWaterGaugeText(MyWaterLogs, WaterGaugeTexts);
    ChangePeeGaugeImage(MyPeeLogs, PeeGauges, AfterPeeGauges, PeeBoundary);
    ChangeWaterGaugeImage(MyWaterLogs, WaterGauges, AfterWaterGauges, WaterBoundary);
    ChangePoopGaugeImage(MyPoopLogs, PoopGauges, AfterPoopGauges);
  }


  private void Start() {

      StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));

      StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));

      StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));

      StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));

      StartCoroutine(CheckLoadAndVisualize());


  }


}
