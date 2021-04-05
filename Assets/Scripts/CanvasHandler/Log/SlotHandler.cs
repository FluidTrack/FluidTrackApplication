using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour {
    public bool isUp = false;
    public int WaterCount = 0;
    public int DrinkCount = 0;
    public int PeeCount = 0;
    public int PooCount = 0;
    public bool isMeal = false;
    public bool isPooNoType = false;
    public LogSpriteHandler WaterTop;
    public LogSpriteHandler DrinkTop;
    public LogSpriteHandler PeeTop;
    public LogSpriteHandler PooTop;

    public void Clear() {
        WaterCount = 0;
        DrinkCount = 0;
        PeeCount = 0;
        PooCount = 0;
        WaterTop = null;
        DrinkTop = null;
        PeeTop = null;
        PooTop = null;
    }
}
