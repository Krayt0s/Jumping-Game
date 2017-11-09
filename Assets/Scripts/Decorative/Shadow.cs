using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadow : MonoBehaviour {
    public bool overground;

    private const int overgroundShadowSortOrder =   51;
    private const int underwaterShadowSortOrder = -800;
    private Vector2 overgroundShadowScale = new Vector2(0.9f, 0.9f);
    private Vector2 underwaterShadowScale = new Vector2(0.9f, 0.9f);

    private const float overgroundShadowAlpha = 0.3f;
    private const float underwaterShadowAlpha = 1.0f;
    public Vector2 overgroundOffset = new Vector2(0.07f, -0.07f);
    private Vector2 underwaterOffset = new Vector2(0.3f, -0.3f);
    private GameObject overgroundShadow;
    private GameObject underwaterShadow;

    private SpriteRenderer srorg;
    private SpriteRenderer overgroundsr;
    private SpriteRenderer underwatersr;

    void Start () {
        GameObject shadowContainer = GameObject.Find("Shadow Container");
        srorg = GetComponent<SpriteRenderer>();

        // Underwater Shadow
        underwaterShadow = new GameObject("Underwater Shadow (" + name + ")");
        underwatersr = underwaterShadow.AddComponent<SpriteRenderer>();

        CopySpriteRenderer(underwatersr, srorg);
        underwatersr.sortingOrder = underwaterShadowSortOrder;
        Color shadowCol = Color.black;
        shadowCol.a = underwaterShadowAlpha;
        underwatersr.color = shadowCol;

        if(shadowContainer) {
            underwaterShadow.transform.SetParent(shadowContainer.transform);
        }

        // Overground shadow
        if(!overground) {
            return;
        }
        overgroundShadow = new GameObject("Overground Shadow (" + name + ")");
        overgroundsr = overgroundShadow.AddComponent<SpriteRenderer>();

        CopySpriteRenderer(overgroundsr, srorg);
        overgroundsr.sortingOrder = overgroundShadowSortOrder;
        shadowCol.a = overgroundShadowAlpha;
        overgroundsr.color = shadowCol;

        overgroundsr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        var lc = GetComponent<LandStateController>();
        if (lc) {
            lc.OnSink += HideOvergroundShadow;
            lc.OnSurface += ShowOvergroundShadow;
        }

        if (shadowContainer) {
            overgroundShadow.transform.SetParent(shadowContainer.transform);
        }
    }

    private void CopySpriteRenderer(SpriteRenderer sr, SpriteRenderer srorg) {
        // Copy all data
        sr.sprite = srorg.sprite;
        sr.drawMode = srorg.drawMode;
        sr.size = srorg.size;
        sr.tileMode = srorg.tileMode;
    }

    void LateUpdate() {
        if(overground) {
            UpdateShadow(overgroundShadow, overgroundsr, overgroundOffset, overgroundShadowScale);
        }
        UpdateShadow(underwaterShadow, underwatersr, underwaterOffset, underwaterShadowScale);
    }

    private void UpdateShadow(GameObject shadow, SpriteRenderer sr, Vector2 offset, Vector2 shadowScale) {
        shadow.transform.position = transform.position + (Vector3)offset;
        shadow.transform.rotation = transform.rotation;

        Vector3 scale = transform.localScale;
        scale.x *= shadowScale.x;
        scale.y *= shadowScale.y;
        shadow.transform.localScale = scale;

        sr.sprite = srorg.sprite;
    }

    void OnDestroy() {
        Destroy(underwaterShadow);
        if(overground) {
            Destroy(overgroundShadow);
            var lc = GetComponent<LandStateController>();
            if (lc) {
                lc.OnSink -= HideOvergroundShadow;
                lc.OnSurface -= ShowOvergroundShadow;
            }
        }
    }

    private void HideOvergroundShadow() {
        overgroundShadow.SetActive(false);
    }

    private void ShowOvergroundShadow() {
        overgroundShadow.SetActive(true);
    }
}
