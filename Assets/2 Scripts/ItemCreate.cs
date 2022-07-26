using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCreate : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI boxTxt;
    [SerializeField]
    TextMeshProUGUI knifeTxt;
    [SerializeField]
    TextMeshProUGUI goldTxt;

    public int boxCnt = 0;
    public int knifeCnt = 0;
    public int goldCnt = 0;

    public void CntRemember() {
        // 박스 개수
        switch (boxTxt.text.Length) {
            case 2:
                boxCnt = int.Parse(boxTxt.text[0].ToString());
                break;
            case 3:
                string a = boxTxt.text[0].ToString() + boxTxt.text[1].ToString();
                boxCnt = int.Parse(a);
                if(boxCnt > 20) {
                    boxCnt = 20;
                }
                break;
            default:
                boxCnt = 1;
                break;
        }
        // 칼 개수
        switch (knifeTxt.text.Length) {
            case 2:
                knifeCnt = int.Parse(knifeTxt.text[0].ToString());
                break;
            case 3:
                string a = knifeTxt.text[0].ToString() + knifeTxt.text[1].ToString();
                knifeCnt = int.Parse(a);
                if (knifeCnt > 18) {
                    knifeCnt = 18;
                }
                break;
            default:
                knifeCnt = 1;
                break;
        }
        // 동전 개수
        switch (goldTxt.text.Length) {
            case 2:
                goldCnt = int.Parse(goldTxt.text[0].ToString());
                break;
            case 3:
                string a = goldTxt.text[0].ToString() + goldTxt.text[1].ToString();
                goldCnt = int.Parse(a);
                if(goldCnt > 10) {
                    goldCnt = 10;
                }
                break;
            default:
                goldCnt = 1;
                break;
        }
    }
}
