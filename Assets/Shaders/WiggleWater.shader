Shader "Custom/WigglyWater"
{
	Properties {
		// Not actually used, just for surpressing warnings.
		_MainTex("Required Texture for SpriteRenderer ???", 2D) = "white" {}

	    _Direction("Wave Direction | Shape", Vector) = (0,1,-1,0)
		_Freq("Frequency", Float) = 40
		_Waves("# Waves", Float) = 5

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
	        	float4 grabPos : TEXCOORD0;
	        	float4 pos : SV_POSITION;
	        };
	        
	        v2f vert(appdata_base v) {
	        	v2f o;
	        	// use UnityObjectToClipPos from UnityCG.cginc to calculate 
	        	// the clip-space of the vertex
	        	o.pos = UnityObjectToClipPos(v.vertex);
	        	// use ComputeGrabScreenPos function from UnityCG.cginc
	        	// to get the correct texture coordinate
	        	o.grabPos = ComputeGrabScreenPos(o.pos);
	        	return o;
	        }
	        
	        sampler2D _BackgroundTexture;
			fixed4 _Direction;
			float _Refraction;
			fixed _Freq;
			fixed _Waves;
			float _WCP;
			fixed4 _WaveCol;
	        
	        half4 frag(v2f i) : SV_Target
	        {
				float perp = dot(i.grabPos.xy, _Direction.zw);
				float par = dot(i.grabPos.xy, _Direction.xy);
				float px = (par + sin(perp * 20) / 50) * _Waves;

				float z = sin(px + _Time.x * _Freq);
				float2 disp = _Direction * -z * _Refraction;
				float rim = (z + 1) * _WCP;

				float newX = clamp(i.grabPos.x + disp.x, 0, 1);
				float newY = clamp(i.grabPos.y + disp.y, 0, 1);
				
				float4 col = tex2D(_BackgroundTexture, float2(newX, newY));
				col = (rim * _WaveCol) + ((1 - rim) * col);
				return col;
	        }
	    	ENDCG
	    }

	}
}