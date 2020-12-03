using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Welcome4Handler : MonoBehaviour
{
    public TimerHandler2 WaterSkip;
    public TimerHandler2 PooSkip;
    public TimerHandler2 PeeSkip;
    public Dropdown font_family;

    private void OnEnable() {
    }

    public void OkayButton() {
        DataHandler.User_water_skip = WaterSkip.getTime();
        DataHandler.User_drink_skip = "00:00";
        DataHandler.User_poop_skip = PooSkip.getTime();
        DataHandler.User_pee_skip = PeeSkip.getTime();
        switch(font_family.value) {
            case 0: DataHandler.User_font_family = "나눔고딕"; break;
            case 1: DataHandler.User_font_family = "HY엽서"; break;
            case 2: DataHandler.User_font_family = "타이포다방구"; break;
            case 3: DataHandler.User_font_family = "D2코딩"; break;
        }
        DataHandler.User_font_size =  40;
        StartCoroutine(DataHandler.Create_users());
    }

    


    public void ChangeFont() {
        TotalManager.instance.font_change(font_family.value,TotalManager.CANVAS.WELCOME4);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME3].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
