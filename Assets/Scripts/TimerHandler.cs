using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler : MonoBehaviour
{
    public Text TimezoneText;
    public Text HourText;
    public Text MinText;

    public int Hour = 0;
    public int Min = 0;

    public string getTime() {
        return ( ( Hour / 10 ) + "" + ( Hour % 10 ) + ":"
               + ( Min / 10 ) + "" + ( Min % 10 ) );
    }

    public void OnEnable() {
        drawClock();
    }

    public void TimezoneButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        Hour = (Hour >= 12) ? Hour - 12 : Hour + 12;
        drawClock();
    }

    public void HourMinusButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);

        Hour = ( Hour <= 0 ) ? 23 : Hour - 1;
        drawClock();
    }

    public void HourPlusButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        Hour = ( Hour >= 23 ) ? 0 : Hour + 1;
        drawClock();
    }

    public void MinMinusButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        if (Min <= 0) {
            Min = 45;
            HourMinusButton();
        } else {
            Min -= 15;
            drawClock();
        }
    }

    public void MinPlusButton() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        if (Min >= 45) {
            Min = 0;
            HourPlusButton();
        } else {
            Min += 15;
            drawClock();
        }
    }

    public void drawClock() {
        TimezoneText.text = ( Hour >= 12 ) ? "오후" : "오전";
        if(Hour == 0) HourText.text = "12시";
        else {
            HourText.text = ( Hour > 12 ) ? ((Hour-12) + "시") : (Hour + "시");
        }
        MinText.text = (Min/10) + "" + (Min%10) + "분";
    }
}
