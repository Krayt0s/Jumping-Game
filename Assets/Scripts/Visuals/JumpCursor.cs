using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class JumpCursor : MonoBehaviour {
    private LineRenderer lr;

    private GameObject home;
    private Vector3[] jumpCursorPositions = new Vector3[2];

    private bool contracting;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update () {
        if(!contracting) {
            jumpCursorPositions[0] = home.transform.position;
        }

        lr.SetPositions(jumpCursorPositions);
    }

    public void EndContraction() {
        contracting = false;
        jumpCursorPositions[0] = jumpCursorPositions[1] = home.transform.position;
    }

    public void StartContraction() {
        contracting = true;
    }

    public void SetHomeObject (GameObject home) {
        this.home = home;
	}

    public void SetTowardsDisp(Vector3 disp) {
        jumpCursorPositions[1] = jumpCursorPositions[0] + disp;
    }

    public void SetTowardsPoint(Vector3 topoint) {
        jumpCursorPositions[1] = topoint;
    }

    public void RemoveTo() {
        jumpCursorPositions[1] = jumpCursorPositions[0];
    }
}
