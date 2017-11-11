using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
    public float frequency;
    public float parallelDensity;
    public Vector2 tileSize;
    public float refraction;

    public Color crestColour;
    [Range(0, 1)]
    public float crestVisibility;

    public Vector2 wave2Direction;
    public float wave2Density;
    public float wave2Strength;

    void Start() {
        var mat = GetComponent<Renderer>().material;

        float radrot = Mathf.Deg2Rad * transform.rotation.z;

        mat.SetFloat(Shader.PropertyToID("_Rotation"), radrot);
        mat.SetFloat(Shader.PropertyToID("_Freq"), frequency);
        mat.SetFloat(Shader.PropertyToID("_Waves"), parallelDensity);
        mat.SetFloat(Shader.PropertyToID("_TileX"), tileSize.x);
        mat.SetFloat(Shader.PropertyToID("_TileY"), tileSize.y);
        mat.SetFloat(Shader.PropertyToID("_Refraction"), refraction);
        mat.SetColor(Shader.PropertyToID("_WaveCol"), crestColour);
        mat.SetFloat(Shader.PropertyToID("_WCP"), crestVisibility);

        var wave2Properties = new Vector4(wave2Direction.x, wave2Direction.y, wave2Density, wave2Strength);
        mat.SetVector(Shader.PropertyToID("_Crosswave"), wave2Properties);
    }
}
