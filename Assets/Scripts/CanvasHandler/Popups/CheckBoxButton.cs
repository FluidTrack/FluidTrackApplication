using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoxButton : MonoBehaviour
{
    public GameObject statusIcon;
    private bool status = true;

    public void OnEnable() {
        statusIcon.SetActive(status);
    }

    public void ChangeStatus(bool value) {
        status = value;
        statusIcon.SetActive(status);
    }

    public void ButtonClick() {
        ChangeStatus(!status);
    }
}
