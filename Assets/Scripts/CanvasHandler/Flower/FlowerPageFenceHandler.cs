using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerPageFenceHandler : MonoBehaviour
{
    int step = 8;
    public Color[] colors;
    public Image FenceLeft;
    public Image FenceRight;

    public void OnEnable() {
        ChangeColor(step);
    }

    public void ChangeColor(int step) {
        this.step = step;
        FenceLeft.color = colors[step];
        FenceRight.color = colors[step];
    }
}
