using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationHandler : MonoBehaviour
{
    public static NavigationHandler Instance;
    public GameObject DebugModeLogo;

    public void Awake() {
        Instance = this;
    }

    public void ModeSetting(bool mode) {
        DebugModeLogo.SetActive(mode);
    }
}
