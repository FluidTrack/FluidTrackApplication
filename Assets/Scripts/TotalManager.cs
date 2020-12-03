using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalManager : MonoBehaviour
{
    public static TotalManager instance;
    public GameObject OpeningCanvas;
    public GameObject[] OtherCanvas;
    public Font[] fonts;
    public bool SkipOpening;
    public enum FONT_FAMILY {
        NAUM_GOTHIC,
        HY_YUBSU,
        TYPO_DABANGGU,
        D2_CODING,
    }

    public enum CANVAS {
        NAVI_BAR,
        WELCOME,
        WELCOME2,
        WELCOME3,
        WELCOME4,
    }

    public void Start() {
        instance = this;
        if (!SkipOpening) {
            OpeningCanvas.SetActive(true);
            foreach (GameObject go in OtherCanvas)
                go.SetActive(false);
        }
    }

    public void font_change(FONT_FAMILY ff,CANVAS can) {
        foreach (GameObject go in OtherCanvas)
            go.SetActive(true);

        Text[] allObjects = FindObjectsOfType<Text>();
        foreach (Text t in allObjects) {
            t.font = fonts[(int)ff];
        }

        for (int i = 0; i < OtherCanvas.Length; i++) {
            if(i != (int)can)
                OtherCanvas[i].SetActive(false);
        }
    }

    public void font_change(int ff, CANVAS can) {
        if (ff >= fonts.Length) return;
        foreach (GameObject go in OtherCanvas)
            go.SetActive(true);

        Text[] allObjects = FindObjectsOfType<Text>();
        foreach (Text t in allObjects) {
            t.font = fonts[(int)ff];
        }

        for (int i = 0; i < OtherCanvas.Length; i++) {
            if (i != (int)can)
                OtherCanvas[i].SetActive(false);
        }
    }
}
