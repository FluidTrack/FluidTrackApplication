using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class TotalManager : MonoBehaviour
{
    public static TotalManager instance;
    public GameObject OpeningCanvas;
    public GameObject[] OtherCanvas;
    public Font[] fonts;
    public bool SkipOpening;
    public GameObject FlashEffect;
    public enum FONT_FAMILY {
        NAUM_GOTHIC,
        HY_YUBSU,
        TYPO_DABANGGU,
        D2_CODING,
    }

    public enum CANVAS {
        NAVI_BAR,
        WELCOME, WELCOME2, WELCOME3, WELCOME4,
        HOME, FOOTER_BAR,
        LOG,FLOWER,TABLE,CALENDAR,
    }


    public void OnEnable() {
        DataHandler.dataPath = Application.persistentDataPath;

        if (SkipOpening) {
            try {
                FileStream fs = new FileStream(DataHandler.dataPath + "/userData", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                DataHandler.User_id = int.Parse(sr.ReadLine());
                sr.Close();
                fs.Close();
            } catch (System.Exception e) {
                e.ToString();
                Debug.LogWarning("Cannot found userData\nDataHandler.User_id set 1 as default value.");
                DataHandler.User_id = 1;
            }

            StartCoroutine(DataHandler.ReadUsers(DataHandler.User_id));
        }
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
