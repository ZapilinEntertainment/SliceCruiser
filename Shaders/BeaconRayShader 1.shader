Shader "Custom/BeaconRayShader"
{

	//acceptable only for special cube model

	Properties
	{		
		_MainColor("Main color", Color) = (1,1,1,1)
		_Intensity("Intensity", float) = 2
		_MaxDistance("Visibility distance", float) = 10000
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent"  "Queue" = "Transparent" }
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _MainColor;
	float _MaxDistance, _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;				
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float distance : COLOR;
				float height : TEXCOORD0;
            };


            v2f vert (appdata v)
            {
                v2f o;
				float distance = length(WorldSpaceViewDir(v.vertex));
				o.height = 1 - (v.vertex.y + 0.5); //from - 0.5 to 0.5
				o.vertex = UnityObjectToClipPos(float4(v.vertex.x * (1 + distance / 200), v.vertex.y * (1 + distance / 20000), v.vertex.z, v.vertex.w));
				o.distance = distance;
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 col = _MainColor;
				float d = 1 - clamp(i.distance / _MaxDistance, 0, 1);
				float sh = sin(i.height);
				float its = _Intensity * d * sh ;
				col = float4(_MainColor.r * its, _MainColor.g *  its, _MainColor.b * its, d * sh);
				return col;
            }
            ENDCG
        }
    }
}
