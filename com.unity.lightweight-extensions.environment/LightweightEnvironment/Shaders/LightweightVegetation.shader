Shader "LightweightPipeline/Vegetation (Physically Based)"
{
    Properties
    {
        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        _Color("Color", Color) = (0.5,0.5,0.5,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}
        
        // Vegetation
        _TrunkStiffness("Trunk Stiffness", Range(0.0, 1.0)) = 0.5
        _BranchStiffness("Branch Stiffness", Range(0.0, 1.0)) = 0.5
        _LeafStiffness("Leaf Stiffness", Range(0.0, 1.0)) = 0.5

        [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
    }

    SubShader
    {
        // Lightweight Pipeline tag is required. If Lightweight pipeline is not set in the graphics settings
        // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
        // material work with both Lightweight Pipeline and Builtin Unity Pipeline
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True"}
        LOD 300

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        Pass
        {
            // Lightmode matches the ShaderPassName set in LightweightPipeline.cs. SRPDefaultUnlit and passes with
            // no LightMode tag are also rendered by Lightweight Pipeline
            Name "StandardLit"
            Tags{"LightMode" = "LightweightForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _OCCLUSIONMAP

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP

            #pragma shader_feature _VOLUME_WIND

            // -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _VERTEX_LIGHTS
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ _SHADOWS_ENABLED
            #pragma multi_compile _ _LOCAL_SHADOWS_ENABLED
            #pragma multi_compile _ _SHADOWS_SOFT

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex VegetationLitPassVertex
            #pragma fragment VegetationLitPassFragment

            #include "../ShaderLibrary/LightweightVegetation.hlsl"
            #include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
            #include "LWRP/ShaderLibrary/LightweightPassLit.hlsl"

            struct VegetationLitVertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            LightweightVertexOutput VegetationLitPassVertex(VegetationLitVertexInput v)
            {
                LightweightVertexOutput o = (LightweightVertexOutput)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

            #ifdef _VOLUME_WIND
                float3 objectOrigin = UNITY_ACCESS_INSTANCED_PROP(Props, _Position);
                v.vertex.xyz = VegetationDeformation(v.vertex.xyz, objectOrigin, v.normal, v.color.xyz);
            #endif

                float3 posWS = TransformObjectToWorld(v.vertex.xyz);
                o.clipPos = TransformWorldToHClip(posWS);

                half3 viewDir = VertexViewDirWS(GetCameraPositionWS() - posWS);

            #ifdef _NORMALMAP
                o.normal.w = viewDir.x;
                o.tangent.w = viewDir.y;
                o.binormal.w = viewDir.z;
            #else
                o.viewDir = viewDir;
            #endif

                // initializes o.normal and if _NORMALMAP also o.tangent and o.binormal
                OUTPUT_NORMAL(v, o);

                // We either sample GI from lightmap or SH.
                // Lightmap UV and vertex SH coefficients use the same interpolator ("float2 lightmapUV" for lightmap or "half3 vertexSH" for SH)
                // see DECLARE_LIGHTMAP_OR_SH macro.
                // The following funcions initialize the correct variable with correct data
                OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
                OUTPUT_SH(o.normal.xyz, o.vertexSH);

                half3 vertexLight = VertexLighting(posWS, o.normal.xyz);
                half fogFactor = ComputeFogFactor(o.clipPos.z);
                o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

            #ifdef _SHADOWS_ENABLED
            #if SHADOWS_SCREEN
                o.shadowCoord = ComputeShadowCoord(o.clipPos);
            #else
                o.shadowCoord = TransformWorldToShadowCoord(posWS);
            #endif
            #endif

            #ifdef _ADDITIONAL_LIGHTS
                o.posWS = posWS;
            #endif

                return o;
            }

            half4 VegetationLitPassFragment(LightweightVertexOutput IN, half facing : VFACE) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(IN.uv, surfaceData);

                InputData inputData;
                InitializeInputData(IN, surfaceData.normalTS, inputData);

                inputData.normalWS *= facing;

                half4 color = LightweightFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);

                ApplyFog(color.rgb, inputData.fogCoord);
                return color;
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
            #include "LWRP/ShaderLibrary/LightweightPassShadow.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex VegetationDepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma shader_feature _VOLUME_WIND

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "../ShaderLibrary/LightweightVegetation.hlsl"
            #include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
            #include "LWRP/ShaderLibrary/LightweightPassDepthOnly.hlsl"

            struct VegetationDepthOnlyVertexInput
            {
                float4 position     : POSITION;
                float2 texcoord     : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            VertexOutput VegetationDepthOnlyVertex(VegetationDepthOnlyVertexInput v)
            {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            #ifdef _VOLUME_WIND
                float3 objectOrigin = UNITY_ACCESS_INSTANCED_PROP(Props, _Position);
                v.position.xyz = VegetationDeformation(v.position.xyz, objectOrigin, v.normal, v.color.xyz);
            #endif

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.clipPos = TransformObjectToHClip(v.position.xyz);
                return o;
            }

            ENDHLSL
        }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex LightweightVertexMeta
            #pragma fragment LightweightFragmentMeta

            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature EDITOR_VISUALIZATION

            #pragma shader_feature _SPECGLOSSMAP

            #include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
            #include "LWRP/ShaderLibrary/LightweightPassMetaPBR.hlsl"

            ENDHLSL
        }

    }
    FallBack "Hidden/InternalErrorShader"
    CustomEditor "LightweightVegetationGUI"
}