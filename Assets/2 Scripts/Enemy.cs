using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Enemy : MonoBehaviour
{
    enum State { Idle, Move, Chase, Attack, Die}
    [SerializeField]
    State state;

    Transform initPos;

    // �������ͽ�
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;

    // �� Ȯ��
    [SerializeField]
    Transform eye;
    [SerializeField, Tooltip("�þ� ��")]
    float fieldOfView;
    [SerializeField, Tooltip("���� �ν� �� �� �ִ� �ִ� �Ÿ�")]
    float viewDistance;
    [SerializeField, Tooltip("�߰����϶� �ν��� �� �ִ� �ִ� �Ÿ�")]
    float chaseViewDistance;
    [SerializeField, Tooltip("�߰����϶� �þ߰�")]
    float chaseFieldOfView;
    bool isMiss;

    //�� ����
    [SerializeField]
    Transform atkRoot;
    [SerializeField, Tooltip("���� ������")]
    float atkRadius;
    float atkDistance;

    [SerializeField]
    float forceGravity;

    Vector3 targetEntity;
    Transform targetEntityTr;

    // �����̴� ��ǥ
    public List<Transform> movePoints = new List<Transform>();

    bool PlayerHouseSee;
    bool isHitHouse;

    // �����
    [Header("BackgroundSound")]
    [SerializeField]
    AudioClip normalBackgroundSound;
    [SerializeField]
    AudioClip chaseBackgroundSound;

    [Header("MonsterSound")]
    [SerializeField]
    AudioClip soundIdle;
    [SerializeField]
    AudioClip soundMove;
    [SerializeField]
    AudioClip soundScream;
    [SerializeField]
    AudioClip soundAttack;

    // Components
    Rigidbody rigid;
    Animator anim;
    NavMeshAgent nma;
    AudioSource monsterSound;
    AudioSource backgroundSound;

    GameObject overZip;

    void Awake() {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        monsterSound = GetComponent<AudioSource>();
        backgroundSound = Camera.main.GetComponent<AudioSource>();

        initPos = GameObject.Find("initailPos").transform;
        atkDistance = Vector3.Distance(transform.position, new Vector3(atkRoot.position.x, transform.position.y, atkRoot.position.z)) + atkRadius;
        foreach(Transform tr in GameObject.Find("EnemyMovePoint").transform) {
            movePoints.Add(tr);
        }
        targetEntityTr = GameObject.Find("Player").transform;
        overZip = GameObject.Find("OverZip");
    }
    void Start() {
        walkSpeed = 10;
        runSpeed = 20;
        nma.isStopped = true;
        overZip.SetActive(false);
        StartCoroutine(Idle());
    }
    void FixedUpdate() {
        //RaycastHit hit;

        // ���� 10�� �Ÿ� �տ� �� Ȥ�� ���� �ִٸ�
        //int layerMask = (1 << 7) + (1 << 8);
        //if (state == State.Move) {
        //    if (Physics.SphereCast(eye.position, 2, eye.transform.forward, out hit, 10, layerMask)) {
        //        StopAllCoroutines();
        //        nma.speed = walkSpeed;
        //        nma.isStopped = false;
        //        nma.SetDestination(initPos.position);

        //        // #### �ʱ������� �������� ��?
        //    }
        //}

        // �߰� �߷� ����
        rigid.AddForce(Vector3.down * forceGravity);
    }
    void Update() {
        // ���� �߰����� ��
        if (state == State.Move || state == State.Idle) {
            if (!Player.InCabinet) {
                var collider = Physics.OverlapSphere(eye.position, viewDistance, 1 << 3);

                if (collider.Length > 0) {
                    foreach (var col in collider) {
                        if (!isTargetOnSight(col.transform, viewDistance, fieldOfView))
                            continue;
                        isMiss = false;
                        targetEntity = col.transform.position;
                        StopAllCoroutines();
                        StartCoroutine(Chase());
                    }
                }
            }
        }
        // �߰� ��ġ ����
        if(state == State.Chase && !isHitHouse) {
            if (!Player.InCabinet) {
                if (!isMiss) {
                    var collider = Physics.OverlapSphere(eye.position, chaseViewDistance, 1 << 3);

                    if (collider.Length > 0) {
                        foreach (var col in collider) {
                            if (!isTargetOnSight(col.transform, chaseViewDistance, chaseFieldOfView))
                                continue;
                            if (Player.inHouse) {
                                // �÷��̾ ���� ���� Ȯ��.
                                // �Ϲ� �þ߰Ÿ����� �߰� �þ߰Ÿ��� �� ª�� �Ѿƿ��� �־, ���� ���°� Ȯ�� ���� ���� ����.
                                PlayerHouseSee = true;
                                targetEntity = Player.PlayerInHouseObject.transform.GetChild(Player.PlayerInHouseObject.transform.childCount - 1).transform.position;
                            }
                            else {
                                PlayerHouseSee = false;
                                targetEntity = col.transform.position;
                            }
                            isMiss = false;
                            StopAllCoroutines();
                            StartCoroutine(Chase());
                        }
                    }
                }

                // �߰� �� ���ƴٸ�, �þ߸� �� ��� ����.
                else if (isMiss) {
                    var collider = Physics.OverlapSphere(eye.position, viewDistance, 1 << 3);

                    if (collider.Length > 0) {
                        foreach (var col in collider) {
                            if (!isTargetOnSight(col.transform, viewDistance, chaseFieldOfView))
                                continue;
                            if (Player.inHouse) {
                                // �÷��̾ ���� ���� Ȯ��.
                                // �Ϲ� �þ߰Ÿ����� �߰� �þ߰Ÿ��� �� ª�Ƽ� �Ѿƿ��� ������, ���� ���°� Ȯ�� ���� ���� ����.
                                PlayerHouseSee = true;
                                targetEntity = Player.PlayerInHouseObject.transform.GetChild(Player.PlayerInHouseObject.transform.childCount - 1).transform.position;
                            }
                            else {
                                PlayerHouseSee = false;
                                targetEntity = col.transform.position;
                            }
                            isMiss = false;
                            StopAllCoroutines();
                            StartCoroutine(Chase());
                        }
                    }
                }
            }
        }
        Debug.Log("�÷��̾ ���� �� �� Ȯ����. " + PlayerHouseSee);
        Debug.Log("�÷��̾ �� �ȿ� ����. " + Player.inHouse);
        // ���� �Ÿ��� ���� ��
        if (state == State.Chase) {
            if (!Player.InCabinet) {
                if (Vector3.Distance(targetEntityTr.position, transform.position) <= atkDistance) {
                    // ����
                    Debug.Log("���� ���ٰŸ�");
                    int layerMask = (1 << 9) | (1 << 12);
                    layerMask = ~layerMask;
                    Vector3 rayDir = targetEntityTr.position - transform.position;
                    RaycastHit hit;

                    // Ÿ�ٿ� �� ���̿� ���� ������ �ȵ�.
                    if (Physics.Raycast(transform.position, rayDir, out hit, 100, layerMask)){
                        if (hit.collider.CompareTag("Player")) {
                            StopAllCoroutines();
                            StartCoroutine(Attack());
                        }
                    }
                }
            }
        }

        rigid.velocity = Vector3.zero;
        if (!nma.isStopped) {
            Vector3 lookrotation = nma.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), 80 * Time.deltaTime);
        }
        else {
            nma.velocity = Vector3.zero;
        }
    }
    void LateUpdate() {
        if(state == State.Idle || state == State.Move || state == State.Die) {
            if(backgroundSound.clip != normalBackgroundSound) { 
                backgroundSound.clip = normalBackgroundSound;
                backgroundSound.Play();
            }
        }
        else {
            if (backgroundSound.clip != chaseBackgroundSound) {
                backgroundSound.clip = chaseBackgroundSound;
                backgroundSound.Play();
            }
        }
    }
    bool isTargetOnSight(Transform target, float targetViewDis, float viewAngle) {
        RaycastHit hit;
        Vector3 dir = target.position - eye.position;
        dir.y = eye.position.y;

        // �þ߰��� ��� ������ �ȵ�
        if(Vector3.Angle(dir, eye.forward) > viewAngle * 0.5f) {
            return false;
        }
        dir = target.position - eye.position;
        // �þ߰� ���� �ִ��� ��ֹ��� ������ ������ �ȵ�
        Debug.DrawRay(eye.position, dir * 10f, Color.yellow);
        int layerMask = 1 << 12;
        layerMask = ~layerMask;
        Physics.Raycast(eye.position, dir, out hit, targetViewDis, layerMask);
        if (Physics.Raycast(eye.position, dir, out hit, targetViewDis, layerMask)) {
            if (hit.transform == target) {
                return true;
            }
        }
        return false;
    }
    List<Transform> pointsSearch() {
        List<Transform> returnTr = new List<Transform>();
        foreach (Transform point in movePoints) {
            if (Vector3.Distance(point.position, targetEntityTr.position) < 70) {
                returnTr.Add(point);
            }
        }
        return returnTr;
    }
    IEnumerator Idle() {
        monsterSound.loop = false;
        state = State.Idle;
        nma.isStopped = true;
        nma.velocity = Vector3.zero;
        float randTime;
        //�ݹ��� Ȯ���� ����, ������ ȸ������ Ž��
        int pattern = Random.Range(0, 2);
        if (pattern == 0) {
            monsterSound.clip = soundIdle;
            monsterSound.Play();

            anim.SetBool("turnRight", true);
            randTime = Random.Range(2f, 4f);
            float curTime = 0;
            while (randTime > curTime) {
                transform.Rotate(Vector3.up * Time.deltaTime * 20);
                curTime += Time.deltaTime;
                yield return null;
            }
            anim.SetBool("turnRight", false);
        }
        else {
            monsterSound.clip = soundIdle;
            monsterSound.Play();

            anim.SetBool("turnLeft", true);
            randTime = Random.Range(2f, 4f);
            float curTime = 0;
            while (randTime > curTime) {
                transform.Rotate(Vector3.down * Time.deltaTime * 20);
                curTime += Time.deltaTime;
                yield return null;
            }
            anim.SetBool("turnLeft", false);
        }
        //���� �ð� ��� ��
        randTime = Random.Range(1f, 5f);
        yield return new WaitForSeconds(randTime);

        //���� ���� ����
        pattern = Random.Range(0, 3);
        if (pattern <= 1) {
            // 66% �� �̵�
            List<Transform> randTr = pointsSearch();
            int randPos = Random.Range(0, randTr.Count);
            Vector3 targetVec = randTr[randPos].position;

            StartCoroutine(Move(targetVec));
        }
        else {
            // 33%�� ���ڸ��� �ֱ�
            StartCoroutine(Idle());
        }
    }
    IEnumerator Move(Vector3 targetVec) {
        monsterSound.loop = true;
        state = State.Move;
        anim.SetBool("turnLeft", false);
        anim.SetBool("turnRight", false);
        anim.SetBool("isRun", false);
        anim.SetBool("isWalk", true);

        nma.SetDestination(targetVec);
        nma.isStopped = false;
        nma.speed = walkSpeed;
        monsterSound.clip = soundMove;
        monsterSound.Play();
        while (true) {
            // ��ǥ���� ����
            if (Vector3.Distance(transform.position, targetVec) <= 3) {
                monsterSound.loop = false;
                nma.isStopped = true;
                anim.SetBool("isWalk", false);
                monsterSound.clip = soundIdle;
                monsterSound.Play();
                // �����ð� �޽� ��
                float randTime = Random.Range(1, 4f);
                yield return new WaitForSeconds(randTime);

                // ���� ���� ����
                int pattern = Random.Range(0, 3);
                if (pattern <= 1) {
                    // 66% �� �̵�
                    List<Transform> randTr = pointsSearch();
                    int randPos = Random.Range(0, randTr.Count);
                    targetVec = randTr[randPos].position;

                    StartCoroutine(Move(targetVec));
                }
                else {
                    // 33%�� ���ڸ��� �ֱ�
                    StartCoroutine(Idle());
                }
                yield break;
            }
            yield return null;
         }
    }
    IEnumerator rotate(float rotY) {
        float progress = 0;
        float maxProgress = transform.eulerAngles.y - rotY;
        maxProgress = maxProgress > 0 ? maxProgress : -maxProgress;
        while (progress < maxProgress) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotY, 0), progress/maxProgress);
            progress++;
            yield return new WaitForSeconds(0.03f);
        }
    }
    IEnumerator Chase() {
        monsterSound.loop = false;
        state = State.Chase;
        nma.speed = runSpeed;
        nma.isStopped = false;
        // �÷��̾� �߰� �������� �޷���.
        nma.SetDestination(targetEntity);
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", true);

        // �������� Ȯ���� ��ġ���� �߰��� ������, �÷��̾��� ����� ������ ������.
        float chaseMaxTime = 5f;
        float chaseCurTime = 0;
        while (chaseMaxTime > chaseCurTime) {
            // ��ǥ������ ���� �������� ��.
            if (Vector3.Distance(transform.position, targetEntity) <= 3) {
                nma.isStopped = true;
                anim.SetBool("isRun", false);
                nma.velocity = Vector3.zero;

                // ���� �� �� Ȯ�� ������.
                if (PlayerHouseSee) {
                    Debug.Log("���� ���� �� �ô�.");
                    // ���� �μŶ�.
                    isHitHouse = true;
                    StartCoroutine(rotate(Player.PlayerInHouseObject.transform.GetChild(Player.PlayerInHouseObject.transform.childCount - 1).eulerAngles.y));
                    yield return new WaitForSeconds(0.7f);
                    if (!Player.PlayerInHouseObject.GetComponentInChildren<Door>().isBroken) {
                        anim.SetTrigger("ClawAttack");
                        yield return new WaitForSeconds(1.4f);
                        Player.PlayerInHouseObject.GetComponentInChildren<Door>().HitDoor();
                        yield return new WaitForSeconds(1.9f);
                        anim.SetTrigger("Scream");
                        yield return new WaitForSeconds(1.2f);
                        monsterSound.loop = true;
                        monsterSound.clip = soundScream;
                        monsterSound.Play();

                        yield return new WaitForSeconds(1.6f);
                        monsterSound.loop = false;
                        monsterSound.Stop();
                        StartCoroutine(Move(initPos.position));
                        isHitHouse = false;
                        yield break;
                    }
                    else {
                        // ���� �̹� �μ��� ���� ��.
                        isHitHouse = true;
                        anim.SetTrigger("Scream");
                        yield return new WaitForSeconds(2.8f);

                        runSpeed += 2.5f;
                        walkSpeed += 1.5f;
                        StartCoroutine(Move(initPos.position));
                        isHitHouse = false;
                        yield break;
                    }
                }

                // ��ǥ Ÿ��(�÷��̾�)�� ������ ��.
                else {
                    // ���� : [ȸ���ϸ鼭 ���� Ȯ��(����, ������) / �� �μ���(���� �� ����) Ȥ�� �� �� Ȯ��]

                    // ������ 15 �̳��� �� �� ������ 1�� Ȯ��
                    int rand = 0;
                    var houses = Physics.OverlapSphere(transform.position, 15, 1 << 8);
                    List<GameObject> houseObjects = new List<GameObject>();

                    // ������ ����Ʈ�� �ֱ�
                    foreach (var house in houses) {
                        GameObject par = house.transform.parent.gameObject;
                        bool isPass = false;
                        // �̹� ����Ʈ �ȿ� ���� ������Ʈ�� ��������� �ȳְ� �׳� �ѱ�.
                        foreach (GameObject houseList in houseObjects) {
                            if (houseList != par) {
                                continue;
                            }
                            else {
                                isPass = true;
                            }
                        }
                        if (!isPass) {
                            houseObjects.Add(par);
                        }
                    }
                    if (houseObjects.Count > 0) {
                        // ���� ���� ������ 1�� �̻��̶��
                        rand = Random.Range(0, 10); // 70% Ȯ���� ȸ���ϸ� Ȯ��, 30% Ȯ���� ���� Ȯ��
                    }
                    isMiss = true;
                    if (rand < 7) {
                        // ȸ���ϸ� Ȯ��
                        int cnt = 0;
                        int maxCnt = Random.Range(2, 5);

                        while (cnt < maxCnt) {
                            rand = Random.Range(0, 2);
                            cnt++;

                            if (rand == 0) {
                                anim.SetBool("turnRight", true);
                                float rotTime = Random.Range(2f, 4f);
                                float curTime = 0;
                                while (rotTime > curTime) {
                                    transform.Rotate(Vector3.up * Time.deltaTime * 20);
                                    curTime += Time.deltaTime;
                                    yield return null;
                                }
                                anim.SetBool("turnRight", false);
                            }

                            else {
                                anim.SetBool("turnLeft", true);
                                float rotTime = Random.Range(2f, 4f);
                                float curTime = 0;
                                while (rotTime > curTime) {
                                    transform.Rotate(Vector3.down * Time.deltaTime * 20);
                                    curTime += Time.deltaTime;
                                    yield return null;
                                }
                                anim.SetBool("turnLeft", false);
                            }
                            float randTime = Random.Range(0.4f, 1.2f);
                            yield return new WaitForSeconds(randTime);
                         }
                        StartCoroutine(Move(initPos.position));
                        yield break;
                    }
                    else {
                        // ������ �� Ȯ��
                        nma.isStopped = false;
                        anim.SetBool("isWalk", true);
                        // ����Ʈ ���� ���� �� ������ 1�� Ȯ��
                        rand = Random.Range(0, houseObjects.Count);
                        Vector3 targetPos = houseObjects[rand].transform.GetChild(houseObjects[rand].transform.childCount - 1).transform.position;
                        nma.SetDestination(targetPos);
                        nma.speed = walkSpeed;
                        while (Vector3.Distance(transform.position, targetPos) >= 3) {
                            yield return null;
                        }

                        // ������ 1���� �տ� ���� ���� �μ��ų� Ȥ�� ���θ� Ȯ��
                        StartCoroutine(rotate(Player.PlayerInHouseObject.transform.GetChild(Player.PlayerInHouseObject.transform.childCount - 1).eulerAngles.y));
                        yield return new WaitForSeconds(0.7f);
                        if (!Player.PlayerInHouseObject.GetComponentInChildren<Door>().isBroken) {
                            anim.SetTrigger("ClawAttack");
                            yield return new WaitForSeconds(1.4f);
                            Player.PlayerInHouseObject.GetComponentInChildren<Door>().HitDoor();
                            yield return new WaitForSeconds(1.9f);
                            anim.SetTrigger("Scream");
                            yield return new WaitForSeconds(1.2f);
                            monsterSound.loop = true;
                            monsterSound.clip = soundScream;
                            monsterSound.Play();

                            yield return new WaitForSeconds(1.6f);
                            StartCoroutine(Idle());
                            yield break;
                        }
                        else {
                            // ���� �̹� �μ��� ���� ��.
                            anim.SetTrigger("Scream");
                            yield return new WaitForSeconds(1.2f);
                            monsterSound.loop = true;
                            monsterSound.clip = soundScream;
                            monsterSound.Play();
                            yield return new WaitForSeconds(1.6f);
                            StartCoroutine(Idle());
                            yield break;
                        }
                    }
                }
            }

            // ��ǥ�������� ���� �̵���....
            else {
                chaseCurTime += Time.deltaTime;
                yield return null;
            }
         }
        anim.SetBool("isRun", false);
        StartCoroutine(Idle());
    }
    IEnumerator Attack() {
        state = State.Attack;
        anim.SetBool("isRun", false);
        anim.SetBool("turnLeft", false);
        anim.SetBool("turnRight", false);
        anim.SetBool("isWalk", false);
        nma.isStopped = true;
        nma.velocity = Vector3.zero;

        Vector3 dir = targetEntityTr.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        Player.InCabinet = true;

        int progress = 0;
        int maxProgress = 100;
        StartCoroutine(targetEntityTr.GetComponent<Player>().SeeEnemy(transform));
        while (maxProgress > progress) {
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20);
            progress++;
            yield return new WaitForSeconds(0.01f);
        }

        anim.SetTrigger("attack");
        yield return new WaitForSeconds(0.3f);
        monsterSound.clip = soundAttack;
        monsterSound.Play();
        yield return new WaitForSeconds(0.6f);
        FadeCtrl.instance.ColorChange(Color.red);
        StartCoroutine(FadeCtrl.instance.FadeOut(10, false));

        yield return new WaitForSeconds(0.5f);
        // ���� ����
        overZip.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Die() {
        StopAllCoroutines();
        state = State.Die;
    }

    private void OnDrawGizmos() {
        var leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
        var leftRayDirection = leftRayRotation * eye.transform.forward;
        Handles.color = new Color(1.0f, 0, 0, 0.2f);
        Handles.DrawSolidArc(eye.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);

        bool test = Physics.SphereCast(eye.position, 2, eye.forward, out RaycastHit hit, 10);

        if (test) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(eye.position, eye.forward * hit.distance);
            Gizmos.DrawWireSphere(eye.position + eye.forward * hit.distance, 2);
        }
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(eye.position, eye.forward * 10);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(atkRoot.position, atkRadius);
    }
}
