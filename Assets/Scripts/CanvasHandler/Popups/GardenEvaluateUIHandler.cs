using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenEvaluateUIHandler : MonoBehaviour
{
    public Text WindowText;
    public LogCanvasHandler Log;
    public Sprite NoneCheck;
    public Sprite Check;
    public Image[] Buttons;
    public Button OkayButton;
    public Button CancelButton;
    public GameObject ThanksUI;

    public int SelectNum = 0;
    public int InitSelect = 0;

    public void OnEnable() {
        OkayButton.interactable = false;
        CancelButton.interactable = true;
        if (Log.TargetGardenLog == null) this.gameObject.SetActive(false);

        SelectNum = Log.TargetGardenLog.item_4;
        InitSelect = SelectNum;
        if (SelectNum > 8 || SelectNum < 0) SelectNum = 0;

        for(int i = 0; i < 8; i++) 
           Buttons[i].sprite = ( SelectNum - 1 == i ) ? Check : NoneCheck;

        TimeHandler.DateTimeStamp targetStamp
            = new TimeHandler.DateTimeStamp(Log.TargetGardenLog.timestamp);

        WindowText.text = targetStamp.Months + "/" + targetStamp.Days + "의 데이터가 얼마나 정확한가요?";
    }

    public void ScaleButtonClick(int num) {
        if (SelectNum == num) return;
        OkayButton.interactable = InitSelect != num;
        SelectNum = num;
        for (int i = 0; i < 8; i++)
            Buttons[i].sprite = ( SelectNum - 1 == i ) ? Check : NoneCheck;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED3);
    }

    public void OkayButtonClick() {
        OkayButton.interactable = false;
        CancelButton.interactable = false;
        Log.TargetGardenLog.item_4 = SelectNum;
        StartCoroutine(DataHandler.UpdateGardenLogs(Log.TargetGardenLog));
        StartCoroutine(Done());
    }

    public IEnumerator Done() {
        yield return new WaitForSeconds(0.2f);
        ThanksUI.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
