Shader "Hidden/CircleDraw" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0.3, 0.9, 0.1, 0)
		_CircleParams("Circle", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Cull Off
		ZWrite Off
		ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
			
				return o;
			}
			
			sampler2D _MainTex;
			float4 _CircleParams;
			float4 _Color;

			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);

				float2 worldPos = float2(i.uv.x - 0.5, i.uv.y - 0.5) * unity_OrthoParams.xy + 0.5 * _WorldSpaceCameraPos.xy;
				worldPos.x = floor(worldPos.x * 16) * 0.0625;
				worldPos.y = floor(worldPos.y * 16) * 0.0625;
				float l = 2 * length(worldPos - 0.5 * _CircleParams.xy);

				if(l >= _CircleParams.z && l < _CircleParams.w) {
					col += _Color;
				}

				return col;
			}
			ENDCG
		}
	}
}
