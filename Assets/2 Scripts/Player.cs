using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Move
    float h, v;
    float applySpeed;
    [SerializeField]
    float walkSpeed;
    bool isRun;
    [SerializeField]
    float runSpeed;

    // Jump
    bool isJump;
    bool canJump;
    bool isJumping;
    [SerializeField, Tooltip("���� �� ����")]
    float jumpPower;
    [SerializeField]
    Transform foot;
    [SerializeField]
    float footRadius;

    // Camera Rotation
    new Camera camera;
    [SerializeField, Tooltip("���콺 ����")]
    float lookSensitivity;
    [SerializeField, Tooltip("ī�޶� �����̼� ���Ʒ� ȸ���� ����")]
    float cameraRotationLimit;
    float currentCameraRotationX;

    // �߷�
    [SerializeField, Tooltip("ĳ���Ϳ��� ����Ǵ� �߷°�")]
    float forceGravity;

    public static bool InCabinet;
    public static bool inHouse;
    public static GameObject PlayerInHouseObject;

    // Components
    Rigidbody rigid;

    PlayerSlopeCheck slopeCheck;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InCabinet = false;

        rigid = GetComponent<Rigidbody>();
        slopeCheck = GetComponent<PlayerSlopeCheck>();
        camera = Camera.main;
    }
    void Update() {
        InputEvent();

        if (!InCabinet) {
            CameraXRotation();
            CameraYRotation();
            Jump();
        }
    }
    void FixedUpdate() {
        if (!InCabinet) {
            Move();
            Run();
            JumpLimit();

            // �߷�
            rigid.AddForce(Vector3.down * forceGravity);
        }
    }
    void InputEvent() {
        // ����� Ű�Է� �̺�Ʈ ó��
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        isJump = Input.GetButtonDown("Jump");
        isRun = Input.GetButton("Run");
    }
    void Move() {
        if (slopeCheck.maxGroundAngle <= slopeCheck.groundAngle)
            return;
        Vector3 moveH = transform.right * h;
        Vector3 moveV = transform.forward * v;

        Vector3 moveVec = (moveH + moveV).normalized * applySpeed;

        rigid.velocity = new Vector3(moveVec.x, rigid.velocity.y, moveVec.z);
    }
    void Run() {
        if (isRun) {
            applySpeed = runSpeed;
        }
        else {
            applySpeed = walkSpeed;
        }
    }
    void Jump() {
        if (!isJump || !canJump || isJumping)
            return;

        isJumping = true;
        canJump = false;
        rigid.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }
    void JumpLimit() {
        // ���� 1ȸ ����

        if (Physics.SphereCast(foot.position, footRadius, Vector3.down, out RaycastHit hit, 0.1f) && !isJumping) {
            canJump = true;
        }
        if (rigid.velocity.y <= -0.1f) {
            isJumping = false;
        }
    }
    void CameraXRotation() {
        //Camera Rotation
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        camera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
    void CameraYRotation() {
        //Charactor Rotation
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 charactorRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(charactorRotationY));
    }

    public IEnumerator SeeEnemy(Transform target) {
        rigid.velocity = Vector3.zero;
        Vector3 dir = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot.eulerAngles = new Vector3(rot.eulerAngles.x -30f, rot.eulerAngles.y, rot.eulerAngles.z);
        camera.transform.localEulerAngles = Vector3.zero;
        int progress = 0;
        int maxProgress = 100;
        while (maxProgress > progress) {
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20);
            progress++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("InHouse")) {
            // �� ������ ��.
            inHouse = true;
            PlayerInHouseObject = other.transform.parent.gameObject;
        }
        
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Ground")) {
            // ���� ��� ����. ��� �� �ۿ� �ִ�.
            inHouse = false;
        }
    }
    void OnDrawGizmos() {
        //RaycastHit hit;
        //bool test = Physics.SphereCast(foot.position, footRadius, Vector3.down, out hit, 0.1f, 1<<6);

        //if (test) {
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(foot.position, Vector3.down * hit.distance);
        //    Gizmos.DrawWireSphere(foot.position + Vector3.down * hit.distance, footRadius);
        //}
        //else {
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawRay(foot.position, Vector3.down * 0.1f);
        //}
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(foot.position, footRadius);

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, 70);
    }
}
