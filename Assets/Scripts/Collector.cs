using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {
    private float points = 0;
	
	void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Collectable")) {
            Destroy(coll.gameObject);
            points++;
        }
    }
}
