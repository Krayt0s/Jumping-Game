using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSpawner : MonoBehaviour {
    public GameObject logPrefab;
    public float spawnTimer;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnLogt", 1f, 5f);
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void SpawnLogt() {
        SpawnLog(new Vector2(0.1f, -2f));
    }

    private void SpawnLog(Vector2 velocity) {
        var log = Instantiate(logPrefab, transform.position, Quaternion.identity);
        var rb2d = log.GetComponent<Rigidbody2D>();
        rb2d.velocity = velocity;
    }
}
