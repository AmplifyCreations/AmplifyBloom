// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

#ifndef AMPLIFY_BLOOMSRPTOOLS_INCLUDED
#define AMPLIFY_BLOOMSRPTOOLS_INCLUDED

#ifdef USING_HDRP
//HDRP
//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#define ASETexLoad(textureName,uvName) LOAD_TEXTURE2D_X(textureName,uvName)
#define ASETexSample(textureName,uvName) SAMPLE_TEXTURE2D(textureName,sampler##textureName,uvName)
#define ASETexDeclare(textureName) TEXTURE2D_X(textureName)
#define ASETexParams(textureName) TEXTURE2D_X_PARAM(textureName)
#else
//STANDARD
#define ASETexDeclare(textureName) sampler2D textureName
#define ASETexPass(textureName) textureName
#define ASETexArgs(textureName) sampler2D textureName

#define ASETexLoad(textureName,uvName) tex2D(textureName,uvName)
#define ASETexSample(textureName,uvName) tex2D(textureName,uvName)

#ifndef UNITY_DECLARE_DEPTH_TEXTURE
#define UNITY_DECLARE_DEPTH_TEXTURE(tex) sampler2D_float tex
#endif

#define ASEDefineDepthTexture(textureName) UNITY_DECLARE_DEPTH_TEXTURE( textureName )

#define ASESampleDepthTexture(textureName,uvName) SAMPLE_DEPTH_TEXTURE(textureName,uvName)

#endif

#endif
