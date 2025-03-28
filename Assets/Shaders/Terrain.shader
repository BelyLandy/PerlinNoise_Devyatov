Shader "Custom/TerrainShaderURP" 
{
    Properties
    {
        _HeightTex ("Height Map", 2D) = "white" {}
        _GTex ("Grass Texture", 2D) = "white" {}
        _RTex ("Rock Texture", 2D) = "white" {}
        _STex ("Snow Texture", 2D) = "white" {}
        _SandTex ("Sand Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_HeightTex); SAMPLER(sampler_HeightTex);
            TEXTURE2D(_SandTex); SAMPLER(sampler_SandTex);
            TEXTURE2D(_GTex); SAMPLER(sampler_GTex);
            TEXTURE2D(_RTex); SAMPLER(sampler_RTex);
            TEXTURE2D(_STex); SAMPLER(sampler_STex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv = IN.uv;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float height = SAMPLE_TEXTURE2D(_HeightTex, sampler_HeightTex, IN.uv).r;
                float4 grass = SAMPLE_TEXTURE2D(_GTex, sampler_GTex, IN.uv);
                float4 rock = SAMPLE_TEXTURE2D(_RTex, sampler_RTex, IN.uv);
                float4 snow = SAMPLE_TEXTURE2D(_STex, sampler_STex, IN.uv);
                float4 sand = SAMPLE_TEXTURE2D(_SandTex, sampler_SandTex, IN.uv);
                
                float4 color;
                
                if (height < 0.01)
                {
                    color = sand;
                }
                else if (0.18 < height < 0.4)
                {
                    float t = saturate((height - 0.01) / 0.2);
                    color = lerp(sand, grass, t);
                }
                else if (height < 0.7)
                {
                    float t = saturate((height - 0.4) / 0.3);
                    color = lerp(grass, rock, t);
                }
                else
                {
                    float t = saturate((height - 0.7) / 0.3);
                    color = lerp(rock, snow, t);
                }
                
                return color;
            }
            ENDHLSL
        }
    }
}
