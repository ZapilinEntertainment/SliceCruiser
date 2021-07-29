Shader "Custom/InnerHemisphereShader"
{
    Properties
    {
		[HDR]_MainColor("Color", Color) = (1,1,1,1)
		[HDR]_UpperColor("Upper color", Color) = (1,1,1,1)
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;				
            };

            struct v2f
            {
				float4 vertex : POSITION;
				float height : TEXCOORD0;
            };

			half4 _MainColor, _UpperColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.height = v.vertex.z;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = lerp(_MainColor, _UpperColor, i.height);                
                return col;
            }
            ENDCG
        }
    }
}
