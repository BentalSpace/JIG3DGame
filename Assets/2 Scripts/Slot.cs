using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviour
{
    public ObjectCtrl item;
    public TextMeshProUGUI itemName;

    public void AddItem(ObjectCtrl _item) {
        item = _item;
        itemName.text = _item.objectName;
    }

    public void ThrowItem() {
        item = null;
        itemName.text = "";
    }
}
