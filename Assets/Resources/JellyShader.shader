

Shader "Unlit/JellyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Initensity("Intensity",float )=1
        _Mass("Mass",float)=1
        _Stiffness("Stiffness",float)=1
        _Damping("Damping",float)=0.75
        _CenterOffset("CenterOffset",vector)=(0,0,0,0)
        _LastCenter("LastCenter",vector)=(0,0,0,0)
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _CenterOffset,_LastCenter;
            float _Initensity,_Mass,_Stiffness,_Damping,_SizeY;

            v2f vert (appdata v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 center=float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
                float intensity=((worldPos.y-_CenterOffset.y)/_SizeY)*_Initensity;

                //shake
                float3 force=(_LastCenter-center)*_Stiffness;
                float3 velocity=(force/_Mass)*_Damping; //?velocity = (velocity + Force / m) * d;
                float3 modelVelcity=mul(unity_WorldToObject,velocity);

                float3 tarVertex=v.vertex+modelVelcity;
                float3 newVertex=lerp(v.vertex,tarVertex,intensity);
                o.vertex = UnityObjectToClipPos(newVertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
