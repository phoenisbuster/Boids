Shader "Shader Converted/ImpactEffect2DShader"
    {
        Properties
        {
            [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
            Vector1_86BAB8C1("Distortion strength", Range(-0.04, 0)) = -0.01
            Vector1_69DCFDC2("Size", Float) = 1
            [NoScaleOffset]Texture2D_9D556072("Distortion texture", 2D) = "white" {}
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Transparent"
                // DisableBatching: <None>
                "ShaderGraphShader"="true"
                "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
            }
            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "Universal2D"
                }
            
            // Render State
            Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            #define UNIVERSAL_USELEGACYSPRITEBLOCKS
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float3 ObjectSpacePosition;
                     float2 NDCPosition;
                     float2 PixelPosition;
                     float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                     float4 color : INTERP1;
                     float3 positionWS : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    output.color.xyzw = input.color;
                    output.positionWS.xyz = input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    output.color = input.color.xyzw;
                    output.positionWS = input.positionWS.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float Vector1_86BAB8C1;
                float Vector1_69DCFDC2;
                float4 Texture2D_9D556072_TexelSize;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraOpaqueTexture);
                SAMPLER(sampler_CameraOpaqueTexture);
                float4 _CameraOpaqueTexture_TexelSize;
                TEXTURE2D(Texture2D_9D556072);
                SAMPLER(samplerTexture2D_9D556072);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A * B;
                }
                
                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                {
                    Out = UV * Tiling + Offset;
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_A_4_Float = 0;
                    float _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float = Vector1_69DCFDC2;
                    float _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float);
                    float _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float);
                    float3 _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3 = float3(_Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float, _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float);
                    description.Position = _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float3 BaseColor;
                    float4 SpriteColor;
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    UnityTexture2D _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_CameraOpaqueTexture);
                    float4 _ScreenPosition_c58a4dde6aa61f87925aef71edfb1317_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
                    float _Property_b3ed086066f3b88b94a16a40e2c0143c_Out_0_Float = Vector1_86BAB8C1;
                    UnityTexture2D _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(Texture2D_9D556072);
                    float4 _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.tex, _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.samplerstate, _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_R_4_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.r;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_G_5_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.g;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_B_6_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.b;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_A_7_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.a;
                    float _Split_65504422f7050f889d4cbac138b8620a_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_65504422f7050f889d4cbac138b8620a_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_65504422f7050f889d4cbac138b8620a_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_65504422f7050f889d4cbac138b8620a_A_4_Float = 0;
                    float2 _Vector2_48592434a144908fad7bdceb9e5d2b18_Out_0_Vector2 = float2(_Split_65504422f7050f889d4cbac138b8620a_R_1_Float, _Split_65504422f7050f889d4cbac138b8620a_G_2_Float);
                    float2 _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2;
                    Unity_Multiply_float2_float2((_SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.xy), _Vector2_48592434a144908fad7bdceb9e5d2b18_Out_0_Vector2, _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2);
                    float2 _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2;
                    Unity_Multiply_float2_float2((_Property_b3ed086066f3b88b94a16a40e2c0143c_Out_0_Float.xx), _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2, _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2);
                    float2 _TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2;
                    Unity_TilingAndOffset_float((_ScreenPosition_c58a4dde6aa61f87925aef71edfb1317_Out_0_Vector4.xy), float2 (1, 1), _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2, _TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2);
                    float4 _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.tex, _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.samplerstate, _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2) );
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_R_4_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.r;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_G_5_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.g;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_B_6_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.b;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_A_7_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.a;
                    UnityTexture2D _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                    float4 _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.tex, _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.samplerstate, _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_R_4_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.r;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_G_5_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.g;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_B_6_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.b;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_A_7_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.a;
                    float4 _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4;
                    Unity_Multiply_float4_float4(_SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4, (_SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_A_7_Float.xxxx), _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4);
                    surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
                    surface.SpriteColor = _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4;
                    surface.Alpha = 1;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    
                
                
                
                
                
                    output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
                
                    #if UNITY_UV_STARTS_AT_TOP
                    output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                    #else
                    output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                    #endif
                
                    output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
                    output.NDCPosition.y = 1.0f - output.NDCPosition.y;
                
                    output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "SceneSelectionPass"
                Tags
                {
                    "LightMode" = "SceneSelectionPass"
                }
            
            // Render State
            Cull Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define UNIVERSAL_USELEGACYSPRITEBLOCKS
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENESELECTIONPASS 1
                
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float Vector1_86BAB8C1;
                float Vector1_69DCFDC2;
                float4 Texture2D_9D556072_TexelSize;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraOpaqueTexture);
                SAMPLER(sampler_CameraOpaqueTexture);
                float4 _CameraOpaqueTexture_TexelSize;
                TEXTURE2D(Texture2D_9D556072);
                SAMPLER(samplerTexture2D_9D556072);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_A_4_Float = 0;
                    float _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float = Vector1_69DCFDC2;
                    float _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float);
                    float _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float);
                    float3 _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3 = float3(_Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float, _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float);
                    description.Position = _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    surface.Alpha = 1;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "ScenePickingPass"
                Tags
                {
                    "LightMode" = "Picking"
                }
            
            // Render State
            Cull Back
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define UNIVERSAL_USELEGACYSPRITEBLOCKS
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
                #define SCENEPICKINGPASS 1
                
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float Vector1_86BAB8C1;
                float Vector1_69DCFDC2;
                float4 Texture2D_9D556072_TexelSize;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraOpaqueTexture);
                SAMPLER(sampler_CameraOpaqueTexture);
                float4 _CameraOpaqueTexture_TexelSize;
                TEXTURE2D(Texture2D_9D556072);
                SAMPLER(samplerTexture2D_9D556072);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_A_4_Float = 0;
                    float _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float = Vector1_69DCFDC2;
                    float _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float);
                    float _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float);
                    float3 _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3 = float3(_Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float, _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float);
                    description.Position = _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    surface.Alpha = 1;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    
                
                
                
                
                
                
                    #if UNITY_UV_STARTS_AT_TOP
                    #else
                    #endif
                
                
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }
            
            // Render State
            Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest LEqual
                ZWrite Off
            
            // Debug
            // <None>
            
            // --------------------------------------------------
            // Pass
            
            HLSLPROGRAM
            
            // Pragmas
            #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
            
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
            
            // Defines
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            #define UNIVERSAL_USELEGACYSPRITEBLOCKS
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
            
            
            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
            
            // --------------------------------------------------
            // Structs and Packing
            
            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
            
            struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float3 ObjectSpacePosition;
                     float2 NDCPosition;
                     float2 PixelPosition;
                     float4 uv0;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float4 texCoord0 : INTERP0;
                     float4 color : INTERP1;
                     float3 positionWS : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
            
            PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.texCoord0.xyzw = input.texCoord0;
                    output.color.xyzw = input.color;
                    output.positionWS.xyz = input.positionWS;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.texCoord0 = input.texCoord0.xyzw;
                    output.color = input.color.xyzw;
                    output.positionWS = input.positionWS.xyz;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }
                
            
            // --------------------------------------------------
            // Graph
            
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float Vector1_86BAB8C1;
                float Vector1_69DCFDC2;
                float4 Texture2D_9D556072_TexelSize;
                CBUFFER_END
                
                
                // Object and Global properties
                SAMPLER(SamplerState_Linear_Repeat);
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraOpaqueTexture);
                SAMPLER(sampler_CameraOpaqueTexture);
                float4 _CameraOpaqueTexture_TexelSize;
                TEXTURE2D(Texture2D_9D556072);
                SAMPLER(samplerTexture2D_9D556072);
            
            // Graph Includes
            // GraphIncludes: <None>
            
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
            
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
            
            // Graph Functions
            
                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }
                
                void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
                {
                    Out = A * B;
                }
                
                void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
                {
                    Out = UV * Tiling + Offset;
                }
                
                void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
                {
                    Out = A * B;
                }
            
            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
            
            // Graph Vertex
            struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };
                
                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_1c77163b1423ad8381384e4c70d3bdb0_A_4_Float = 0;
                    float _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float = Vector1_69DCFDC2;
                    float _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_R_1_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float);
                    float _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float;
                    Unity_Multiply_float_float(_Split_1c77163b1423ad8381384e4c70d3bdb0_G_2_Float, _Property_23f5f771fcef1084a755281e79e4a27f_Out_0_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float);
                    float3 _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3 = float3(_Multiply_cd5ce3c0f656e58eaca4edc145c2cc81_Out_2_Float, _Multiply_a2c8024ed9cb3f8ab0833ee8584cb0c6_Out_2_Float, _Split_1c77163b1423ad8381384e4c70d3bdb0_B_3_Float);
                    description.Position = _Vector3_39fff91d497bc8859d537c24b1fec6c0_Out_0_Vector3;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }
            
            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif
            
            // Graph Pixel
            struct SurfaceDescription
                {
                    float3 BaseColor;
                    float4 SpriteColor;
                    float Alpha;
                };
                
                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    UnityTexture2D _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_CameraOpaqueTexture);
                    float4 _ScreenPosition_c58a4dde6aa61f87925aef71edfb1317_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
                    float _Property_b3ed086066f3b88b94a16a40e2c0143c_Out_0_Float = Vector1_86BAB8C1;
                    UnityTexture2D _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(Texture2D_9D556072);
                    float4 _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.tex, _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.samplerstate, _Property_ff70c7d5c7772186ac5f84c8b5f57f9f_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_R_4_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.r;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_G_5_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.g;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_B_6_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.b;
                    float _SampleTexture2D_67efa7c50306018e9f538b266f481655_A_7_Float = _SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.a;
                    float _Split_65504422f7050f889d4cbac138b8620a_R_1_Float = IN.ObjectSpacePosition[0];
                    float _Split_65504422f7050f889d4cbac138b8620a_G_2_Float = IN.ObjectSpacePosition[1];
                    float _Split_65504422f7050f889d4cbac138b8620a_B_3_Float = IN.ObjectSpacePosition[2];
                    float _Split_65504422f7050f889d4cbac138b8620a_A_4_Float = 0;
                    float2 _Vector2_48592434a144908fad7bdceb9e5d2b18_Out_0_Vector2 = float2(_Split_65504422f7050f889d4cbac138b8620a_R_1_Float, _Split_65504422f7050f889d4cbac138b8620a_G_2_Float);
                    float2 _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2;
                    Unity_Multiply_float2_float2((_SampleTexture2D_67efa7c50306018e9f538b266f481655_RGBA_0_Vector4.xy), _Vector2_48592434a144908fad7bdceb9e5d2b18_Out_0_Vector2, _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2);
                    float2 _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2;
                    Unity_Multiply_float2_float2((_Property_b3ed086066f3b88b94a16a40e2c0143c_Out_0_Float.xx), _Multiply_d87f69c26d41dd81a374f4cb5f9ad423_Out_2_Vector2, _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2);
                    float2 _TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2;
                    Unity_TilingAndOffset_float((_ScreenPosition_c58a4dde6aa61f87925aef71edfb1317_Out_0_Vector4.xy), float2 (1, 1), _Multiply_ba4305a74b360484a4299c7742c236d5_Out_2_Vector2, _TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2);
                    float4 _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.tex, _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.samplerstate, _Property_f8ed4d001eea2e8d8ee991be70e99cb6_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_02eb253d4dba6b88afabb66a4bba8c50_Out_3_Vector2) );
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_R_4_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.r;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_G_5_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.g;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_B_6_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.b;
                    float _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_A_7_Float = _SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4.a;
                    UnityTexture2D _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                    float4 _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.tex, _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.samplerstate, _Property_69a58856847d648f9859315e03837fd1_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_R_4_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.r;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_G_5_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.g;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_B_6_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.b;
                    float _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_A_7_Float = _SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_RGBA_0_Vector4.a;
                    float4 _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4;
                    Unity_Multiply_float4_float4(_SampleTexture2D_f98f95a0aa8d6a82999ae9e929a8e12b_RGBA_0_Vector4, (_SampleTexture2D_d1c50c957c8b6b8286fbd794c530052b_A_7_Float.xxxx), _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4);
                    surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
                    surface.SpriteColor = _Multiply_5be059ea4b3ab483bb2d31c83bf4157d_Out_2_Vector4;
                    surface.Alpha = 1;
                    return surface;
                }
            
            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);
                
                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;
                
                    return output;
                }
                
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
                
                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
                    uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                    /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                    /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
                
                #endif
                
                    
                
                
                
                
                
                    output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
                
                    #if UNITY_UV_STARTS_AT_TOP
                    output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                    #else
                    output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
                    #endif
                
                    output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
                    output.NDCPosition.y = 1.0f - output.NDCPosition.y;
                
                    output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                
                        return output;
                }
                
            
            // --------------------------------------------------
            // Main
            
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
            
            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            
            ENDHLSL
            }
        }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
    }