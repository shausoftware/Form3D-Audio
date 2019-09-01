Shader "Form/FrameShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Colour ("Colour", Color) = (1,1,1,1)
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
			fixed4 _Colour;
		    int _Background;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldSpacePos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {

				float3 pos = i.worldSpacePos,
				       rd = normalize(pos - _WorldSpaceCameraPos),
				       ld = normalize(LP - pos);
			    float df = max(0.001, dot(ld, i.worldNormal)),
				      spec = pow(max(dot(reflect(-ld, i.worldNormal), -rd), 0.0), 64.0),
					  fresnel = pow(clamp(1.0 + dot(rd, i.worldNormal), 0.0, 1.0), 4.0);

			    float3 pc = _Colour.rgb*df;

				//reflections
			    float3 rrd = reflect(rd, i.worldNormal);
				float3 rCol = Planes(rrd, _Time.y);
				if (_Background==2) {
					rCol = Kali(rrd, _Time.y);
				} else if (_Background==3) {
					rCol = Snow(normalize(float3(0.1,1.0,0.2)), i.uv, _ScreenParams.zw, rrd.y, _Time.y);
				}
				pc += rCol*0.4*fresnel;

				//specular
				pc += float3(1.0,0.8,0.5)*spec;

				return float4(pc, 1);
			}
			ENDCG
		}
	}
}
