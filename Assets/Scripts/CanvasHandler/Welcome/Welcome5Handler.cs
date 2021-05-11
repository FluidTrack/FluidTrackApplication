using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Welcome5Handler : MonoBehaviour
{
    private static Welcome5Handler Instance;
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
            ScanButtonText.text = "모아밴드 시작";
            RectTransform view = ScrollView.gameObject.GetComponent<RectTransform>();
            BluetoothLEHardwareInterface.StopScan();
        }
        colorBlock.colorMultiplier = 1;
        colorBlock.fadeDuration = 0.1f;
        ScanButton.colors = colorBlock;
        isLocked = !isLocked;
        RingAnim.SetBool("RingRing", isLocked);
    }


    public void OkayButton() {
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
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FOOTER_BAR].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].SetActive(true);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.NAVI_BAR].SetActive(true);
        Instantiate(TotalManager.instance.FlashEffect);
        GreetingMongMong.Instance.SayHello();
        this.gameObject.SetActive(false);
    }

}
