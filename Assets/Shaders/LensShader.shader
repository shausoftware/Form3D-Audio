Shader "Form/LensShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Frequency ("Frequency", Range(800.0, 2000.0)) = 1800.0
		_Shift ("Shift", Range(0.1, 10.0)) = 0.8
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent"}
		LOD 100

		GrabPass { "_GrabTexture" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 grabUv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _GrabTexture;
			float _Frequency;
			float _Shift;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2Dproj(_GrabTexture, i.grabUv);
				col.xyz += float3(1,0,0)*0.05;
				col.xyz *= sin((i.uv.y + _Time * _Shift) * _Frequency) * 0.3 + 0.8; 
				return col;
			}
			ENDCG
		}
	}
}
