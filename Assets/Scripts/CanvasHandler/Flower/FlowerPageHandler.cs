using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageHandler : MonoBehaviour
{
    public GameObject WateringAnim;
    public GameObject WateringAnim2;
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
    public Button WateringAllButton;

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
        WateringAllButton.interactable = false;
        StartCoroutine(FetchUser());
    }

    public void OnDisable() {
        for(int i = 0; i < WaterIcons.Count; i++) {
            GameObject temp = WaterIcons[i];
            WaterIcons[i] = null;
            Destroy(temp);
        }
        WaterIcons.Clear();
        WateringAllButton.interactable = false;

        EffectSpawnZone.gameObject.SetActive(false);
    }

    public IEnumerator FetchUser() {
        yield return 0;

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
            yield return 0;
        log.log_id = DataHandler.User_isGardenDataCreatedId;
    }

    public IEnumerator FetchData() {
            yield return 0;

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
        WateringAllButton.interactable = waterIconCount > 0;
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

    public void Watering_all() {
        WateringAllButton.interactable = false;
        StartCoroutine(Watering_all_Routine());
    }

    public IEnumerator Watering_all_Routine() {
        int rawWaterIcon = TargetGardenLog.log_water >= 10 ? 10 : TargetGardenLog.log_water;
        int waterIconCount = ( rawWaterIcon + TargetGardenLog.flower >= 10 ) ? 10 - TargetGardenLog.flower : rawWaterIcon;
        int Scale = ( waterIconCount > 3 ) ? 3 : waterIconCount;

        EffectSpawnZone.gameObject.SetActive(true);
        isTouchAble = false;
        StartCoroutine(TouchAbleControl( (( ( 1.9f ) / (float)Scale ) * waterIconCount) + 1f));
        StartCoroutine(EffectSpwanZoneOff2(( ( ( 1.9f ) / (float)Scale ) * waterIconCount ) + 1f));
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA3);
        Instantiate(Ring, EffectSpawnZone);
        for(int i = 0; i < waterIconCount; i++) {
            TargetGardenLog.flower = ( TargetGardenLog.flower >= 10 ) ? 10 : TargetGardenLog.flower + 1;
            TargetGardenLog.log_water = ( TargetGardenLog.log_water <= 0 ) ? 0 : TargetGardenLog.log_water - 1;
            SpotHandler.Watering2((float)(Scale+2));
            StartCoroutine(ReDrawSpot());
            WateringAnim2.SetActive(true);
            WateringAnim2.GetComponent<Animator>().speed = (float)Scale;
            iconCount--;
            yield return new WaitForSeconds(( ( 1.9f ) / (float)Scale ));
        }
        MongMong.WaterDrop(TargetGardenLog.flower);
    }

    public void DragPee() {
        MongMong.PeeDrop(TargetGardenLog.flower);

        if(TargetGardenLog.flower <= 0) {
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        } else {
            EffectSpawnZone.gameObject.SetActive(true);
            isTouchAble = false;
            StartCoroutine(TouchAbleControl(1.1f));
            StartCoroutine(EffectSpwanZoneOff());
            Instantiate(Ring, EffectSpawnZone);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA5);
            TargetGardenLog.item_0 = 1;
            SpotHandler.DragPee();
            iconCount--;
        }
        StartCoroutine(ReDrawSpot());
    }

    public void DragPoo() {
        MongMong.PooDrop(TargetGardenLog.flower);

        if (TargetGardenLog.flower <= 0) {
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
        } else {
            EffectSpawnZone.gameObject.SetActive(true);
            isTouchAble = false;
            StartCoroutine(TouchAbleControl(1.1f));
            StartCoroutine(EffectSpwanZoneOff());
            Instantiate(Ring, EffectSpawnZone);
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.TADA5);
            TargetGardenLog.item_1 = 1;
            SpotHandler.DragPoo();
            iconCount--;
        }
        StartCoroutine(ReDrawSpot());
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

    public IEnumerator EffectSpwanZoneOff2(float t) {
        yield return new WaitForSeconds(t);
        EffectSpawnZone.gameObject.SetActive(false);
    }

    public IEnumerator TouchAbleControl(float timeAmount) {
        yield return new WaitForSeconds(timeAmount);
        isTouchAble = true;
    }

    IEnumerator ReDrawSpot() {
        yield return 0;
        StartCoroutine(DataHandler.UpdateGardenLogs(TargetGardenLog));
        for (int i = 0; i < WaterIcons.Count; i++) {
            GameObject temp = WaterIcons[i];
            WaterIcons[i] = null;
            Destroy(temp);
        }

        for (int i = 0; i < DataHandler.Garden_logs.GardenLogs.Length; i++) {
            if (DataHandler.Garden_logs.GardenLogs[i].log_id == TargetGardenLog.log_id) {
                DataHandler.Garden_logs.GardenLogs[i].item_0 = TargetGardenLog.item_0;
                DataHandler.Garden_logs.GardenLogs[i].item_1 = TargetGardenLog.item_1;
                DataHandler.Garden_logs.GardenLogs[i].log_water = TargetGardenLog.log_water;
                DataHandler.Garden_logs.GardenLogs[i].log_poop = TargetGardenLog.log_poop;
                DataHandler.Garden_logs.GardenLogs[i].log_pee = TargetGardenLog.log_pee;
                DataHandler.Garden_logs.GardenLogs[i].flower = TargetGardenLog.flower;
                break;
            }
        }

        WaterIcons.Clear();
        int rawWaterIcon = TargetGardenLog.log_water >= 10 ? 10 : TargetGardenLog.log_water;
        int waterIconCount = ( rawWaterIcon + TargetGardenLog.flower >= 10 ) ? 10 - TargetGardenLog.flower : rawWaterIcon;
        int peeIconCount = ( TargetGardenLog.item_0 == 0 && TargetGardenLog.log_pee > 0 ) ? 1 : 0;
        int pooIconCount = ( TargetGardenLog.item_1 == 0 && TargetGardenLog.log_poop > 0 ) ? 1 : 0;
        int totalIconCount = waterIconCount + peeIconCount + pooIconCount;
        WateringAllButton.interactable = waterIconCount > 0;
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
