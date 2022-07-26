using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public Slot[] slots;
    [HideInInspector]
    public int clickSlot;

    Transform playerTr;
    void Awake() {
        slots = GetComponentsInChildren<Slot>();
        playerTr = GameObject.Find("Player").transform;
        clickSlot = 0;
    }
    void Update() {
        // 슬롯 선택
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SlotClickUpdate(0, clickSlot);
            clickSlot = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SlotClickUpdate(1, clickSlot);
            clickSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SlotClickUpdate(2, clickSlot);
            clickSlot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SlotClickUpdate(3, clickSlot);
            clickSlot = 3;
        }

        if (Input.GetKeyDown(KeyCode.Q)){
            ThrowItem();
        }
    }
    void SlotClickUpdate(int _clickSlot, int agoSlot) {
        slots[agoSlot].GetComponent<Image>().color = new Color32(150, 150, 150, 120);
        slots[agoSlot].GetComponent<RectTransform>().localScale = Vector3.one;

        slots[_clickSlot].GetComponent<Image>().color = new Color32(60, 60, 60, 255);
        slots[_clickSlot].GetComponent<RectTransform>().localScale = Vector3.one * 1.2f;
    }
    public void AcquireItem(ObjectCtrl item) {
        for(int i = 0; i < slots.Length; i++) {
            if(slots[i].item == null) {
                slots[i].AddItem(item);
                Help.instance.help.text = "아이템 획득";
                Help.instance.HideText(1.0f);
                return;
            }
        }
        // 인벤토리가 꽉 차있다.
        Help.instance.help.text = "인벤토리가 꽉 차있습니다.";
        Help.instance.HideText(1.0f);
    }
    public void ThrowItem() {
        if(slots[clickSlot].item != null) {
            slots[clickSlot].item.gameObject.transform.position = playerTr.position + playerTr.forward;
            slots[clickSlot].item.gameObject.transform.rotation = Quaternion.identity;
            slots[clickSlot].item.gameObject.SetActive(true);
            slots[clickSlot].item.GetComponent<Rigidbody>().AddForce(playerTr.forward * 5, ForceMode.Impulse);
        }
        slots[clickSlot].ThrowItem();
    }
}
