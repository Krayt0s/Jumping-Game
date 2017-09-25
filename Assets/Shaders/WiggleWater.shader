Shader "Custom/WigglyWater"
{
	Properties {
		[HideInInspector]
		_MainTex("I am error.", 2D) = "white" {}

		_Rotation("Direction (rads)", Float) = 0

		_Freq("Frequency", Float) = 40
		_Waves("Parallel Density", Float) = 5

		_TileX("Tile X", Float) = 1
		_TileY("Tile Y", Float) = 1

		_Refraction("Refraction", Range(0, 0.2)) = 0.2
		_WaveCol("Wave Colour", Color) = (1, 1, 1, 1)
	    _WCP("Wave Colour %", Range(0, 1)) = 0.3

		_Crosswave("Wave 2: X, Y, T, %", Vector) = (-1,1,20,0.9)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		GrabPass
	    {
		    "_BackgroundTexture"
     	}

		Pass
	    {
			// Animates a wave using xy world coordinates as an offset for a wavefunction
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
	        
			static const float TAU = 2 * 3.14159265f;
			fixed _Freq;
			fixed _TileX;
			fixed _TileY;
			fixed _Rotation;

	        v2f vert(appdata_base v) {
	        	v2f o;
	        	o.pos = UnityObjectToClipPos(v.vertex);
	        	o.grabPos = ComputeGrabScreenPos(o.pos);

				o.info.xy = mul(UNITY_MATRIX_M, v.vertex).xy / float2(_TileX, _TileY);

				// Rotate the animation
				float cos0 = cos(_Rotation);
				float sin0 = sin(_Rotation);
				float2x2 rot = { cos0, -sin0, sin0, cos0 };
				o.info.xy = mul(rot, o.info.xy);

				o.info.z = _Time.y * _Freq;
	        	return o;
	        }
	        
			static const fixed2 _Forward = float2(0, 1);
			sampler2D _MainTex;
	        sampler2D _BackgroundTexture;
			fixed4 _Crosswave;
			float _Refraction;
			fixed _Waves;
			float _WCP;
			fixed4 _WaveCol;
	        
	        half4 frag(v2f i) : SV_Target
	        {
				fixed _;
				// TAU ensures continuity with trig functions, i.e. seamless waves
				float2 xy = float2(modf(i.info.x, _), modf(i.info.y, _)) * TAU;

				//TODO actually come up with a wave function that makes sense
				float par = dot(xy, _Forward);
				float perp = dot(xy, _Crosswave.xy);
				// Offset calculated from the main wave and crosswave
				float offset = (par * _Waves) + (sin(perp * _Crosswave.z) * _Crosswave.w);

				float z = sin(offset + i.info.z); // Add time (i.info.z) to animate
				float2 disp = _Forward * -z * _Refraction;
				float rim = (z + 1) / 2; // Bound between 0 and 1
				
				float4 col = tex2D(_BackgroundTexture, i.grabPos.xy + disp.xy);
				col = lerp(col, _WaveCol, rim * _WCP);
				return col;
	        }
	    	ENDCG
	    }

	}
}