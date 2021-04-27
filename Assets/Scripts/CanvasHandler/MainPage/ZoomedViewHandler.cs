using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomedViewHandler : MonoBehaviour
{
    public int Week;
    public List<GameObject> NormalViewComponents;
    public List<GameObject> SmallestViewComponents;
    public BigCloudController BCC;
    public FlowerGroupController FlowersGroup;
    public int Day;

    public void ZoomChange(bool isZoomIn) {
        StartCoroutine(ZoomChangeCoroutine(isZoomIn));
    }

    public IEnumerator ZoomChangeCoroutine(bool isZoomIn) {
        CloudTransition.Instance.TransitionEffect(isZoomIn);
        
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject go in NormalViewComponents)
            go.SetActive(isZoomIn);
        foreach (GameObject go in SmallestViewComponents)
            go.SetActive(!isZoomIn);

        if (!isZoomIn) { 
            BCC.ChangeCloudState(Day);
            FlowersGroup.ChangeSprite(HomeHandler.Instance.totalWeek);
        } else {
            //HomeHandler.Instance.ReturnButtonClick(true);
        }
    }

    public void Awake() {
        foreach (GameObject go in NormalViewComponents)
            go.SetActive(true);
        foreach (GameObject go in SmallestViewComponents)
            go.SetActive(false);
    }

    public void OnDisable() {
        foreach (GameObject go in NormalViewComponents)
            go.SetActive(true);
        foreach (GameObject go in SmallestViewComponents)
            go.SetActive(false);
    }

    public void ResetButton() {
        foreach (GameObject go in NormalViewComponents)
            go.SetActive(true);
        foreach (GameObject go in SmallestViewComponents)
            go.SetActive(false);
        TouchAndMouseManager.Instance.isTouchEnable = true;
        TouchAndMouseManager.Instance.zoomFlag = false;
    }
}
