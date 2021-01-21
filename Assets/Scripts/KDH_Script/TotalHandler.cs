using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalHandler : MonoBehaviour
{
  DataHandler.PeeLog[] logs;

  public Image[] BeginImages = new Image[7];
  public Text[] BeginTexts = new Text[7];
  public string[] Dates = new string[7];
  public Sprite Sprite1;
  public Sprite Sprite2;
  public Sprite Sprite3;
  public Sprite Sprite4;
  public Sprite Sprite5;
  public int Boundary1;
  public int Boundary2;
  public int Boundary3;
  public int Boundary4;

  public int CountPeeByTime(string Date) {
    int Count = 0;
    for (int k = 0; k < 100; k++) {
      if (logs[k].timestamp == Date) {
        Count++;
      }
    }
    return Count;
  }

  public void ChangeText_TH() {
    for (int i = 0; i < Dates.Length; i++) {
      BeginTexts[i].text = CountPeeByTime(Dates[i]).ToString();
    }
  }

  public void ChangeGauge_TH() {
    for (int j = 0; j < Dates.Length; j++) {
      if (CountPeeByTime(Dates[j]) <= Boundary1) {
        BeginImages[j].sprite = Sprite1;
      }
      else if (CountPeeByTime(Dates[j]) <= Boundary2) {
        BeginImages[j].sprite = Sprite2;
      }
      else if (CountPeeByTime(Dates[j]) <= Boundary3) {
        BeginImages[j].sprite = Sprite3;
      }
      else if (CountPeeByTime(Dates[j]) <= Boundary4) {
        BeginImages[j].sprite = Sprite4;
      }
      else {
        BeginImages[j].sprite = Sprite5;
      }
    }
  }

  /*public void ChangeTotal() {

  }*/

    // Start is called before the first frame update
    void Start()
    {
      logs = DataHandler.GetTempPeeData();
      ChangeText_TH();
      ChangeGauge_TH();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
