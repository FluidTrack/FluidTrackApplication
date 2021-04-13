using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCloudController : MonoBehaviour
{
    public GameObject[] CloudParents;
    public int weeks;
    public int day = 0;

    public void ChangeCloudState(int day) {
        weeks = CloudParents.Length;
        this.day = day;

        if (day / 7 >= weeks) {
            foreach (GameObject go in CloudParents)
                go.SetActive(false);
        } else {
            int current = ( day - 1 ) / 7;
            int detail = ( day - 1 ) % 7;
            for(int i = 0; i < weeks; i++) {
                if (i == current) {
                    CloudParents[i].SetActive(true);
                    if (detail != 0) {
                        detail--;
                        for (int j = 0; j < 6; j++) {
                            if (j <= detail)
                                 CloudParents[i].transform.GetChild(j).gameObject.SetActive(false);
                            else CloudParents[i].transform.GetChild(j).gameObject.SetActive(true);
                        }
                    }
                } else { CloudParents[i].SetActive(false); }
            }
        }

    }
}
