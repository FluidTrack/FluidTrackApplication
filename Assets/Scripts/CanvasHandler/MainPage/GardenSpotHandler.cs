using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenSpotHandler : MonoBehaviour
{
    public static List<int> weeklyData;
    public GameObject TodayUI;
    public GameObject NotYetObject;
    public GameObject FlowerBody;
    public GameObject FlowerHead;
    public GameObject GrassPrefabs;
    public GameObject FencePrefab;
    public GameObject ButterFlies;
    public GameObject Particle;
    public Transform FlowerParents;
    public Color[] colors;
    public Text DateText;
    
    public TimeHandler.DateTimeStamp.DATE Date = TimeHandler.DateTimeStamp.DATE.MON;
    public bool isToday = false;
    public bool isNotUse = false;
    public bool isFuture = false;
    public string DateString = "2021-1-1";
    public int flowerCount = 0;
    public int Step = 0;
    public List<GameObject> FlowerParts;

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
    //private float[] Head_Offset_X_3 = { -95f, 0f, 95f, -48f, 48f, -138f, 138f, -95f, 0f, 95f, };
    //private float[] Head_Offset_Y_3 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f, 247f, 247f };
    private float[] Head_Offset_X_3 = { -95f, 0f, 95f};
    private float[] Head_Offset_Y_3 = { 105.3f, 105.3f, 105.3f};

    private float[] Head_Offset_X_4 = { -95f, 0f, 95f, 0f };
    private float[] Head_Offset_Y_4 = { 105.3f, 105.3f, 105.3f, 174f};

    private float[] Head_Offset_X_5 = { -95f, 0f, 95f, -48f, 48f };
    private float[] Head_Offset_Y_5 = { 105.3f, 105.3f, 105.3f, 174f, 174f };

    private float[] Head_Offset_X_6 = { -95f, 0f, 95f, -95f, 0f, 95f };
    private float[] Head_Offset_Y_6 = { 105.3f, 105.3f, 105.3f, 174f, 174f , 174f };

    private float[] Head_Offset_X_7 = { -95f, 0f, 95f, -48f, 48f, -138f, 138f };
    private float[] Head_Offset_Y_7 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f };

    private float[] Head_Offset_X_8 = { -95f, 0f, 95f, -48f, 48f, -138f, 138f, 0f, };
    private float[] Head_Offset_Y_8 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f };

    private float[] Head_Offset_X_9 = { -95f, 0f, 95f, -48f, 48f, -138f, 138f, -48f, 48f };
    private float[] Head_Offset_Y_9 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f, 247f};

    private float[] Head_Offset_X_10 = { -95f, 0f, 95f, -48f, 48f, -138f, 138f, -95f, 0f, 95f, };
    private float[] Head_Offset_Y_10 = { 105.3f, 105.3f, 105.3f, 174f, 174f, 174f, 174f, 247f, 247f, 247f };

    public void Awake() {
        TodayUI.SetActive(false);
        NotYetObject.SetActive(false);
        isToday = false;
        ButterFlies.SetActive(false);
        DateText.text = "";
        FlowerParts = new List<GameObject>();
    }

    public void OnEnable() {
        if(FlowerParts == null)
            FlowerParts = new List<GameObject>();
    }

    public void InitSpot(DataHandler.GardenLog logData, TimeHandler.DateTimeStamp logDate) {
        int new_flowerCount = (logData != null) ? logData.flower : 0;
        flowerCount = new_flowerCount;
        try {
            foreach (GameObject go in FlowerParts)
                Destroy(go);
            FlowerParts.Clear();
        } catch(System.Exception e) { e.ToString(); }
        TodayUI.SetActive(false);
        NotYetObject.SetActive(false);
        isToday = false;
        ButterFlies.SetActive(false);
        this.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
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
            if (cmpResult == -1) {
                this.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
                DateText.text = "";
                isFuture = true; }
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

        FlowerParents.localScale = new Vector3(0.7f,0.7f,0.7f);

        if (drawFlowerFlag) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            }
            else if (flowerCount == 3) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 4) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_4[i], Head_Offset_Y_4[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 5) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_5[i], Head_Offset_Y_5[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 6) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_6[i], Head_Offset_Y_6[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 7) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_7[i], Head_Offset_Y_7[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 8) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_8[i], Head_Offset_Y_8[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 9) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_9[i], Head_Offset_Y_9[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            } else if (flowerCount == 10) {
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
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
                for (int i = 3; ( i < flowerCount ) && ( i < 10 ); i++) {
                    GameObject head = Instantiate(FlowerHead, FlowerParents);
                    head.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(Head_Offset_X_10[i], Head_Offset_Y_10[i]);
                    FlowerParts.Add(head);
                    head.GetComponent<FlowerShapeHandler>().
                        Change((FlowerShapeHandler.STAGE_TYPE)Step, logData.item_0 > 0);
                }
            }

            ButterFlies.SetActive(flowerCount >= 10);

            Particle.SetActive(logData.item_0 > 0);

            if(logData.item_1 > 0)
                FlowerParents.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            else FlowerParents.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            weeklyData[Step] += logData.flower;
        }
    }

    public void GardenClick() {
        if (isFuture) return;
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
        TotalManager.instance.TargetDateString = DateString;
        FooterBarHandler.Instance.FooterButtonClick(1);
    }
}
