using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandingController))]
public class Collector : MonoBehaviour {
    public AudioClip eatSound;

    private LandingController lc;
    private float points = 0;

    void Awake() {
        lc = GetComponent<LandingController>();
    }
	
	void OnTriggerEnter2D(Collider2D coll) {
        if(coll.gameObject.layer != LayerMask.NameToLayer("Collectable")) {
            return;
        }

        if(lc.submerged) {
            // Nothing yet is collectable underwater
        } else {
            if (true) {
                Destroy(coll.gameObject);
                AudioSource.PlayClipAtPoint(eatSound, transform.position);
                points++;
            }
        }
    }
}
