Shader "Custom/ThresholdShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold ("Threshold", Range(0,1)) = 0.5
        _Darkness ("Darkness", Range(0.0001,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Lightness ("Lightness", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Threshold;
            float _Darkness;
            float _Smoothness;
            float _Lightness;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
    
                // Convert to grayscale for thresholding
                float luminance = dot(col.rgb, float3(0.3, 0.59, 0.11));


                // Smooth threshold mask (creates a gradual blend instead of a hard cut)
                float mask = smoothstep(_Threshold - _Smoothness, _Threshold + _Smoothness, luminance);


                float3 darkColor = _Lightness < 0.5 ? col.rgb * _Darkness : col.rgb / _Darkness; 
                // Blend between original and darkened version
                float3 finalColor = lerp(darkColor, col.rgb, mask);

                return fixed4(finalColor, col.a);
            }

            ENDCG
        }
    }
}
