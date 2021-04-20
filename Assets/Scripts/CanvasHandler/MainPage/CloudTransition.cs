using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTransition : MonoBehaviour
{
    public static CloudTransition Instance;
    public Animator Anim;

    public void Awake() {
        Instance = this;
    }

    public void TransitionEffect(bool isZoomIn) {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.SMALLEST_VEIW);
        Anim.SetTrigger("Transition");
        TouchAndMouseManager.Instance.isTouchEnable = false;
        StartCoroutine(TouchEnable());
    }

    IEnumerator TouchEnable() {
        yield return new WaitForSeconds(1f);
        TouchAndMouseManager.Instance.isTouchEnable = true;
    }
}
