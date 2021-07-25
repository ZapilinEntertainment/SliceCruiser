Shader "Custom/BeaconRayShader"
{
	Properties
	{
		_MainColor("Main color", Color) = (1,1,1,1)
		_MaxDistance("Visibility distance", float) = 5000
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

			half4 _MainColor;
			float _MaxDistance;

            struct appdata
            {
                float4 vertex : POSITION;				
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float distance : COLOR;
            };


            v2f vert (appdata v)
            {
                v2f o;
				float distance = length(WorldSpaceViewDir(v.vertex));
                o.vertex = UnityObjectToClipPos(float4(v.vertex.x * (1 + distance / 200), v.vertex.y * (1 + distance / 20000), v.vertex.z, v.vertex.w));
				o.distance = distance;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = _MainColor;
				//col.a = clamp(1 - i.distance / _MaxDistance, 0, 1);
			col = (1, 1, 1, 1 - clamp(i.distance / _MaxDistance,0,1) );
                return col;
            }
            ENDCG
        }
    }
}
