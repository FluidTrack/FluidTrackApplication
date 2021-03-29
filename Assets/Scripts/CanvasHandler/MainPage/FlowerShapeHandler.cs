using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerShapeHandler : MonoBehaviour
{
    public GameObject[] Flowers;
    public enum STAGE_TYPE {
        FIRST_STAGE, SECOND_STAGE, THIRD_STAGE,FOURTH_STAGE, FIFTH_STAGE, SIXTH_STAGE,SEVENTH_STAGE,EIGHTH_STAGE, NONE
    }
    public STAGE_TYPE Step = STAGE_TYPE.NONE;


    public void OnEnable() {
        foreach (GameObject go in Flowers)
            go.SetActive(false);
        if (Step == STAGE_TYPE.NONE) return;
        Flowers[(int)Step].SetActive(true);
    }
    
    public void Change(STAGE_TYPE type) {
        Step = type;
        foreach (GameObject go in Flowers)
            go.SetActive(false);
        if (Step == STAGE_TYPE.NONE) return;
        Flowers[(int)Step].SetActive(true);
    }
}
