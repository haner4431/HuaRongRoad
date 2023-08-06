Shader "Unlit/JellyShader_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",COLOR)=(1,1,1,1)
        _Initensity("Intensity",float )=1
        _CenterOffset("CenterOffset",vector)=(0,0,0,0)
        _SizeY("SizeY",float)=1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST,_Color;
            float4 _CenterOffset;
            float3 _AbstracPos;
            float _Initensity,_SizeY;

            v2f vert (appdata v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float intensity=((worldPos.y-_CenterOffset.y)/_SizeY)*_Initensity;  

                float3 newVertex=lerp(v.vertex,_AbstracPos+v.vertex,intensity);
                o.vertex = UnityObjectToClipPos(newVertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
