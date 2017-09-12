using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
    public GameObject target;
    public Vector2 offset;
    public float speed;
    public bool lockY;
    public bool lockX;
	
	void Update () {
        Vector2 followPos = (Vector2)target.transform.position + offset;
        if(lockY) {
            followPos.y = transform.position.y;
        }
        if(lockX) {
            followPos.x = transform.position.x;
        }
        Vector3 lerpXY = Vector2.Lerp(transform.position, followPos, speed * Time.deltaTime);
        transform.position = lerpXY + new Vector3(0f, 0f, transform.position.z);
	}
}
