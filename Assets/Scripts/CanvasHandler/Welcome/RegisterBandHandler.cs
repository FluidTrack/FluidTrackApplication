using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterBandHandler : MonoBehaviour
{
    public static RegisterBandHandler Instance;

    public Color ActiveColor_Circle;
    public Color ActiveColor_Icon;
    public Color InactiveColor_Circle;
    public Color InactiveColor_Icon;
    public Button CancelButton;
    public Image WaterCircle;
    public Image PeeCircle;
    public Image PooCircle;
    public Image WaterIcon;
    public Image PeeIcon;
    public Image PooIcon;

    public GameObject FinalWindow;

    private bool WaterButtonClicked = false;
    private bool PeeButtonClicked = false;
    private bool PooButtonClicked = false;

    public void Awake() {
        Instance = this;
        CancelButton.interactable = true;
        WaterCircle.color = InactiveColor_Circle;
        PeeCircle.color = InactiveColor_Circle;
        PooCircle.color = InactiveColor_Circle;
        WaterIcon.color = InactiveColor_Icon;
        PeeIcon.color = InactiveColor_Icon;
        PooIcon.color = InactiveColor_Icon;
        WaterButtonClicked = false;
        PeeButtonClicked = false;
        PooButtonClicked = false;
    }
    
    public void PooButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED2);
        PooButtonClicked = true;
        PooCircle.color = ActiveColor_Circle;
        PooIcon.color = ActiveColor_Icon;
        StartCoroutine(ButtonClickCheck());
    }

    public void WaterButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED2);
        WaterButtonClicked = true;
        WaterCircle.color = ActiveColor_Circle;
        WaterIcon.color = ActiveColor_Icon;
        StartCoroutine(ButtonClickCheck());
    }

    public void PeeButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED2);
        PeeButtonClicked = true;
        PeeCircle.color = ActiveColor_Circle;
        PeeIcon.color = ActiveColor_Icon;
        StartCoroutine(ButtonClickCheck());
    }

    public void BackButtonClick() {
        WaterCircle.color = InactiveColor_Circle;
        PeeCircle.color = InactiveColor_Circle;
        PooCircle.color = InactiveColor_Circle;
        WaterIcon.color = InactiveColor_Icon;
        PeeIcon.color = InactiveColor_Icon;
        PooIcon.color = InactiveColor_Icon;
        WaterButtonClicked = false;
        PeeButtonClicked = false;
        PooButtonClicked = false;
        TotalManager.instance.targetName = "";
        try {
            BluetoothLEHardwareInterface.DisconnectAll();
            BluetoothManager.GetInstance()
                    .SetState(BluetoothManager.States.Disconnect, 0.1f);
        }catch (System.Exception e) { e.ToString(); }

        Invoke("TurnOff", 0.15f);
    }

    public void TurnOff() {
        this.gameObject.SetActive(false);
    }

    private string temp = "";
    public IEnumerator ButtonClickCheck() {
        if(WaterButtonClicked && PeeButtonClicked && PooButtonClicked &&
            TotalManager.instance.targetName != "") {
            CancelButton.interactable = false;
            temp = TotalManager.instance.targetName;
            TotalManager.instance.isRegisterMode = false;
            TotalManager.instance.targetName = "";
            //try {
            //    BluetoothLEHardwareInterface.DisconnectAll();
            //    BluetoothManager.GetInstance()
            //            .SetState(BluetoothManager.States.Disconnect, 0.1f);
            //} catch (System.Exception e) { e.ToString(); }
            TotalManager.instance.isDebugMode = false;
            if (TotalManager.instance.BLECheckCoroutine != null) {
                try { 
                    StopCoroutine(TotalManager.instance.BLECheckCoroutine);
                } catch(System.Exception e) { e.ToString(); }
                try {
                    TotalManager.instance.BLECheckCoroutine = null;
                } catch (System.Exception e) { e.ToString(); }
            }
            yield return new WaitForSeconds(1f);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA1);
            FinalWindow.SetActive(true);

        }
        yield return 0;
    }

    public void FinalButton() {
        DataHandler.User_water_skip = "00:00";
        DataHandler.User_drink_skip = "00:00";
        DataHandler.User_poop_skip = "00:00";
        DataHandler.User_pee_skip = "00:00";
        DataHandler.User_font_family = "Bazzi";
        DataHandler.User_font_size = 40;
        DataHandler.User_creation_date = TimeHandler.GetCurrentTime();
        DataHandler.User_periode = 4;
        DataHandler.User_moa_band_name = temp;
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
        FinalWindow.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
