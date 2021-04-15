using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollbarSound : MonoBehaviour, IPointerUpHandler {
    public void OnPointerUp(PointerEventData eventData) {
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CLICKED4);
    }
}
