Shader "Custom/WigglyWater"
{
	Properties {
		// Not actually used, just for surpressing warnings.
		_MainTex("Required Texture for SpriteRenderer ???", 2D) = "white" {}

		_FreqPerp("Frequency X", Float) = 20
		_WavesX("# Waves X", Float) = 5
		_FreqPar("Frequency Y", Float) = 40
		_WavesY("# Waves Y", Float) = 5

		_Refraction("Refraction", Range(0, 0.3)) = 0.2
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
			float _FreqPerp;
			float _FreqPar;
			float _Refraction;
			float _WavesX;
			float _WavesY;
			float _WCP;
			fixed4 _WaveCol;
	        
	        half4 frag(v2f i) : SV_Target
	        {
			    float px = (i.grabPos.x + sin(i.grabPos.y * 20) / 50) * _WavesX;
				float py = (i.grabPos.y + sin(i.grabPos.x * 20) / 50) * _WavesY;
			    float xWave = sin(px + _Time.x * _FreqPerp);
			    float yWave = sin(py + _Time.x * _FreqPar);

				float rim = ((xWave + yWave) / 2) * _WCP;
				float2 disp = float2(xWave, yWave) * _Refraction;

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