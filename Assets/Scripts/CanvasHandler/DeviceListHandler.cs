using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceListHandler : MonoBehaviour {
    private BluetoothManager BT;
    public Text DeviceName;
    public Text MacAddress;

    public void OnEnable() {
        BT = GameObject.Find("Total Manager").GetComponent<BluetoothManager>();
    }

    public void Init(int index, string deviceName, string macAddress) {
        this.GetComponent<RectTransform>().transform.localPosition = new Vector2(0f, -180f * index);
        MacAddress.text = macAddress;
        DeviceName.text = deviceName;
    }

    public void OnClickConnectButton() {
        if (ScanPanelHandler.GetInstance().isLocked)
            ScanPanelHandler.GetInstance().ScanButtonClick();
        else BluetoothLEHardwareInterface.StopScan();
        BT.OnConnectStart(DeviceName.text, "", "6e400001-b5a3-f393-e0a9-e50e24dcca9e", "6e400002-b5a3-f393-e0a9-e50e24dcca9e", "6e400003-b5a3-f393-e0a9-e50e24dcca9e");
    }
}
