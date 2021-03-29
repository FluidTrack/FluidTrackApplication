using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ScrollViewHandler2 : MonoBehaviour, IBeginDragHandler, IDragHandler {
    public float MinHeight = 277f;
    public float ContentHeight;
    public float SpriteHeight = 80f;
    public float MaxHeight = 0;
    public RectTransform Content;
    public GameObject[] Slots;
    public List<List<GameObject>> Elements;
    public Vector2 CursorPos;
    public float MaxMovement = 0f;
    public GameObject PooLog;
    public Text PooCount;

    public void OnEnable() {
        Elements = new List<List<GameObject>>();
        for (int i = 0; i < 21; i++)
            Elements.Add(new List<GameObject>());
        MaxHeight = 0;
        ContentHeight = Content.sizeDelta.y;
        StartCoroutine(FetchData());
        Debug.Log("Down : " + Content.localPosition.y);
    }

    public void OnDisable() {
        for (int i = 0; i < 21; i++) {
            for (int j = 0; j < Elements[i].Count; j++) {
                GameObject target = Elements[i][j];
                Destroy(target);
            }
            Elements[i].Clear();
        }
    }

    public GameObject AddElement(GameObject go, int x) {
        GameObject target = Instantiate(go, Slots[x].transform);
        Elements[x].Add(target);
        MaxHeight = ( MaxHeight < Elements[x].Count * SpriteHeight ) ?
            Elements[x].Count * SpriteHeight : MaxHeight;
        target.GetComponent<RectTransform>().localPosition
            = new Vector2(0f, ( Elements[x].Count - 1 ) * -SpriteHeight + 277);
        target.GetComponent<LogSpriteHandler>().Index = x;
        ResizeContent();
        return target;
    }

    public void ResizeContent() {
        float height = ( MaxHeight > MinHeight ) ? MaxHeight : MinHeight;
        ContentHeight = height;
        Content.sizeDelta = new Vector2(1720f, height);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        CursorPos = eventData.position;
        MaxMovement = ContentHeight - MinHeight;
    }

    public void OnDrag(PointerEventData eventData) {
        if (MaxMovement <= 0) return;
        float movement = CursorPos.y - eventData.position.y;
        float result_y = Content.localPosition.y - movement;
        result_y = ( result_y < 277 ) ? 277 : result_y;
        result_y = ( result_y > 277 + MaxMovement ) ? 277 + MaxMovement : result_y;
        Content.localPosition = new Vector2(Content.localPosition.x, result_y);
    }

    public IEnumerator FetchData() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isDataLoaded) {
                DataHandler.User_isPooDataLoaded = false;
                StartCoroutine(DataHandler.ReadPoopLogs(DataHandler.User_id));
                StartCoroutine(DataLoadCompleted());
                break;
            }
        }
    }

    IEnumerator DataLoadCompleted() {
        while (true) {
            yield return 0;
            if (DataHandler.User_isPooDataLoaded) {
                DataHandler.User_isPooDataLoaded = false;

                CreateIcon(TimeHandler.LogCanvasTime);
                break;
            }
        }
    }


    public void CreateIcon(TimeHandler.DateTimeStamp stamp) {
        DataHandler.PoopLog[] pooList = DataHandler.Poop_logs.PoopLogs;
        List<LogSpriteHandler.LogScript> logList = new List<LogSpriteHandler.LogScript>();

        for (int i = 0; i < pooList.Length; i++) {
            if (TimeHandler.DateTimeStamp.
                CmpDateTimeStamp(pooList[i].timestamp,
                stamp.ToString()) == 0)
                logList.Add(new LogSpriteHandler.LogScript(pooList[i].timestamp,
                                                 pooList[i].log_id,
                                                 pooList[i].type,
                                                 LogSpriteHandler.LOG.POO));
        }
        PooCount.text = logList.Count.ToString();
        logList.Sort(new LogSpriteHandler.Comparer());

        for (int i = 0; i < logList.Count; i++) {
            int hour = logList[i].TimeStamp.Hours;
            if (hour > 1 && hour < 5) continue;
            GameObject target = AddElement(PooLog, hour - 5);
            target.GetComponent<LogSpriteHandler>().
                SetData(logList[i]);
        }
        Content.localPosition = new Vector2(0, 277);
    }
}
