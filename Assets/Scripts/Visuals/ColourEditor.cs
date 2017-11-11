using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColourEditor : MonoBehaviour {
    [Range(0, 255)]
    public int alpha = 1;
    private int lastAlpha = 1;
	
	void Update () {
		if(alpha != lastAlpha) {
            lastAlpha = alpha;
            float a = (float) alpha / 255;
            foreach(var sr in GetComponentsInChildren<SpriteRenderer>()) {
                Color c = sr.color;
                c.a = a;
                sr.color = c;
            }
        }
	}
}
