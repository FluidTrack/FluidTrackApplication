using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageGroundHandler : MonoBehaviour
{
    public Sprite[] Sprites;
    public int step = 0;

    public void ChangeStep(int step) {
        this.step = step;
        this.GetComponent<Image>().sprite = Sprites[step];
    }

    public void OnEnable() {
        ChangeStep(step);
    }
}
