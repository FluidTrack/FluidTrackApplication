using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MongMongQuoteHandler : MonoBehaviour
{
    public GameObject[] Elements;
    public Text QuoteText;
    private Transform worldMap;
    private bool isClose = false;
    
    public void Start() {
        worldMap = this.transform.parent.parent;
    }

    public void Update() {
        if(worldMap.localScale.x <= 0.48 && !isClose) {
            foreach (GameObject go in Elements)
                go.SetActive(false);
            isClose = true;
        } else if (worldMap.localScale.x > 0.48 && isClose) {
            foreach (GameObject go in Elements)
                go.SetActive(true);
            isClose = false;
        }
    }
}
