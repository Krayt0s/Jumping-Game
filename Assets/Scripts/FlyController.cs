using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour {
    public Vector2 homePoint;
    public float wanderMin;
    public float wanderMax;
    public float recallDist;

    public float speed;

    private Vector2 targetLocation;
    
	void Start () {
        targetLocation = homePoint;
	}

    void Update() {
        if (((Vector2)transform.position - homePoint).magnitude > recallDist) {
            targetLocation = homePoint;
        } else if (MoveDirection().magnitude < 0.1f) {
            SetNewTarget();
        }
        transform.position += (Vector3)MoveDirection().normalized * speed * Time.deltaTime;
    }

    private Vector2 MoveDirection() {
        return targetLocation - (Vector2)transform.position;
    }

    private void SetNewTarget() {
        Vector2 wanderVec = Random.Range(wanderMin, wanderMax) * Random.insideUnitCircle.normalized;
        targetLocation = (Vector2)transform.position + wanderVec;
    }
}
