using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WelcomHandler : MonoBehaviour
{
    public InputField nameInputField;
    public Text nameText;
    public Animator confirmName;
    public Animator anim;
    internal bool confirmAlert = false;

    // Update is called once per frame
    void Update()
    {
        if(nameInputField.isFocused) {
            if(!anim.GetBool("Active"))
                anim.SetBool("Active", true);
        } else {
            if(anim.GetBool("Active"))
                anim.SetBool("Active", false);
        }
    }

    public void nameOkayButton() {
        if (confirmAlert) return;
        if(nameInputField.text == "") {
            anim.SetTrigger("Reject");
            return;
        }
        nameText.text = nameInputField.text;
        confirmName.SetTrigger("active");
        confirmAlert = true;
    }

    public void nameConfirmButton() {
        DataHandler.User_name = nameInputField.text;
        confirmName.SetTrigger("inactive");
        confirmAlert = false;
        Invoke("selfDestruction", 0.16f);
        TotalManager.instance.OtherCanvas[(int)TotalManager.CANVAS.WELCOME2].SetActive(true);
    }

    public void selfDestruction() {
        this.gameObject.SetActive(false);
    }

    public void nameCancelButton() {
        confirmName.SetTrigger("inactive");
        confirmAlert = false;
    }
}
