using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAimTouch : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI itemNameTxt;
    [SerializeField]
    Image itemNameImg;
    [SerializeField]
    Inventory inventory;

    Cabinet playerInCabinet;
    bool inCabinet;
    void Update() {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Vector3 rayDir = Camera.main.transform.forward;
        int layerMask = 1 << 3;
        layerMask = ~layerMask;
        Debug.DrawRay(rayOrigin, rayDir * 5f, Color.magenta);

        RaycastHit hit;
        if(Physics.Raycast(rayOrigin, rayDir, out hit, 5f, layerMask)) {
            // 아이템에 마우스를 가져다 댈때.
            if (hit.collider.GetComponent<ObjectCtrl>()) {
                ObjectCtrl item = hit.collider.GetComponent<ObjectCtrl>();
                itemNameTxt.text = item.objectName;

                ItemNameImgSizeControl();

                //상호작용 키 추가
                if (Input.GetButtonDown("interaction")) {
                    inventory.AcquireItem(item);
                    item.gameObject.SetActive(false);
                }
            }
            // 문에 마우스를 가져다 댈때.
            else if(hit.collider.GetComponent<Door>()) {
                itemNameTxt.text = "문";
                ItemNameImgSizeControl();
                if (Input.GetButtonDown("interaction")) {
                    hit.collider.GetComponent<Door>().DoorCtrl();
                }
            }
            // 캐비닛에 마우스를 가져다 댈때.
            else if (hit.collider.GetComponent<Cabinet>()) {
                itemNameTxt.text = "캐비닛";
                ItemNameImgSizeControl();
                if (Input.GetButtonDown("interaction") && !playerInCabinet) {
                    Debug.Log("들어감");
                    playerInCabinet = hit.collider.GetComponent<Cabinet>();
                    playerInCabinet.CabinetCtrl(10);

                    StartCoroutine(CabinetEnter(10, hit.transform));

                    Player.InCabinet = true;
                    GetComponent<CapsuleCollider>().enabled = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().useGravity = false;

                }
            }
            // 최종문에 마우스를 가져다 댈때.
            else if (hit.collider.GetComponent<LastDoor>()) {
                hit.collider.GetComponent<LastDoor>().OnAim();

                if (Input.GetButtonDown("interaction")) {
                    hit.collider.GetComponent<LastDoor>().Interaction();
                }
            }
            else {
                itemNameTxt.text = "";
                itemNameImg.enabled = false;
            }
        }
        else {
            itemNameTxt.text = "";
            itemNameImg.enabled = false;
        }


        // 플레이어가 캐비닛 안에 들어가 있을 때 나오기
        if (Player.InCabinet && inCabinet) {
            if (Input.GetButtonDown("interaction") && playerInCabinet) {
                Debug.Log("나옴");
                playerInCabinet.CabinetCtrl(10);

                StartCoroutine(CabinetExit(10, playerInCabinet.transform));
                playerInCabinet = null;
            }
        }
    }
    IEnumerator CabinetEnter(float fadeTime, Transform hitTr) {
        yield return new WaitForSeconds(fadeTime * 0.05f);
        inCabinet = true;
        transform.position = hitTr.transform.position + (hitTr.forward * 0.5f) + (hitTr.up * 1.2f);
        transform.rotation = hitTr.transform.rotation;
        Camera.main.transform.rotation = transform.rotation;
    }
    IEnumerator CabinetExit(float fadeTime, Transform hitTr) {
        yield return new WaitForSeconds(fadeTime * 0.05f);
        Debug.Log("EXIT");
        inCabinet = false;
        transform.position = hitTr.transform.position + (hitTr.forward * 1.5f) + (hitTr.up * 1.1f);

        Player.InCabinet = false;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
    void ItemNameImgSizeControl() {
        if (itemNameTxt.GetRenderedValues(true).x >= 0.0f) {
            itemNameImg.enabled = true;
            Vector2 imgSize = itemNameTxt.GetRenderedValues(true);
            imgSize.x += 20f;
            imgSize.y += 10f;
            itemNameImg.rectTransform.sizeDelta = imgSize;
        }
    }
}
