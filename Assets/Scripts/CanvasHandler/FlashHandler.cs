using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashHandler : MonoBehaviour
{
    public GameObject parents;
    public Animator PlayAnimator;

    public void OnEnable() {
        parents = gameObject.transform.parent.gameObject;
        PlayAnimator = parents.GetComponent<Animator>();
    }

    public void EndAnim() {
        Destroy(parents);
    }

    public void Update() {
        if (EndAnimationDone())
            EndAnim();
    }

    bool EndAnimationDone() {
        return PlayAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
            PlayAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;

    }

}
