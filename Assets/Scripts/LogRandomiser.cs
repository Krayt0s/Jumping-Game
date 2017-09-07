using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class LogRandomiser : MonoBehaviour {
    public Vector2 widthSizes;
    public Vector2 lengthSizes;

	// Use this for initialization
	void Start () {
        Vector2 randSize = new Vector2(
            Random.Range(widthSizes.x, widthSizes.y), 
            Random.Range(lengthSizes.x, lengthSizes.y));

        var sr = GetComponent<SpriteRenderer>();
        sr.size = randSize;

        var bc2d = GetComponent<BoxCollider2D>();
        bc2d.size = randSize - new Vector2(0.1f, 0.1f);
	}
}
