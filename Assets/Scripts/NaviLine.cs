using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviLine : MonoBehaviour {
    public Vector2[] wayPoints;
    public float speed;
    private int marker;

    void Update() {
        if (wayPoints.Length == 0) {
            return;
        }

        Vector3 dir = (wayPoints[marker] - (Vector2)transform.position);
        transform.position += dir * speed * Time.deltaTime;
        if ((wayPoints[marker] - (Vector2)transform.position).magnitude < 0.3) {
            marker = (marker + 1) % wayPoints.Length;
        }
    }
}
