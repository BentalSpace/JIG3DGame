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
            // �����ۿ� ���콺�� ������ �.
            if (hit.collider.GetComponent<ObjectCtrl>()) {
                ObjectCtrl item = hit.collider.GetComponent<ObjectCtrl>();
                itemNameTxt.text = item.objectName;

                ItemNameImgSizeControl();

                //��ȣ�ۿ� Ű �߰�
                if (Input.GetButtonDown("interaction")) {
                    inventory.AcquireItem(item);
                    item.gameObject.SetActive(false);
                }
            }
            // ���� ���콺�� ������ �.
            else if(hit.collider.GetComponent<Door>()) {
                itemNameTxt.text = "��";
                ItemNameImgSizeControl();
                if (Input.GetButtonDown("interaction")) {
                    hit.collider.GetComponent<Door>().DoorCtrl();
                }
            }
            // ĳ��ֿ� ���콺�� ������ �.
            else if (hit.collider.GetComponent<Cabinet>()) {
                itemNameTxt.text = "ĳ���";
                ItemNameImgSizeControl();
                if (Input.GetButtonDown("interaction") && !playerInCabinet) {
                    Debug.Log("��");
                    playerInCabinet = hit.collider.GetComponent<Cabinet>();
                    playerInCabinet.CabinetCtrl(10);

                    StartCoroutine(CabinetEnter(10, hit.transform));

                    Player.InCabinet = true;
                    GetComponent<CapsuleCollider>().enabled = false;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    GetComponent<Rigidbody>().useGravity = false;

                }
            }
            // �������� ���콺�� ������ �.
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


        // �÷��̾ ĳ��� �ȿ� �� ���� �� ������
        if (Player.InCabinet && inCabinet) {
            if (Input.GetButtonDown("interaction") && playerInCabinet) {
                Debug.Log("����");
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
