using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBlocker : MonoBehaviour
{
    public LogCanvasHandler log;
    public static LogBlocker Instance;
    public List<GameObject> Borders;
    public RectTransform Center;
    public RectTransform UpLeft;
    public RectTransform UpRight;
    public RectTransform DownLeft;
    public RectTransform DownRight;

    public void Awake() {
        Instance = this;
    }

    public void OnEnable() {
        BlockOff();
    }

    public void OnDisable() {
        BlockOff();
    }

    public void BlockOff() {
        foreach (GameObject go in Borders)
            go.SetActive(false);
        UpLeft.sizeDelta = new Vector2(0,280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(0, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);

        UpLeft.gameObject.SetActive(false);
        UpRight.gameObject.SetActive(false);
        DownLeft.gameObject.SetActive(false);
        DownRight.gameObject.SetActive(false);
        Center.gameObject.SetActive(false);
        log.TimeLeftButton2.interactable = true;
        log.TimeRightButton2.interactable = true;
    }

    public void BlockOn(bool isUp) {
        log.TimeLeftButton2.interactable = true;
        log.TimeRightButton2.interactable = true;
        foreach (GameObject go in Borders)
            go.SetActive(true);
        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);

        Center.gameObject.SetActive(false);
        if (isUp) {
            UpLeft.sizeDelta = new Vector2(0, 280f);
            UpRight.sizeDelta = new Vector2(0, 280f);
            DownLeft.sizeDelta = new Vector2(1540, 280f);
            DownRight.sizeDelta = new Vector2(0, 280f);
        } else {
            UpLeft.sizeDelta = new Vector2(1540, 280f);
            UpRight.sizeDelta = new Vector2(0, 280f);
            DownLeft.sizeDelta = new Vector2(0, 280f);
            DownRight.sizeDelta = new Vector2(0, 280f);
        }
    }

    public void BlockOnDetailUp(int index) {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);

        UpLeft.sizeDelta = new Vector2(130f * index, 280f);
        UpRight.sizeDelta = new Vector2(130f * (11-index), 280f);
        DownLeft.sizeDelta = new Vector2(1540, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }

    public void BlockOnDetailDown(int index) {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);
        Center.gameObject.SetActive(true);

        UpLeft.sizeDelta = new Vector2(1540, 280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(130f * index, 280f);
        DownRight.sizeDelta = new Vector2(130f * ( 11 - index ), 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }

    public void BlockOnAll() {
        foreach (GameObject go in Borders)
            go.SetActive(true);
        UpLeft.gameObject.SetActive(true);
        UpRight.gameObject.SetActive(true);
        DownLeft.gameObject.SetActive(true);
        DownRight.gameObject.SetActive(true);

        UpLeft.sizeDelta = new Vector2(1540, 280f);
        UpRight.sizeDelta = new Vector2(0, 280f);
        DownLeft.sizeDelta = new Vector2(1540, 280f);
        DownRight.sizeDelta = new Vector2(0, 280f);
        log.TimeLeftButton2.interactable = false;
        log.TimeRightButton2.interactable = false;
    }
}
