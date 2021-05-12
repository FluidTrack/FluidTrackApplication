using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBandConfirmUI : MonoBehaviour
{
    public void OkayButton() {
        DataHandler.User_moa_band_name = "";
        DataHandler.User_water_skip = "00:00";
        DataHandler.User_drink_skip = "00:00";
        DataHandler.User_poop_skip = "00:00";
        DataHandler.User_pee_skip = "00:00";
        DataHandler.User_font_family = "Bazzi";
        DataHandler.User_font_size = 40;
        DataHandler.User_creation_date = TimeHandler.GetCurrentTime();
        DataHandler.User_periode = 4;
        TotalManager.instance.isRegisterMode = false;
        StartCoroutine(DataHandler.CreateUsers());
        StartCoroutine(FinalCheck());
    }

    IEnumerator FinalCheck() {
        while (!DataHandler.User_isDataLoaded)
            yield return 0;
        DataHandler.User_isDataLoaded = false;
        GreetingMongMong.Instance.SayHello();
        yield return new WaitForSeconds(0.001f);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FOOTER_BAR].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.NAVI_BAR].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME5].SetActive(false);

        Instantiate(TotalManager.instance.FlashEffect);
        this.gameObject.SetActive(false);
    }

    public void BackButton() {
        this.gameObject.SetActive(false);
    }
}
