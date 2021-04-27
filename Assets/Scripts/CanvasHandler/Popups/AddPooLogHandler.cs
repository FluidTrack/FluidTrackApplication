using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPooLogHandler : MonoBehaviour
{
    public Button OkayButton;
    public bool isClicked;

    public void ClickTypeButton() {
        isClicked = true;
    }

    public void OnEnable() {
        OkayButton.interactable = false;
    }

    public void OnDisable() {
        isClicked = false;
    }

    public void Update() {
        OkayButton.interactable = isClicked;
    }
}
