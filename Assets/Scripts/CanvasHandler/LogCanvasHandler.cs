using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCanvasHandler : MonoBehaviour
{
    public ScrollViewHandler scroll;
    public ScrollViewHandler2 scroll2;
    public Sprite BrightArrow;
    public Sprite Arrow;
    public Image LeftButton;
    public Image RightButton;
    public GameObject WaterButton;
    public GameObject PeeButton;
    public GameObject PooButton;
    public GameObject DrinkButton;
    public Color ActiveColor;
    public Color InactiveColor;
    public bool WaterButtonClicked = false;
    public bool PooButtonClicked = false;
    public bool PeeButtonClicked = false;
    public bool DrinkButtonClicked = false;

    public Text LogCanvasTimeText;
    public GameObject UpHighlight;
    public GameObject DownHighlight;

    public void OnEnable() {
        StartCoroutine(ArrowCheckThread());
    }

    IEnumerator ArrowCheckThread() {
        while (true) {
            if(TimeHandler.CreationTime != null &&
               TimeHandler.LogCanvasTime != null &&
               TimeHandler.CurrentTime != null ) {
                LogCanvasTimeText.text = TimeHandler.LogCanvasTime.Months + "월 " +
                                         TimeHandler.LogCanvasTime.Days + "일 " +
                                         TimeHandler.DateTimeStamp.DateList[(int)TimeHandler.LogCanvasTime.Date];
                ArrowCheck();
                break;
            }
            yield return 0;
        }
    }

    public void LeftButtonClick() {
        TimeHandler.LogCanvasTime -= 1;
        if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CreationTime,TimeHandler.LogCanvasTime) == 1 ) {
            TimeHandler.LogCanvasTime += 1;
            return;
        }
        LogCanvasTimeText.text = TimeHandler.LogCanvasTime.Months + "월 " +
                         TimeHandler.LogCanvasTime.Days + "일 " +
                         TimeHandler.DateTimeStamp.DateList[(int)TimeHandler.LogCanvasTime.Date];
        scroll.OnDisable();
        scroll2.OnDisable();
        StartCoroutine(scroll.FetchData());
        StartCoroutine(scroll2.FetchData());
        ArrowCheck();
    }

    public void RightButtonClick() {
        TimeHandler.LogCanvasTime += 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, TimeHandler.LogCanvasTime) == -1) {
            TimeHandler.LogCanvasTime -= 1;
            return;
        }
        LogCanvasTimeText.text = TimeHandler.LogCanvasTime.Months + "월 " +
                         TimeHandler.LogCanvasTime.Days + "일 " +
                         TimeHandler.DateTimeStamp.DateList[(int)TimeHandler.LogCanvasTime.Date];
        scroll.OnDisable();
        scroll2.OnDisable();
        StartCoroutine(scroll.FetchData());
        StartCoroutine(scroll2.FetchData());
        ArrowCheck();
    }

    public void ArrowCheck() {
        TimeHandler.LogCanvasTime -= 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CreationTime, TimeHandler.LogCanvasTime) == 1)
            LeftButton.sprite = BrightArrow;
        else LeftButton.sprite = Arrow;
        TimeHandler.LogCanvasTime += 1;

        TimeHandler.LogCanvasTime += 1;
        if (TimeHandler.DateTimeStamp.CmpDateTimeStamp(TimeHandler.CurrentTime, TimeHandler.LogCanvasTime) == -1)
            RightButton.sprite = BrightArrow;
        else RightButton.sprite = Arrow;
        TimeHandler.LogCanvasTime -= 1;
    }

    public void OnWaterButtonClick() {
        WaterButtonClicked = !WaterButtonClicked;
        PeeButton.GetComponent<Image>().color = ( WaterButtonClicked ) ? InactiveColor : ActiveColor;
        PooButton.GetComponent<Image>().color = ( WaterButtonClicked ) ? InactiveColor : ActiveColor;
        DrinkButton.GetComponent<Image>().color = ( WaterButtonClicked ) ? InactiveColor : ActiveColor;
        PeeButton.GetComponent<Button>().interactable = !WaterButtonClicked;
        PooButton.GetComponent<Button>().interactable = !WaterButtonClicked;
        DrinkButton.GetComponent<Button>().interactable = !WaterButtonClicked;
        DownHighlight.SetActive(WaterButtonClicked);
    }

    public void OnPeeButtonClick() {
        PeeButtonClicked = !PeeButtonClicked;
        WaterButton.GetComponent<Image>().color = ( PeeButtonClicked ) ? InactiveColor : ActiveColor;
        PooButton.GetComponent<Image>().color = ( PeeButtonClicked ) ? InactiveColor : ActiveColor;
        DrinkButton.GetComponent<Image>().color = ( PeeButtonClicked ) ? InactiveColor : ActiveColor;
        WaterButton.GetComponent<Button>().interactable = !PeeButtonClicked;
        PooButton.GetComponent<Button>().interactable = !PeeButtonClicked;
        DrinkButton.GetComponent<Button>().interactable = !PeeButtonClicked;
        DownHighlight.SetActive(PeeButtonClicked);
    }

    public void OnPooButtonClick() {
        PooButtonClicked = !PooButtonClicked;
        PeeButton.GetComponent<Image>().color = ( PooButtonClicked ) ? InactiveColor : ActiveColor;
        WaterButton.GetComponent<Image>().color = ( PooButtonClicked ) ? InactiveColor : ActiveColor;
        DrinkButton.GetComponent<Image>().color = ( PooButtonClicked ) ? InactiveColor : ActiveColor;
        PeeButton.GetComponent<Button>().interactable = !PooButtonClicked;
        WaterButton.GetComponent<Button>().interactable = !PooButtonClicked;
        DrinkButton.GetComponent<Button>().interactable = !PooButtonClicked;
        UpHighlight.SetActive(PooButtonClicked);
    }

    public void OnDrinkButtonClick() {
        DrinkButtonClicked = !DrinkButtonClicked;
        PeeButton.GetComponent<Image>().color = ( DrinkButtonClicked ) ? InactiveColor : ActiveColor;
        WaterButton.GetComponent<Image>().color = ( DrinkButtonClicked ) ? InactiveColor : ActiveColor;
        PooButton.GetComponent<Image>().color = ( DrinkButtonClicked ) ? InactiveColor : ActiveColor;
        PeeButton.GetComponent<Button>().interactable = !DrinkButtonClicked;
        WaterButton.GetComponent<Button>().interactable = !DrinkButtonClicked;
        PooButton.GetComponent<Button>().interactable = !DrinkButtonClicked;
        DownHighlight.SetActive(DrinkButtonClicked);
    }
}
