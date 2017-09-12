using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviLine : MonoBehaviour {
    public Vector2[] wayPoints;
    public float speed;
    private int marker;
    
    public GameObject anchor;

    void Update() {
        if (wayPoints.Length == 0) {
            return;
        }

        Vector2 wayPoint = wayPoints[marker];
        if(anchor) {
            wayPoint += (Vector2)anchor.transform.position;
        }
        Vector3 dir = (wayPoint - (Vector2)transform.position);
        transform.position += dir * speed * Time.deltaTime;
        if ((wayPoint - (Vector2)transform.position).magnitude < 0.3) {
            marker = (marker + 1) % wayPoints.Length;
        }
    }
}
