using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyPooLogHandler : MonoBehaviour
{
    public static DataHandler.PoopLog Target;
    public Text TitleText;
    public Button OkayButton;
    public bool isClicked;
    public Image[] Images;

    private int tempType = 0;

    public void ClickTypeButton() {
        isClicked = true;
    }

    public void OnEnable() {
        OkayButton.interactable = false;
        int showTime = new TimeHandler.DateTimeStamp(Target.timestamp).Hours;
        string str = "변경할 새벽";
        if (showTime >= 6 && showTime <= 11) str = "변경할 아침";
        else if (showTime >= 12 && showTime <= 14) str = "변경할 점심";
        else if (showTime >= 15 && showTime <= 17) str = "변경할 낮";
        else if (showTime >= 18 && showTime <= 20) str = "변경할 저녁";
        else if (showTime >= 21) str = "변경할 밤";
        if (showTime >= 12)
            showTime -= 12;
        showTime = ( showTime == 0 ) ? 12 : showTime;
        str += " " + showTime + "시 대변의 모양을 골라주세요";
        TitleText.text = str;

        tempType = Target.type;
        for (int i = 0; i < 8; i++) {
            if (tempType == i) Images[i].color = new Color(1, 1, 1, 1);
            else Images[i].color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void IconClick(int index) {
        if (index == tempType) return;
        tempType = index;
        for(int i = 0; i < 8; i++) {
            if (tempType == i) Images[i].color = new Color(1, 1, 1, 1);
            else Images[i].color = new Color(1, 1, 1, 0.3f);
        }
        OkayButton.interactable = true;
    }

    public void OnDisable() {
        isClicked = false;
    }

    public void UpdateLog() {
        LogCanvasHandler.Instance.WaterButton.GetComponent<Button>().enabled = false;
        LogCanvasHandler.Instance.PeeButton.GetComponent<Button>().enabled = false;
        LogCanvasHandler.Instance.PooButton.GetComponent<Button>().enabled = false;
        LogCanvasHandler.Instance.DrinkButton.GetComponent<Button>().enabled = false;
        LogCanvasHandler.Instance.WaterButton2.interactable = false;
        LogCanvasHandler.Instance.PeeButton2.interactable = false;
        LogCanvasHandler.Instance.PooButton2.interactable = false;
        LogCanvasHandler.Instance.DrinkButton2.interactable = false;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        LogCanvasHandler.Instance.modifyingTargetId =  Target.log_id;
        LogCanvasHandler.Instance.isModifyingPoo = true;
        LogBlocker.Instance.DisableDetailShield();
        LogCanvasHandler.Instance.TimeLineModifyInit(false);
        this.gameObject.SetActive(false);
    }

    public void CloseButtonClick() {
        LogCanvasHandler.Instance.isModifying = false;
        LogCanvasHandler.Instance.isModifyingWater = false;
        LogCanvasHandler.Instance.isModifyingDrink = false;
        LogCanvasHandler.Instance.isModifyingPoo = false;
        LogCanvasHandler.Instance.isModifyingPee = false;
        LogCanvasHandler.Instance.WaterButton.GetComponent<Button>().enabled = true;
        LogCanvasHandler.Instance.PeeButton.GetComponent<Button>().enabled = true;
        LogCanvasHandler.Instance.PooButton.GetComponent<Button>().enabled = true;
        LogCanvasHandler.Instance.DrinkButton.GetComponent<Button>().enabled = true;
        LogCanvasHandler.Instance.WaterButton2.interactable = true;
        LogCanvasHandler.Instance.PeeButton2.interactable = true;
        LogCanvasHandler.Instance.PooButton2.interactable = true;
        LogCanvasHandler.Instance.DrinkButton2.interactable = true;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        this.gameObject.SetActive(false);
    }

    public void OkayButtonClick() {
        Target.type = tempType;
        StartCoroutine(DataHandler.UpdatePoopLogs(Target));
        StartCoroutine(CheckUpdate());
    }

    IEnumerator CheckUpdate() {
        while (!DataHandler.User_isPooDataUpdated)
            yield return 0;
        DataHandler.User_isPooDataUpdated = false;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED2);
        LogCanvasHandler.Instance.Fetching();
        this.gameObject.SetActive(false);
    }
}
