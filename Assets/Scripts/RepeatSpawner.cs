using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatSpawner : MonoBehaviour {
    public GameObject prefab;
    public float initialDelay;
    public float interArrivalMin;
    public float interArrivalMax;

    public bool randomAngle;
    public Vector2[] possibleVelocities;

    private GameObject container;

    // Use this for initialization
    void Start () {
        container = GameObject.Find("Spawned Container");
        Invoke("DelayedStart", initialDelay);
	}

    private void DelayedStart() {
        StartCoroutine("SpawnLoop");
    }

    private IEnumerator SpawnLoop() {
        while(true) {
            Spawn();
            yield return new WaitForSeconds(Random.Range(interArrivalMin, interArrivalMax));
        }
    }

    private void Spawn() {
        var q = randomAngle ? Quaternion.Euler(0, 0, Random.Range(0, 360)) : prefab.transform.rotation;
        var p = Instantiate(prefab, transform.position, q);
        p.SetActive(true);
        var rb2d = p.GetComponent<Rigidbody2D>();
        if(possibleVelocities.Length != 0) {
            rb2d.velocity = possibleVelocities[Random.Range(0, possibleVelocities.Length)];
        }

        if(container) {
            p.transform.SetParent(container.transform);
        }
    }
}
