using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPageHandler : MonoBehaviour
{
    public static DataHandler.GardenLog CurrentLog;
    public static FlowerPageHandler Instance;
    public FlowerPageSpotHandler SpotHandler;
    public GameObject WaterPrefab;
    public Transform WaterSlot;
    public Transform EffectSpawnZone;
    public GameObject Flash;
    public GameObject Ring;
    public List<GameObject> WaterIcons;
    public RectTransform TargetZone;

    public string DateString;
    public static DataHandler.GardenLog TargetGardenLog;

    public void Awake() {
        Instance = this;
    }

    public void OnEnable() {
        EffectSpawnZone.gameObject.SetActive(false);
        if (TimeHandler.LogCanvasTime == null)
            TimeHandler.GetCurrentTime();
        DateString = TimeHandler.LogCanvasTime.ToDateString();
        
        StartCoroutine(FetchUser());
    }

    public void OnDisable() {
        for(int i = 0; i < WaterIcons.Count; i++) {
            GameObject temp = WaterIcons[i];
            WaterIcons[i] = null;
            Destroy(temp);
        }
        WaterIcons.Clear();
        EffectSpawnZone.gameObject.SetActive(false);
    }

    public IEnumerator FetchUser() {
        while (!DataHandler.User_isDataLoaded)
            yield return 0;
        DataHandler.User_isDataLoaded = false;
        StartCoroutine(DataHandler.ReadGardenLogs(DataHandler.User_id));

        int index = 0;
        TimeHandler.DateTimeStamp indexTime =
            new TimeHandler.DateTimeStamp(DataHandler.User_creation_date);
        for(int i = 0; i < 56; i ++) {
            if(TimeHandler.DateTimeStamp.CmpDateTimeStamp(
                indexTime, new TimeHandler.DateTimeStamp(DateString)) >= 0) {
                break;
            }
            indexTime = indexTime + 1;
            index++;
        }
        SpotHandler.Step = ( index / 7 );
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData() {
        while(!DataHandler.User_isGardenDataLoaded)
            yield return 0;
        DataHandler.User_isGardenDataLoaded = false;

        TargetGardenLog = null;
        for(int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i ++) {
            if(TimeHandler.DateTimeStamp.CmpDateTimeStamp( DataHandler.Garden_logs.GardenLogs[i].timestamp, DateString ) == 0 ){
                TargetGardenLog = DataHandler.Garden_logs.GardenLogs[i];
                break;
            }
        }
        if(TargetGardenLog == null) {
            DataHandler.GardenLog newGarden = new DataHandler.GardenLog();
            newGarden.id = DataHandler.User_id;
            newGarden.timestamp = TimeHandler.LogCanvasTime.ToString();
            newGarden.flower = 0;
            newGarden.log_water = 0; newGarden.log_poop = 0; newGarden.log_pee = 0;
            newGarden.item_0 = 0; newGarden.item_1 = 0; newGarden.item_2 = 0; newGarden.item_3 = 0; newGarden.item_4 = 0;
            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            TargetGardenLog = newGarden;
        }

        SpotHandler.InitSpot(TargetGardenLog);

        int waterIconCount = ( TargetGardenLog.log_water >= 10 ) ? 10 : TargetGardenLog.log_water;
        WaterSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(( waterIconCount * 90f ) + ( waterIconCount - 1 ) * 80, 131f);
        for(int i = 0; i < waterIconCount; i++) {
            GameObject go = Instantiate(WaterPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2((i * 170),0);
            go.GetComponent<DraggableWaterIcon>().SetinitPos();
            WaterIcons.Add(go);
        }
    }

    public void BackButtonClicked() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.BACK);
        TotalManager.instance.TargetDateString = DateString;
        FooterBarHandler.Instance.FooterButtonClick(1);
    }

    public void Watering() {
        EffectSpawnZone.gameObject.SetActive(true);
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
        Instantiate(Flash, EffectSpawnZone);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA3);
        StartCoroutine(ReDrawSpot());
    }

    public void SpawnEffect() {
        EffectSpawnZone.gameObject.SetActive(true);
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
        Instantiate(Flash, EffectSpawnZone);
    }

    public IEnumerator EffectSpwanZoneOff() {
        yield return new WaitForSeconds(1f);
        EffectSpawnZone.gameObject.SetActive(false);
    }

    IEnumerator ReDrawSpot() {
        yield return new WaitForSeconds(0.2f);
        TargetGardenLog.flower = ( TargetGardenLog.flower >= 10) ? 10 : TargetGardenLog.flower+1;
        TargetGardenLog.log_water = ( TargetGardenLog.log_water <= 0 ) ? 0 : TargetGardenLog.log_water-1;
        SpotHandler.InitSpot(TargetGardenLog);
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        for (int i = 0; i < WaterIcons.Count; i++) {
            GameObject temp = WaterIcons[i];
            WaterIcons[i] = null;
            Destroy(temp);
        }
        WaterIcons.Clear();
        int waterIconCount = ( TargetGardenLog.log_water >= 10 ) ? 10 : TargetGardenLog.log_water;
        WaterSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(( waterIconCount * 90f ) + ( waterIconCount - 1 ) * 80, 131f);
        for (int i = 0; i < waterIconCount; i++) {
            GameObject go = Instantiate(WaterPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( i * 170 ), 0);
            go.GetComponent<DraggableWaterIcon>().SetinitPos();
            WaterIcons.Add(go);
        }
    }
}
