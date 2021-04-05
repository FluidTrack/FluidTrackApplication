using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenSpotHandler : MonoBehaviour
{
    public GameObject TodayUI;
    public GameObject NotYetObject;
    public GameObject FlowerBody;
    public GameObject FlowerHead;
    public GameObject ButterFlies;
    public Transform FlowerParents;
    public Text DateText;
    
    public TimeHandler.DateTimeStamp.DATE Date = TimeHandler.DateTimeStamp.DATE.MON;
    public bool isToday = false;
    public bool isNotUse = false;
    public bool isFuture = false;
    public string DateString = "2021-1-1";
    public int flowerCount = 0;
    public int Step = 0;
    private List<GameObject> FlowerParts;

    private float[] Body_Offset_X_1 = { 0f };
    private float[] Body_Offset_Y_1 = { 0f };
    private float[] Body_Offset_X_2 = { -50f, 50f };
    private float[] Body_Offset_Y_2 = { 0f, 0f };
    private float[] Body_Offset_X_3 = { -90f, 0f, 90f};
    private float[] Body_Offset_Y_3 = { 0f, 0f, 0f };

    private float[] Head_Offset_X_1 = { 0f};
    private float[] Head_Offset_Y_1 = { 105.3f};
    private float[] Head_Offset_X_2 = { -50f, 50f };
    private float[] Head_Offset_Y_2 = { 105.3f, 105.3f};
    private float[] Head_Offset_X_3 = { -95f, 0f, 95f, -138f, -48f, 48, 138f, -95f, 0f, 95f, };
    private float[] Head_Offset_Y_3 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f, 247f, 247f };

    public void Awake() {
        TodayUI.SetActive(false);
        NotYetObject.SetActive(false);
        isToday = false;
        ButterFlies.SetActive(false);
        DateText.text = "";
        FlowerParts = new List<GameObject>();
    }

    public void InitSpot(DataHandler.GardenLog logData, TimeHandler.DateTimeStamp logDate) {
        int new_flowerCount = (logData != null) ? logData.flower : 0;
        if ((flowerCount != 0) &&flowerCount == new_flowerCount) return;
        flowerCount = new_flowerCount;
        foreach (GameObject go in FlowerParts)
            Destroy(go);
        FlowerParts.Clear();
        TodayUI.SetActive(false);
        NotYetObject.SetActive(false);
        isToday = false;
        ButterFlies.SetActive(false);

        this.DateString = logDate.ToDateString();
        DateText.text = TimeHandler.DateTimeStamp.DateList[logDate.Date];
        int cmpResult = TimeHandler.DateTimeStamp.CmpDateTimeStamp(
            TimeHandler.HomeCanvasTime,
            new TimeHandler.DateTimeStamp(DateString));
        if(cmpResult == 0) {
            isToday = true;
            TodayUI.SetActive(true);
        }

        if (logData == null) {
            if (cmpResult == -1) { NotYetObject.SetActive(true); isFuture = true; }
            return;
        }

        bool drawFlowerFlag = true;
        if(logData.log_water > 0) {
            isNotUse = true;
            if (flowerCount == 0) {
                NotYetObject.SetActive(true);
                drawFlowerFlag = false;
            }
        }
        if(drawFlowerFlag) {
            if (flowerCount == 1) {
                for(int i = 0; i < 1; i ++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_1[i],Body_Offset_Y_1[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_1[i], Head_Offset_Y_1[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            }
            else if (flowerCount == 2) {
                for (int i = 0; i < 2; i++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_2[i], Body_Offset_Y_2[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_2[i], Head_Offset_Y_2[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            }
            else if (flowerCount >= 3) {
                for (int i = 0; i < 3; i++) {
                    GameObject body = Instantiate(FlowerBody, FlowerParents);
                    body.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Body_Offset_X_3[i], Body_Offset_Y_3[i]);
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_3[i], Head_Offset_Y_3[i]);
                    FlowerParts.Add(body);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
                for (int i = 3; (i < flowerCount) && (i<10); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_3[i], Head_Offset_Y_3[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step);
                }
            }

            if (flowerCount >= 10)
                ButterFlies.SetActive(true);
        }
    }

    public void GardenClick() {
        if (isFuture) return;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED);
        if (isNotUse) Debug.Log("하루보기 화면으로 이동");
        else Debug.Log("꽃키우기 화면으로 이동");
    }
}
