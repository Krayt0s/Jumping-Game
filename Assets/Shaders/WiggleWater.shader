Shader "Custom/GrabPassInvert"
{
	Properties {
		_FlowSpeed("Speed", Float) = 1
		_FreqPerp("Perpendicular Wave Frequency", Float) = 20
		_FreqPar("Parallel Wave Frequency", Float) = 40
		_WavelengthPerp("Perpendicular Wavelength", Range(0.01, 1)) = 0.5
		_WavelengthPar("Parallel Wavelength", Range(0.01, 1)) = 0.5
		_Refraction("Refraction", Range(0, 1)) = 0.2
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
			float _FlowSpeed;
			float _FreqPerp;
			float _FreqPar;
			float _Refraction;
			float _WavelengthPerp;
			float _WavelengthPar;
	        
	        half4 frag(v2f i) : SV_Target
	        {
				float unused;
			    float xWave = sin((i.grabPos.x / _WavelengthPerp + _Time.x * _FlowSpeed) * _FreqPerp) * _Refraction;
			    float yWave = sin((i.grabPos.y / _WavelengthPar + _Time.x * _FlowSpeed) * _FreqPar) * _Refraction;
				float2 disp = float2(xWave, yWave);

				float newX = modf(i.grabPos.x + disp.x, unused);
				float newY = modf(i.grabPos.y + disp.y, unused);
				
				float4 col = tex2D(_BackgroundTexture, float2(newX, newY)) / 2;
				return col;
	        }
	    	ENDCG
	    }

	}
}