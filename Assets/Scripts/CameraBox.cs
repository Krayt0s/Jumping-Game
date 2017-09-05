using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBox : MonoBehaviour {
    /* Camera Unity Height Units = Camera_Size
     * Camera Unity Width Units  = Camera_Size * (visWidth / visHeight)
     */
    public const float visWidth = 6f;
    public const float visHeight = 5f;

    private Camera cam;
    private float lastScreenWidth = 0f;
    private float lastScreenHeight = 0f;

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void Start() {
        cam.orthographicSize = 5;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        AdjustScreen();
    }

    void Update() {
        float curScreenHeight = Screen.height;
        float curScreenWidth = Screen.width;
        if(lastScreenWidth != Screen.width || lastScreenHeight != Screen.height) {
            lastScreenWidth = curScreenWidth;
            lastScreenHeight = curScreenWidth;
            AdjustScreen();
        }
    }

    void AdjustScreen() {
        float windowAspect = Screen.width / Screen.height;
        float targetAspect = visWidth / visHeight;
        float aspectScale = windowAspect / targetAspect;

        if(aspectScale < 1.0f) {
            // Shorter / Wider Resolution, showing too much width
            float scale = Screen.height / visHeight;
            float scaledWidth = visWidth * scale;
            float visRatio = scaledWidth / Screen.width;       
            cam.rect = new Rect((1.0f - visRatio) / 2.0f, 0f, 
                                 visRatio, 1.0f);
        } else {            
            // Taller / Thinner Resolution, showing too much height
            float scale = Screen.width / visWidth;
            float scaledHeight = visHeight * scale;
            float visRatio = scaledHeight / Screen.width;
            cam.rect = new Rect(0f, (1.0f - visRatio) / 2.0f, 
                                1.0f, visRatio);
        }
    }
}
