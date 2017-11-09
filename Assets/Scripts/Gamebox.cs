using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Gamebox : MonoBehaviour {
    /** Kills gameobjects that try to exit the trigger area. 
        Assumes one gameobject has only one collider2D. **/ 
    private List<GameObject> entitiesEntered = new List<GameObject>();
	
	void OnTriggerEnter2D(Collider2D coll) {
        entitiesEntered.Add(coll.gameObject);
    } 

    void OnTriggerExit2D(Collider2D coll) {
        GameObject cullee = coll.gameObject;
        entitiesEntered.Remove(cullee);

        var desp = cullee.GetComponent<Despawnable>();
        if(desp) {
            desp.despawnFunction.Invoke();
        } else if(cullee.transform.parent == null) {
            Destroy(cullee);
        }
    }
}
