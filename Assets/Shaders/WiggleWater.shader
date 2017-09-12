Shader "Custom/WigglyWater"
{
	Properties {
		// Not actually used, just for surpressing warnings.
		_MainTex("Required Texture for SpriteRenderer ???", 2D) = "white" {}

		_Direction("Wave Direction | Shape", Vector) = (0,1,-1,0)
		_Freq("Frequency", Float) = 40
		_Waves("# Waves", Float) = 5

		_TileX("Tile X", Float) = 1
		_TileY("Tile Y", Float) = 1

		_Refraction("Refraction", Range(0, 0.2)) = 0.2
		_WaveCol("Wave Colour", Color) = (1, 1, 1, 1)
	    _WCP("Wave Colour %", Range(0, 1)) = 0.3
	}
	SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent" }

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
	    {
		    "_BackgroundTexture"
     	}

		// Render the object with the texture generated above, and invert the colors
		Pass
	    {
		    CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
		    
	        struct v2f
	        {
	        	float4 pos : SV_POSITION;
	        	float4 grabPos : TEXCOORD0;
				float3 info : TEXCOORD1;
	        };
	        
			fixed _Freq;

	        v2f vert(appdata_base v) {
	        	v2f o;
	        	o.pos = UnityObjectToClipPos(v.vertex);
	        	o.grabPos = ComputeGrabScreenPos(o.pos);

				float3 wpos = mul(UNITY_MATRIX_M, v.vertex).xyz;

				o.info.x = wpos.x;
				o.info.y = wpos.y;
				o.info.z = _Time.y * _Freq;
	        	return o;
	        }
	        
	        sampler2D _BackgroundTexture;
			fixed4 _Direction;
			float _Refraction;
			fixed _Waves;
			fixed _TileX;
			fixed _TileY;
			float _WCP;
			fixed4 _WaveCol;
	        
	        half4 frag(v2f i) : SV_Target
	        {
				fixed _;
				float2 xy = float2(i.info.x, i.info.y);
				xy.x = modf(i.info.x / _TileX, _);
				xy.y = modf(i.info.y / _TileY, _);

				float perp = dot(xy, _Direction.zw);
				float par = dot(xy, _Direction.xy);
				float pz = (par + sin(perp * 20) / 50) * _Waves;

				float z = sin(pz + i.info.z);
				float2 disp = _Direction * -z * _Refraction;
				float rim = (z + 1) / 2 * _WCP;
				
				float4 col = tex2D(_BackgroundTexture, i.grabPos.xy + disp.xy);
				col = lerp(col, _WaveCol, rim);
				return col;
	        }
	    	ENDCG
	    }

	}
}