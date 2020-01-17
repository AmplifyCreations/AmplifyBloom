Shader "Hidden/AmplifyBloomHDR"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		HLSLINCLUDE
			#pragma target 3.0
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			TEXTURE2D( _MainTex );
		ENDHLSL

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment

			struct Attributes
			{
				uint vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 texcoord   : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			float4 _Color;

			Varyings Vertex (Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO (output);
				output.positionCS = GetFullScreenTriangleVertexPosition (input.vertexID);
				output.texcoord = GetFullScreenTriangleTexCoord (input.vertexID);
				return output;
			}

			float4 Fragment (Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX (input);
				uint2 positionSS = input.texcoord * _ScreenSize.xy;
				float4 c1 = LOAD_TEXTURE2D (_MainTex, positionSS);
				return c1*_Color;
			}
			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment

			struct Attributes
			{
				uint vertexID : SV_VertexID;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 texcoord   : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};


			float4 _Color;

			Varyings Vertex (Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID (input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO (output);
				output.positionCS = GetFullScreenTriangleVertexPosition (input.vertexID);
				output.texcoord = GetFullScreenTriangleTexCoord (input.vertexID);
				return output;
			}

			float4 Fragment (Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX (input);
				uint2 positionSS = input.texcoord * _ScreenSize.xy;
				float4 c1 = LOAD_TEXTURE2D (_MainTex, positionSS);
				return c1;
			}
			ENDHLSL
		}
    }
}
