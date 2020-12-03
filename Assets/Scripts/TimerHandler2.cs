using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler2 : MonoBehaviour
{
    public Text MinText;

    public int Min = 0;

    public string getTime() {
        int tempHour = (Min/60), tempMin = (Min%60);

        return ( (tempHour/10) +""+ (tempHour%10) + ":" 
               + (tempMin/10) + "" + (tempMin%10) );
    }

    public void OnEnable() {
        drawClock();
    }

    public void MinMinusButton() {
        if (Min <= 0) return;
        Min = (Min <= 5) ? Min - 1 : Min - 5;
        drawClock();
    }

    public void MinPlusButton() {
        if (Min >= 300) return;
        Min = ( Min <= 4 ) ? Min + 1 : Min + 5;
        drawClock();
    }

    public void drawClock() {
        MinText.text = Min + "분";
    }
}
