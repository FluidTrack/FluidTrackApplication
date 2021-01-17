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
      currentValue = int.Parse(inputField.text);
      valueText.text = currentValue.ToString();
    }
}
