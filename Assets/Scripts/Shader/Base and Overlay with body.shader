Shader "Custom/BaseAndOverlayWithBody"
{
    Properties
    {
        _BodyTex ("Body Texture", 2D) = "white" {}      // Texture for the body (green portion of mask)
        _BaseTex ("Base Texture", 2D) = "black" {}      // Base texture (black portion of mask)
        _MaskTex ("Mask Texture", 2D) = "white" {}      // Mask texture (green = body, black = base)
        _OverlayTex ("Overlay Texture", 2D) = "white" {} // Overlay texture to apply on top of base
        _UseOverlay ("Use Overlay", Float) = 0          // Boolean to determine overlay usage
        _BaseColor ("Base Color", Color) = (1,1,1,1)    // Color to modify the base texture
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

            sampler2D _BodyTex;
            sampler2D _BaseTex;
            sampler2D _MaskTex;
            sampler2D _OverlayTex;
            float _UseOverlay;
            float4 _BaseColor; // Color to modify the base texture

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample all textures (Mask, Body, Base, and Overlay)
                float4 mask = tex2D(_MaskTex, i.uv); // Mask texture
                float4 bodyTex = tex2D(_BodyTex, i.uv); // Body texture (green region)
                float4 baseTex = tex2D(_BaseTex, i.uv); // Base texture (black region)
                float4 overlayTex = tex2D(_OverlayTex, i.uv); // Overlay texture

                // Modify the base texture with the base color
                float4 coloredBaseTex = baseTex * _BaseColor;

                // Apply Body Texture when mask is green (mask.g == 1)
                float4 finalBaseAndBodyTex = lerp(coloredBaseTex, bodyTex, mask.g);

                // Only apply overlay if _UseOverlay is set and overlay alpha is significant
                if (_UseOverlay > 0.5 && overlayTex.a > 0.0)
                {
                    // Blend overlay texture with the base texture using the overlay alpha
                    finalBaseAndBodyTex = lerp(finalBaseAndBodyTex, overlayTex, overlayTex.a * (1.0 - mask.g));
                }

                return finalBaseAndBodyTex;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
