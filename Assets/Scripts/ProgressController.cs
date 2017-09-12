using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour {
    public GameObject progressor;
    private float maxY = 0;
	
	void Update () {
        maxY = Mathf.Max(progressor.transform.position.y, maxY);
        float dy = Mathf.Max(maxY - transform.position.y, 0);
        transform.position += (Vector3) new Vector2(0, dy);
	}
}
