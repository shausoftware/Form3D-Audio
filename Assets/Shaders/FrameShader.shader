Shader "Form/FrameShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Colour ("Colour", Color) = (1,1,1,1)
		_Background ("Background", Range(1, 3)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf SimpleSpecular
		#include "FormCommon.glslinc"

		sampler2D _MainTex;
		fixed4 _Colour;
		int _Background;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			float3 worldPos;
            float3 worldNormal;
		};

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (lightDir + viewDir);

			half diff = max (0, dot (s.Normal, lightDir));

			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0);

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			c.a = s.Alpha;
			return c;
		}
		void surf (Input IN, inout SurfaceOutput o) {

			//reflections
            float3 rd = normalize(IN.worldPos - _WorldSpaceCameraPos);
			float3 rrd = reflect(rd, IN.worldNormal);
        	float fresnel = pow(clamp(1.0 + dot(rd, IN.worldNormal), 0.0, 1.0), 8.0);
			float3 rCol = Planes(rrd, _Time.y);
			if (_Background==2) {
				rCol = Kali(rrd, _Time.y);
			} else if (_Background==3) {
				rCol = Snow(normalize(float3(0.1,1.0,0.2)), IN.uv_MainTex, _ScreenParams.zw, rrd.y, _Time.y);
				rCol = clamp(rCol, float3(0,0,0), float3(1,1,1));
			}

			o.Albedo = _Colour.rgb + rCol*fresnel;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
