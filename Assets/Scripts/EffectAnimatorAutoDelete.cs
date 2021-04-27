using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimatorAutoDelete : MonoBehaviour
{
    private Animator anim;

    public void OnEnable() {
        anim = this.GetComponent<Animator>();    
    }

    public void Update() {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
            Destroy(this.gameObject);
        }
    }

    public void OnDisable() {
        Destroy(this.gameObject);
    }
}
