Shader "Custom/GreenMaskedTextureShader"
{
    Properties
    {
        _BaseTex ("Base Texture", 2D) = "white" {}
        _Mask ("Mask (Black-Green)", 2D) = "white" {}
        _NewTex ("New Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _BaseTex;
        sampler2D _Mask;
        sampler2D _NewTex;

        struct Input
        {
            float2 uv_BaseTex;
            float2 uv_Mask;
            float2 uv_NewTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseColor = tex2D(_BaseTex, IN.uv_BaseTex);  // Base texture color
            fixed4 mask = tex2D(_Mask, IN.uv_Mask);             // Mask texture color
            fixed4 newColor = tex2D(_NewTex, IN.uv_NewTex);     // New texture color
            
            // Use the green channel of the mask as the blend factor
            float blendFactor = mask.g;
            
            // Blend the new texture onto the base texture using the mask's green channel
          o.Albedo = lerp(baseColor.rgb, newColor.rgb, blendFactor * newColor.a);
            o.Alpha = baseColor.a; // Retain original alpha
        }
        ENDCG
    }
    FallBack "Diffuse"
}
