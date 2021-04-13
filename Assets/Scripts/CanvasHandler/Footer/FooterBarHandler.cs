using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterBarHandler : MonoBehaviour
{
    public static FooterBarHandler Instance;
    public Image[] Buttons;
    public Image[] ButtonImage;
    public Text[] ButtonText;
    public Color ActiveContentColor;
    public Color InactiveContentColor;
    public Color Active;
    public Color Inactive;
    public FOOTER_BTN currentPage;

    public enum FOOTER_BTN {
        HOME, LOG, TABLE, CALENDAR,
    };

    public void Awake() {
        Instance = this;
    }

    public void Start() {
        GameObject[] pages = {
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER],
        };

        for (int i = 0; i < pages.Length; i++) {
            if (pages[i].activeSelf) {
                currentPage = (FOOTER_BTN)i;
                Buttons[i].color = Active;
                ButtonText[i].color = ActiveContentColor;
                ButtonImage[i].color = ActiveContentColor;
                break;
            }
        }
    }

    public void FooterButtonClick(int index) {
        if (currentPage == (FOOTER_BTN)index)
            return;
    
        GameObject[] pages = {
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.HOME],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.TABLE],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.CALENDAR],
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER],
        };

        if(index > (int)currentPage) {
            pages[(int)currentPage].GetComponent<Animator>().SetTrigger("OnDisableMirrored");
            pages[index].SetActive(true);
            pages[index].GetComponent<Animator>().SetTrigger("OnEnableMirrored");
        } else {
            pages[(int)currentPage].GetComponent<Animator>().SetTrigger("OnDisable");
            pages[index].SetActive(true);
        }

        for(int i = 0; i < Buttons.Length; i ++) {
            if(i == index) {
                Buttons[i].color = Active;
                ButtonText[i].color = ActiveContentColor;
                ButtonImage[i].color = ActiveContentColor;
            } else {
                Buttons[i].color = Inactive;
                ButtonText[i].color = InactiveContentColor;
                ButtonImage[i].color = InactiveContentColor;
            }
        }

        currentPage = (FOOTER_BTN)index;
    }
}
