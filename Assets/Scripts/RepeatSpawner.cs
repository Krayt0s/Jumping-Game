using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatSpawner : MonoBehaviour {
    public GameObject prefab;
    public float interArrivalMin;
    public float interArrivalMax;

    public bool randomAngle;
    public Vector2[] possibleVelocities;

    // Use this for initialization
    void Start () {
        StartCoroutine("SpawnLoop");
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
