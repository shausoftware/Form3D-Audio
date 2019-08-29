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

		#define UI0 1597334673U
		#define UI1 3812015801U
		#define UI2 uint2(UI0, UI1)
		#define UI3 uint3(UI0, UI1, 2798796415U)
		#define UIF (1.0 / float(0xffffffffU))

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

		float3 Kali(float3 rd) {
			float v = 0.0;
			float k = 0.0;
			for (float i = 0.0; i < 7.0; i++) {
				rd = abs(rd) / dot(rd, rd) - 0.63;
				k += length(rd) * length(hash33(rd + _Time.y*0.1));
				v += k*k*k*0.0003;
			}
			return v/7.0;
		}

		float3 Planes(float3 rd) {
			float a = (atan2(rd.y, rd.x) / 6.283185) + 0.5, //polar
					fla = floor(a * 32.0) / 32.0, //split into 32 segemnts
					fra = frac(a * 32.0),
					frnd = hash11(fla * 400.0);

			float3 pc = PT(fla*4.0+_Time.y)*16.0; //mix colours radially
			pc += (step(0.1, fra) * step(fra, 0.2)) * 16.0;
			pc += (step(0.8, fra) * step(fra, 0.9)) * 16.0;
			float mt = (abs(rd.y) + frnd * 4.0 - _Time.y * 0.01) % 0.3; //split segments
			pc *= step(mt, 0.16) * mt; //split segments
			pc *= step(0.1, fra) * step(fra, 0.9); //edges
			return pc * max(abs(rd.y), 0.); //fade middle
		}

		// particles (Andrew Baldwin)
		float Snow(float3 direction, float2 uv, float2 R) {
			float help = 0.0;
			const float3x3 p = float3x3(13.323122,23.5112,21.71123,21.1212,28.7312,11.9312,21.8112,14.7212,61.3934);
			float2 uvx = float2(direction.x,direction.z)+float2(1.0,R.y/R.x)*uv.xy / R.xy;
			float acc = 0.0;
			float DEPTH = direction.y*direction.y-0.3;
			float WIDTH =0.1;
			float SPEED = 0.1;
			for (int i=0;i<10;i++) {
				float fi = float(i);
				float2 q = uvx*(1.+fi*DEPTH);
				q += float2(q.y*(WIDTH* (fi*7.238917 % 1.0) -WIDTH*0.5),SPEED*_Time.y/(1.0+fi*DEPTH*0.03));
				float3 n = float3(floor(q),31.189+fi);
				float3 m = floor(n)*0.00001 + frac(n);
				float3 mp = (31415.9+m)/frac(mul(p, m));
				float3 r = frac(mp);
				float2 s = abs((q % 1.0)-0.5+0.9*r.xy-0.45);
				float d = 0.7*max(s.x-s.y,s.x+s.y)+max(s.x,s.y)-0.01;
				float edge = 0.04;
				acc += smoothstep(edge,-edge,d)*(r.x/1.0);
				help = acc;
			}
			return help;
		}

		void surf (Input IN, inout SurfaceOutput o) {

			//reflections
            float3 rd = normalize(IN.worldPos - _WorldSpaceCameraPos);
			float3 rrd = reflect(rd, IN.worldNormal);
        	float fresnel = pow(clamp(1.0 + dot(rd, IN.worldNormal), 0.0, 1.0), 4.0);
			float rCol = Planes(rrd)*fresnel;
			if (_Background==2) {
				rCol = Kali(rrd)*fresnel;
			} else if (_Background==3) {
				rCol = Snow(rrd, IN.uv_MainTex, _ScreenParams.zw)*fresnel;
			}

			o.Albedo = _Colour.rgb + rCol;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
