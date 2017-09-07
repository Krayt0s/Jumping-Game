using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatSpawner : MonoBehaviour {
    public GameObject prefab;
    public float interArrivalMin;
    public float interArrivalMax;

    public bool randomAngle;
    public Vector2[] possibleVelocities;

    public Vector2[] wayPoints;
    public float speed;
    private int marker;

    // Use this for initialization
    void Start () {
        StartCoroutine("SpawnLoop");
	}

    void Update() {
        if(wayPoints.Length == 0) {
            return;
        }

        Vector3 dir = (wayPoints[marker] - (Vector2)transform.position);
        transform.position += dir * speed * Time.deltaTime;
        if((wayPoints[marker] - (Vector2)transform.position).magnitude < 0.3) {
            marker = (marker + 1) % wayPoints.Length;
        }
    }

    private IEnumerator SpawnLoop() {
        while(true) {
            Spawn();
            yield return new WaitForSeconds(Random.Range(interArrivalMin, interArrivalMax));
        }
    }

    private void Spawn() {
        var q = randomAngle ? Quaternion.Euler(0, 0, Random.Range(0, 360)) : Quaternion.identity;
        var p = Instantiate(prefab, transform.position, q);
        var rb2d = p.GetComponent<Rigidbody2D>();
        rb2d.velocity = possibleVelocities[Random.Range(0, possibleVelocities.Length)];
    }
}
