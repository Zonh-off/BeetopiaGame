Shader "Unlit/PixelOutlineWithWind"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Outline Color", Color) = (1,1,1,1)
        _Radius ("Outline Radius", Range(0,10)) = 1
        _WindStrength ("Wind Strength", Float) = 0.05
        _WindSpeed ("Wind Speed", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            float4 _Color;
            float _Radius;
            float _WindStrength;
            float _WindSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float heightFactor = saturate(v.vertex.y);
                float phaseOffset = v.vertex.x * 15 + v.vertex.y * 7;
                float sway = sin(_Time.y * _WindSpeed + phaseOffset * 10.0) * _WindStrength * heightFactor;

                float4 displacedVertex = v.vertex;
                displacedVertex.x += sway;

                o.vertex = UnityObjectToClipPos(displacedVertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float alphaSum = 0;
                float r = _Radius;

                // Loop through surrounding texels for outline detection
                for (int nx = -r; nx <= r; nx++)
                {
                    for (int ny = -r; ny <= r; ny++)
                    {
                        if (nx * nx + ny * ny <= r * r)
                        {
                            float2 offset = float2(_MainTex_TexelSize.x * nx, _MainTex_TexelSize.y * ny);
                            fixed4 sample = tex2D(_MainTex, i.uv + offset);
                            alphaSum += ceil(sample.a);
                        }
                    }
                }

                alphaSum = clamp(alphaSum, 0, 1);
                fixed4 c = tex2D(_MainTex, i.uv);
                alphaSum -= ceil(c.a); // don't outline the center pixel

                return lerp(c, _Color, alphaSum);
            }
            ENDCG
        }
    }
}


/*Shader "Unlit/PixelOutline"
{ 
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
         _Color("Color", Color) = (1,1,1,1)
         _Radius("Radius", Range(0,10)) = 1
    } 
    SubShader 
    {
        Tags
        {
            "RenderType"="Transparent"
            
        }
    Blend SrcAlpha OneMinusSrcAlpha 
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
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            float4 _Color;

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float na = 0;
                float r = _Radius;

                for (int nx = -r; nx <= r; nx++)
                {
                    for (int ny = -r; ny <= r; ny++)
                    {
                        if (nx*nx+ny*ny <= r)
                        {
                            fixed4 nc = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*nx, _MainTex_TexelSize.y*ny));
                            na+=ceil(nc.a);
                        }
                    }
                }

                na = clamp(na,0,1);

                fixed4 c = tex2D(_MainTex, i.uv);
                na-=ceil(c.a);

                return lerp(c, _Color, na);
            }
            ENDCG
        }
    }
}*/