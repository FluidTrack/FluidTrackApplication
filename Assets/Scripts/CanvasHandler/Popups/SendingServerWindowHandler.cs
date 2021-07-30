using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendingServerWindowHandler : MonoBehaviour
{
    public int TotalNum = 0;
    public int CurrentNum = 0;
    public float percent = 0;
    public float BarMax = 600f;
    public RectTransform Bar;
    public Text StatusText;

    public void DrawBar(int totalNum = 0, int currentNum = 0) {
        CurrentNum = currentNum;
        TotalNum = totalNum;
        StatusText.text = CurrentNum + "/" + TotalNum;

        percent = ( (float)CurrentNum ) / ( (float)TotalNum );
        Bar.sizeDelta = new Vector2(BarMax * percent, Bar.sizeDelta.y);
    }
}
