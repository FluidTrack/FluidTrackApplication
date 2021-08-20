using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogAndroidScroll : MonoBehaviour
{
    public int counter; 
    public bool isRight;
    // Start is called before the first frame update

    public GameObject UI_Setting;
    public GameObject UI_Scanning;
    public GameObject UI_Connecting;
    public GameObject UI_ConnectError;
    public GameObject UI_AddDrinkLog;
    public GameObject UI_AddPooLog;
    public GameObject UI_ModifyDrinkLog;
    public GameObject UI_ModifyPooLog;
    public GameObject UI_SelectDrinkLog;
    public GameObject UI_SelectPooLog;
    public GameObject UI_DeletePeeLog;
    public GameObject UI_DeleteWaterLog;
    //public GameObject UI_ScoreBoard;
    public GameObject UI_SendingServer;
    public GameObject UI_ExitMongMong;
    public GameObject UI_LogMongMong;
    public GameObject UI_ErrorLog;

    void Start()
    {
        //Initiate Variable
        counter = 0;
        isRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        //To check if the right canvas are on
        if (!TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.FLOWER].activeSelf &&
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf &&
            !UI_Setting.activeSelf &&
            !UI_Scanning.activeSelf &&
            !UI_Connecting.activeSelf &&
            !UI_ConnectError.activeSelf &&
            !UI_AddDrinkLog.activeSelf &&
            !UI_AddPooLog.activeSelf &&
            !UI_ModifyDrinkLog.activeSelf &&
            !UI_ModifyPooLog.activeSelf &&
            !UI_SelectDrinkLog.activeSelf &&
            !UI_SelectPooLog.activeSelf &&
            !UI_SendingServer.activeSelf &&
            !UI_ExitMongMong.activeSelf &&
            !UI_LogMongMong.activeSelf &&
            !UI_ErrorLog.activeSelf &&
            !UI_DeletePeeLog.activeSelf &&
            !UI_DeleteWaterLog.activeSelf &&
            Input.touchCount == 1) {
            //Threshold 5
            if (Input.GetTouch(0).deltaPosition.x < -5)
            {
                if (isRight)
                {
                    counter++;
                } else
                {
                    isRight = true;
                    counter = 0;
                }
            } else if (Input.GetTouch(0).deltaPosition.x > 5)
            {
                if (!isRight)
                {
                    counter++;
                } else
                {
                    isRight = false;
                    counter = 0;
                }
            }

            //Move if counter hits 5
            if (counter == 5)
            {
                if (isRight)
                {
                    LogCanvasHandler.Instance.TimeRightButtonClick();
                }
                else
                {
                    LogCanvasHandler.Instance.TimeLeftButtonClick();
                }
                counter = 0;
            }
        }
    }
}
