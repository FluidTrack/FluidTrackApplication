using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteWaterHandler : MonoBehaviour
{
    public static DeleteWaterHandler Instance;
    public Text CountText;
    public Scrollbar slider;
    public Button MinusButton;
    public Button PlusButton;
    public Button OkayButton;
    public Button CancelButton;
    

    internal List<int> auto;
    internal List<int> noneauto;
    internal List<DataHandler.WaterLog> WaterLogs;

    private int currentNum = 1;
    private int totalNum = 1;
    private DataHandler.GardenLog gardenLog;

    public void Awake() {
        Instance = this;
    }

    public void Init(DataHandler.GardenLog gardenLog) {
        this.gardenLog = gardenLog;
        OkayButton.interactable = true;
        CancelButton.interactable = true;
        currentNum = 1; totalNum = 1;
        slider.value = 0;
        WaterLogs = new List<DataHandler.WaterLog>();

        for (int i = 0; i < noneauto.Count; i++)
            foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
                if (log.log_id == noneauto[i]) {
                    WaterLogs.Add(log);
                    break;
                }

        for (int i = 0; i < auto.Count; i++)
            foreach (DataHandler.WaterLog log in DataHandler.Water_logs.WaterLogs)
                if (log.log_id == auto[i]) {
                    WaterLogs.Add(log);
                    break;
                }
        totalNum = auto.Count + noneauto.Count;
        
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

        List<DataHandler.WaterLog> DeleteWaterLogs = new List<DataHandler.WaterLog>();

        for(int i = 0; i < currentNum; i++) {
            DeleteWaterLogs.Add(WaterLogs[i]);
        }

        for (int i = 0; i < DeleteWaterLogs.Count; i++)
            StartCoroutine(DataHandler.DeleteWaterLogs(DeleteWaterLogs[i].log_id));

        DataHandler.WaterLog[] array = new DataHandler.WaterLog[DataHandler.Water_logs.WaterLogs.Length - currentNum];
        int index = 0;
        if(DataHandler.Water_logs.WaterLogs.Length - currentNum > 0) {
            for (int i = 0; i < DataHandler.Water_logs.WaterLogs.Length; i++) {
                bool contains = false;
                foreach (DataHandler.WaterLog log in DeleteWaterLogs) {
                    if (DataHandler.Water_logs.WaterLogs[i].log_id == log.log_id) {
                        contains = true;
                        break;
                    }
                }

                if (contains) continue;

                array[index++] = DataHandler.Water_logs.WaterLogs[i];
            }
        } 
        DataHandler.Water_logs.WaterLogs = array;

        for(int i = 0; i < currentNum; i ++) {
            if (gardenLog.log_pee > 0) gardenLog.log_pee--;
            if (gardenLog.log_pee == 0) gardenLog.item_0 = 0;
        }

        index = 0;
        for(int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if(DataHandler.Garden_logs.GardenLogs[i].log_id == gardenLog.log_id) {
                index = i;
                break;
            }
        }

        StartCoroutine(DataHandler.UpdateGardenLogs(DataHandler.Garden_logs.GardenLogs[index]));
        StartCoroutine(DeleteLogs());
    }

    public IEnumerator DeleteLogs() {
        yield return new WaitForSeconds(0.3f);
        LogCanvasHandler.Instance.Fetching();
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        this.gameObject.SetActive(false);
    }

    public void SetSliderValue(int value) {
        int max = totalNum-1;
        float new_value = ( ((float)value-1) / (float)max );
        slider.value = new_value;
    }

    public void OnSliderValueChange() {
        int max = totalNum - 1;
        int new_currentNum = Mathf.RoundToInt(max * slider.value);
        currentNum = new_currentNum + 1;
        CountText.text = currentNum.ToString();

        MinusButton.interactable = ( currentNum != 1 );
        PlusButton.interactable = ( currentNum != totalNum );
    }
}
