using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarProgress : MonoBehaviour
{
    public Scrollbar scroll;
    public RectTransform thisTransform;
    private float maxValue = 0;

    private void OnEnable() {
        thisTransform = this.GetComponent<RectTransform>();
        maxValue = scroll.GetComponent<RectTransform>().sizeDelta.x - 6;
    }

    public void ChangeScrollValue() {
        thisTransform = this.GetComponent<RectTransform>();
        maxValue = scroll.GetComponent<RectTransform>().sizeDelta.x - 6;
        float value = scroll.value;
        thisTransform.offsetMax = new Vector2((value-1f) * maxValue,-4f);
    }
}
