using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWaterIcon : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {

    public FlowerPageHandler pageHandler;
    public RectTransform TargetZone;
    public static float moveId = -10f;
    private RectTransform rect;
    private float garo, sero;
    private float garo_init, sero_init;

    private Vector2 initPosition;
    void OnEnable() {
        pageHandler = FlowerPageHandler.Instance;
        TargetZone = pageHandler.TargetZone;
        rect = this.GetComponent<RectTransform>();
        garo = rect.sizeDelta.x;
        sero = rect.sizeDelta.y;

        garo_init = (Screen.width - pageHandler.WaterSlot.GetComponent<RectTransform>().sizeDelta.x)/2;
        sero_init = ( Screen.height / 2 ) +   pageHandler.WaterSlot.GetComponent<RectTransform>().anchoredPosition.y
                                          - ( pageHandler.WaterSlot.GetComponent<RectTransform>().sizeDelta.y / 2 );
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(moveId == initPosition.x) {
            rect.anchoredPosition = new Vector2(eventData.position.x - ( garo / 2 ) - garo_init, eventData.position.y - ( sero / 2 ) - sero_init);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(moveId == initPosition.x) {
            Vector2 newPos = new Vector2(rect.anchoredPosition.x + (rect.sizeDelta.x / 2) + garo_init, rect.anchoredPosition.y + (rect.sizeDelta.y / 2) + sero_init);
            if(newPos.x >= TargetZone.anchoredPosition.x && newPos.x <= (TargetZone.anchoredPosition.x + TargetZone.sizeDelta.x) &&
               newPos.y >= TargetZone.anchoredPosition.y && newPos.y <= ( TargetZone.anchoredPosition.y + TargetZone.sizeDelta.y ) ) {
                pageHandler.Watering();
                moveId = -10f;
                Destroy(this.gameObject);
            } else {
                SoundHandler.Instance.Play_SFX(SoundHandler.SFX.ERROR);
                rect.anchoredPosition = initPosition;
            }
            moveId = -10f;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if (!FlowerPageHandler.Instance.isTouchAble) return;
        
        if(moveId == -10f) {
            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.POPED);
            moveId = initPosition.x;
        }
    }

    public void SetinitPos() {
        initPosition = rect.anchoredPosition;
    }
}
