using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeHandler : MonoBehaviour
{
    public Text GardenName;

    public void Start() {
        StartCoroutine(SetGardenName());
    }

    IEnumerator SetGardenName() {
        while(true) {
            yield return 0;
            if(DataHandler.User_isDataLoaded) {
                string middleName = DataHandler.User_name.Remove(0, 1);
                GardenName.text =
                    ( KoreanUnderChecker.UnderCheck(DataHandler.User_name) ) ?
                    middleName + "이네 정원" : middleName + "네 정원";
                break;
            }
        }
    }


}
