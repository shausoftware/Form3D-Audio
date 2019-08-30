Shader "Form/BackgroundShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Animation ("Animation", Vector) = (0,0,0,0)
		_Background ("Background", Range(1, 3)) = 1
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
			#include "FormCommon.glslinc"

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

			fixed4 _Animation;
			int _Background;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewT = normalize(WorldSpaceViewDir(v.vertex));//ObjSpaceViewDir is similar, but localspace.
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {

                float nz = noise(i.uv*10.0, 12.0, 4.0, floor(_Time.y), floor(_Time.y), 0.96);                
				
				float3 col = Planes(i.viewT, _Time.y);
				if (_Background==2) {
					col = Kali(i.viewT, _Time.y);
				} else if (_Background==3) {
					col = Snow(normalize(float3(0.1,2.0,0.2)), i.uv*2.0, _ScreenParams.zw, i.viewT.y, _Time.y);
				}
                col *= nz;

				return float4(col, 1.0);
			}
			ENDCG
		}
	}
}
