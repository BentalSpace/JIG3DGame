using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastDoor : MonoBehaviour
{
    [SerializeField]
    Inventory inventory;

    public bool isBox;
    public bool isKnife;
    public int goldCnt;

    GameObject enemy;
    GameObject ClearZip;

    bool isOneAim;
    void Awake() {
        isBox = false;
        isKnife = false;
        goldCnt = 0;

        isOneAim = false;
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        ClearZip = GameObject.Find("ClearZip");
    }
    void Start() {
        ClearZip.SetActive(false);
    }
    public void OnAim() {
        if (!isOneAim) {
            isOneAim = true;
            Help.instance.help.text = "제물과 칼 그리고 금을 바쳐라....";
            Help.instance.HideText(2.0f);
        }
    }
    public void Interaction() {
        if(inventory.slots[inventory.clickSlot].item == null) {
            if (!isBox) {
                Help.instance.help.text = "제물을 바쳐라!!!";
            }
            else if (!isKnife) {
                Help.instance.help.text = "칼을 바쳐라!!!";
            }
            else if(goldCnt == 0) {
                Help.instance.help.text = "금을 바쳐라!!!";
            }
            else {
                // 클리어
                enemy.SetActive(false);
                Player.InCabinet = true;
                Help.instance.help.text = ".....문을 열어주마..";
                StartCoroutine(GameClear());
            }
            Help.instance.HideText(2.0f);
        }
        else {
            switch (inventory.slots[inventory.clickSlot].item.type) {
                case ObjectType.box:
                    Help.instance.help.text = "제물을 받았다...";
                    isBox = true;
                    break;
                case ObjectType.knife:
                    Help.instance.help.text = "칼을 받았다...";
                    isKnife = true;
                    break;
                case ObjectType.coin:
                    Help.instance.help.text = $"금을 {++goldCnt}개 받았다...";
                    break;
                default:
                    break;
            }
            Help.instance.HideText(2.0f);
            inventory.slots[inventory.clickSlot].ThrowItem();
        }
    }
    IEnumerator GameClear() {
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeCtrl.instance.FadeOut(80, false));

        yield return new WaitForSeconds(4f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ClearZip.SetActive(true);
    }
}
