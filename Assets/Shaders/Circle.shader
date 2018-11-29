Shader "Custom/Circle" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0.3, 0.9, 0.1, 0)
		_Color2 ("Color", Color) = (0.3, 0.9, 0.1, 0)
		_CircleParams("Circle", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		 Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ TWO_CIRCLES
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 worldPos : TEXCOORD0;
			};
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
				return o;
			}

			float4 _Color, _Color2, _CircleParams;

			fixed4 frag (v2f i) : SV_Target {
				float l = length(floor(i.worldPos * 8) * 0.125 - _CircleParams.xy);

				if(l >= _CircleParams.z && l < _CircleParams.z + 0.125) {
					return _Color;
				}

				#ifdef TWO_CIRCLES
				if(l >= _CircleParams.w && l < _CircleParams.w + 0.125) {
					return _Color2;
				}
				#endif

				return fixed4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
