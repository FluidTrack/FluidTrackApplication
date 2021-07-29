using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPageHandler : MonoBehaviour
{
    public GameObject WateringAnim;
    public static DataHandler.GardenLog CurrentLog;
    public static FlowerPageHandler Instance;
    public FlowerPageMongMongHandler MongMong;
    public GameObject AlertWindow;
    public FlowerPageSpotHandler SpotHandler;
    public GameObject WaterPrefab;
    public GameObject PeePrefab;
    public GameObject PooPrefab;
    public Transform WaterSlot;
    public Transform EffectSpawnZone;
    public GameObject Ring;
    public List<GameObject> WaterIcons;
    public RectTransform TargetZone;

    private int iconCount = 0;

    public string DateString;
    public static DataHandler.GardenLog TargetGardenLog;
    public bool isTouchAble = true;

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

    IEnumerator writeGardenLogId(DataHandler.GardenLog log) {
        while (!DataHandler.User_isGardenDataCreated)
            yield return 0;
        DataHandler.User_isGardenDataCreated = false;
        log.log_id = DataHandler.User_isGardenDataCreatedId;
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

            DataHandler.GardenLog[] array =
                            new DataHandler.GardenLog[DataHandler.Garden_logs.GardenLogs.Length + 1];
            DataHandler.CreateGardenIndex.Enqueue(array.Length - 1);
            for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++)
                array[i] = DataHandler.Garden_logs.GardenLogs[i];
            array[array.Length - 1] = newGarden;
            DataHandler.Garden_logs.GardenLogs = array;

            StartCoroutine(DataHandler.CreateGardenlogs(newGarden));
            StartCoroutine(writeGardenLogId(newGarden));
            TargetGardenLog = newGarden;
        }

        SpotHandler.InitSpot(TargetGardenLog);
        int rawWaterIcon = TargetGardenLog.log_water >= 10 ? 10 : TargetGardenLog.log_water;
        int waterIconCount = ( rawWaterIcon + TargetGardenLog.flower >= 10 ) ? 10 - TargetGardenLog.flower : rawWaterIcon;
        int peeIconCount = ( TargetGardenLog.item_0 == 0 && TargetGardenLog.log_pee > 0 ) ? 1 : 0;
        int pooIconCount = ( TargetGardenLog.item_1 == 0 && TargetGardenLog.log_poop > 0 ) ? 1 : 0;
        int totalIconCount = waterIconCount + peeIconCount + pooIconCount;
        WaterSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(( totalIconCount * 90f ) + ( totalIconCount - 1 ) * 40, 131f);
        int k = 0;
        iconCount = 0;
        for (k = 0; k < waterIconCount; k++) {
            GameObject go = Instantiate(WaterPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2((k * 130),0);
            go.GetComponent<DraggableWaterIcon>().SetinitPos();
            WaterIcons.Add(go);
            iconCount++;
        }
        if(peeIconCount > 0) {
            GameObject go = Instantiate(PeePrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( k * 130 ), 0);
            go.GetComponent<DraggablePeeIcon>().SetinitPos();
            WaterIcons.Add(go);
            k++;
            iconCount++;
        }
        if(pooIconCount > 0) {
            GameObject go = Instantiate(PooPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( k * 130 ), 0);
            go.GetComponent<DraggablePooIcon>().SetinitPos();
            WaterIcons.Add(go);
            iconCount++;
        }
    }

    public void BackButtonClicked() {
        if (!isTouchAble) return;
        if (iconCount == 0) BackButtonRealActive();
        else {
            AlertWindow.SetActive(true);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        }
    }

    public void BackButtonRealActive() {
        LogCanvasHandler.Instance.Fetching();
        this.gameObject.SetActive(false);
    }

    public void Watering() {
        EffectSpawnZone.gameObject.SetActive(true);
        isTouchAble = false;
        StartCoroutine(TouchAbleControl(2.1f));
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA3);
        TargetGardenLog.flower = ( TargetGardenLog.flower >= 10 ) ? 10 : TargetGardenLog.flower + 1;
        MongMong.WaterDrop(TargetGardenLog.flower);
        TargetGardenLog.log_water = ( TargetGardenLog.log_water <= 0 ) ? 0 : TargetGardenLog.log_water - 1;
        SpotHandler.Watering();
        StartCoroutine(ReDrawSpot());
        WateringAnim.SetActive(true);
        iconCount--;
    }

    public void DragPee() {
        EffectSpawnZone.gameObject.SetActive(true);
        isTouchAble = false;
        MongMong.PeeDrop();
        StartCoroutine(TouchAbleControl(1.1f));
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA5);
        TargetGardenLog.item_0 = 1;
        SpotHandler.DragPee();
        StartCoroutine(ReDrawSpot());
        iconCount--;
    }

    public void DragPoo() {
        EffectSpawnZone.gameObject.SetActive(true);
        isTouchAble = false;
        MongMong.PooDrop();
        StartCoroutine(TouchAbleControl(1.1f));
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA5);
        TargetGardenLog.item_1 = 1;
        SpotHandler.DragPoo();
        StartCoroutine(ReDrawSpot());
        iconCount--;
    }

    public void SpawnEffect() {
        EffectSpawnZone.gameObject.SetActive(true);
        StartCoroutine(EffectSpwanZoneOff());
        Instantiate(Ring, EffectSpawnZone);
    }

    public IEnumerator EffectSpwanZoneOff() {
        yield return new WaitForSeconds(1f);
        EffectSpawnZone.gameObject.SetActive(false);
    }

    public IEnumerator TouchAbleControl(float timeAmount) {
        yield return new WaitForSeconds(timeAmount);
        isTouchAble = true;
    }

    IEnumerator ReDrawSpot() {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        for (int i = 0; i < WaterIcons.Count; i++) {
            GameObject temp = WaterIcons[i];
            WaterIcons[i] = null;
            Destroy(temp);
        }

        for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (DataHandler.Garden_logs.GardenLogs[i].log_id == TargetGardenLog.log_id) {
                DataHandler.Garden_logs.GardenLogs[i] = TargetGardenLog;
                break;
            }
        }

        WaterIcons.Clear();
        int rawWaterIcon = TargetGardenLog.log_water >= 10 ? 10 : TargetGardenLog.log_water;
        int waterIconCount = ( rawWaterIcon + TargetGardenLog.flower >= 10 ) ? 10 - TargetGardenLog.flower : rawWaterIcon;
        int peeIconCount = ( TargetGardenLog.item_0 == 0 && TargetGardenLog.log_pee > 0 ) ? 1 : 0;
        int pooIconCount = ( TargetGardenLog.item_1 == 0 && TargetGardenLog.log_poop > 0 ) ? 1 : 0;
        int totalIconCount = waterIconCount + peeIconCount + pooIconCount;
        WaterSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(( totalIconCount * 90f ) + ( totalIconCount - 1 ) * 40, 131f);
        int k = 0;
        for (k = 0; k < waterIconCount; k++) {
            GameObject go = Instantiate(WaterPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( k * 130 ), 0);
            go.GetComponent<DraggableWaterIcon>().SetinitPos();
            WaterIcons.Add(go);
        }
        if (peeIconCount > 0) {
            GameObject go = Instantiate(PeePrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( k * 130 ), 0);
            go.GetComponent<DraggablePeeIcon>().SetinitPos();
            WaterIcons.Add(go);
            k++;
        }
        if (pooIconCount > 0) {
            GameObject go = Instantiate(PooPrefab, WaterSlot);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(( k * 130 ), 0);
            go.GetComponent<DraggablePooIcon>().SetinitPos();
            WaterIcons.Add(go);
        }
    }
}
