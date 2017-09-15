using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {
    public AudioClip eatSound;

    public delegate void Collect(GameObject gameObject);
    public event Collect onCollect;
	
	void OnTriggerEnter2D(Collider2D coll) {
        if(coll.gameObject.layer != LayerMask.NameToLayer("Collectable") || onCollect == null) {
            return;
        }
        
        var tag = coll.tag;
        onCollect(coll.gameObject);

        // Tag is changed to "X" to mark for destruction
        if(coll.tag == "X") {
            Destroy(coll.gameObject);
        }
    }
}
