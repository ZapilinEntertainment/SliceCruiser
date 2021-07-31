// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// hey look https://catlikecoding.com/unity/tutorials/flow/texture-distortion/

Shader "Custom/WaterShader"
{
    Properties
	{
		_DeepGlowTexture("Deep glow texture", 2D) = "black" {}
		_UpGlowTexture("Up glow texture", 2D) = "black"{}
		_MainColor("Main color", Color) = (0,1,1,1)
		_SecondaryColor("Secondary color", Color) = (0,0,0,0)
		_WaveMask("Wave mask", 2D) = "white" {}
		_UVScale("UV Scale", float) = 1
		_LightVal("Lightnings value", Range(0.01,1)) = 0.8
		_LightPower("Light power", float) = 100
		_DarkVal("Darkening", Range(0.01,1)) = 0.8
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 rpos : COLOR;
				float3 uv : TEXCOORD0;
            };

			half _UVScale, _LightVal, _DarkVal, _LightPower;
			sampler2D _DeepGlowTexture, _UpGlowTexture, _WaveMask;
			half4 _MainColor, _SecondaryColor;

            v2f vert (appdata v)
            {
                v2f o;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float height = tex2Dlod(_UpGlowTexture, float4(v.uv.xy, 0, 0));
				//worldPos.y += height * 1.5;
				o.vertex = mul(UNITY_MATRIX_VP, worldPos);
				o.rpos.xyz = worldPos.xyz;
				o.rpos.w = length(WorldSpaceViewDir(v.vertex));				
               
				o.uv.xy = float2(worldPos.x, worldPos.z) / _UVScale;
				o.uv.z= _Time.x * 0.1;
				
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float time = i.uv.z;
				float waveVal = tex2D(_WaveMask, float2(i.uv.x,i.uv.y + time * 6));
				float x = tex2D(_UpGlowTexture, float2(i.uv.x + time * 0.3 , i.uv.y - time * 0.25)).rgb;
                float y = tex2D(_DeepGlowTexture, float2(i.uv.x + time + sin(time) , i.uv.y + time)) ;
				
				
				float4 col = lerp(fixed4(lerp(_SecondaryColor.rgb, _MainColor.rgb, y * x).rgb,0), 1, waveVal * x);
                
				col = (0.55 + y) * _SecondaryColor + x * _MainColor * 0.3 + waveVal * x * y * x * _MainColor;
				// color corrections
				// - lightening:
				col.rgb += clamp(col.rgb - _LightVal, 0, 1) * _LightPower;
				// - darkening:
				float sr = length(col);
				float ld = clamp(col.g / _DarkVal,0,1);
				ld -= 1;
				ld *= -1;
				col.rgb = lerp (col.rgb, _SecondaryColor, ld);
				
				// apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
