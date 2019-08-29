Shader "Form/CoreShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_FormObject ("Form Object", Vector) = (0,0,0,0)
		_Animation ("Animation", Vector) = (0,0,0,0)
		_Background ("Background", Range(1, 3)) = 1
	}
	SubShader {

		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf SimpleSpecular		
		#include "FormCommon.glslinc"

		sampler2D _MainTex;
		fixed4 _FormObject;
		fixed4 _Animation;
		float _Background;
        
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

		//sphere functions from IQ
        //http://www.iquilezles.org/www/index.htm
		float sphDensity(float3 ro, float3 rd, float4 sph, float dbuffer) {
			float ndbuffer = dbuffer / sph.w;
    		float3  rc = (ro - sph.xyz) / sph.w;
    		float b = dot(rd, rc);
    		float c = dot(rc, rc) - 1.0;
    		float h = b * b - c;
            if (h < 0.0) return 0.0;
            h = sqrt(h);
            float t1 = -b - h;
            float t2 = -b + h;
            if (t2 < 0.0 || t1 > ndbuffer) return 0.0;
			t1 = max(t1, 0.0);
			t2 = min(t2, ndbuffer);
    		float i1 = -(c * t1 + b * t1 * t1 + t1 * t1 * t1 / 3.0);
    		float i2 = -(c * t2 + b * t2 * t2 + t2 * t2 * t2 / 3.0);
    		return (i2 - i1) * (3.0 / 4.0);
		}

		void surf (Input IN, inout SurfaceOutput o) {

            float3 rd = normalize(IN.worldPos - _WorldSpaceCameraPos);
			float r = length(IN.worldPos - _FormObject.xyz); //radius of sphere
			float w = sphDensity(IN.worldPos, rd, float4(_FormObject.xyz, r), 20.0);
			
            float3 glow = PT(_Time.x + _Animation.z); //glow colour 
			float av = clamp(_Animation.x*0.5, 0.0, 3.0);
			float3 pc = glow*0.1 + glow*pow(w, 8.0); //glow
			pc += glow*av*w*w*w*w*w*5.0; //audio glow boost

			//reflections
			float3 rrd = reflect(rd, IN.worldNormal);
        	float fresnel = pow(clamp(1.0 + dot(rd, IN.worldNormal), 0.0, 1.0), 2.0);
			float rCol = Planes(rrd);
			if (_Background==2) {
				rCol = Kali(rrd);
			} else if (_Background==3) {
				rCol = Snow(float3(0.1,2.0,0.2), IN.uv_MainTex, _ScreenParams.zw, rrd.y);
			}

			o.Albedo = pc;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
