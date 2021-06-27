using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MoabandReconnection : MonoBehaviour
{
    public GameObject Parents;
    public List<GameObject> DeviceList;
    public GameObject NormalPrefab;
    public GameObject WatchPrefab;
    public RectTransform ScrollView;
    public ScrollRect Scroll;
    public Button CancelButton;
    public GameObject ConnectError;
    public bool isScanning = false;

    public void OnEnable() {
        DeviceList = new List<GameObject>();
        CancelButton.interactable = true;
        if (DataHandler.User_moa_band_name == null) {
            StartCoroutine(readUsers());
        } else {
            ScanBand();
        }
    }

    IEnumerator readUsers() {
        yield return new WaitForSeconds(1f);
        if(DataHandler.User_moa_band_name == null) {
            try {
                FileStream fs = new FileStream(DataHandler.dataPath + "/userData", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                DataHandler.User_id = int.Parse(sr.ReadLine());
                sr.Close();
                fs.Close();
            } catch (System.Exception e) {
                e.ToString();
                Debug.LogWarning("Cannot found userData\nDataHandler.User_id set 1 as default value.");
                DataHandler.User_id = 1;
            }
            StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
            StartCoroutine(WaitFetchBandName());
        } else ScanBand();
        yield return 0;
    }

    Coroutine checkName = null;
    Coroutine checkList = null;
    bool isFindDevice = false;
    private BluetoothManager BT;
    private string address_;

    IEnumerator checkingName() {
        while(true) {
            if(isFindDevice) {
                yield return new WaitForSeconds(0.5f);
                try {
                    BluetoothLEHardwareInterface.StopScan();
                } catch (System.Exception e) { e.ToString(); }

                isFindDevice = false;
                CancelButton.interactable = false;
                TotalManager.instance.targetName = DataHandler.User_moa_band_name;
                BT = GameObject.Find("[SYSTEM] Total Manager").GetComponent<BluetoothManager>();
                BT.OnConnectStart(DataHandler.User_moa_band_name, address_,
                    "6e400001-b5a3-f393-e0a9-e50e24dcca9e",
                    "6e400002-b5a3-f393-e0a9-e50e24dcca9e",
                    "6e400003-b5a3-f393-e0a9-e50e24dcca9e");

                StartCoroutine(checkingConnect());
                while(!BluetoothManager.GetInstance().isConnected)
                    yield return 0;
                CancelButtonClick();
                break;
            } 
            yield return 0;
        }
        yield return 0;
    }

    IEnumerator checkingConnect() {
        yield return new WaitForSeconds(10f);
        if(!BluetoothManager.GetInstance().isConnected) {
            ConnectError.SetActive(true);
            BluetoothManager.GetInstance().isReconnectEnable = true;
            CancelButtonClick();
        }
    }

    IEnumerator checkingList() {
        yield return new WaitForSeconds(1.5f);
        if(DeviceList.Count == 0) {
            try {
                BluetoothLEHardwareInterface.StopScan();
            } catch (System.Exception e) { e.ToString(); }
            isScanning = true;
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(checkingList());
            BluetoothLEHardwareInterface.Initialize(true, false, () => {
                BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                    if (name.Contains("Touch")) {
                        DeviceList.Add(Instantiate(WatchPrefab, ScrollView));
                        DeviceList[DeviceList.Count - 1].GetComponent<DeviceLog2>()
                            .Init(DeviceList.Count - 1, name, address, true);
                        if (name == DataHandler.User_moa_band_name) {
                            address_ = address;
                            isFindDevice = true;
                        }
                    } else {
                        DeviceList.Add(Instantiate(NormalPrefab, ScrollView));
                        DeviceList[DeviceList.Count - 1].GetComponent<DeviceLog2>()
                            .Init(DeviceList.Count - 1, name, address, false);
                    }
                    ScrollView.sizeDelta = new Vector2(377.22f, 80f * DeviceList.Count);
                    Scroll.verticalNormalizedPosition = 0f;
                }, null);
            }, (error) => {
                Debug.LogError("BLE Error : " + error);
                BluetoothLEHardwareInterface.Log("BLE Error: " + error);
            });
        }
        yield return 0;
    }


    public void ScanBand() {
        try {
            BluetoothLEHardwareInterface.StopScan();
        } catch (System.Exception e) { e.ToString(); }

        isScanning = true;
        checkName = StartCoroutine(checkingName());
        StartCoroutine(checkingList());
        BluetoothLEHardwareInterface.Initialize(true, false, () => {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                if (name.Contains("Touch")) {
                    DeviceList.Add(Instantiate(WatchPrefab, ScrollView));
                    DeviceList[DeviceList.Count - 1].GetComponent<DeviceLog2>()
                        .Init(DeviceList.Count - 1, name, address,true);
                    if(name == DataHandler.User_moa_band_name) {
                        address_ = address;
                        isFindDevice = true;
                    }
                } else {
                    DeviceList.Add(Instantiate(NormalPrefab, ScrollView));
                    DeviceList[DeviceList.Count - 1].GetComponent<DeviceLog2>()
                        .Init(DeviceList.Count - 1, name, address, false);
                }
                ScrollView.sizeDelta = new Vector2(377.22f, 80f * DeviceList.Count);
                Scroll.verticalNormalizedPosition = 0f;
            }, null);
        }, (error) => {
            Debug.LogError("BLE Error : " + error);
            BluetoothLEHardwareInterface.Log("BLE Error: " + error);
        });
    }

    public void CancelButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        if (isScanning) {
            try {
                BluetoothLEHardwareInterface.StopScan();
            } catch (System.Exception e) { e.ToString(); }
            isFindDevice = false;
        }
        try {
            int listLength = DeviceList.Count;
            for (int i = 0; i < listLength; i++)
                Destroy(DeviceList[i]);
            DeviceList.Clear();
        } catch (System.Exception e) {
            e.ToString();
        }
        if (checkName != null)
            StopCoroutine(checkName);
        BluetoothManager.GetInstance().AutoConnect = true;
        if(TotalManager.instance.BLECheckCoroutine == null) {
            TotalManager.instance.BLECheckCoroutine
                = StartCoroutine(TotalManager.instance.BLE_Check());
        }
        Parents.gameObject.SetActive(false);
    }

    public IEnumerator WaitFetchBandName() {

        while (DataHandler.User_moa_band_name == null)
            yield return 0;
        DataHandler.User_isDataLoaded = false;
        ScanBand();
     
        yield return 0;
    }
}
