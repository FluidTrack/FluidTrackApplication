using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHandler : MonoBehaviour
{
  public int MyUserId;
  public string MyCreationDate;
  public DataHandler.PeeLog[] MyPeeLogs;
  public DataHandler.WaterLog[] MyWaterLogs;

  public string[] MyDates = new string[28];
  public Image[] PeeGauges = new Image[28];
  public Image[] WaterGauges = new Image[28];
  public Text[] PeeGaugeTexts = new Text[28];
  public Text[] WaterGaugeTexts = new Text[28];
  public Sprite[] AfterPeeGauges = new Sprite[5];
  public Sprite[] AfterWaterGauges = new Sprite[6];
  public int[] PeeBoundary = new int[4];
  public int[] WaterBoundary = new int[6];


  public int CountPee(DataHandler.PeeLog[] logs, string Date) {
    int Count = 0;
    int Current = 0;
    // if (logs[Current].timestamp.Substring(0, 10) > Date) {
    //   return 0;
    // }
    while (logs[Current].timestamp.Substring(0, 10) != Date) {
    // 또는
    // string OnlyDateNoTime[] = logs[k].timestamp.Split(new string[] {" "}, StringSplitOptions.None);
      Current++;
      if (Current == logs.Length) {
        return 0;
      }
    }
    while (logs[Current].timestamp.Substring(0, 10) == Date) {
      Count++;
      Current++;
      if (Current == logs.Length) {
        break;
      }
    }
    return Count;
  }

  public int CountWater(DataHandler.WaterLog[] logs, string Date) {
    int Count = 0;
    int Current = 0;
    // if (logs[Current].timestamp.Substring(0, 10) > Date) {
    //   return 0;
    // }
    while (logs[Current].timestamp.Substring(0, 10) != Date) {
    // 또는
    // string OnlyDateNoTime[] = logs[k].timestamp.Split(new string[] {" "}, StringSplitOptions.None);
      Current++;
      if (Current == logs.Length) {
        return 0;
      }
    }
    while (logs[Current].timestamp.Substring(0, 10) == Date) {
      Count++;
      Current++;
      if (Current == logs.Length) {
        break;
      }
    }
    return Count;
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
      else if (CountWater(logs, MyDates[i]) == boundary[1]){
        beforeImages[i].sprite = afterImages[1];
      }
      else if (CountWater(logs, MyDates[i]) == boundary[2]){
        beforeImages[i].sprite = afterImages[2];
      }
      else if (CountWater(logs, MyDates[i]) == boundary[3]){
        beforeImages[i].sprite = afterImages[3];
      }
      else if (CountWater(logs, MyDates[i]) == boundary[4]){
        beforeImages[i].sprite = afterImages[4];
      }
      else if (CountWater(logs, MyDates[i]) >= boundary[5]) {
        beforeImages[i].sprite = afterImages[5];
      }
    }
  }

  public void FillDateFromCreation (string CreatingDate, string[] dates) {
    TimeHandler.DateTimeStamp CreatingDateStamp = new TimeHandler.DateTimeStamp(CreatingDate);
    for (int i = 0; i < dates.Length; i++) {
      dates[i] = (CreatingDateStamp + i).ToDateString();
    }
  }





  // IEnumerator CheckLoad() {
  //     while (!DataHandler.User_isDataLoaded)
  //         yield return 0;
  //     DataHandler.User_isDataLoaded = false;
  //     Debug.Log(DataHandler.User_creation_date);
  // }
  //
  // IEnumerator CheckLoad2() {
  //     while (!DataHandler.User_isPeeDataLoaded)
  //         yield return 0;
  //     DataHandler.User_isPeeDataLoaded = false;
  //     Debug.Log(DataHandler.Pee_logs.PeeLogs.Length);
  // }
  //
  // IEnumerator CheckLoad3() {
  //   while (!DataHandler.User_isWaterDataLoaded) {
  //     yield return 0;
  //   }
  //   DataHandler.User_isWaterDataLoaded = false;
  //   Debug.Log(DataHandler.Water_logs.WaterLogs.Length);
  // }

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

    MyUserId = DataHandler.User_id;
    MyCreationDate = DataHandler.User_creation_date;
    MyPeeLogs = DataHandler.Pee_logs.PeeLogs;
    MyWaterLogs = DataHandler.Water_logs.WaterLogs;

    FillDateFromCreation(MyCreationDate, MyDates);

    ChangePeeGaugeText(MyPeeLogs, PeeGaugeTexts);
    ChangeWaterGaugeText(MyWaterLogs, WaterGaugeTexts);
    ChangePeeGaugeImage(MyPeeLogs, PeeGauges, AfterPeeGauges, PeeBoundary);
    ChangeWaterGaugeImage(MyWaterLogs, WaterGauges, AfterWaterGauges, WaterBoundary);
  }


  private void Start() {

      //Debug.Log(DataHandler.User_creation_date);
      StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
      //StartCoroutine(CheckLoad());

      StartCoroutine(DataHandler.ReadPeeLogs(DataHandler.User_id));
      //StartCoroutine(CheckLoad2());

      StartCoroutine(DataHandler.ReadWaterLogs(DataHandler.User_id));
      //StartCoroutine(CheckLoad3());

      StartCoroutine(CheckLoadAndVisualize());


  }


}
