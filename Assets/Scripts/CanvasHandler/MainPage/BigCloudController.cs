using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCloudController : MonoBehaviour
{
    public GameObject[] Clouds;

    public int weeks;
    public int day = 0;
    public int pass = 0;

    public void ChangeCloudState(int day) {
        weeks = Clouds.Length;
        this.day = day;

        if(day / 7 >= weeks) {
            foreach (GameObject go in Clouds)
                go.SetActive(false);
            pass = ( day - 1 ) / 7;
        } else { 
            pass = ( day - 1 ) / 7;
            for (int i = 0; i < pass; i++)
                Clouds[i].SetActive(false);
            for (int i = pass; i < weeks; i++)
                Clouds[i].SetActive(true);
        }

    }
}
