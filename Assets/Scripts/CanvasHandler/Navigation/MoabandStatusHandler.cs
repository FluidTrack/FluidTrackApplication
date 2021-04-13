using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoabandStatusHandler : MonoBehaviour
{
    public Image Icon;
    public bool isConnected = false;
    public int value;

    public GameObject DisconnectedText;
    public GameObject ConnectedText;
    public GameObject ConnectionTimeText;

    public Sprite Moaband_Disconnected;
    public Sprite Moaband_GoodBattery;
    public Sprite Moaband_WarningBattery;
    public Sprite Moaband_BadBattery;
    public Color Moaband_DisconnectedColor;
    public Color Moaband_GoodBatteryColor;
    public Color Moaband_WarningBatteryColor;
    public Color Moaband_BadBatteryColor;
    public int Boundary_1 = 60;
    public int Boundary_2 = 20;
    // Update is called once per frame
    void Update() {
        if(isConnected) {
            if(value > Boundary_1) {
                Icon.color = Moaband_GoodBatteryColor;
                Icon.sprite = Moaband_GoodBattery; 
            } else if (value > Boundary_2) {
                Icon.color = Moaband_WarningBatteryColor;
                Icon.sprite = Moaband_WarningBattery;
            } else {
                Icon.color = Moaband_BadBatteryColor;
                Icon.sprite = Moaband_BadBattery; 
            }
            ConnectedText.SetActive(true);
            ConnectionTimeText.SetActive(true);
            DisconnectedText.SetActive(false);
        } else {
            ConnectedText.SetActive(false);
            ConnectionTimeText.SetActive(false);
            DisconnectedText.SetActive(true);
            Icon.color = Moaband_DisconnectedColor;
            Icon.sprite = Moaband_Disconnected;
        }
    }

    public void ConnectButtonClikc() {
        TimeHandler.GetCurrentTime();
        isConnected = true;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        ConnectionTimeText.GetComponent<Text>().text
            = TimeHandler.HomeCanvasTime.ToString();
    }
}
