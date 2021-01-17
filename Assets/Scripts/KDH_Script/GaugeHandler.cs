using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeHandler : MonoBehaviour
{
    public InputField inputField;
    public Image beforeImage;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Sprite sprite5;

    public void ChangeGauge() {
      if (int.Parse(inputField.text) <= 0)
      {
        beforeImage.sprite = sprite1;
      }
      else if (int.Parse(inputField.text) <= 2)
      {
        beforeImage.sprite = sprite2;
      }
      else if (int.Parse(inputField.text) <= 10)
      {
        beforeImage.sprite = sprite3;
      }
      else if (int.Parse(inputField.text) <= 30)
      {
        beforeImage.sprite = sprite4;
      }
      else
      {
        beforeImage.sprite = sprite5;
      }
    }

}
