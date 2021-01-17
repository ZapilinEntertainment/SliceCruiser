// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TextureFillingShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Main color", Color) = (0,1,1,1)
		_TextureSize("Texture size", Range(0, 1000)) = 75
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
				};

				struct v2f
				{
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					float3 rpos : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				half4 _MainColor;
				float _TextureSize;

				v2f vert(appdata v)
				{
					v2f o;
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.rpos = worldPos;
					o.vertex = UnityObjectToClipPos(v.vertex);

					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					float x = sin(tex2D(_MainTex, float2(i.rpos.x / _TextureSize + i.rpos.y / _TextureSize * 0.5 ,  i.rpos.z / _TextureSize - i.rpos.y / _TextureSize * 0.5)));
					fixed4 col = x * x * _MainColor;
					//col = i.rpos.x / 100 * 0.5 + i.rpos.z / 100 * 0.5;
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}
