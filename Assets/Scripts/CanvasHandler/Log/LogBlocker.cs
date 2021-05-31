using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBlocker : MonoBehaviour
{
    public LogCanvasHandler log;
    public static LogBlocker Instance;
    public List<GameObject> Borders;
    public RectTransform Center;
    public RectTransform UpLeft;
    public RectTransform UpRight;
    public RectTransform DownLeft;
    public RectTransform DownRight;

    public RectTransform Up_WaterShield;
    public RectTransform Up_DrinkShield;
    public RectTransform Down_PooShield;
    public RectTransform Down_PeeShield;

    public GameObject DeleteButton;
    public GameObject ModifyeButton;

    private Vector2 WaterShieldOrigin;
    private Vector2 DrinkShieldOrigin;
    private Vector2 PooShieldOrigin;
    private Vector2 PeeShieldOrigin;

    private bool isPressLog = false;

    public void OnSideClick() {
        if(isPressLog) {
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
            isPressLog = false;
            BlockOff();
        }
    }

    public void Awake() {
        Instance = this;
        WaterShieldOrigin = Up_WaterShield.anchoredPosition;
        DrinkShieldOrigin = Up_DrinkShield.anchoredPosition;
        PooShieldOrigin = Down_PooShield.anchoredPosition;
        PeeShieldOrigin = Down_PeeShield.anchoredPosition;
    }

    public void OnEnable() {
        BlockOff();
    }

    public void OnDisable() {
        BlockOff();
    }

    public void BlockOn(bool isUp) {
        BlockOn(isUp,true,false);
    }

    public void BlockOnDetailUp(int index) {
        BlockOnDetailUp(index,true,false);
    }

    public void BlockOnDetailDown(int index) {
        BlockOnDetailDown(index, true, false) ;
    }

    public void BlockOnAll() {
        BlockOnAll(true,false);
    }

    public void BlockOff() {
        foreach (GameObject go in Borders)
            go.SetActive(false);

        UpLeft.sizeDelta = new Vector2(0,280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(0, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);


        UpLeft.gameObject.SetActive(false);
        UpRight.gameObject.SetActive(false);
        DownLeft.gameObject.SetActive(false);
        DownRight.gameObject.SetActive(false);
        Center.gameObject.SetActive(false);
        log.TimeLeftButton2.interactable = true;
        log.TimeRightButton2.interactable = true;
        DeleteButton.SetActive(false);
        ModifyeButton.SetActive(false);

        Up_WaterShield.gameObject.SetActive(false);
        Up_DrinkShield.gameObject.SetActive(false);
        Down_PeeShield.gameObject.SetActive(false);
        Down_PooShield.gameObject.SetActive(false);
    }

    public void BlockOn(bool isUp, bool isControlable, bool isModifyable) {
        log.TimeLeftButton2.interactable = true;
        log.TimeRightButton2.interactable = true;
        foreach (GameObject go in Borders)
            go.SetActive(true);
        if (!isControlable)
            for (int i = 4; i < 7; i++)
                Borders[i].SetActive(false);
        DeleteButton.SetActive(!isControlable);
        ModifyeButton.SetActive(isModifyable);
        isPressLog = !isControlable;
        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);

        Center.gameObject.SetActive(false);
        if (isUp) {
            UpLeft.sizeDelta = new Vector2(0, 280f);
            UpRight.sizeDelta = new Vector2(0, 280f);
            DownLeft.sizeDelta = new Vector2(1540, 280f);
            DownRight.sizeDelta = new Vector2(0, 280f);
        } else {
            UpLeft.sizeDelta = new Vector2(1540, 280f);
            UpRight.sizeDelta = new Vector2(0, 280f);
            DownLeft.sizeDelta = new Vector2(0, 280f);
            DownRight.sizeDelta = new Vector2(0, 280f);
        }
    }

    public void BlockOnDetailUp(int index, bool isControlable, bool isModifyable) {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        if (!isControlable)
            for (int i = 4; i < 7; i++)
                Borders[i].SetActive(false);
        DeleteButton.SetActive(!isControlable);
        ModifyeButton.SetActive(isModifyable);
        isPressLog = !isControlable;

        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);

        UpLeft.sizeDelta = new Vector2(130f * index, 280f);
        UpRight.sizeDelta = new Vector2(130f * (11-index), 280f);
        DownLeft.sizeDelta = new Vector2(1540, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }

    public void BlockOnDetailDown(int index, bool isControlable, bool isModifyable) {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        if (!isControlable)
            for (int i = 4; i < 7; i++)
                Borders[i].SetActive(false);
        DeleteButton.SetActive(!isControlable);
        ModifyeButton.SetActive(isModifyable);
        isPressLog = !isControlable;

        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);

        UpLeft.sizeDelta = new Vector2(1540, 280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(130f * index, 280f);
        DownRight.sizeDelta = new Vector2(130f * ( 11 - index ), 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }

    public void BlockOnAll(bool isControlable, bool isModifyable) {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        if (!isControlable)
            for (int i = 4; i < 7; i++)
                Borders[i].SetActive(false);
        DeleteButton.SetActive(!isControlable);
        ModifyeButton.SetActive(isModifyable);
        isPressLog = !isControlable;

        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);


        UpLeft.sizeDelta = new Vector2(1540, 280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(1540, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }

    public void BlockOnDetailWater(int index) {
        BlockOnDetailUp(index, false, true);
        RectTransform targetTransform = Up_WaterShield;
        targetTransform.gameObject.SetActive(true);
        targetTransform.anchoredPosition =
            new Vector2(WaterShieldOrigin.x + ( 130f * index ),
                        WaterShieldOrigin.y);
        Vector2 ButtonOffset = new Vector2(
            targetTransform.anchoredPosition.x + targetTransform.sizeDelta.x / 2,
            targetTransform.anchoredPosition.y - targetTransform.sizeDelta.y / 2
        );

        DeleteButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x - 87.5f, ButtonOffset.y);
        ModifyeButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x + 87.5f, ButtonOffset.y);
    }

    public void BlockOnDetailPee(int index) {
        BlockOnDetailDown(index, false, true);
        RectTransform targetTransform = Down_PeeShield;
        targetTransform.gameObject.SetActive(true);
        targetTransform.anchoredPosition =
            new Vector2(PeeShieldOrigin.x + ( 130f * index ),
                        PeeShieldOrigin.y);
        Vector2 ButtonOffset = new Vector2(
            targetTransform.anchoredPosition.x + targetTransform.sizeDelta.x / 2,
            targetTransform.anchoredPosition.y - targetTransform.sizeDelta.y / 2
        );

        DeleteButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x - 87.5f, ButtonOffset.y);
        ModifyeButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x + 87.5f, ButtonOffset.y);
    }

    public void BlockOnDetailPoo(int index) {
        BlockOnDetailDown(index, false, true);
        RectTransform targetTransform = Down_PooShield;
        targetTransform.gameObject.SetActive(true);
        targetTransform.anchoredPosition =
            new Vector2(PooShieldOrigin.x + ( 130f * index ),
                        PooShieldOrigin.y);
        Vector2 ButtonOffset = new Vector2(
            targetTransform.anchoredPosition.x + targetTransform.sizeDelta.x / 2,
            targetTransform.anchoredPosition.y - targetTransform.sizeDelta.y / 2
        );

        DeleteButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x - 87.5f, ButtonOffset.y);
        ModifyeButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x + 87.5f, ButtonOffset.y);
    }

    public void BlockOnDetailDrink(int index) {
        BlockOnDetailUp(index, false, true);
        RectTransform targetTransform = Up_DrinkShield;
        targetTransform.gameObject.SetActive(true);
        targetTransform.anchoredPosition =
            new Vector2(DrinkShieldOrigin.x + ( 130f * index ),
                        DrinkShieldOrigin.y);
        Vector2 ButtonOffset = new Vector2(
            targetTransform.anchoredPosition.x + targetTransform.sizeDelta.x / 2,
            targetTransform.anchoredPosition.y - targetTransform.sizeDelta.y / 2
        );


        DeleteButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x - 87.5f, ButtonOffset.y);
        ModifyeButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(ButtonOffset.x + 87.5f, ButtonOffset.y);
    }
}
