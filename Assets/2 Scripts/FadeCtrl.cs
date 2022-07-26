using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCtrl : MonoBehaviour
{
    [SerializeField]
    Image fadeImg;

    public static FadeCtrl instance;
    Color nowColor;
    void Awake() {
        instance = this;
        nowColor = Color.black;
    }
    public void ColorChange(Color color) {
        fadeImg.color = color;
        nowColor = fadeImg.color;
    }
    void ColorBlackBack() {
        //fadeImg.color = Color.black;
        nowColor = Color.black;
    }
    public IEnumerator FadeOut(int fadeTime, bool isFadeIn) {
        // Á¡Á¡ ¾îµÓ°Ô
        int cnt = 0;
        int maxCnt = fadeTime;
        while(cnt < maxCnt) {
            cnt++;
            fadeImg.color = Color.Lerp(new Color(nowColor.r,nowColor.g,nowColor.b,0), new Color(nowColor.r, nowColor.g, nowColor.b, 1), (float)cnt / maxCnt);
            yield return new WaitForSeconds(0.05f);
        }
        if (isFadeIn) {
            StartCoroutine(FadeIn(20));
        }
        else {
            ColorBlackBack();
        }
    }
    IEnumerator FadeIn(int fadeTime) {
        // Á¡Á¡ ¹à°Ô
        int cnt = 0;
        int maxCnt = fadeTime;
        while (cnt < maxCnt) {
            cnt++;
            fadeImg.color = Color.Lerp(new Color(nowColor.r, nowColor.g, nowColor.b, 1), new Color(nowColor.r, nowColor.g, nowColor.b, 0), (float)cnt / maxCnt);
            yield return new WaitForSeconds(0.05f);
        }
        ColorBlackBack();
    }
}
