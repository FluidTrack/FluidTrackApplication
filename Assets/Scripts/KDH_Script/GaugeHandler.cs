using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeHandler : MonoBehaviour
{
    public InputField inputField;
    public Image BeforeImage;
    public Sprite Sprite1;
    public Sprite Sprite2;
    public Sprite Sprite3;
    public Sprite Sprite4;
    public Sprite Sprite5;
    public int Boundary1;
    public int Boundary2;
    public int Boundary3;
    public int Boundary4;

    public void ChangeGauge() {
      if (int.Parse(inputField.text) <= Boundary1)
      {
        BeforeImage.sprite = Sprite1;
      }
      else if (int.Parse(inputField.text) <= Boundary2)
      {
        BeforeImage.sprite = Sprite2;
      }
      else if (int.Parse(inputField.text) <= Boundary3)
      {
        BeforeImage.sprite = Sprite3;
      }
      else if (int.Parse(inputField.text) <= Boundary4)
      {
        BeforeImage.sprite = Sprite4;
      }
      else
      {
        BeforeImage.sprite = Sprite5;
      }
    }

}
