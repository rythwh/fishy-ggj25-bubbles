Shader "Custom/FirewatchFog"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FogAmount ("Fog Amount", float) = 1.0
        _FogIntensity ("Fog Intensity", float) = 1.0
        _ColorRamp ("Color Ramp", 2D) = "white" {}
    }
    SubShader
    {
        // Disable culling and depth writes
        Cull Off ZWrite Off ZTest Always

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
                float4 screenPos : TEXCOORD1; // For depth sampling
            };

            // Shader properties
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _ColorRamp;
            float _FogAmount;
            float _FogIntensity;

            // Vertex function
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex); // Compute screen-space position
                return o;
            }

            // Fragment function
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the original color from the main texture
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                // Sample the depth from the camera's depth texture
                float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));

                // Scale depth value by the fog amount
                float depthScaled = saturate(depthValue * _FogAmount);

                // Use the scaled depth to sample from the color ramp texture
                fixed4 fogColor = tex2D(_ColorRamp, float2(depthScaled, 0));

                // Blend the fog color with the original color based on fog intensity
                fixed4 finalColor = lerp(originalColor, fogColor, fogColor.a * _FogIntensity);

                // Return the blended color
                return finalColor;
            }
            ENDCG
        }
    }
}
