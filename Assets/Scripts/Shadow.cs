using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadow : MonoBehaviour {
    private const int shadowSortOrder = -800;
    private const float shadowScaleX = 0.9f;
    private const float shadowScaleY = 0.9f;

    private const float shadowAlpha = 0.3f;
    private Vector2 offset = new Vector2(0.3f, -0.3f);
    private GameObject shadow;

    void Start() {
        Invoke("LateStart", 0.1f);
    }

    void LateStart () {
        shadow = new GameObject("Shadow");
        Vector3 shadowScale = transform.localScale;
        shadowScale.x *= shadowScaleX;
        shadowScale.y *= shadowScaleY;
        shadow.transform.localScale = shadowScale;

        var srorg = GetComponent<SpriteRenderer>();
        var sr = shadow.AddComponent<SpriteRenderer>();
        // Copy all data
        sr.sprite = srorg.sprite;
        sr.drawMode = srorg.drawMode;
        sr.size = srorg.size;
        sr.tileMode = srorg.tileMode;

        // Shadow data
        sr.sortingOrder = shadowSortOrder;

        Color shadowCol = Color.black;
        shadowCol.a = shadowAlpha;
        sr.color = Color.black;
    }

    void LateUpdate() {
        shadow.transform.position = transform.position + (Vector3)offset;
        shadow.transform.rotation = transform.rotation;
    }
}
