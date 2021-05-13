using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoabandStatusHandler : MonoBehaviour
{
    public static MoabandStatusHandler Instance;
    public Image Icon;
    public bool isConnected = false;
    public int value;

    public Text statusText;

    public Sprite Moaband_Disconnected;
    public Sprite Moaband_FullBattery;
    public Sprite Moaband_GoodBattery;
    public Sprite Moaband_WarningBattery;
    public Sprite Moaband_BadBattery;
    public int Boundary_0 = 89;
    public int Boundary_1 = 60;
    public int Boundary_2 = 20;
    // Update is called once per frame

    public void Awake() {
        Instance = this;
    }

    public void Start() {
        if (BluetoothManager.GetInstance()._connected) {
            ConnectTimeRefresh();
        }
    }

    void Update() {
        isConnected = BluetoothManager.GetInstance()._connected;
        if(isConnected) {
            if(value > Boundary_0)       Icon.sprite = Moaband_FullBattery; 
            else if (value > Boundary_1) Icon.sprite = Moaband_GoodBattery;
            else if (value > Boundary_2) Icon.sprite = Moaband_WarningBattery;
            else                         Icon.sprite = Moaband_BadBattery; 
        } else {
            statusText.text = "모아밴드가 연결되지 않음";
            Icon.sprite = Moaband_Disconnected;
        }
    }

    public void ConnectTimeRefresh() {
        TimeHandler.GetCurrentTime();
        isConnected = true;
        statusText.GetComponent<Text>().text = MakingConnectTime();
    }

    public string MakingConnectTime() {
        string str = "마지막 연결 시간 : " +
              TimeHandler.HomeCanvasTime.Months + "월 " +
              TimeHandler.HomeCanvasTime.Days + "일 ";
        bool isOver = false; int hour = TimeHandler.HomeCanvasTime.Hours;
        if (TimeHandler.HomeCanvasTime.Hours >= 12) { isOver = true; hour -= 12; }
        hour = ( hour == 0 ) ? 12 : hour;
        str += ( isOver ) ? "오후 " : "오전 ";
        str += hour + "시 " + TimeHandler.HomeCanvasTime.Minutes + "분";
        return str;
    }
}
