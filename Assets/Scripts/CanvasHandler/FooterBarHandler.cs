using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBarHandler : MonoBehaviour
{
    public Image[] Buttons;
    public Color Active;
    public Color Inactive;
    public enum FOOTER_BTN {
        HOME, LOG, FLOWER, TABLE, CALENDAR,
    };

    public void Update() {
        Buttons[(int)FOOTER_BTN.HOME].color =
            ( TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME].activeSelf ) ?
            Active : Inactive;

        Buttons[(int)FOOTER_BTN.LOG].color =
            ( TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf ) ?
            Active : Inactive;

        Buttons[(int)FOOTER_BTN.FLOWER].color =
            ( TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf ) ?
            Active : Inactive;

        Buttons[(int)FOOTER_BTN.TABLE].color =
            ( TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE].activeSelf ) ?
            Active : Inactive;

        Buttons[(int)FOOTER_BTN.CALENDAR].color =
            ( TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR].activeSelf ) ?
            Active : Inactive;
    }

    public void FooterButtonClick(int index) {
        GameObject[] pages = {
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR]
        };

        for(int i = 0; i < pages.Length; i++)
            pages[i].SetActive(index == i);
    }
}
