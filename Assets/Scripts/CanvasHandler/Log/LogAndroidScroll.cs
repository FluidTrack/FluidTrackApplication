using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogAndroidScroll : MonoBehaviour
{
    public int counter; 
    public bool isRight; 
    // Start is called before the first frame update

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
            TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.LOG].activeSelf && Input.touchCount == 1) {
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
