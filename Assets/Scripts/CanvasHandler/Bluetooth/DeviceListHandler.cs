using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceListHandler : MonoBehaviour {
    private BluetoothManager BT;
    public Text DeviceName;
    public Text MacAddress;
    private string deviceString;

    public void OnEnable() {
        BT = GameObject.Find("[SYSTEM] Total Manager").GetComponent<BluetoothManager>();
    }

    public void Init(int index, string deviceName, string macAddress) {
        this.GetComponent<RectTransform>().transform.localPosition = new Vector2(0f, -120f * index);
        MacAddress.text = macAddress;
        try {
            DeviceName.text = "모아밴드 _ " + deviceName.Split('_')[1];
            this.deviceString = deviceName;
        } catch (System.Exception e) {
            e.ToString();
            DeviceName.text = "모아밴드 _ ??";
        }
    }

    public void OnClickConnectButton() {
        if (Welcome5Handler.GetInstance().isLocked)
            Welcome5Handler.GetInstance().ScanButtonClick();
        else BluetoothLEHardwareInterface.StopScan();
        TotalManager.instance.targetName = deviceString;
        BT.OnConnectStart(deviceString, "", "6e400001-b5a3-f393-e0a9-e50e24dcca9e", "6e400002-b5a3-f393-e0a9-e50e24dcca9e", "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
    }
}
