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
        // ���� ����
        // �Ȱ��� ������ ������ �Ǹ� �ȵ�.
        if (!createManager)
            return;

        // ������ ��� ���������� ������.
        List<Transform> createRemember = new List<Transform>();
        Transform createTr = GameObject.Find("BoxPoints").transform;
        foreach (Transform tr in createTr) {
            createRemember.Add(tr);
        }
        // �Է¹��� ������ �����ϰ� ������Ʈ ����
        for(int i = 0; i < createManager.boxCnt; i++) {
            int rand = Random.Range(0, createRemember.Count);
            Instantiate(box, createRemember[rand].position, Quaternion.identity);
            createRemember.RemoveAt(rand);
        }

        // Į ����
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

        // ���� ����
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
