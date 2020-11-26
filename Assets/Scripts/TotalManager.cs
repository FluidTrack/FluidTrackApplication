using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalManager : MonoBehaviour
{
    public GameObject OpeningCanvas;
    public GameObject[] OtherCanvas;

    public bool SkipOpening;

    public void Start() {
        OpeningCanvas.SetActive(!SkipOpening);
        foreach (GameObject go in OtherCanvas)
            go.SetActive(SkipOpening);
    }
}
