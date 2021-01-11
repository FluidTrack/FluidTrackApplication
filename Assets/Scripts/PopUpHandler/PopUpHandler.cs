using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpHandler : MonoBehaviour
{
    public Text SubText;
    public Animator anim;

    public void Start() {
        anim = this.GetComponent<Animator>();
    }
}
