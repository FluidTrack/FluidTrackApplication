
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarWaterLogHandler : MonoBehaviour
{
    SpriteRenderer sr;
    public Image testimage;
    public Sprite water_0;
    public Sprite water_1;
    public Sprite water_2;
    public Sprite water_3;
    public Sprite water_4;
    public Sprite water_5;

    public InputField i;
    public Text t;
    private int water;


    public void change()
    {
        Debug.Log(water);
        switch (water)
        {
            case 0:
                testimage.sprite = water_0;
                break;
            case 1:
                testimage.sprite = water_1;
                break;
            case 2:
                testimage.sprite = water_2;
                break;
            case 3:
                testimage.sprite = water_3;
                break;
            case 4:
                testimage.sprite = water_4;
                break;
            case 5:
                testimage.sprite = water_5;
                break;
            default:
                testimage.sprite = water_5;
                break;

        }
    }
    public void HSY_input()
    {
        water = int.Parse(i.text);
        change();
    }

}
