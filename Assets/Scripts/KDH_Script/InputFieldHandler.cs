using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour
{
    public Text valueText;
    public InputField inputField;

    public int currentValue;

    public void changevalue() {
      try {
        currentValue = int.Parse(inputField.text);
      }
      catch (System.Exception e){
        Debug.Log(e.ToString());
        currentValue = 0;
        inputField.text = "0";
      }
      valueText.text = currentValue.ToString();
    }
}
