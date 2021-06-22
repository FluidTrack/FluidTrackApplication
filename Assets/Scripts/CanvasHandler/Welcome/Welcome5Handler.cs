using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome5Handler : MonoBehaviour
{
    public static Welcome5Handler Instance;
    internal bool isLocked = false;

    public Color BlueColor;
    public Color BlueColor2;
    public Color RedColor;
    public Color RedColor2;
    public Button ScanButton;
    public Text ScanButtonText;
    public GameObject Blind;
    public GameObject DeviceObjectPrefabs;
    public Transform ScrollView;
    public ScrollRect Scroll;
    public List<GameObject> DeviceList;

    public Animator RingAnim;
    public GameObject NoBandUI;
    public GameObject RegisterUI;

    public void Awake() {
        Instance = this;
    }

    public static Welcome5Handler GetInstance() {
        return Instance;
    }

    public void Start() {
        Instance = this;
        DeviceList = new List<GameObject>();
        TotalManager.instance.isRegisterMode = true;
    }

    public void BlindControl(bool value) {
        Blind.SetActive(value);
    }

    public void ScanButtonClick() {
        ColorBlock colorBlock = new ColorBlock();
        if (!isLocked) {
            colorBlock.normalColor = RedColor;
            colorBlock.highlightedColor = RedColor;
            colorBlock.pressedColor = RedColor2;
            colorBlock.selectedColor = RedColor;
            ScanButtonText.text = "스캔 종료";
            int listLength = DeviceList.Count;
            for (int i = 0; i < listLength; i++)
                Destroy(DeviceList[i]);
            DeviceList.Clear();

            BluetoothLEHardwareInterface.Initialize(true, false, () => {
                BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {
                    if (name.Contains("Touch")) {
                        DeviceList.Add(Instantiate(DeviceObjectPrefabs, ScrollView));
                        DeviceList[DeviceList.Count - 1].GetComponent<DeviceListHandler>()
                            .Init(DeviceList.Count - 1, name, address);

                        RectTransform view = ScrollView.gameObject.GetComponent<RectTransform>();
                        view.sizeDelta = new Vector2(1050f, 120f * DeviceList.Count);
                        Scroll.verticalNormalizedPosition = 0f;
                    }
                }, null);
            }, (error) => {
                Debug.LogError("BLE Error : " + error);
                BluetoothLEHardwareInterface.Log("BLE Error: " + error);
            });

        } else {
            colorBlock.normalColor = BlueColor;
            colorBlock.highlightedColor = BlueColor;
            colorBlock.pressedColor = BlueColor2;
            colorBlock.selectedColor = BlueColor;
            ScanButtonText.text = "모아밴드 스캔";
            RectTransform view = ScrollView.gameObject.GetComponent<RectTransform>();
            BluetoothLEHardwareInterface.StopScan();
        }
        colorBlock.colorMultiplier = 1;
        colorBlock.fadeDuration = 0.1f;
        ScanButton.colors = colorBlock;
        isLocked = !isLocked;
        RingAnim.SetBool("RingRing", isLocked);
    }

    public void NoBandButtonClick() {
        NoBandUI.SetActive(true);
    }
}
