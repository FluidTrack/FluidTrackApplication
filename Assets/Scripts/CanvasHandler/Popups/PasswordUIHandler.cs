using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordUIHandler : MonoBehaviour
{
    public enum PASS_TYPE { CREATE, CONFIRM, CHECK};

    public Text WindowText;
    public PASS_TYPE WindowType = PASS_TYPE.CHECK;
    public Text[] PassText;
    public Sprite OpenEye;
    public Sprite CloseEye;
    public Image ButtonImage;
    public Button[] NumButton;
    public Button BackButton;
    public Button NextButton;
    public Button PrevButton;

    public GameObject PassWordErrorUI;
    public GameObject NextUI;

    private Vector2 num_pos = new Vector2(200f,240f);
    private Vector2 star_pos = new Vector2(200f,150f);

    internal static bool showPassWord = false;
    private int[] input_text;
    private string truePassWord = "";
    private string inputPassWord = "";
    private int cursor = 0;

    public void OnEnable() {
        PassWordReset();

        switch (WindowType) {
            case PASS_TYPE.CHECK:   WindowText.text = "모아정원의 부모님용 비밀번호를 입력해주세요!"; break;
            case PASS_TYPE.CONFIRM: WindowText.text = "모아정원의 부모님용 비밀번호를 다시 입력해주세요!"; break;
            case PASS_TYPE.CREATE:  WindowText.text = "모아정원의 부모님용 비밀번호를 설정해주세요!"; break;
        }
        

        if(WindowType == PASS_TYPE.CHECK || WindowType == PASS_TYPE.CONFIRM) {
            try {
                DataHandler.User_password = ( DataHandler.User_item_4 / 1000 ).ToString() +
                                            ( ( DataHandler.User_item_4 / 100 ) % 10 ).ToString() +
                                            ( ( DataHandler.User_item_4 / 10 ) % 10 ).ToString() +
                                            ( DataHandler.User_item_4 % 10 ).ToString();
            } catch(System.Exception e) {
                e.ToString();
                DataHandler.User_password = "0000";
            }
        }
    }

    public void NumButtonClick(int num) {
        if (cursor >= 4) return;
        input_text[cursor] = num;
        PassText[cursor].text = ( showPassWord ) ? num.ToString() : "*";
        PassText[cursor].GetComponent<RectTransform>().sizeDelta = ( showPassWord ) ? num_pos : star_pos;
        cursor++;
        BackButton.interactable = true;
        if(cursor == 4) {
            foreach (Button b in NumButton)
                b.interactable = false;
            NextButton.interactable = true;
        }
    }

    public void DeleteButtonClick() {
        if (cursor <= 0) return;
        cursor--;
        input_text[cursor] = -1;
        PassText[cursor].text = "";
        foreach (Button b in NumButton)
            b.interactable = true;
        NextButton.interactable = false;
        if (cursor == 0) 
            BackButton.interactable = false;
    }

    public void EyeButtonClick() {
        showPassWord = !showPassWord;
        ButtonImage.sprite = ( showPassWord ) ? CloseEye : OpenEye;
        for (int i = 0; i < 4; i++) {
            PassText[i].text = ( input_text[i] == -1 ) ? "" : ( showPassWord ) ? input_text[i].ToString() : "*";
            PassText[i].GetComponent<RectTransform>().sizeDelta = ( showPassWord ) ? num_pos : star_pos;
        }
    }

    public void NextButtonClick() {
        inputPassWord = "";
        foreach (int num in input_text)
            inputPassWord += num.ToString();
        bool checkResult = inputPassWord == DataHandler.User_password;
        
        if (WindowType == PASS_TYPE.CREATE) {
            DataHandler.User_password = inputPassWord;
            DataHandler.User_item_4 = int.Parse(DataHandler.User_password);
            truePassWord = inputPassWord;
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME7].SetActive(true);
            this.gameObject.SetActive(false);
        } else if (WindowType == PASS_TYPE.CONFIRM) {
            if (checkResult) {
                DataHandler.User_item_4 = int.Parse(DataHandler.User_password);
                Debug.Log(DataHandler.User_item_4);
                TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME5].SetActive(true);
                this.gameObject.SetActive(false);
            } else {
                PassWordErrorUI.SetActive(true);
                PassWordReset();
            }
        } else {
            if (checkResult) {
                this.NextUI.SetActive(true);
                this.gameObject.SetActive(false);
            } else {
                PassWordReset();
                PassWordErrorUI.SetActive(true);
            }
        }
    }

    public void PassWordReset() {
        ButtonImage.sprite = ( showPassWord ) ? CloseEye : OpenEye;
        BackButton.interactable = false;
        foreach (Button b in NumButton)
            b.interactable = true;
        NextButton.interactable = false;
        if (PrevButton != null)
            PrevButton.interactable = true;

        input_text = new int[4];
        for (int i = 0; i < 4; i++) {
            input_text[i] = -1;
            PassText[i].text = "";
            PassText[i].GetComponent<RectTransform>().sizeDelta = num_pos;
        }
        cursor = 0;
    }

    public void PrevButtonClick() {
        if (WindowType == PASS_TYPE.CREATE) return;
        if (WindowType == PASS_TYPE.CONFIRM)
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME6].SetActive(true);
        this.gameObject.SetActive(false);
    }
}
