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

    // 스테이터스
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float runSpeed;

    // 적 확인
    [SerializeField]
    Transform eye;
    [SerializeField, Tooltip("시야 각")]
    float fieldOfView;
    [SerializeField, Tooltip("적을 인식 할 수 있는 최대 거리")]
    float viewDistance;
    [SerializeField, Tooltip("추격중일때 인식할 수 있는 최대 거리")]
    float chaseViewDistance;
    [SerializeField, Tooltip("추격중일때 시야각")]
    float chaseFieldOfView;
    bool isMiss;

    //적 공격
    [SerializeField]
    Transform atkRoot;
    [SerializeField, Tooltip("공격 반지름")]
    float atkRadius;
    float atkDistance;

    [SerializeField]
    float forceGravity;

    Vector3 targetEntity;
    Transform targetEntityTr;

    // 움직이는 좌표
    public List<Transform> movePoints = new List<Transform>();

    bool PlayerHouseSee;
    bool isHitHouse;

    // 배경음
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

        // 만약 10의 거리 앞에 벽 혹은 집이 있다면
        //int layerMask = (1 << 7) + (1 << 8);
        //if (state == State.Move) {
        //    if (Physics.SphereCast(eye.position, 2, eye.transform.forward, out hit, 10, layerMask)) {
        //        StopAllCoroutines();
        //        nma.speed = walkSpeed;
        //        nma.isStopped = false;
        //        nma.SetDestination(initPos.position);

        //        // #### 초기지점에 도착했을 때?
        //    }
        //}

        // 추가 중력 적용
        rigid.AddForce(Vector3.down * forceGravity);
    }
    void Update() {
        // 적을 발견했을 때
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
        // 추격 위치 갱신
        if(state == State.Chase && !isHitHouse) {
            if (!Player.InCabinet) {
                if (!isMiss) {
                    var collider = Physics.OverlapSphere(eye.position, chaseViewDistance, 1 << 3);

                    if (collider.Length > 0) {
                        foreach (var col in collider) {
                            if (!isTargetOnSight(col.transform, chaseViewDistance, chaseFieldOfView))
                                continue;
                            if (Player.inHouse) {
                                // 플레이어가 집에 들어간걸 확인.
                                // 일반 시야거리보다 추격 시야거리가 더 짧게 쫓아오고 있어서, 집에 들어가는걸 확인 못할 수도 있음.
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

                // 추격 중 놓쳤다면, 시야를 더 길게 봐라.
                else if (isMiss) {
                    var collider = Physics.OverlapSphere(eye.position, viewDistance, 1 << 3);

                    if (collider.Length > 0) {
                        foreach (var col in collider) {
                            if (!isTargetOnSight(col.transform, viewDistance, chaseFieldOfView))
                                continue;
                            if (Player.inHouse) {
                                // 플레이어가 집에 들어간걸 확인.
                                // 일반 시야거리보다 추격 시야거리가 더 짧아서 쫓아오고 있지만, 집에 들어가는걸 확인 못할 수도 있음.
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
        Debug.Log("플레이어가 집에 들어간 걸 확인함. " + PlayerHouseSee);
        Debug.Log("플레이어가 집 안에 있음. " + Player.inHouse);
        // 공격 거리에 들어갔을 때
        if (state == State.Chase) {
            if (!Player.InCabinet) {
                if (Vector3.Distance(targetEntityTr.position, transform.position) <= atkDistance) {
                    // 공격
                    Debug.Log("공격 지근거리");
                    int layerMask = (1 << 9) | (1 << 12);
                    layerMask = ~layerMask;
                    Vector3 rayDir = targetEntityTr.position - transform.position;
                    RaycastHit hit;

                    // 타겟와 나 사이에 벽이 있으면 안됨.
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

        // 시야각을 벗어나 있으면 안됨
        if(Vector3.Angle(dir, eye.forward) > viewAngle * 0.5f) {
            return false;
        }
        dir = target.position - eye.position;
        // 시야각 내에 있더라도 장애물에 가려져 있으면 안됨
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
        //반반의 확률로 왼쪽, 오른쪽 회전으로 탐색
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
        //일정 시간 대기 후
        randTime = Random.Range(1f, 5f);
        yield return new WaitForSeconds(randTime);

        //다음 패턴 진행
        pattern = Random.Range(0, 3);
        if (pattern <= 1) {
            // 66% 로 이동
            List<Transform> randTr = pointsSearch();
            int randPos = Random.Range(0, randTr.Count);
            Vector3 targetVec = randTr[randPos].position;

            StartCoroutine(Move(targetVec));
        }
        else {
            // 33%로 제자리에 있기
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
            // 목표지점 도달
            if (Vector3.Distance(transform.position, targetVec) <= 3) {
                monsterSound.loop = false;
                nma.isStopped = true;
                anim.SetBool("isWalk", false);
                monsterSound.clip = soundIdle;
                monsterSound.Play();
                // 일정시간 휴식 후
                float randTime = Random.Range(1, 4f);
                yield return new WaitForSeconds(randTime);

                // 다음 패턴 진행
                int pattern = Random.Range(0, 3);
                if (pattern <= 1) {
                    // 66% 로 이동
                    List<Transform> randTr = pointsSearch();
                    int randPos = Random.Range(0, randTr.Count);
                    targetVec = randTr[randPos].position;

                    StartCoroutine(Move(targetVec));
                }
                else {
                    // 33%로 제자리에 있기
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
        // 플레이어 발견 지점으로 달려감.
        nma.SetDestination(targetEntity);
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", true);

        // 마지막에 확인한 위치까지 추격해 왔지만, 플레이어의 모습이 보이지 않을때.
        float chaseMaxTime = 5f;
        float chaseCurTime = 0;
        while (chaseMaxTime > chaseCurTime) {
            // 목표지점에 거의 도달했을 때.
            if (Vector3.Distance(transform.position, targetEntity) <= 3) {
                nma.isStopped = true;
                anim.SetBool("isRun", false);
                nma.velocity = Vector3.zero;

                // 집에 들어간 걸 확인 했을때.
                if (PlayerHouseSee) {
                    Debug.Log("집에 들어간거 다 봤다.");
                    // 문을 부셔라.
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
                        // 문이 이미 부셔져 있을 때.
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

                // 목표 타겟(플레이어)을 놓쳤을 때.
                else {
                    // 패턴 : [회전하면서 주위 확인(왼쪽, 오른쪽) / 문 부수기(주위 문 개수) 혹은 집 안 확인]

                    // 반지름 15 이내의 집 중 무작위 1개 확인
                    int rand = 0;
                    var houses = Physics.OverlapSphere(transform.position, 15, 1 << 8);
                    List<GameObject> houseObjects = new List<GameObject>();

                    // 집들을 리스트에 넣기
                    foreach (var house in houses) {
                        GameObject par = house.transform.parent.gameObject;
                        bool isPass = false;
                        // 이미 리스트 안에 같은 오브젝트가 들어있으면 안넣고 그냥 넘김.
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
                        // 주위 집의 개수가 1개 이상이라면
                        rand = Random.Range(0, 10); // 70% 확률로 회전하며 확인, 30% 확률로 집안 확인
                    }
                    isMiss = true;
                    if (rand < 7) {
                        // 회전하며 확인
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
                        // 무작위 집 확인
                        nma.isStopped = false;
                        anim.SetBool("isWalk", true);
                        // 리스트 안의 집들 중 무작위 1개 확인
                        rand = Random.Range(0, houseObjects.Count);
                        Vector3 targetPos = houseObjects[rand].transform.GetChild(houseObjects[rand].transform.childCount - 1).transform.position;
                        nma.SetDestination(targetPos);
                        nma.speed = walkSpeed;
                        while (Vector3.Distance(transform.position, targetPos) >= 3) {
                            yield return null;
                        }

                        // 무작위 1개집 앞에 가서 문을 부수거나 혹은 내부를 확인
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
                            // 문이 이미 부셔져 있을 때.
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

            // 목표지점으로 아직 이동중....
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
        // 게임 오버
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
