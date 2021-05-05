using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPageMongMongHandler : MonoBehaviour
{
    public GameObject LeftQuote;
    public GameObject RightQuote;

    private MongMongQuoteHandler LeftQuoteHandler;
    private MongMongQuoteHandler RightQuoteHandler;
    private bool isRight = false;

    public void Awake() {
        LeftQuoteHandler = LeftQuote.GetComponent<MongMongQuoteHandler>();
        RightQuoteHandler = RightQuote.GetComponent<MongMongQuoteHandler>();
    }

    public void SelectQuoteDirection(bool isRight) {
        this.isRight = isRight;
        LeftQuote.SetActive(!isRight);
        RightQuote.SetActive(isRight);
    }
}
