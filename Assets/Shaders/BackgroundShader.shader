Shader "Form/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_KaliFactor("Kali Factor", Range(0.1, 2.0)) = 0.63
		_KaliIterations("Kali Iterations", Range(5, 10)) = 6
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

			float3 kali(float3 rd) {
    			float3 pc = float3(0.0,0.0,0.0);
    			float k = 0.0;
    			for (float i = 0.0; i < float(_KaliIterations); i++) {
        			rd = abs(rd) / dot(rd, rd) - _KaliFactor;
        			k += length(rd) * length(hash33(rd + _Time*0.01));
        			pc += lerp(float3(1.0,0.5,0.0), float3(0.0,1.0,0.0), i/float(_KaliIterations)) * k*k*k * 0.0003;
    			}
    			return pc;
			}

			float3 planes(float3 rd, float3 c) {
    			float a = (atan2(rd.y, rd.x) / 6.283185) + 0.5, //polar
					  fla = floor(a * 24.0) / 24.0, //split into 24 segemnts
					  fra = frac(a * 24.0),
					  frnd = hash11(fla * 400.0);
    			float3 pc = c * frnd * step(0.1, fra); //mix colours radially
				float mt = (abs(rd.y) + frnd * 4. - _Time * 0.1) % 0.3; //split segments
    			pc *= step(mt, 0.16) * mt * 16.; //split segments
    			return pc * max(abs(rd.y), 0.); //fade middle
			}

			//IQ cosine palattes
			//http://www.iquilezles.org/www/articles/palettes/palettes.htm
			float3 PT(float t) {
				return float3(0.5,0.5,0.5) + 
			       	   float3(0.5,0.5,0.5) * cos(6.28318 * (float3(1.0,1.0,1.0) * t * 0.1 + float3(0.0, 0.33, 0.67)));
		    }

			float noise(float2 uv, float s1, float s2, float t1, float t2, float c1) {
				return clamp(hash33(float3(uv.xy * s1, t1)).x +
					         hash33(float3(uv.xy * s2, t2)).y, c1, 1.);
			}

			fixed4 frag (v2f i) : SV_Target {

		        //float cy = 1.0 - abs(i.uv.y - 0.5) * 2.0;
				float cy = 1.0 - abs(i.viewT.y)*2.0;
                float nz = noise(i.uv, 12.0, 4.0, floor(_Time), floor(_Time), 0.96);
                
				float3 gc = PT(_Time);
				
                //float3 col = gc * pow(cy,0.5) + float3(1,1,1) * pow(cy,3.) * nz * 0.3;
				//float3 col = kali(i.viewT) * gc * 0.002;// + gc * pow(cy,6.0) * nz;
				//col -= nz*0.05;
                float3 col = planes(i.viewT, gc) * 0.3;

				return float4(col, 1.0);
			}
			ENDCG
		}
	}
}
