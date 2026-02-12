// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace AmplifyBloom
{
	public static class EditorUtils
	{
		public static bool GroupFoldout( ref bool fold, GUIContent gc )
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space( 3 );
			fold = EditorGUILayout.Foldout( fold, gc, true );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return fold;
		}
	}
}
