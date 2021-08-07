using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerShapeHandler : MonoBehaviour
{
    public GameObject[] Flowers;
    public GameObject[] Flowers2;
    public enum STAGE_TYPE {
        FIRST_STAGE, SECOND_STAGE, THIRD_STAGE,FOURTH_STAGE, FIFTH_STAGE, SIXTH_STAGE,SEVENTH_STAGE,EIGHTH_STAGE, NONE
    }
    public float[] Offset_Y = { -5.8f, -18.5f, -0.9f, -15.6f, -23.9f, -23.9f, -23.5f, -15.9f };
    public float[] Offset_X = {    0f,     0f,    0f,     0f,     0f,     0f,  -2.9f,  -0.8f };
    public STAGE_TYPE Step = STAGE_TYPE.NONE;
    public GameObject FlowerInstance;

    public void Change(STAGE_TYPE type, bool isPowerUp) {
        if (FlowerInstance != null) {
            DestroyImmediate(FlowerInstance);
            FlowerInstance = null;
        }
        if(!isPowerUp)
            FlowerInstance = Instantiate(Flowers[(int)type], this.transform);
        else FlowerInstance = Instantiate(Flowers2[(int)type], this.transform);
        //Debug.Log(this.name + " : " + ((int)type).ToString());
        FlowerInstance.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(Offset_X[(int)type], Offset_Y[(int)type]);
    }
}
