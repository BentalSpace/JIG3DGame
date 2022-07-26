using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool isOpen;
    public bool isBroken;

    int curCnt;
    void Start() {
    }
    void Update() {
    }
    public void DoorCtrl() {
        StopAllCoroutines();
        if(!isOpen)
            StartCoroutine(Open());
        else
            StartCoroutine(Close());
    }
    IEnumerator Open() {
        int power = 3;
        int maxCnt = 90 / power;
        isOpen = true;

        while (maxCnt > curCnt) {
            transform.Rotate(-Vector3.up * 3);
            curCnt++;
            yield return new WaitForSeconds(0.03f);
        }
        //while (transform.localEulerAngles.y < 90) {
        //    transform.Rotate(Vector3.up * 3);
        //    yield return new WaitForSeconds(0.03f);
        //}
    }
    IEnumerator Close() {
        int power = 3;
        int maxCnt = 90 / power;
        isOpen = false;
        while(0 < curCnt) {
            transform.Rotate(Vector3.up * power);
            curCnt--;
            yield return new WaitForSeconds(0.03f);
        }
    }
    public void HitDoor() {
        StartCoroutine(HitToEnemy());
    }
    IEnumerator HitToEnemy() {
        int power = 6;
        int curCnt = 0;
        int maxCnt = 90/power;

        while(maxCnt > curCnt) {
            transform.Rotate(Vector3.left * power);
            curCnt++;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<BoxCollider>().enabled = false;
        isBroken = true;
    }
}
