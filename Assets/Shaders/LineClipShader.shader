Shader "Custom/LineClipShader"{
	//show values to edit in inspector
	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
		_myRange("Brightness",Range(0,3)) = 0.8
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_Metallic("Metalness", Range(0, 1)) = 0
		[HDR]_Emission("Emission", color) = (0,0,0)

		[HDR]_CutoffColor("Cutoff Color", Color) = (1,0,0,0)
	}

		SubShader{
			//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

			// render faces regardless if they point towards the camera or away from it
			Cull Off

			CGPROGRAM
			//the shader is a surface shader, meaning that it will be extended by unity in the background 
			//to have fancy lighting and other features
			//our surface shader function is called surf and we use our custom lighting model
			//fullforwardshadows makes sure unity adds the shadow passes the shader might need
			//vertex:vert makes the shader use vert as a vertex shader function
			#pragma surface surf MyTexColor
			#pragma target 3.0
			half4 LightingMyTexColor(SurfaceOutput s, half3 lightDir, half atten) {
			  half4 c;
			  c.rgb = s.Albedo;
			  c.a = s.Alpha;
			  return c;

			}
			sampler2D _MainTex;
			fixed4 _Color;

			half _Smoothness;
			half _Metallic;
			half3 _Emission;
			half _myRange;
			float4 _Plane[4];

			float4 _CutoffColor;

			//input struct which is automatically filled by unity
			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
				float facing : VFACE;
			};

			//the surface shader function which sets parameters the lighting function then uses
			void surf(Input i, inout SurfaceOutput o) {
				//calculate signed distance to plane
				float distance = dot(i.worldPos, _Plane[0].xyz);
				distance = distance + _Plane[0].w;
				//discard surface above plane
				clip(-distance);


				 distance = dot(i.worldPos, _Plane[1].xyz);
				distance = distance + _Plane[1].w;
				//discard surface above plane
				clip(-distance);

				 distance = dot(i.worldPos, _Plane[2].xyz);
				distance = distance + _Plane[2].w;
				//discard surface above plane
				clip(-distance);

				 distance = dot(i.worldPos, _Plane[3].xyz);
				distance = distance + _Plane[3].w;
				//discard surface above plane
				clip(-distance);

				float facing = i.facing * 0.5 + 0.5;

				//normal color stuff
				fixed4 col = _Color;
				//col *= _Color;
				o.Albedo = col.rgb * facing;
				o.Alpha = col.a;
				//o.Metallic = _Metallic * facing;
				//o.Smoothness = _Smoothness * facing;
				o.Emission = lerp(_CutoffColor, _Emission, facing);
			}
			ENDCG
		}
			FallBack "Standard" //fallback adds a shadow pass so we get shadows on other objects
}