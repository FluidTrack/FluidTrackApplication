using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeviceBatteryIconHandler : MonoBehaviour
{
    public Image Icon;
    public Text statusText;

    public Sprite FullSprite;
    public Sprite goodSprite;
    public Sprite warningSprite;
    public Sprite badSprite;

    public int currentValue;
    public int boundary_0 = 89;
    public int boundary_1 = 60;
    public int boundary_2 = 20;

    // Update is called once per frame
    void Update() {
#if UNITY_ANDROID
        currentValue = (int)( SystemInfo.batteryLevel * 100 );
#endif

        if (currentValue > boundary_0)       Icon.sprite = FullSprite;
        else if (currentValue > boundary_1) Icon.sprite = goodSprite;
        else if (currentValue > boundary_2) Icon.sprite = warningSprite;
        else Icon.sprite = badSprite;
        statusText.text = currentValue + "%";
    }
}
