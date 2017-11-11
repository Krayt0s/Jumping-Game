using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JumpCursor : MonoBehaviour {
    private LineRenderer lr;

    private GameObject home;
    private Vector3[] jumpCursorPositions = new Vector3[2];

    private bool frozen;
    private bool aiming;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update () {
        if(!frozen) {
            jumpCursorPositions[0] = home.transform.position;
            if(!aiming) {
                jumpCursorPositions[1] = jumpCursorPositions[0];
            }
        }

        lr.SetPositions(jumpCursorPositions);
    }

    public void Unfreeze() {
        frozen = false;
        jumpCursorPositions[0] = jumpCursorPositions[1] = home.transform.position;
    }

    public void Freeze() {
        frozen = true;
    }

    public void SetHomeObject (GameObject home) {
        this.home = home;
	}

    public void Aim(Vector3 disp) {
        aiming = true;
        jumpCursorPositions[1] = jumpCursorPositions[0] + disp;
    }

    public void StopAiming() {
        aiming = false;
    }
}
