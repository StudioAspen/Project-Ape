// Made with Amplify Shader Editor v1.9.9.13
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/Cubemap Extended"
{
	Properties
	{
		[StyledBanner(Skybox Cubemap Extended)] _SkyboxExtended( "< SkyboxExtended >", Float ) = 1
		[StyledCategory(Cubemap Settings, 5, 10)] _Cubemapp( "[ Cubemapp ]", Float ) = 1
		[NoScaleOffset][StyledTextureSingleLine] _Tex( "Cubemap (HDR)", CUBE ) = "black" {}
		[Space(10)] _Exposure( "Cubemap Exposure", Range( 0, 8 ) ) = 1
		[Gamma] _TintColor( "Cubemap Tint Color", Color ) = ( 0.5, 0.5, 0.5, 1 )
		_CubemapPosition( "Cubemap Position", Float ) = 0
		[StyledCategory(Rotation Settings)] _Rotationn( "[ Rotationn ]", Float ) = 1
		[Toggle( _ENABLEROTATION_ON )] _EnableRotation( "Enable Rotation", Float ) = 0
		[IntRange][Space(10)] _Rotation( "Rotation", Range( 0, 360 ) ) = 0
		_RotationSpeed( "Rotation Speed", Float ) = 1
		[StyledCategory(Fog Settings)] _Fogg( "[ Fogg ]", Float ) = 1
		[Toggle( _ENABLEFOG_ON )] _EnableFog( "Enable Fog", Float ) = 0
		[StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableFog, 1, 10, 0)] _FogMessage( "# FogMessage", Float ) = 0
		[Space(10)] _FogIntensity( "Fog Intensity", Range( 0, 1 ) ) = 1
		_FogHeight( "Fog Height", Range( 0, 1 ) ) = 1
		_FogSmoothness( "Fog Smoothness", Range( 0.01, 1 ) ) = 0.01
		_FogFill( "Fog Fill", Range( 0, 1 ) ) = 0.5
		[HideInInspector] _Tex_HDR( "DecodeInstructions", Vector ) = ( 0, 0, 0, 0 )
		_FogPosition( "Fog Position", Float ) = 0

	}
	
	SubShader
	{
		

		

		Tags { "RenderType"="Background" "Queue"="Background" "PreviewType"="Skybox" }

	LOD 0

		ZWrite Off
		ZClip False
		Cull Off
		AlphaToMask Off
		ColorMask RGBA
		Blend One Zero, One Zero
		BlendOp Add, Add
		Offset 0,0

		

		Blend Off
		

		CGINCLUDE
			#pragma target 2.0
			// ensure rendering platforms toggle list is visible

			#ifndef GLOBAL_HEADER_INCLUDED
			#define GLOBAL_HEADER_INCLUDED
			float4 ComputeClipSpacePosition( float2 screenPosNorm, float deviceDepth )
			{
				float4 positionCS = float4( screenPosNorm * 2.0 - 1.0, deviceDepth, 1.0 );
			#if UNITY_UV_STARTS_AT_TOP
				positionCS.y = -positionCS.y;
			#endif
				return positionCS;
			}
			#endif
		ENDCG

		
		Pass
		{
			
			Name "Unlit"

			Cull Back
			ZWrite Off
			ZTest LEqual
			Offset 0,0
			ColorMask RGBA
			Blend One Zero, One Zero
			BlendOp Add, Add

			

			CGPROGRAM
				#define ASE_VERSION 19913

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#include "UnityCG.cginc"

				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_VERT_POSITION
				#pragma shader_feature_local _ENABLEFOG_ON
				#pragma shader_feature_local _ENABLEROTATION_ON


				#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
					#define ASE_SV_DEPTH SV_DepthLessEqual
					#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
				#else
					#define ASE_SV_DEPTH SV_Depth
					#define ASE_SV_POSITION_QUALIFIERS
				#endif

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					ASE_SV_POSITION_QUALIFIERS float4 pos : SV_POSITION;
					float4 ase_texcoord : TEXCOORD0;
					float4 ase_texcoord1 : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				uniform half _Cubemapp;
				uniform half _SkyboxExtended;
				uniform half _Rotationn;
				uniform half _FogMessage;
				uniform half4 _Tex_HDR;
				uniform half _Fogg;
				uniform samplerCUBE _Tex;
				uniform float _CubemapPosition;
				uniform half _Rotation;
				uniform half _RotationSpeed;
				uniform half4 _TintColor;
				uniform half _Exposure;
				uniform float _FogPosition;
				uniform half _FogHeight;
				uniform half _FogSmoothness;
				uniform half _FogFill;
				uniform half _FogIntensity;


				inline half3 DecodeHDR1189( float4 Data )
				{
					return DecodeHDR(Data, _Tex_HDR);
				}
				

				v2f vert( appdata v  )
				{
					UNITY_SETUP_INSTANCE_ID(v);
					v2f o;
					UNITY_INITIALIZE_OUTPUT(v2f,o);
					UNITY_TRANSFER_INSTANCE_ID(v,o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
					half CAMERA_MODE300 = lerpResult268;
					float3 appendResult1220 = (float3(v.vertex.xyz.x , ( v.vertex.xyz.y * CAMERA_MODE300 ) , v.vertex.xyz.z));
					float3 appendResult1208 = (float3(0.0 , -_CubemapPosition , 0.0));
					float3 staticSwitch1164 = ( float3 )0;
					#ifdef _ENABLEROTATION_ON
					{
						half3 VertexPos40_g1 = appendResult1220;
						float3 appendResult74_g1 = (float3(0.0 , VertexPos40_g1.y , 0.0));
						float3 VertexPosRotationAxis50_g1 = appendResult74_g1;
						float3 break84_g1 = VertexPos40_g1;
						float3 appendResult81_g1 = (float3(break84_g1.x , 0.0 , break84_g1.z));
						float3 VertexPosOtherAxis82_g1 = appendResult81_g1;
						half Angle44_g1 = ( 1.0 - radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) );
						staticSwitch1164 = ( ( VertexPosRotationAxis50_g1 + ( VertexPosOtherAxis82_g1 * cos( Angle44_g1 ) ) + ( cross( float3( 0, 1, 0 ) , VertexPosOtherAxis82_g1 ) * sin( Angle44_g1 ) ) ) + appendResult1208 );
					}
					#else
					{
						staticSwitch1164 = ( appendResult1220 + appendResult1208 );
					}
					#endif
					float3 vertexToFrag774 = staticSwitch1164;
					o.ase_texcoord.xyz = vertexToFrag774;
					
					o.ase_texcoord1 = v.vertex;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord.w = 0;

					#ifdef ASE_ABSOLUTE_VERTEX_POS
						float3 defaultVertexValue = v.vertex.xyz;
					#else
						float3 defaultVertexValue = float3(0, 0, 0);
					#endif
					float3 vertexValue = defaultVertexValue;
					#ifdef ASE_ABSOLUTE_VERTEX_POS
						v.vertex.xyz = vertexValue;
					#else
						v.vertex.xyz += vertexValue;
					#endif
					v.vertex.w = 1;
					v.normal = v.normal;
					v.tangent = v.tangent;

					o.pos = UnityObjectToClipPos( v.vertex );

					#if defined( ASE_SHADOWS )
						UNITY_TRANSFER_SHADOW( o, v.texcoord );
					#endif
					return o;
				}

				half4 frag( v2f IN 
							#if defined( ASE_WRITE_DEPTH )
								, out float outputDepth : SV_Depth
							#endif
				) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( IN );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

					float4 ScreenPosNorm = float4( IN.pos.xy * ( _ScreenParams.zw - 1.0 ), IN.pos.zw );
					float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, IN.pos.z ) * IN.pos.w;
					float4 ScreenPos = ComputeScreenPos( ClipPos );

					float3 vertexToFrag774 = IN.ase_texcoord.xyz;
					half4 Data1189 = texCUBE( _Tex, vertexToFrag774 );
					half3 localDecodeHDR1189 = DecodeHDR1189( Data1189 );
					half4 CUBEMAP222 = ( float4( localDecodeHDR1189 , 0.0 ) * unity_ColorSpaceDouble * _TintColor * _Exposure );
					float4 staticSwitch1179 = ( float4 )0;
					#ifdef _ENABLEFOG_ON
					{
						float lerpResult678 = lerp( saturate( pow(  (0.0 + ( abs( ( IN.ase_texcoord1.xyz.y + -_FogPosition ) ) - 0.0 ) * ( 1.0 - 0.0 ) / ( _FogHeight - 0.0 ) ) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
						float lerpResult1205 = lerp( 1.0 , lerpResult678 , _FogIntensity);
						half FOG_MASK359 = lerpResult1205;
						float4 lerpResult317 = lerp( unity_FogColor , CUBEMAP222 , FOG_MASK359);
						staticSwitch1179 = lerpResult317;
					}
					#else
					{
						staticSwitch1179 = CUBEMAP222;
					}
					#endif
					

					float3 Color = staticSwitch1179.rgb;
					float Alpha = 1;
					half AlphaClipThreshold = 0.5;
					half AlphaClipThresholdShadow = 0.5;

					#if defined( ASE_WRITE_DEPTH )
						outputDepth = IN.pos.z;
					#endif

					#ifdef _ALPHATEST_ON
						clip( Alpha - AlphaClipThreshold );
					#endif

				#if defined( ASE_SURFACE_TRANSPARENT ) || defined( ASE_OPAQUE_KEEP_ALPHA )
					return half4( Color, Alpha );
				#else
					return half4( Color, 1.0 );
				#endif
				}
			ENDCG
		}

	
	}
	
	CustomEditor "SkyboxExtended.MaterialGUI"
	
	Fallback "Skybox/Cubemap"
}
/*ASEBEGIN
Version=19913
{"type":"AmplifyShaderEditor.OrthoParams, AmplifyShaderEditor","id":267,"pos":[-892.4822,894.6918],"params":["Inherit","False","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1007,"pos":[-444.4821,894.6918],"params":["Half","False","Constant","_Float7","Float 7","47","0","Create","True","0","0","0","False","0","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor","id":309,"pos":[-588.4823,894.6918],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":260,"pos":[-896,2048],"params":["Half","False","Property","_RotationSpeed","Rotation Speed","9","0","Create","True","0","0","0","False","0","False","Object","-1","","1","2","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor","id":701,"pos":[-896,1920],"params":["Inherit","False","1","0","FLOAT","1","False","5","FLOAT","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":268,"pos":[-252.4821,894.6918],"params":["Inherit","False","3","0","FLOAT","1","False","1","FLOAT","0.5","False","2","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":48,"pos":[-896,1792],"params":["Half","False","Property","_Rotation","Rotation","8","1","[IntRange]","Create","True","0","0","0","False","1","Space(10)","False","Object","-1","","0","0","0","360","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":255,"pos":[-640,1920],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":300,"pos":[3.51777,894.6918],"params":["Half","False","CAMERA_MODE","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":276,"pos":[-512,1792],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1207,"pos":[-896,3008],"params":["Inherit","False","Property","_FogPosition","Fog Position","18","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":1218,"pos":[-896,1680],"params":["Inherit","False","300","CAMERA_MODE","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1211,"pos":[-128,1920],"params":["Inherit","False","Property","_CubemapPosition","Cubemap Position","5","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RadiansOpNode, AmplifyShaderEditor","id":47,"pos":[-384,1792],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.NegateNode, AmplifyShaderEditor","id":1214,"pos":[-704,3008],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor","id":1193,"pos":[-896,2560],"params":["Inherit","False","0","0","5","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.PosVertexDataNode, AmplifyShaderEditor","id":1221,"pos":[-896,1536],"params":["Inherit","False","0","0","5","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":1219,"pos":[-640,1664],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.NegateNode, AmplifyShaderEditor","id":1215,"pos":[128,1920],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor","id":1222,"pos":[-256,1792],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":1210,"pos":[-640,2560],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":1220,"pos":[-512,1536],"params":["Inherit","False","FLOAT3","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor","id":1208,"pos":[320,1920],"params":["Inherit","False","FLOAT3","4","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","3","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor","id":1217,"pos":[-128,1536],"params":["Inherit","False","Compute Rotation Y","-1","","1","693b7d13a80c93a4e8b791a9cd5e5ab2","0","2","38","FLOAT3","0,0,0","False","43","FLOAT","0","False","1","FLOAT3","19"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":325,"pos":[-896,2880],"params":["Half","False","Property","_FogSmoothness","Fog Smoothness","15","0","Create","True","0","0","0","False","0","False","Object","-1","","0.01","0.01","0.01","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.AbsOpNode, AmplifyShaderEditor","id":314,"pos":[-512,2560],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":313,"pos":[-896,2752],"params":["Half","False","Property","_FogHeight","Fog Height","14","0","Create","True","0","0","0","False","0","False","Object","-1","","1","0.6","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1109,"pos":[-512,2784],"params":["Half","False","Constant","_Float40","Float 40","55","0","Create","True","0","0","0","False","0","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1108,"pos":[-512,2688],"params":["Half","False","Constant","_Float39","Float 39","55","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":1206,"pos":[512,1792],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":1212,"pos":[512,1536],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor","id":329,"pos":[-256,2880],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TFHCRemapNode, AmplifyShaderEditor","id":315,"pos":[-320,2560],"params":["Inherit","False","5","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","1","False","3","FLOAT","0","False","4","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":1164,"pos":[704,1536],"params":["Float","False","Property","_EnableRotation","Enable Rotation","7","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","False","False","True","9","1","FLOAT3","0,0,0","False","0","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT3","0,0,0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":677,"pos":[-64,2560],"params":["Inherit","False","False","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.VertexToFragmentNode, AmplifyShaderEditor","id":774,"pos":[1024,1536],"params":["Inherit","False","False","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1110,"pos":[128,2752],"params":["Half","False","Constant","_Float41","Float 41","55","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":316,"pos":[128,2560],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":679,"pos":[128,2880],"params":["Half","False","Property","_FogFill","Fog Fill","16","0","Create","True","0","0","0","False","0","False","Object","-1","","0.5","0","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":41,"pos":[1536,1536],"params":["Inherit","True","Property","_Tex","Cubemap (HDR)","2","1","[NoScaleOffset]","Create","False","0","0","0","False","1","StyledTextureSingleLine","False","","-1","None","beb1457d375110e468b8d8e1f29fccea","True","0","False","black","LockedToCube","False","Object","-1","Auto","Cube","False","8","0","SAMPLERCUBE","","False","1","FLOAT3","0,0,0","False","2","FLOAT","0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1204,"pos":[448,2880],"params":["Half","False","Property","_FogIntensity","Fog Intensity","13","0","Create","True","0","0","0","False","1","Space(10)","False","Object","-1","","1","0","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":678,"pos":[384,2560],"params":["Inherit","False","3","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1177,"pos":[1920,1968],"params":["Half","False","Property","_Exposure","Cubemap Exposure","3","0","Create","False","0","0","0","False","1","Space(10)","False","Object","-1","","1","1","0","8","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CustomExpressionNode, AmplifyShaderEditor","id":1189,"pos":[1920,1536],"params":["Half","False","DecodeHDR(Data, _Tex_HDR)","3","Create","1","True","Data","FLOAT4","0,0,0,0","In","","Float","False","DecodeHDR","True","False","0","","False","1","0","FLOAT4","0,0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":1173,"pos":[1920,1792],"params":["Half","False","Property","_TintColor","Cubemap Tint Color","4","1","[Gamma]","Create","False","0","0","0","False","0","False","Object","-1","","0.5,0.5,0.5,1","0.5,0.5,0.5,1","False","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.ColorSpaceDouble, AmplifyShaderEditor","id":1175,"pos":[1920,1616],"params":["Inherit","False","0","5","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":1205,"pos":[640,2560],"params":["Inherit","False","3","0","FLOAT","1","False","1","FLOAT","0","False","2","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":1174,"pos":[2432,1536],"params":["Inherit","False","4","4","0","FLOAT3","0,0,0","False","1","COLOR","0,0,0,0","False","2","COLOR","0,0,0,0","False","3","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":359,"pos":[832,2560],"params":["Half","False","FOG_MASK","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":222,"pos":[2624,1536],"params":["Half","False","CUBEMAP","-1","True","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.FogAndAmbientColorsNode, AmplifyShaderEditor","id":312,"pos":[-896,128],"params":["Inherit","False","unity_FogColor","0","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":436,"pos":[-896,320],"params":["Inherit","False","359","FOG_MASK","1","0","OBJECT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":228,"pos":[-896,240],"params":["Inherit","False","222","CUBEMAP","1","0","OBJECT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":317,"pos":[-512,128],"params":["Inherit","False","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1197,"pos":[-640,-384],"params":["Half","False","Property","_Cubemapp","[ Cubemapp ]","1","0","Create","True","0","0","0","True","1","StyledCategory(Cubemap Settings, 5, 10)","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1196,"pos":[-896,-384],"params":["Half","False","Property","_SkyboxExtended","< SkyboxExtended >","0","0","Create","True","0","0","0","True","1","StyledBanner(Skybox Cubemap Extended)","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1198,"pos":[-448,-384],"params":["Half","False","Property","_Rotationn","[ Rotationn ]","6","0","Create","True","0","0","0","True","1","StyledCategory(Rotation Settings)","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1216,"pos":[-896,3136],"params":["Half","False","Property","_FogMessage","# FogMessage","12","0","Create","True","0","0","0","True","1","StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableFog, 1, 10, 0)","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":1179,"pos":[-224,224],"params":["Float","False","Property","_EnableFog","Enable Fog","11","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","False","False","True","9","1","COLOR","0,0,0,0","False","0","COLOR","0,0,0,0","False","2","COLOR","0,0,0,0","False","3","COLOR","0,0,0,0","False","4","COLOR","0,0,0,0","False","5","COLOR","0,0,0,0","False","6","COLOR","0,0,0,0","False","7","COLOR","0,0,0,0","False","8","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.Vector4Node, AmplifyShaderEditor","id":1190,"pos":[1536,1792],"params":["Half","False","Property","_Tex_HDR","DecodeInstructions","17","1","[HideInInspector]","Create","False","0","0","0","True","0","False","Object","-1","","0,0,0,0","1,1,0,0","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":1199,"pos":[-272,-384],"params":["Half","False","Property","_Fogg","[ Fogg ]","10","0","Create","True","0","0","0","True","1","StyledCategory(Fog Settings)","False","Object","-1","","1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":1194,"pos":[128,128],"params":["Float","False","True","-1","2","SkyboxExtended.MaterialGUI","0","12","Skybox/Cubemap Extended","0770190933193b94aaa3065e307002fa","True","Unlit","0","1","Unlit","8","False","True","0","1","False","","0","False","","0","1","False","","0","False","","True","0","False","","0","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","2","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","255","False","","255","False","","255","False","","7","False","","1","False","","1","False","","1","False","","7","False","","1","False","","1","False","","1","False","","True","True","2","False","","False","True","False","0","False","","0","False","","True","2","False","","True","3","RenderType=Background=RenderType","Queue=Background=Queue=0","PreviewType=Skybox","True","0","True","14","all","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","True","True","2","False","","True","0","False","","True","False","0","False","","0","False","","False","False","False","False","0","Skybox/Cubemap","0","0","Standard","10","Surface","0","0","  Keep Alpha","0","0","  Blend","0","0","Alpha Clipping","0","0","  Use Shadow Threshold","0","0","Cast Shadows","0","639270863057484492","Write Depth","0","0","  Conservative","0","0","Extra Pre Pass","0","0","Vertex Position","1","0","0","3","False","True","False","False","","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":1223,"pos":[128,128],"params":["Float","False","False","-1","3","AmplifyShaderEditor.MaterialInspector","0","1","New Amplify Shader","0770190933193b94aaa3065e307002fa","True","ExtraPrePass","0","0","ExtraPrePass","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","True","True","0","False","","0","False","","True","1","False","","True","1","RenderType=Opaque=RenderType","True","3","True","14","all","0","False","True","1","1","False","","0","False","","0","1","False","","0","False","","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","True","3","False","","True","True","0","False","","0","False","","False","False","False","False","0","","0","0","Standard","0","True","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":1224,"pos":[128,138],"params":["Float","False","False","-1","3","AmplifyShaderEditor.MaterialInspector","0","1","New Amplify Shader","0770190933193b94aaa3065e307002fa","True","ShadowCaster","0","2","ShadowCaster","0","False","True","1","1","False","","0","False","","1","1","False","","0","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","True","0","False","","False","True","0","False","","False","True","True","True","True","True","0","False","","False","False","False","False","False","False","False","True","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","True","True","0","False","","0","False","","True","1","False","","True","1","RenderType=Opaque=RenderType","True","3","True","14","all","0","False","False","False","False","False","False","False","False","False","False","False","False","True","0","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","False","","True","3","False","","False","False","True","1","LightMode=ShadowCaster","False","False","0","","0","0","Standard","0","True","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":700,"pos":[-896,2432],"params":["Inherit","False","1920.275","100","Fog Coords on Screen","0","","0,0.4980392,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":1180,"pos":[-896,1408],"params":["Inherit","False","2179.583","100","Cubemap Coordinates","0","","0,0.4980392,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":1195,"pos":[-896,-512],"params":["Inherit","False","1203","100","Drawers","0","","1,0.6827586,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":1191,"pos":[1536,1408],"params":["Inherit","False","1280.6","100","Base","0","","0,0.4980392,1,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":1167,"pos":[-896,0],"params":["Inherit","False","1236","100","Final Color","0","","0.4980392,1,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":431,"pos":[-896,768],"params":["Inherit","False","1094","100","Switch between Perspective / Orthographic camera","0","","1,0,1,1","0","0"]}
{"wire":[309,0,267,2]}
{"wire":[309,1,267,1]}
{"wire":[268,0,1007,0]}
{"wire":[268,1,309,0]}
{"wire":[268,2,267,4]}
{"wire":[255,0,701,0]}
{"wire":[255,1,260,0]}
{"wire":[300,0,268,0]}
{"wire":[276,0,48,0]}
{"wire":[276,1,255,0]}
{"wire":[47,0,276,0]}
{"wire":[1214,0,1207,0]}
{"wire":[1219,0,1221,2]}
{"wire":[1219,1,1218,0]}
{"wire":[1215,0,1211,0]}
{"wire":[1222,0,47,0]}
{"wire":[1210,0,1193,2]}
{"wire":[1210,1,1214,0]}
{"wire":[1220,0,1221,1]}
{"wire":[1220,1,1219,0]}
{"wire":[1220,2,1221,3]}
{"wire":[1208,1,1215,0]}
{"wire":[1217,38,1220,0]}
{"wire":[1217,43,1222,0]}
{"wire":[314,0,1210,0]}
{"wire":[1206,0,1220,0]}
{"wire":[1206,1,1208,0]}
{"wire":[1212,0,1217,19]}
{"wire":[1212,1,1208,0]}
{"wire":[329,0,325,0]}
{"wire":[315,0,314,0]}
{"wire":[315,1,1108,0]}
{"wire":[315,2,313,0]}
{"wire":[315,3,1108,0]}
{"wire":[315,4,1109,0]}
{"wire":[1164,1,1206,0]}
{"wire":[1164,0,1212,0]}
{"wire":[677,0,315,0]}
{"wire":[677,1,329,0]}
{"wire":[774,0,1164,0]}
{"wire":[316,0,677,0]}
{"wire":[41,1,774,0]}
{"wire":[678,0,316,0]}
{"wire":[678,1,1110,0]}
{"wire":[678,2,679,0]}
{"wire":[1189,0,41,0]}
{"wire":[1205,1,678,0]}
{"wire":[1205,2,1204,0]}
{"wire":[1174,0,1189,0]}
{"wire":[1174,1,1175,0]}
{"wire":[1174,2,1173,0]}
{"wire":[1174,3,1177,0]}
{"wire":[359,0,1205,0]}
{"wire":[222,0,1174,0]}
{"wire":[317,0,312,0]}
{"wire":[317,1,228,0]}
{"wire":[317,2,436,0]}
{"wire":[1179,1,228,0]}
{"wire":[1179,0,317,0]}
{"wire":[1194,0,1179,0]}
ASEEND*/
//CHKSM=9BB843BDC27CE3AA66184CFD88CC88A030ABEB3B