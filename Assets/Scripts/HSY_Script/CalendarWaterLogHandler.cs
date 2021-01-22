
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarWaterLogHandler : MonoBehaviour
{
    SpriteRenderer sr;
    public Image basicimage_1;
    public Image basicimage_2;
    public Image basicimage_3;
    public Image basicimage_4;
    public Image basicimage_5;
    public Image basicimage_6;
    public Image basicimage_7;
    public Sprite water_0;
    public Sprite water_1;
    public Sprite water_2;
    public Sprite water_3;
    public Sprite water_4;
    public Sprite water_5;

    public Text day1;
    public Text day2;
    public Text day3;
    public Text day4;
    public Text day5;
    public Text day6;
    public Text day7;


    Dictionary<string, int> waterlog = new Dictionary<string, int>();

    //ONE Exception Handling MISSING!
    //If a day exists that user did not use the device, the record(0 cups of water) of day must be dealt in a different way. 
    void Start()
    {
        DataHandler.WaterLog[] waterlog_raw = DataHandler.GetTempWaterData();
        for(int i=0; i < waterlog_raw.Length; i++)
        {
            string day = waterlog_raw[i].timestamp;
            if (waterlog.ContainsKey(day))
            {
                try {
                    waterlog[day]++; }
                catch (System.Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            else
            {
                waterlog.Add(day, 1);
            }
        }

        Image[] Images_7days = new Image[7]{ basicimage_1, basicimage_2, basicimage_3, basicimage_4, basicimage_5, basicimage_6, basicimage_7 };

        int orderof7Days = 1;
        foreach(var log in waterlog) //7days scope images will be changed
        {
            if (orderof7Days > 7) { break;}
            string day = log.Key;
            int amounts = log.Value; 
            Sprite water_today;
            switch(amounts) //water - image mapping 
            {
                case 0:
                    water_today = water_0;
                    break;
                case 1:
                    water_today = water_1;
                    break;
                case 2:
                    water_today = water_2;
                    break;
                case 3:
                    water_today = water_3;
                    break;
                case 4:
                    water_today = water_4;
                    break;
                case 5:
                    water_today = water_5;
                    break;
                default:
                    water_today = water_5;
                    break;
            }

            switch (orderof7Days)
            {
                case 1:
                    basicimage_1.sprite = water_today;
                    day1.text = day;
                    break;
                case 2:
                    basicimage_2.sprite = water_today;
                    day2.text = day;
                    break;
                case 3:
                    basicimage_3.sprite = water_today;
                    day3.text = day;
                    break;
                case 4:
                    basicimage_4.sprite = water_today;
                    day4.text = day;
                    break;
                case 5:
                    basicimage_5.sprite = water_today;
                    day5.text = day;
                    break;
                case 6:
                    basicimage_6.sprite = water_today;
                    day6.text = day;
                    break;
                case 7:
                    basicimage_7.sprite = water_today;
                    day7.text = day;
                    break;
                default:
                    break;
            }
            orderof7Days++;
            
        }


    }

}
