using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTimer : MonoBehaviour {
    public float timeUntilPause;
	
	void Update () {
        timeUntilPause -= Time.deltaTime;
        if(timeUntilPause <= 0) {
            Time.timeScale = 0;
        }
	}
}
