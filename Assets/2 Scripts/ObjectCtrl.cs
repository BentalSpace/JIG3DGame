using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType { coin, knife, box, test }
public class ObjectCtrl : MonoBehaviour
{
    [SerializeField]
    public ObjectType type;
    public string objectName;
    GameObject spawnPointZip;

    [SerializeField]
    List<Transform> spawnPoints = new List<Transform>();
    void Awake() {
        switch (type) {
            case ObjectType.coin:
                spawnPointZip = GameObject.Find("CoinPoints");
                break;
            case ObjectType.knife:
                spawnPointZip = GameObject.Find("KnifePoints");
                break;
            case ObjectType.box:
                spawnPointZip = GameObject.Find("BoxPoints");
                break;
            default:
                break;
        }
    }
    void Start() {
        if (spawnPointZip) {
            foreach (Transform point in spawnPointZip.transform) {
                spawnPoints.Add(point);
            }
        }
        if (spawnPoints.Count > 0) {
            transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
    }
}
