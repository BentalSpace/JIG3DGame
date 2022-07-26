using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] objects;

    public ItemCreate createManager;

    [SerializeField]
    GameObject box;
    [SerializeField]
    GameObject knife;
    [SerializeField]
    GameObject gold;

    void Awake() {
        objects = GameObject.FindGameObjectsWithTag("Object");
        createManager = GameObject.Find("CreateManager")?.GetComponent<ItemCreate>();
    }
    void Start() {
        // 상자 생성
        // 똑같은 지점에 생성이 되면 안됨.
        if (!createManager)
            return;

        // 상자의 모든 스폰지점을 가져옴.
        List<Transform> createRemember = new List<Transform>();
        Transform createTr = GameObject.Find("BoxPoints").transform;
        foreach (Transform tr in createTr) {
            createRemember.Add(tr);
        }
        // 입력받은 개수로 랜덤하게 오브젝트 생성
        for(int i = 0; i < createManager.boxCnt; i++) {
            int rand = Random.Range(0, createRemember.Count);
            Instantiate(box, createRemember[rand].position, Quaternion.identity);
            createRemember.RemoveAt(rand);
        }

        // 칼 생성
        createRemember = new List<Transform>();
        createTr = GameObject.Find("KnifePoints").transform;
        foreach(Transform tr in createTr) {
            createRemember.Add(tr);
        }
        for(int i = 0; i < createManager.knifeCnt; i++) {
            int rand = Random.Range(0, createRemember.Count);
            Instantiate(knife, createRemember[rand].position, Quaternion.identity);
            createRemember.RemoveAt(rand);
        }

        // 동전 생성
        createRemember = new List<Transform>();
        createTr = GameObject.Find("CoinPoints").transform;
        foreach (Transform tr in createTr) {
            createRemember.Add(tr);
        }
        for(int i = 0; i < createManager.goldCnt; i++) {
            int rand = Random.Range(0, createRemember.Count);
            Instantiate(gold, createRemember[rand].position, Quaternion.identity);
            createRemember.RemoveAt(rand);
        }
    }
}
