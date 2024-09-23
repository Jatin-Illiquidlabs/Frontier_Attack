// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Vision-Custom/Sprites-ScrollingNoise"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Color2 ("Tint2", Color) = (1,1,1,1)

        _ScrollSpeed ("Scroll Speed - Layer 1 XY, Layer 2 XY", Vector) = (0.25, 0.25, 0.25, 0.25)
        _ScrollTiling1 ("_ScrollTiling", Vector) = (0, 0, 1, 1)
        _ScrollTiling2 ("_ScrollTiling2", Vector) = (0, 0, 1, 1)

        _NoiseFrequency ("_NoiseFrequency - Layer 1, Layer 2", Vector) = (1, 1, 1, 1)
        _NoiseAmplitude ("_NoiseAmplitude - Layer 1, Layer 2", Vector) = (0.25, 0.25, 0.25, 0.25)

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex CustomSpriteVert
            #pragma fragment CustomSpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            fixed4 _NoiseFrequency;
            fixed4 _NoiseAmplitude;
            fixed4 _ScrollSpeed;
            fixed4 _ScrollTiling1;
            fixed4 _ScrollTiling2;
            fixed4 _Color2;

            struct custom_v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord2 : TEXCOORD1;
                float2 texShiftParams : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            custom_v2f CustomSpriteVert(appdata_t IN)
            {
                v2f OUT_original = SpriteVert(IN);
                custom_v2f OUT;
                OUT.vertex = OUT_original.vertex;
                OUT.color = OUT_original.color;
                OUT.texcoord = (IN.texcoord + _ScrollTiling1.xy) * _ScrollTiling1.zw + _ScrollSpeed.xy * _Time.y;
                OUT.texcoord2 = (IN.texcoord + _ScrollTiling2.xy) * _ScrollTiling2.zw + _ScrollSpeed.zw * _Time.y;

                const float sinTime = abs(sin(_Time.y));
                const float absSinTime = sin(_Time.y);
                const float stepThreshold = 0.97;
                const float phase = step(stepThreshold, absSinTime) * ((absSinTime - stepThreshold) / (1 - stepThreshold));

                OUT.texShiftParams.x = phase;
                OUT.texShiftParams.y = sinTime;

                return OUT;
            }

            fixed4 CustomSpriteFrag(custom_v2f IN) : SV_Target
            {
                float phase = IN.texShiftParams.x;
                IN.texcoord.x += sin(IN.texcoord.y * _NoiseFrequency.x + phase) * phase * _NoiseAmplitude.x;
                //IN.texcoord.y += sin(IN.texcoord.x * _NoiseFrequency.x * 0.15) * phase * 0.03;

                IN.texcoord2.x += sin(IN.texcoord2.y * _NoiseFrequency.y + IN.texShiftParams.y + phase * 1.345) * _NoiseAmplitude.y;
                fixed4 color = SampleSpriteTexture (IN.texcoord) * IN.color;
                fixed4 color2 = SampleSpriteTexture (IN.texcoord2) * IN.color * _Color2;

                color.rgb *= color.a;
                color2.rgb *= color2.a;

                color.rgb = (color.rgb + color2.rgb);

                return color;
            }

        ENDCG
        }
    }
}
