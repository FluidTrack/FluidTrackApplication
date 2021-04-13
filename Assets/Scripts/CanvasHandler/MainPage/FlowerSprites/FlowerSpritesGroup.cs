using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpritesGroup : MonoBehaviour
{
    public static FlowerSpritesGroup Instance;
    public static FlowerSpritesSet[] FlowerSprite = new FlowerSpritesSet[8];

    public void Awake() {
        FlowerSpritesSet[] set = this.GetComponentsInChildren<FlowerSpritesSet>();
        for(int i = 0; i < set.Length; i ++) {
            FlowerSprite[i] = set[i];
        }
    }
}
