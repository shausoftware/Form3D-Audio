Shader "Form/FrameShader" {
	Properties {
		_Colour ("Colour", Color) = (1,1,1,1)
		_EnvScale ("Map Scale", Range(0.1, 10.0)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf SimpleSpecular

		fixed4 _Colour;
        float _EnvScale;

		struct Input {
			float3 worldPos;
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

		//Noise function from Shadertoy users IQ & Shane
		float3 n3D(float3 p) {
			float3 s = float3(7.0, 157.0, 113.0);
			float3 ip = floor(p); 
			p -= ip;
			float4 h = float4(0.0, s.yz, s.y + s.z) + dot(ip, s); 
			p = p * p * (3.0 - 2.0 * p);
			h = lerp(frac(sin(h) * 43758.5453), frac(sin(h + s.x) * 43758.5453), p.x);
			h.xy = lerp(h.xz, h.yw, p.y);
			return lerp(h.x, h.y, p.z);
		}

		// Verbatim from Shane - Terrain Lattice
		// https://www.shadertoy.com/view/XslyRH
		float3 envMap(float3 p) {    
    		p *= 2.;
    		p.xz += _Time * .5;
    		float n3D2 = n3D(p*_EnvScale);
    		// A bit of fBm.
    		float c = n3D(p)*.57 + n3D2*0.28 + n3D(p*4.0)*0.15;
    		c = smoothstep(0.5, 1., c); // Putting in some dark space.
    		p = float3(c*0.8, c*0.9, c);//vec3(c*c, c*sqrt(c), c); // Bluish tinge.
    		return lerp(p.zxy, p, n3D2*0.34 + 0.665); // Mixing in a bit of purple.
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Colour.rgb + envMap(IN.worldPos);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
