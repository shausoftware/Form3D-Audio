Shader "Form/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_KaliFactor("Kali Factor", Range(0.1, 2.0)) = 0.63
		_KaliIterations("Kali Iterations", Range(5, 10)) = 6
		_Animation ("Animation", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			#define UI0 1597334673U
			#define UI1 3812015801U
			#define UI2 uint2(UI0, UI1)
			#define UI3 uint3(UI0, UI1, 2798796415U)
			#define UIF (1.0 / float(0xffffffffU))

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewT : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _KaliFactor;
			int _KaliIterations;
			fixed4 _Animation;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewT = normalize(WorldSpaceViewDir(v.vertex));//ObjSpaceViewDir is similar, but localspace.
				return o;
			}

			//Dave Hoskins - Hash without sin
			//https://www.shadertoy.com/view/XdGfRR
			float3 hash33(float3 p) {
				uint3 q = uint3(int3(p)) * UI3;
				q = (q.x ^ q.y ^ q.z)*UI3;
				return float3(q) * UIF;
			}
			float hash11(float p) {
				uint2 n = uint(int(p)) * UI2;
				uint q = (n.x ^ n.y) * UI0;
				return float(q) * UIF;
			}

			//IQ cosine palattes
			//http://www.iquilezles.org/www/articles/palettes/palettes.htm
			float3 PT(float t) {
				return float3(0.5,0.5,0.5) + 
			       	   float3(0.5,0.5,0.5) * cos(6.28318 * (float3(1.0,1.0,1.0) * t * 0.1 + float3(0.0, 0.33, 0.67)));
		    }

			float3 kali(float3 rd) {
    			float v = 0.0;
    			float k = 0.0;
    			for (float i = 0.0; i < float(_KaliIterations); i++) {
        			rd = abs(rd) / dot(rd, rd) - _KaliFactor;
        			k += length(rd) * length(hash33(rd + _Time.y*0.1));
        			v += k*k*k*0.0003;
    			}
    			return v;
			}

			float3 planes(float3 rd) {
    			float a = (atan2(rd.y, rd.x) / 6.283185) + 0.5, //polar
					  fla = floor(a * 32.0) / 32.0, //split into 32 segemnts
					  fra = frac(a * 32.0),
					  frnd = hash11(fla * 400.0);

    			float3 pc = PT(fla*4.0+_Time.y)*16.0; //mix colours radially
				pc += (step(0.1, fra) * step(fra, 0.2)) * 16.0;
				pc += (step(0.8, fra) * step(fra, 0.9)) * 16.0;
				float mt = (abs(rd.y) + frnd * 4.0 - _Time.y * 0.001) % 0.3; //split segments
    			pc *= step(mt, 0.16) * mt; //split segments
    			pc *= step(0.1, fra) * step(fra, 0.9); //edges
				return pc * max(abs(rd.y), 0.); //fade middle
			}

			float noise(float2 uv, float s1, float s2, float t1, float t2, float c1) {
				return clamp(hash33(float3(uv.xy * s1, t1)).x +
					         hash33(float3(uv.xy * s2, t2)).y, c1, 1.);
			}

			fixed4 frag (v2f i) : SV_Target {

				float cy = 1.0 - abs(i.viewT.y)*2.0;
                float nz = noise(i.uv*10.0, 12.0, 4.0, floor(_Time.y), floor(_Time.y), 0.96);
                
				float3 gc = PT(_Time.y);
				
				float at = (_Time.y) % 64;
				float3 ps = planes(i.viewT)*0.1;
				float3 kl = kali(i.viewT) * gc * 0.01;
				float3 col = lerp(ps, kl, smoothstep(28,32,at) * smoothstep(64,60,at));
                col *= nz;

				return float4(col, 1.0);
			}
			ENDCG
		}
	}
}
