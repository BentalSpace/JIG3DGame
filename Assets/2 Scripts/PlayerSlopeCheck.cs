using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlopeCheck : MonoBehaviour
{
    public float height = 0.5f;
    public float heightPadding = 0.05f;
    public LayerMask ground;
    public float maxGroundAngle = 120;
    public bool debug;

    public float groundAngle;

    Vector3 forward;
    RaycastHit hitInfo;
    public bool grounded;

    void Update() {
        CalculateForward();
        CalculateGroundAngle();
        CheckGround();
        ApplyGravity();
        DrawDebugLines();
    }

    void CalculateForward() {
        if (!grounded) {
            forward = transform.forward;
            return;
        }
        forward = Vector3.Cross(hitInfo.normal, -transform.right);
    }
    void CalculateGroundAngle() {
        if (!grounded) {
            groundAngle = 90;
            return;
        }
        groundAngle = Vector3.Angle(hitInfo.normal, transform.forward);
    }
    void CheckGround() {
        if(Physics.Raycast(transform.position, -Vector3.up, out hitInfo, height + heightPadding, ground)) {
            if(Vector3.Distance(transform.position, hitInfo.point) > height) {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * height, 5 * Time.deltaTime);
            }
            grounded = true;
        }
        else {
            grounded = false;
        }
    }
    void ApplyGravity() {
        if (!grounded) {
            transform.position += Physics.gravity * Time.deltaTime;
        }
    }
    void DrawDebugLines() {
        if (!debug) return;

        Debug.DrawLine(transform.position, transform.position + forward * height * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position - Vector3.up * height, Color.red);
    }
}
