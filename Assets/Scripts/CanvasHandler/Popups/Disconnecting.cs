using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disconnecting : MonoBehaviour
{
    public void OnEnable() {
        StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle() {
        yield return new WaitForSeconds(3f);
        this.gameObject.SetActive(false);
    }
}
