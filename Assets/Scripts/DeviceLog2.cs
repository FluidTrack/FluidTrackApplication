using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceLog2 : MonoBehaviour
{
    public Text Name;
    public Text Address;

    public void Init(int index, string name, string address, bool isMoaband) {
        Name.text = name;
        Address.text = address;

        this.GetComponent<RectTransform>().transform.localPosition = new Vector2(0f, -80f * index);
        Address.text = address;
        if (isMoaband) {
            try {
                Name.text = "모아밴드 _ " + name.Split('_')[1];
            } catch (System.Exception e) {
                e.ToString();
                Name.text = "모아밴드 _ ??";
            }
        }
    }
}
