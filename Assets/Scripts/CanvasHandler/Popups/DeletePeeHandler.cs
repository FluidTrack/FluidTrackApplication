using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePeeHandler : MonoBehaviour
{
    public static DeletePeeHandler Instance;
    public Text CountText;
    public Scrollbar slider;
    public Button MinusButton;
    public Button PlusButton;
    public Button OkayButton;
    public Button CancelButton;

    internal List<int> auto;
    internal List<int> noneauto;
    internal List<DataHandler.PeeLog> PeeLogs;

    private int currentNum = 1;
    private int totalNum = 1;

    public void Awake() {
        Instance = this;
    }

    public void Init() {
        OkayButton.interactable = true;
        CancelButton.interactable = true;
        currentNum = 1; totalNum = 1;
        PeeLogs = new List<DataHandler.PeeLog>();

        for (int i = 0; i < noneauto.Count; i++)
            foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs)
                if (log.log_id == noneauto[i]) {
                    PeeLogs.Add(log);
                    break;
                }

        for (int i = 0; i < auto.Count; i++)
            foreach (DataHandler.PeeLog log in DataHandler.Pee_logs.PeeLogs)
                if (log.log_id == auto[i]) {
                    PeeLogs.Add(log);
                    break;
                }
        totalNum = auto.Count + noneauto.Count;

        slider.value = (float)1 / (float)totalNum;
        MinusButton.interactable = false;
        if (totalNum == 1) { PlusButton.interactable = false; slider.interactable = false; }
        else { PlusButton.interactable = true; slider.interactable = true; }
        CountText.text = currentNum.ToString();
    }

    public void MinusButtonClick() {
        currentNum--;
        SetSliderValue(currentNum);
        CountText.text = currentNum.ToString();
        PlusButton.interactable = true;

        if (currentNum == 1)
            MinusButton.interactable = false;
    }

    public void PlusButtonClick() {
        currentNum++;
        SetSliderValue(currentNum);
        CountText.text = currentNum.ToString();
        MinusButton.interactable = true;

        if (currentNum == totalNum)
            PlusButton.interactable = false;
    }

    public void CancelButtonClick() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        this.gameObject.SetActive(false);
    }

    public void OkayButtonClick() {
        slider.interactable = false;
        MinusButton.interactable = false;
        PlusButton.interactable = false;
        OkayButton.interactable = false;
        CancelButton.interactable = false;
        List<DataHandler.PeeLog> DeletePeeLogs = new List<DataHandler.PeeLog>();

        for (int i = 0; i < currentNum; i++) {
            DeletePeeLogs.Add(PeeLogs[i]);
        }

        for (int i = 0; i < DeletePeeLogs.Count; i++)
            StartCoroutine(DataHandler.DeletePeeLogs(DeletePeeLogs[i].log_id));


        DataHandler.PeeLog[] array = new DataHandler.PeeLog[DataHandler.Pee_logs.PeeLogs.Length - currentNum];
        int index = 0;
        if(DataHandler.Pee_logs.PeeLogs.Length - currentNum > 0) {
            for (int i = 0; i < DataHandler.Pee_logs.PeeLogs.Length; i++) {
                bool contains = false;
                foreach (DataHandler.PeeLog log in DeletePeeLogs) {
                    if (DataHandler.Pee_logs.PeeLogs[i].log_id == log.log_id) {
                        contains = true;
                        break;
                    }
                }

                if (contains) continue;

                array[index++] = DataHandler.Pee_logs.PeeLogs[i];
            }
        }


        DataHandler.GardenLog TargetGardenLog = LogCanvasHandler.Instance.TargetGardenLog;
        for (int i = 0; i < currentNum; i++) {
            if (TargetGardenLog.log_pee > 0) TargetGardenLog.log_pee--;
            if (TargetGardenLog.log_pee == 0) TargetGardenLog.item_0 = 0;
        }
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        DataHandler.Pee_logs.PeeLogs = array;
        StartCoroutine(DeleteLogs());
    }

    public IEnumerator DeleteLogs() {
        yield return new WaitForSeconds(0.3f);
        LogCanvasHandler.Instance.Fetching();
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        this.gameObject.SetActive(false);
    }

    public void SetSliderValue(int value) {
        float new_value = ( (float)value / (float)totalNum );
        slider.value = new_value;
    }

    public void OnSliderValueChange() {
        int max = totalNum;
        int new_currentNum = Mathf.RoundToInt(max * slider.value);
        currentNum = new_currentNum;
        if(new_currentNum == 0) {
            currentNum = 1;
            slider.value = (float)1 / (float)totalNum;
        }
        CountText.text = currentNum.ToString();

        MinusButton.interactable = ( currentNum != 1 );
        PlusButton.interactable = ( currentNum != totalNum );
    }
}
