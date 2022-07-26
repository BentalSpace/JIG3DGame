using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Help : MonoBehaviour
{
    public TextMeshProUGUI help;

    public static Help instance;
    void Awake() {
        instance = this;
        help = GetComponent<TextMeshProUGUI>();
    }
    void Start() {
        help.text = "";
    }
    public void HideText(float hideTime) {
        help.color = Color.white;
        StopAllCoroutines();
        StartCoroutine(Hide(hideTime));
    }
    IEnumerator Hide(float hideTime) {
        Debug.Log("EXE");
        yield return new WaitForSeconds(hideTime);
        Debug.Log("GO");
        int progress = 0;
        int maxProgress = 20;
        while(progress < maxProgress) {
            progress++;
            help.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), (float)progress / maxProgress);
            Debug.Log("TEST");
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
