using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome5Handler : MonoBehaviour
{
    public void OkayButton() {
        DataHandler.User_water_skip = "00:00";
        DataHandler.User_drink_skip = "00:00";
        DataHandler.User_poop_skip = "00:00";
        DataHandler.User_pee_skip = "00:00";
        DataHandler.User_font_family = "Bazzi";
        DataHandler.User_font_size = 40;
        DataHandler.User_creation_date = TimeHandler.GetCurrentTime();
        DataHandler.User_periode = 4;
        DataHandler.User_gender = "male";
        DataHandler.User_birthday = "1994.12.23";
        StartCoroutine(DataHandler.CreateUsers());
        StartCoroutine(FinalCheck());
    }

    IEnumerator FinalCheck() {
        while (!DataHandler.User_isDataLoaded)
            yield return 0;
        DataHandler.User_isDataLoaded = false;
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FOOTER_BAR].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.NAVI_BAR].SetActive(true);
        Instantiate(TotalManager.instance.FlashEffect);
        GreetingMongMong.Instance.SayHello();
        this.gameObject.SetActive(false);
    }
}
