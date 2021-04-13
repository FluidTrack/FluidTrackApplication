using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchAndMouseManager : MonoBehaviour {
    public GameObject TouchRing1;
    public GameObject TouchRing2;
    public GameObject TouchRing3;
    public GameObject TouchRing5;
    public GameObject TouchRing6;
    public Text DebugText;
    public Text DebugText2;
    public Text DebugTextLabel;
    public Text DebugText2Label;
    public RectTransform WorldMap;
    public RectTransform Mask;
    public static TouchAndMouseManager Instance;
    public Vector2 WorldMapSize;
    public Vector2 WorldMapPos;
    public Vector2 MaskSize;
    public float ScaleFactorZoomOut = 0.2f;
    public float ScaleFactorZoomIn = 0.2f;
    public float movementFactor = 1.4f;
    public Vector2 ZoomCenter;
    public float zoomAmount = 0f;

    private RectTransform TouchRing1Trans;
    private RectTransform TouchRing2Trans;
    private RectTransform TouchRing3Trans;
    private RectTransform TouchRing5Trans;
    private RectTransform TouchRing6Trans;
    private static HomeHandler homeHandler;
    private TotalManager manager;
    private bool onTouch = false;
    private bool isMaximumZoom  = false;
    private bool isMaximumZoom2 = false;
    private Vector2 WorldMapOriginPivot;
    private Vector3 WorldMapOriginScale;
    private Vector2 WorldMapOriginPos;
    private Vector2 TouchesCenter;
    private Vector2 LengthVec;
    private float zoom_D1 = 0;
    private float zoom_D2 = 0;
    private int PC_zoom_count = 0;
    private Touch[] Touches;
    private Vector2[] TouchPos;
    private Vector2 ClickedAnchor;
    private Vector2 OriginClickedPosition;
    private bool isClicked = false;

#if UNITY_ANDROID
    private Vector2 AdjustedWorldMapPos;
#endif

    public void Start() {
        homeHandler = HomeHandler.Instance;
        manager = TotalManager.instance;
        WorldMapOriginPivot = new Vector2(0f,1f);
        WorldMapOriginPos = WorldMap.position;
        WorldMapOriginScale = WorldMap.localScale;
        TouchRing1Trans = TouchRing1.GetComponent<RectTransform>();
        TouchRing2Trans = TouchRing2.GetComponent<RectTransform>();
        TouchRing3Trans = TouchRing3.GetComponent<RectTransform>();
        TouchRing5Trans = TouchRing5.GetComponent<RectTransform>();
        TouchRing6Trans = TouchRing6.GetComponent<RectTransform>();

        DebugText.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugText2.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugTextLabel.gameObject.SetActive(TotalManager.instance.isDebugMode);
        DebugText2Label.gameObject.SetActive(TotalManager.instance.isDebugMode);
        TouchRing3.SetActive(TotalManager.instance.isDebugMode);
    }


    public void ChangePeriode(RectTransform newWorldMap) {
        WorldMap = newWorldMap;
        WorldMapOriginPivot = new Vector2(0f, 1f);
        WorldMapOriginPos = WorldMap.position;
        WorldMapOriginScale = WorldMap.localScale;
    }

    public void Awake() {
        Touches = new Touch[2];
        TouchPos = new Vector2[2];
        Instance = this;
    }

    public void Update() {
        if (manager.currentCanvas != TotalManager.CANVAS.HOME) return;

        #region Zoom-In/Out
#if UNITY_EDITOR
        //======================================================================================================
        //  Unity Zoom Handler
        //======================================================================================================
        float wheelAmount = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(wheelAmount) > 0) {
            bool flag = true;
            TouchPos[0] = Input.mousePosition;
            if (TouchPos[0].x < Mask.anchoredPosition.x ||
                TouchPos[0].y < -Mask.anchoredPosition.y ||
                TouchPos[0].x > Mask.anchoredPosition.x + Mask.sizeDelta.x ||
                TouchPos[0].y > -Mask.anchoredPosition.y + Mask.sizeDelta.y)
                flag = false;

            if(flag) {
                homeHandler.ReturnButton.SetActive(true);
                WorldMapOriginScale = WorldMap.localScale;
                TouchesCenter = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                Vector2 AdjustedWorldMapPos = new Vector2(
                    WorldMap.anchoredPosition.x - ( WorldMap.sizeDelta.x * WorldMap.localScale.x * WorldMap.pivot.x),
                    WorldMap.anchoredPosition.y + ( WorldMap.sizeDelta.y * WorldMap.localScale.x * ( 1 - WorldMap.pivot.y ) )
                );
                ZoomCenter = new Vector2(
                    ( TouchesCenter.x - AdjustedWorldMapPos.x - Mask.anchoredPosition.x ) / ( WorldMap.sizeDelta.x * WorldMap.localScale.x),
                    1- ( ( TouchesCenter.y + AdjustedWorldMapPos.y + Mask.anchoredPosition.y ) / ( WorldMap.sizeDelta.y * WorldMap.localScale.y) )
                );

                WorldMap.pivot = ZoomCenter;
                WorldMap.anchoredPosition = new Vector2(TouchesCenter.x-Mask.anchoredPosition.x,-(TouchesCenter.y+Mask.anchoredPosition.y));

                zoomAmount = ( wheelAmount < 0 ) ? -0.1f : 0.1f;
                Vector3 zoomedScale = WorldMap.localScale + new Vector3(zoomAmount, zoomAmount, zoomAmount);
                isMaximumZoom = false;
                if (zoomedScale.x >= 1.2f || zoomedScale.y >= 1.2f)
                    zoomedScale = new Vector3(1.2f, 1.2f, 1.2f);
                else if (zoomedScale.x <= 0.23f || zoomedScale.y <= 0.23f) {
                    zoomedScale = new Vector3(0.23f, 0.23f, 0.23f);
                    PC_zoom_count++;
                    if(PC_zoom_count >= 10) {
                        PC_zoom_count = 0;
                        Debug.LogError("Maximum zoom");
                    }
                } else PC_zoom_count = 0;
                WorldMap.localScale = zoomedScale;
            }
        }
#elif UNITY_ANDROID
        //======================================================================================================
        //  Android Zoom Handler
        //======================================================================================================
        DebugText.text = Input.touchCount + "";

        if (!onTouch) {
            if(Input.touchCount == 2) {
                for (int i = 0; i < 2; i++) {
                    Touches[i] = Input.GetTouch(i);
                    TouchPos[i] =
                     new Vector2(Touches[i].position.x,Screen.height - Touches[i].position.y);
                }

                // Touch 유효성 검사 (유효 영역)
                bool flag = true;
                for(int i = 0; i < 2; i ++) {
                    if(TouchPos[i].x < Mask.anchoredPosition.x ||
                       TouchPos[i].y < -Mask.anchoredPosition.y ||
                       TouchPos[i].x > Mask.anchoredPosition.x + Mask.sizeDelta.x ||
                       TouchPos[i].y > -Mask.anchoredPosition.y + Mask.sizeDelta.y) {
                        flag = false;
                        break;
                    }
                    if (i == 0) { 
                        if(TotalManager.instance.isDebugMode) {
                            TouchRing1.SetActive(true);
                            TouchRing5.SetActive(true);
                        }
                        TouchRing5Trans.anchoredPosition = new Vector2(TouchPos[0].x, -TouchPos[0].y);
                        TouchRing1Trans.anchoredPosition = new Vector2(TouchPos[0].x, -TouchPos[0].y);
                    } else if (i == 1) {
                        if (TotalManager.instance.isDebugMode) {
                            TouchRing2.SetActive(true);
                            TouchRing6.SetActive(true);
                        }
                        TouchRing2Trans.anchoredPosition = new Vector2(TouchPos[1].x, -TouchPos[1].y);
                        TouchRing6Trans.anchoredPosition = new Vector2(TouchPos[1].x, -TouchPos[1].y);
                    }
                }

                if(flag) {
                    homeHandler.ReturnButton.SetActive(true);
                    Vector2 Sum = TouchPos[0] + TouchPos[1];
                    zoom_D1 = Vector2.Distance(TouchPos[0], TouchPos[1]);
                    WorldMapOriginScale = WorldMap.localScale;
                    TouchesCenter = new Vector2(Sum.x / 2, Sum.y / 2);
                    AdjustedWorldMapPos = new Vector2(
                        WorldMap.anchoredPosition.x - ( WorldMap.sizeDelta.x * WorldMap.localScale.x * WorldMap.pivot.x ),
                        WorldMap.anchoredPosition.y + ( WorldMap.sizeDelta.y * WorldMap.localScale.x * ( 1 - WorldMap.pivot.y ) )
                    );
                    ZoomCenter = new Vector2(
                        ( TouchesCenter.x - AdjustedWorldMapPos.x - Mask.anchoredPosition.x ) / ( WorldMap.sizeDelta.x * WorldMap.localScale.x ),
                        1 - ( ( TouchesCenter.y + AdjustedWorldMapPos.y + Mask.anchoredPosition.y ) / ( WorldMap.sizeDelta.y * WorldMap.localScale.y ) )
                    );

                    TouchRing3Trans.anchoredPosition = new Vector2(ZoomCenter.x * WorldMap.sizeDelta.x,
                                                                   -( 1 - ( ZoomCenter.y ) ) * WorldMap.sizeDelta.y);
                    WorldMap.pivot = ZoomCenter;
                    WorldMap.anchoredPosition = new Vector2(TouchesCenter.x - Mask.anchoredPosition.x, -( TouchesCenter.y + Mask.anchoredPosition.y ));
                    onTouch = true;
                }
            } else {
                TouchRing1.SetActive(false);
                TouchRing2.SetActive(false);
                TouchRing3.SetActive(false);
                TouchRing5.SetActive(false);
                TouchRing6.SetActive(false);
            }
        } else {
            if(Input.touchCount < 2) {
                onTouch = false;
                TouchRing1.SetActive(false);
                TouchRing2.SetActive(false);
                TouchRing3.SetActive(false);
                if (isMaximumZoom == true) {
                    isMaximumZoom  = false;
                    isMaximumZoom2 = true;
                }
            } else {
                for (int i = 0; i < 2; i++) {
                    Touches[i] = Input.GetTouch(i);
                    TouchPos[i] =
                     new Vector2(Touches[i].position.x, Screen.height - Touches[i].position.y);
                }
                TouchRing1Trans.anchoredPosition = new Vector2(TouchPos[0].x,-TouchPos[0].y);
                TouchRing2Trans.anchoredPosition = new Vector2(TouchPos[1].x, -TouchPos[1].y);
                Vector2 Sum = TouchPos[0] + TouchPos[1];
                zoom_D2 = Vector2.Distance(TouchPos[0], TouchPos[1]);

                zoomAmount = ( 1 - ( zoom_D1 / zoom_D2 ) );
                if (zoomAmount >= 1) zoomAmount *= ScaleFactorZoomOut;
                else if (zoomAmount < 1) zoomAmount *= ScaleFactorZoomIn;
                DebugText2.text = "zoom_D2 = " + zoom_D2 + "\n";
                DebugText2.text += "zoom_D1 = " + zoom_D1 + "\n";
                DebugText2.text += "zoomAmount = " + zoomAmount + "\n";
                Vector3 zoomedScale =WorldMapOriginScale + new Vector3(zoomAmount, zoomAmount, zoomAmount);
                isMaximumZoom = false;
                if (zoomedScale.x >= 1.2f || zoomedScale.y >= 1.2f){
                    zoomedScale = new Vector3(1.2f, 1.2f, 1.2f);
                    isMaximumZoom2 = false;
                }    
                else if (zoomedScale.x <= 0.23f || zoomedScale.y <= 0.23f) {
                    if(isMaximumZoom2 && zoomAmount < -2f) {
                        isMaximumZoom2 = false;
                        Debug.LogError("8 weeks view");
                    } else {
                        zoomedScale = new Vector3(0.23f, 0.23f, 0.23f);
                        isMaximumZoom = true;
                    }
                }
                WorldMap.localScale = zoomedScale;
            }
        }
#endif
        #endregion

#if UNITY_EDITOR
        if (Input.GetMouseButton(0)) {
#elif UNITY_ANDROID
        if (Input.GetMouseButton(0) && Input.touchCount == 1) {
#endif
            if (isClicked) {
                float offset_x = Input.mousePosition.x - ClickedAnchor.x;
                float offset_y = Input.mousePosition.y - ClickedAnchor.y;
                WorldMap.anchoredPosition = OriginClickedPosition + (new Vector2(offset_x, offset_y)) * movementFactor;
                if (!homeHandler.ReturnButton.activeSelf &&
                    new Vector2(offset_x*movementFactor,offset_y*movementFactor).magnitude > 150f) {
                    homeHandler.ReturnButton.SetActive(true);
                }
            } else if(Input.mousePosition.x > Mask.anchoredPosition.x && Input.mousePosition.x < (Mask.anchoredPosition.x + Mask.sizeDelta.x ) &&
                      Input.mousePosition.y < (Screen.height + Mask.anchoredPosition.y ) &&
                      Input.mousePosition.y > (Screen.height + Mask.anchoredPosition.y - Mask.sizeDelta.y)) {
                OriginClickedPosition = WorldMap.anchoredPosition;
                ClickedAnchor = Input.mousePosition;
                isClicked = true;
            }
        } else if(isClicked) { isClicked = false; }

        AdjustedWorldMapPos = new Vector2(
                    WorldMap.anchoredPosition.x - ( WorldMap.sizeDelta.x * WorldMap.localScale.x * WorldMap.pivot.x ),
                    WorldMap.anchoredPosition.y + ( WorldMap.sizeDelta.y * WorldMap.localScale.x * ( 1 - WorldMap.pivot.y ) )
                );

        if (AdjustedWorldMapPos.x > 0) {
            Vector2 amount = new Vector2(-AdjustedWorldMapPos.x, 0);
            WorldMap.anchoredPosition += amount;
        } else if (AdjustedWorldMapPos.x + WorldMap.sizeDelta.x * WorldMap.localScale.x <
             Mask.anchoredPosition.x + Mask.sizeDelta.x) {
            Vector2 amount = new Vector2(
                Mask.anchoredPosition.x + Mask.sizeDelta.x -
                AdjustedWorldMapPos.x - WorldMap.sizeDelta.x * WorldMap.localScale.x
            , 0);
            WorldMap.anchoredPosition += amount;
        }

        if (AdjustedWorldMapPos.y - WorldMap.sizeDelta.y * WorldMap.localScale.y >
             -Mask.sizeDelta.y) {
            Vector2 amount = new Vector2(
                0,
                -( AdjustedWorldMapPos.y - WorldMap.sizeDelta.y * WorldMap.localScale.y
                + Mask.sizeDelta.y )
            );
            WorldMap.anchoredPosition += amount;
        } else if (AdjustedWorldMapPos.y < 0) {
            Vector2 amount = new Vector2(0, -AdjustedWorldMapPos.y);
            WorldMap.anchoredPosition += amount;
        }
    }
}
