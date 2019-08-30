Shader "Form/LensShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScanFrequency ("Scan Frequency", Range(800.0, 2400.0)) = 2000.0
		_ScanShift ("Scan Shift", Range(0.1, 10.0)) = 0.8
		_GlitchAmount ("Glitch Amount", Range(0.0, 1.0)) = 0.0
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
			#include "FormCommon.glslinc"

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
			float _GlitchAmount;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex));
				return o;
			}

			float2x2 rot(float x) {return float2x2(cos(x), sin(x), -sin(x), cos(x));}

			float4 offsetColour(float a, float r, float w, float4 uv) {
				float4 offset = float4(0,r,0,0);
				offset.xy = mul(offset.xy, rot(a));
				return tex2Dproj(_GrabTexture, uv + offset + w);
			}
			
			fixed4 frag (v2f i) : SV_Target {

				//glitch routine
                float nz = noise(i.uv*10.0, 32.0, 63.0, floor(_Time.y*33.0), floor(_Time.y*3.0), 0.06);                
				//TODO: nz2 term is nice
				float nz2 = hash11(i.uv.y*31.0+89.0+_Time.y*9.0);
				float nz3 = (hash11(i.uv.y*1027.0+33.0+_Time.y*29.0) * nz2); 
				float wobble = (sin(i.uv.y*387.0+_Time.y*95.0)*0.001 + 
							    sin(i.uv.y*96.0+_Time.y*21.0)*0.03 * max(0.0, sin(_Time.y*41.0)*0.7)) *
								_GlitchAmount;
				float red = offsetColour(0.0, 
				                         clamp(sin(_Time.y*13.0), 0.0, 0.004), 
										 wobble, 
										 i.grabUv).x;
				float green = offsetColour(2.094395, 
				                           clamp(sin(_Time.y*31.0+27.0), 0.0, 0.007), 
										   wobble, 
										   i.grabUv).y;
				float blue = offsetColour(4.188790, 
				                          clamp(sin(_Time.y*27.0-27.0), 0.0, 0.005), 
										  wobble, 
										  i.grabUv).z;

				float3 col = float3(red, green, blue);
				float3 nzCol = float3(1,1,1) * (1.0 - nz); //snow noise
				col = lerp(col, nzCol*nz3, _GlitchAmount*0.4);
				col = lerp(tex2Dproj(_GrabTexture, i.grabUv).xyz, col, _GlitchAmount);
				col.xyz *= sin((i.uv.y + _Time.x * _ScanShift) * _ScanFrequency) * 0.3 + 0.8; //scan lines
				col = pow(col, float3(1.0/2.2,1.0/2.2,1.0/2.2)); //gamma correction

				return float4(col.x, col.y, col.z, 1.0);
			}
			ENDCG
		}
	}
}
