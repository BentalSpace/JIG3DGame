using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public GameObject cabinetInObject;

    bool inCabinet;
    void Awake() {
        cabinetInObject = Camera.main.transform.GetChild(0).gameObject;
    }
    public void CabinetCtrl(float fadeTime) {
        if (!inCabinet) {
            StartCoroutine(InputCabinet(fadeTime));
        }
        else {
            StartCoroutine(OutputCabinet(fadeTime));
        }
    }

    IEnumerator InputCabinet(float fadeTime) {
        inCabinet = true;
        FadeCtrl.instance.StopAllCoroutines();
        StartCoroutine(FadeCtrl.instance.FadeOut(10, true));
        yield return new WaitForSeconds(fadeTime * 0.05f);
        cabinetInObject.SetActive(true);
    }
    public IEnumerator OutputCabinet(float fadeTime) {
        inCabinet = false;
        FadeCtrl.instance.StopAllCoroutines();
        StartCoroutine(FadeCtrl.instance.FadeOut(10, true));
        yield return new WaitForSeconds(fadeTime * 0.05f);
        cabinetInObject.SetActive(false);
    }
}
