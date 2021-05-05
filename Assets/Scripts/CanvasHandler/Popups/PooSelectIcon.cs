using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PooSelectIcon : MonoBehaviour
{
    public Image Image;
    public Text Description;
    public GameObject OtherText;
    public Sprite[] sprites;
    public string[] strings;
    public int index = 0;

    public void OnClicked() {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED3);
        SelectPooHandler.Instance.IconClick(index);
    }
}
