Shader "Form/CoreShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_FormObject ("Form Object", Vector) = (0,0,0,0)
		_Animation ("Animation", Vector) = (0,0,0,0)
		_Background ("Background", Range(1, 3)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "FormCommon.glslinc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewT : TEXCOORD1;
				half3 worldNormal : TEXCOORD2;
				float4 worldSpacePos : TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _FormObject;
			fixed4 _Animation;
			float _Background;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
				return o;
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
			
			fixed4 frag (v2f i) : SV_Target {
				
				float3 pos = i.worldSpacePos,
				       rd = normalize(pos - _WorldSpaceCameraPos),
				       ld = normalize(LP - pos),
					   glow = PT2(_Time.x + _Animation.z + _Animation.w); //glow colour 
			    float r = length(pos - _FormObject.xyz), //radius of sphere
			          w = sphDensity(pos, rd, float4(_FormObject.xyz, r), 20.0),
				      av = clamp(_Animation.x*1.0, 0.0, 3.0),			  
				      df = max(0.001, dot(ld, i.worldNormal)),
				      spec = pow(max(dot(reflect(-ld, i.worldNormal), -rd), 0.0), 32.0),
				      fresnel = pow(clamp(1.0 + dot(rd, i.worldNormal), 0.0, 1.0), 2.0);

			    float3 pc = glow*0.1*df;
				pc += glow*pow(w, 8.0);
				pc += glow*av*w*w*w*w*w*3.0;

			    //reflections
			    float3 rrd = reflect(rd, i.worldNormal);
				float3 rCol = Planes(rrd, _Time.y);
				if (_Background==2) {
					rCol = Kali(rrd, _Time.y);
				} else if (_Background==3) {
					rCol = Snow(normalize(float3(0.1,1.0,0.2)), i.uv, _ScreenParams.zw, rrd.y, _Time.y);
					rCol = clamp(rCol, float3(0,0,0), float3(1,1,1));
				}
				pc += rCol*fresnel;

				//specular
				pc += float3(1.0,0.8,0.5)*spec;

				return float4(pc, 1);
			}

			ENDCG
		}
	}
}
