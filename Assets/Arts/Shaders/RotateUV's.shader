Shader "Custom/RotateUVs" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("UV Scale", float) = 1.0
        _DebugCenter ("Debug Center", Vector) = (0, 0, 0, 0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        float _Scale;
        float4 _DebugCenter;

        struct Input {
            float2 uv_MainTex;
        };

        float2x2 scale(float2 _scale) {
            return float2x2(_scale.x, 0.0,
                            0.0, _scale.y);
        }

        void vert (inout appdata_full v) {
            // Adjust scale dynamically
            float adjustedScale = _Scale / pow(_Scale, 2);
            float2 center = float2(_DebugCenter.x, _DebugCenter.z) / unity_ObjectToWorld[0][0];
            float2 worldScaleMatrixAdjusted = unity_ObjectToWorld[0][0] * adjustedScale;

            // Translate UVs based on the center
            v.texcoord.xy += center;

            // Scale UVs
            v.texcoord.xy -= float2(0.5, 0.5);
            v.texcoord.xy = mul(scale(worldScaleMatrixAdjusted), v.texcoord.xy);
            v.texcoord.xy += float2(0.5, 0.5);
        }

        void surf (Input IN, inout SurfaceOutput o) {  
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
