using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeviceBatteryIconHandler : MonoBehaviour
{
    public Image Icon;
    public Color goodColor;
    public Color warningColor;
    public Color badColor;

    public Sprite goodSprite;
    public Sprite warningSprite;
    public Sprite badSprite;

    public int currentValue;
    public int boundary_1 = 60;
    public int boundary_2 = 20;

    // Update is called once per frame
    void Update() {
        if(currentValue > boundary_1) {
            Icon.color = goodColor;
            Icon.sprite = goodSprite;
        } else if (currentValue > boundary_2) {
            Icon.color = warningColor;
            Icon.sprite = warningSprite;
        } else {
            Icon.color = badColor;
            Icon.sprite = badSprite;
        }
    }
}
