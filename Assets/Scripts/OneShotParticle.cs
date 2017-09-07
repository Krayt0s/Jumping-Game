using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class OneShotParticle : MonoBehaviour {
	void Start () {
        var ps = GetComponent<ParticleSystem>();
        Invoke("Finish", ps.main.duration + ps.main.startLifetime.constantMax + 0.1f);
	}

    private void Finish() {
        Destroy(gameObject);
    }
}
