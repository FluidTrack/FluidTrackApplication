using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Welcome4Handler : MonoBehaviour
{
    public TimerHandler2 MorningCall;
    public Dropdown font_family;

    //public void OkayButton() {
    //    DataHandler.User_water_skip = WaterSkip.getTime();
    //    DataHandler.User_drink_skip = "00:00";
    //    DataHandler.User_poop_skip = PooSkip.getTime();
    //    DataHandler.User_pee_skip = PeeSkip.getTime();
    //    switch(font_family.value) {
    //        case 0: DataHandler.User_font_family = "나눔고딕"; break;
    //        case 1: DataHandler.User_font_family = "HY엽서"; break;
    //        case 2: DataHandler.User_font_family = "타이포다방구"; break;
    //        case 3: DataHandler.User_font_family = "D2코딩"; break;
    //    }
    //    DataHandler.User_font_size =  40;
    //    DataHandler.User_creation_date = TimeHandler.GetCurrentTime();
    //    StartCoroutine(DataHandler.CreateUsers());
    //    StartCoroutine(FinalCheck());
    //}

    public void OkayButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME5].SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void PrevButton() {
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME3].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
