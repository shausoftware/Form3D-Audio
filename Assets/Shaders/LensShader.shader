Shader "Form/LensShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScanFrequency ("Scan Frequency", Range(800.0, 2400.0)) = 2000.0
		_ScanShift ("Scan Shift", Range(0.1, 10.0)) = 0.8
		_Aberation ("Aberation", Range(0.0, 0.01)) = 0.0
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
			float _ScanFrequency;
			float _ScanShift;
			float _Aberation;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
				return o;
			}

			float2x2 rot(float x) {return float2x2(cos(x), sin(x), -sin(x), cos(x));}

			float4 offsetColour(float o, float4 uv) {
				float4 offset = float4(0,_Aberation,0,0);
				offset.xy = mul(offset.xy, rot(o));
				return tex2Dproj(_GrabTexture, uv + offset);
			}
			
			fixed4 frag (v2f i) : SV_Target {

				fixed4 col = tex2Dproj(_GrabTexture, i.grabUv); //image

				fixed4 colRed = offsetColour(0.0, i.grabUv);
				fixed4 colGreen = offsetColour(2.094395, i.grabUv);
				fixed4 colBlue = offsetColour(4.188790, i.grabUv);

				//col.xyz += float3(1,0,0)*0.2;
				//col.xyz *= sin((i.uv.y + _Time * _Shift) * _Frequency) * 0.3 + 0.8; //scan lines
				
				return float4(colRed.x, colGreen.y, colBlue.z, 1);
			}
			ENDCG
		}
	}
}
