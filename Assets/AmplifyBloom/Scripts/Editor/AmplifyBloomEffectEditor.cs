// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AmplifyBloom
{
	[Serializable]
	[CustomEditor( typeof( AmplifyBloom ) )]
	public class AmplifyBloomEffectEditor : Editor
	{
		private const string DirtHighContrastTextureGUID = "7825356d1f1778140ad12b5dfe6b4d41";
		private const string StarburstTextureGUID = "c2216a0fed1c98742b826a85db28021c";

		private const string IntensityStr = "Intensity";
		private const string AdvancedSettingsStr = "Advanced Settings";

		private readonly Rect TemporalCurveRanges = new Rect( 0, 0, 1, 0.999f );
		private readonly Color TemporalCurveColor = new Color( 0, 1, 0, 1 );

		[SerializeField] private bool m_bokehAdvancedSettingsFoldout = false;
		[SerializeField] private bool m_ghostsAdvancedSettingsFoldout = false;
		[SerializeField] private bool m_haloAdvancedSettingsFoldout = false;
		[SerializeField] private bool m_lensGlareAdvancedSettingsFoldout = false;
		[SerializeField] private bool m_bloomFoldout = true;
		[SerializeField] private bool m_temporalFilterFoldout = false;
		[SerializeField] private bool m_featuresFoldout = false;
		[SerializeField] private bool m_bokehFilterFoldout = false;
		[SerializeField] private bool m_lensFlareFoldout = false;
		[SerializeField] private bool m_ghostsFoldout = false;
		[SerializeField] private bool m_haloFoldout = false;
		[SerializeField] private bool m_lensGlareFoldout = false;
		[SerializeField] private bool m_lensDirtFoldout = false;
		[SerializeField] private bool m_lensStarburstFoldout = false;
		[SerializeField] private bool m_mipSettingsFoldout = false;
		[SerializeField] private bool m_lensDirtWeightsFoldout = false;
		[SerializeField] private bool m_lensStarburstWeightsFoldout = false;
		[SerializeField] private bool m_bloomWeightsFoldout = false;

		public static readonly GUIContent HighPrecisionGC = new GUIContent( "Precision", "Switch between HDR and LDR Render Texture formats." );
		public static readonly GUIContent DebugToScreenGC = new GUIContent( "Debug", "Debug each bloom/feature stage to screen." );
		public static readonly GUIContent LDRRangeGC = new GUIContent( "Range", "LDR Tweakable range. Use to match HDR results." );
		public static readonly GUIContent OverallIntensityGC = new GUIContent( IntensityStr, "Overall bloom intensity. Affects all the effects bellow." );
		public static readonly GUIContent ThresholdGC = new GUIContent( "Threshold", "Luminance threshold to setup what should generate bloom." );
		public static readonly GUIContent BlurStepGC = new GUIContent( "Blur Step", "Number of blur passes done on bloom results. Higher number provides smoother results but decreases performance." );
		public static readonly GUIContent BlurRadiusGC = new GUIContent( "Blur Radius", "Blur radius amount" );
		public static readonly GUIContent UpscaleWeightGC = new GUIContent( "Weight", "Influence of the selected Mip. Only valid when Mip Amount greater than 0." );
		public static readonly GUIContent FeaturesSourceIdGC = new GUIContent( "Features Source Id", "Mip source which will be used to generate features." );
		public static readonly GUIContent UpscaleQualityGC = new GUIContent( "Technique", "Method which will be used to upscale results. Realistic is visually more robust but less efficient." );
		public static readonly GUIContent DownscaleAmountGC = new GUIContent( "Mip Count", "Number of resizes done on main RT before performing bloom. Increasing its the number provides finer tweaking but at the loss at performance." );
		public static readonly GUIContent UpscaleScaleRadiusGC = new GUIContent( "Upscale Blur Radius", "Radius used on the tent filter when upscaling to source size." );
		public static readonly GUIContent FilterCurveGC = new GUIContent( "Filter Curve", "Range of values which defines temporal filter behaviour." );
		public static readonly GUIContent FilterValueGC = new GUIContent( "Filter Value", "Position on the filter curve." );
		public static readonly GUIContent SeparateThresholdGC = new GUIContent( "Threshold", "Threshold value for second threshold layer." );
		public static readonly GUIContent BokehApplyOnBloomSourceGC = new GUIContent( "Apply on Bloom Source", "Bokeh filtering can either be applied on the bloom source and visually affect it or only affect features (lens flare/glare/dirt/starburst)." );
		public static readonly GUIContent BokehApertureShapeGC = new GUIContent( "Aperture Shape", "Type of bokeh filter which will reshape bloom results." );
		public static readonly GUIContent BokehSampleRadiusGC = new GUIContent( "Sample Radius", "Bokeh imaginary camera DOF's radius." );
		public static readonly GUIContent BokehRotationGC = new GUIContent( "Rotation", "Filter overall rotation." );
		public static readonly GUIContent BokehApertureGC = new GUIContent( "Aperture", "Bokeh imaginary camera DOF's aperture." );
		public static readonly GUIContent BokehFocalLengthGC = new GUIContent( "Focal Length", "Bokeh imaginary camera DOF's focal length." );
		public static readonly GUIContent BokehFocalDistanceGC = new GUIContent( "Focal Distance", "Bokeh imaginary camera DOF's focal distance." );
		public static readonly GUIContent BokehMaxCoCDiameterGC = new GUIContent( "Max CoC Diameter", "Bokeh imaginary camera DOF's Max Circle of Confusion diameter." );
		public static readonly GUIContent LensFlareIntensityGC = new GUIContent( IntensityStr, "Overall intensity for both halo and ghosts." );
		public static readonly GUIContent LensFlareBlurAmountGC = new GUIContent( "Blur amount", "Amount of blur applied on generated halo and ghosts." );
		public static readonly GUIContent LensFlareRadialTintGC = new GUIContent( "Radial Tint", "Dynamic tint color applied to halo and ghosts according to its screen position. Left most color on gradient corresponds to screen center." );
		public static readonly GUIContent LensFlareGhostsInstensityGC = new GUIContent( IntensityStr, "Ghosts intensity." );
		public static readonly GUIContent LensFlareGhostAmountGC = new GUIContent( "Count", "Amount of ghosts generated from each bloom area." );
		public static readonly GUIContent LensFlareGhostDispersalGC = new GUIContent( "Dispersal", "Distance between ghost generated from the same bloom area." );
		public static readonly GUIContent LensFlareGhostChrmDistortGC = new GUIContent( "Chromatic Distortion", "Amount of chromatic distortion applied on each ghost." );
		public static readonly GUIContent LensFlareGhostPowerFactorGC = new GUIContent( "Power Factor", "Base on ghost fade power function." );
		public static readonly GUIContent LensFlareGhostPowerFalloffGC = new GUIContent( "Power Falloff", "Exponent on ghost fade power function." );
		public static readonly GUIContent LensFlareHalosIntensityGC = new GUIContent( IntensityStr, "Halo intensity." );
		public static readonly GUIContent LensFlareHaloWidthGC = new GUIContent( "Width", "Width/Radius of the generated halo." );
		public static readonly GUIContent LensFlareHaloChrmDistGC = new GUIContent( "Chromatic Distortion", "Amount of chromatic distortion applied on halo." );
		public static readonly GUIContent LensFlareHaloPowerFactorGC = new GUIContent( "Power Factor", "Base on halo fade power function." );
		public static readonly GUIContent LensFlareHaloPowerFalloffGC = new GUIContent( "Power Falloff", "Exponent on halo fade power function." );
		public static readonly GUIContent LensGlareIntensityGC = new GUIContent( IntensityStr, "Lens Glare intensity." );
		public static readonly GUIContent LensGlareOverallStreakScaleGC = new GUIContent( "Streak Scale", "Overall glare streak length modifier." );
		public static readonly GUIContent LensGlareOverallTintGC = new GUIContent( "Overall Tint", "Tint applied uniformly across each type of glare." );
		public static readonly GUIContent LensGlareTypeGC = new GUIContent( "Type", "Type of glare." );
		public static readonly GUIContent LensGlareTintAlongGlareGC = new GUIContent( "Tint Along Glare", "Tint for spectral types along each ray.Leftmost color on the gradient corresponds to sample near bloom source." );
		public static readonly GUIContent LensGlarePerPassDispGC = new GUIContent( "Per Pass Displacement", "Distance between samples when creating each ray." );
		public static readonly GUIContent LensGlareMaxPerRayGC = new GUIContent( "Max Per Ray Passes", "Max amount of passes used to build each ray. More passes means more defined and propagated rays but decreases performance." );
		public static readonly GUIContent LensDirtIntensityGC = new GUIContent( IntensityStr, "Lens Dirt Intensity." );
		public static readonly GUIContent LensDirtTextureGC = new GUIContent( "Dirt Texture", "Mask from which dirt is going to be created." );
		public static readonly GUIContent LensStarburstIntensityGC = new GUIContent( IntensityStr, "Lens Starburst Intensity." );
		public static readonly GUIContent LensStarburstTextureGC = new GUIContent( "Starburst Texture", "Mask from which starburst is going to be created." );
		public static readonly GUIContent BloomFoldoutGC = new GUIContent( "Bloom", "Settings for bloom generation, will affect all features." );
		public static readonly GUIContent BokehFilterFoldoutGC = new GUIContent( "Bokeh Filter", "Settings for Bokeh filter generation." );
		public static readonly GUIContent LensFlareFoldoutGC = new GUIContent( "Lens Flare", "Overall settings for Lens Flare (Halo/Ghosts) generation." );
		public static readonly GUIContent GhostsFoldoutGC = new GUIContent( "Ghosts", "Settings for Ghosts generation." );
		public static readonly GUIContent HalosFoldoutGC = new GUIContent( "Halo", "Settings for Halo generation." );
		public static readonly GUIContent LensGlareFoldoutGC = new GUIContent( "Lens Glare", "Settings for Anamorphic Lens Glare generation." );
		public static readonly GUIContent LensDirtFoldoutGC = new GUIContent( "Lens Dirt", "Settings for Lens Dirt composition." );
		public static readonly GUIContent LensStarburstFoldoutGC = new GUIContent( "Lens Starburst", "Settings for Lens Starburts composition." );
		public static readonly GUIContent TemporalFilterFoldoutGC = new GUIContent( "Temporal Filter", "Settings for temporal filtering configuration." );
		public static readonly GUIContent FeaturesThresholdFoldoutGC = new GUIContent( "Features Threshold", "Settings for features threshold." );
		public static readonly GUIContent AdvancedSettingsBokehFoldoutGC = new GUIContent( AdvancedSettingsStr, "Advanced settings for Bokeh filter." );
		public static readonly GUIContent AdvancedSettingsGhostsFoldoutGC = new GUIContent( AdvancedSettingsStr, "Advanced settings for Ghosts." );
		public static readonly GUIContent AdvancedSettingsHalosFoldoutGC = new GUIContent( AdvancedSettingsStr, "Advanced settings for Halo." );
		public static readonly GUIContent AdvancedSettingsLensGlareFoldoutGC = new GUIContent( AdvancedSettingsStr, "Advanced settings for Lens Glare." );
		public static readonly GUIContent CustomGlareIdxGC = new GUIContent( "Current", "Current selected custom glare from array bellow." );
		public static readonly GUIContent CustomGlareSizeGC = new GUIContent( "Size", "Amount of customizable glare definitions." );
		public static readonly GUIContent CustomGlareNameGC = new GUIContent( "Name", "Custom glare name." );
		public static readonly GUIContent CustomGlareStarInclinationGC = new GUIContent( "Initial Offset", "Star angle initial offset." );
		public static readonly GUIContent CustomGlareChromaticAberrationGC = new GUIContent( "Chromatic Amount", "Amount of influence from chromatic gradient." );
		public static readonly GUIContent CustomGlareStarlinesCountGC = new GUIContent( "Star Lines Count", "Amount of generated rays." );
		public static readonly GUIContent CustomGlarePassCountGC = new GUIContent( "Pass Count", "Amount of passes used to generate rays." );
		public static readonly GUIContent CustomGlareSampleLengthGC = new GUIContent( "Sample Length", "Spacing between each sample when generating rays." );
		public static readonly GUIContent CustomGlareAttenuationGC = new GUIContent( "Attenuation", "Attenuation factor along ray." );
		public static readonly GUIContent CustomGlareRotationGC = new GUIContent( "Camera Influence", "Amount of influence camera rotation has on rays." );
		public static readonly GUIContent CustomGlareCustomIncrementGC = new GUIContent( "Custom Increment", "Custom angle increment between rays. They will be evenly rotated if specified a value equal to 0." );
		public static readonly GUIContent CustomGlareLongAttenuationGC = new GUIContent( "Long Attenuation", "Second attenuation factor. Rays will alternate between Attenuation ( Odd numbers) and Long Attenuation ( Even numbers). Only active if specified value is greater than 0." );
		public static readonly GUIContent CustomGlareFoldoutGC = new GUIContent( "Custom Label", "Properties for hovered custom glare." );
		public static readonly GUIContent MipSettingGC = new GUIContent( "Mip Settings", "Configurable settings for each mip" );
		public static readonly GUIContent LensWeightsFoldoutGC = new GUIContent( "Mip Weights", "Each mip contribution to the Lens Dirt and Starburts feature." );
		public static readonly GUIContent LensWeightGC = new GUIContent( "Mip ", "Influence of the selected Mip. Only valid when Mip Amount greater than 0." );
		public static readonly GUIContent BloomWeightsFoldoutGC = new GUIContent( "Mip Weights", "Each mip contribution to Bloom." );
		public static readonly GUIContent ShowDebugMessagesGC = new GUIContent( "Show Warnings", "Show relevant Amplify Bloom Warnings to Console." );
		public static readonly GUIContent MainThresholdSizeGC = new GUIContent( "Source Downscale", "Initial render texture scale on which the Main Threshold will be written." );
		public static readonly GUIContent TargetTextureGC = new GUIContent( "Target Texture", "Render Bloom to a render texture instead of destination" );
		public static readonly GUIContent MaskTextureGC = new GUIContent( "Mask Texture", "Render Bloom only on certain areas specifed by mask." );

		private GUIContent[] m_lensWeightGCArr;

		private GUIStyle m_mainFoldoutStyle;
		private GUIStyle m_foldoutClosed;
		private GUIStyle m_foldoutOpened;
		private GUIStyle m_toggleStyle;
		private GUIStyle m_mainLabelStyle;
		private GUIStyle m_disabledToggleStyle;

		private GUIContent[] m_bloomWeightsLabelGCArr;

		void Awake()
		{
			m_bloomWeightsLabelGCArr = new GUIContent[ AmplifyBloom.MaxDownscales ];
			for ( int i = 0; i < m_bloomWeightsLabelGCArr.Length; i++ )
			{
				m_bloomWeightsLabelGCArr[ i ] = new GUIContent( "Mip " + ( i + 1 ), BloomWeightsFoldoutGC.tooltip );
			}

			m_mainLabelStyle = new GUIStyle( EditorStyles.label );
			m_mainLabelStyle.fontStyle = FontStyle.Bold;

			m_disabledToggleStyle = new GUIStyle( EditorStyles.toggle );
			m_disabledToggleStyle.normal = m_disabledToggleStyle.onNormal;

			m_foldoutClosed = new GUIStyle( EditorStyles.foldout );
			m_foldoutClosed.fontStyle = FontStyle.Bold;

			m_foldoutOpened = new GUIStyle( EditorStyles.foldout );
			m_foldoutOpened.normal = m_foldoutOpened.onActive;
			m_foldoutOpened.fontStyle = FontStyle.Bold;

			m_mainFoldoutStyle = new GUIStyle( EditorStyles.foldout );
			m_mainFoldoutStyle.fontStyle = FontStyle.Bold;

			m_toggleStyle = new GUIStyle( EditorStyles.toggle );
			m_toggleStyle.fontStyle = FontStyle.Bold;

			m_lensWeightGCArr = new GUIContent[ AmplifyBloom.MaxDownscales ];
			for ( int i = 0; i < m_lensWeightGCArr.Length; i++ )
			{
				m_lensWeightGCArr[ i ] = new GUIContent( LensWeightGC );
				m_lensWeightGCArr[ i ].text += ( i + 1 ).ToString();
			}

			var bloom = target as AmplifyBloom;
			if ( bloom.LensDirtTexture == null )
			{
				bloom.LensDirtTexture = AssetDatabase.LoadAssetAtPath<Texture>( AssetDatabase.GUIDToAssetPath( DirtHighContrastTextureGUID ) );
			}
			if ( bloom.LensStardurstTex == null )
			{
				bloom.LensStardurstTex = AssetDatabase.LoadAssetAtPath<Texture>( AssetDatabase.GUIDToAssetPath( StarburstTextureGUID ) );
			}
		}

		bool CustomFoldout( bool value , GUIContent content, int space = 4)
		{
			GUIStyle foldoutStyle = value ? m_foldoutOpened : m_foldoutClosed;
			m_mainLabelStyle.fontStyle = FontStyle.Normal;
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space( space );
				value = GUILayout.Toggle( value, GUIContent.none, EditorStyles.foldout, GUILayout.Width( 10 ) );

				if ( GUILayout.Button( content, m_mainLabelStyle ) )
				{
					value = !value;
				}

			}
			EditorGUILayout.EndHorizontal();
			return value;
		}

		void ToggleFoldout( GUIContent content, ref bool foldoutValue, ref bool toggleValue, bool applyBold, int space = -8, bool specialToggle = false )
		{
			GUIStyle foldoutStyle = foldoutValue ? m_foldoutOpened : m_foldoutClosed;
			m_toggleStyle.fontStyle = foldoutStyle.fontStyle = applyBold ? FontStyle.Bold : FontStyle.Normal;
			m_mainLabelStyle.fontStyle = m_toggleStyle.fontStyle;
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space( space );
				foldoutValue = GUILayout.Toggle( foldoutValue, GUIContent.none, EditorStyles.foldout, GUILayout.Width( 10 ) );
				if ( specialToggle )
				{
					GUI.enabled = false;
					GUILayout.Toggle( true, GUIContent.none, GUILayout.Width( 15 ) );
					GUI.enabled = true;
				}
				else
				{
					toggleValue = GUILayout.Toggle( toggleValue, content, m_toggleStyle, GUILayout.Width( 15 ) );
				}

				if ( GUILayout.Button( content, m_mainLabelStyle ) )
				{
					foldoutValue = !foldoutValue;
				}

			}
			EditorGUILayout.EndHorizontal();
		}

		override public void OnInspectorGUI()
		{
			Undo.RecordObject( target, "AmplifyBloomInspector" );
			var bloom = target as AmplifyBloom;
			SerializedObject bloomObj = new SerializedObject( bloom );
			GUILayout.BeginVertical();
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Separator();
				bool applyBloom = true;
				ToggleFoldout( BloomFoldoutGC, ref m_bloomFoldout, ref applyBloom, true, -8, true );
				if ( m_bloomFoldout )
				{
					bloom.UpscaleQuality = ( UpscaleQualityEnum ) EditorGUILayout.EnumPopup( UpscaleQualityGC, bloom.UpscaleQuality );
					bloom.MainThresholdSize = ( MainThresholdSizeEnum ) EditorGUILayout.EnumPopup( MainThresholdSizeGC, bloom.MainThresholdSize );

					GUILayout.BeginHorizontal();
					float originalLabelWidth = EditorGUIUtility.labelWidth;

					bloom.CurrentPrecisionMode = ( PrecisionModes ) EditorGUILayout.EnumPopup( HighPrecisionGC, bloom.CurrentPrecisionMode );
					GUI.enabled = !bloom.HighPrecision;
					{
						EditorGUIUtility.labelWidth = 45;
						bloom.BloomRange = EditorGUILayout.FloatField( LDRRangeGC, bloom.BloomRange, GUILayout.MaxWidth( 1300 ) );
					}
					EditorGUIUtility.labelWidth = originalLabelWidth;
					GUI.enabled = true;

					GUILayout.EndHorizontal();

					bloom.OverallIntensity = EditorGUILayout.FloatField( OverallIntensityGC, bloom.OverallIntensity );

					bloom.OverallThreshold = EditorGUILayout.FloatField( ThresholdGC, bloom.OverallThreshold );

					SerializedProperty maskTextureField = bloomObj.FindProperty( "m_maskTexture" );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( maskTextureField, MaskTextureGC );
					if ( EditorGUI.EndChangeCheck() )
					{
						bloomObj.ApplyModifiedProperties();
					}

					SerializedProperty targetTextureField = bloomObj.FindProperty( "m_targetTexture" );
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( targetTextureField, TargetTextureGC );
					if ( EditorGUI.EndChangeCheck() )
					{
						bloomObj.ApplyModifiedProperties();
					}
					//bloom.TargetTexture = EditorGUILayout.ObjectField( m_targetTextureGC, bloom.TargetTexture, typeof( RenderTexture ),false ) as RenderTexture;
					bloom.DebugToScreen = ( DebugToScreenEnum ) EditorGUILayout.EnumPopup( DebugToScreenGC, bloom.DebugToScreen );
					bloom.ShowDebugMessages = EditorGUILayout.Toggle( ShowDebugMessagesGC, bloom.ShowDebugMessages );
					int weightMaxDowsampleCount = Mathf.Max( 1, bloom.BloomDownsampleCount );
					{
						EditorGUI.indentLevel++;
						m_mipSettingsFoldout = CustomFoldout( m_mipSettingsFoldout, MipSettingGC );
						if ( m_mipSettingsFoldout )
						{
							EditorGUI.indentLevel++;
							bloom.BloomDownsampleCount = EditorGUILayout.IntSlider( DownscaleAmountGC, bloom.BloomDownsampleCount, AmplifyBloom.MinDownscales, bloom.SoftMaxdownscales );
							bool guiState = bloom.BloomDownsampleCount != 0;

							GUI.enabled = ( bloom.UpscaleQuality == UpscaleQualityEnum.Realistic ) && guiState;
							{
								bloom.UpscaleBlurRadius = EditorGUILayout.Slider( UpscaleScaleRadiusGC, bloom.UpscaleBlurRadius, 1f, 3.0f );
							}
							GUI.enabled = guiState;

							int featuresSourceId = bloom.FeaturesSourceId + 1;
							featuresSourceId = EditorGUILayout.IntSlider( FeaturesSourceIdGC, featuresSourceId, 1, bloom.BloomDownsampleCount );
							bloom.FeaturesSourceId = featuresSourceId - 1;
							EditorGUI.indentLevel--;
						}
						GUI.enabled = true;

						m_bloomWeightsFoldout = CustomFoldout( m_bloomWeightsFoldout, BloomWeightsFoldoutGC );
						if ( m_bloomWeightsFoldout )
						{
							EditorGUI.indentLevel++;

							float blurStepSize = 15;
							float blurRadiusSize = 15;
							float weightSize = 25;

							GUILayout.BeginHorizontal();
							GUILayout.Space( 41 );
							EditorGUILayout.LabelField( BlurStepGC, GUILayout.MinWidth( blurStepSize ) );
							GUILayout.Space( -26 );
							EditorGUILayout.LabelField( BlurRadiusGC, GUILayout.MinWidth( blurRadiusSize ) );
							GUILayout.Space( -27 );
							EditorGUILayout.LabelField( UpscaleWeightGC, GUILayout.MinWidth( weightSize ) );
							GUILayout.EndHorizontal();
							for ( int i = 0; i < weightMaxDowsampleCount; i++ )
							{
								GUILayout.BeginHorizontal();
								EditorGUILayout.LabelField( m_bloomWeightsLabelGCArr[ i ], GUILayout.Width( 65 ) );
								GUILayout.Space( -30 );

								bloom.GaussianSteps[ i ] = EditorGUILayout.IntField( string.Empty, bloom.GaussianSteps[ i ], GUILayout.MinWidth( blurStepSize ) );
								bloom.GaussianSteps[ i ] = Mathf.Clamp( bloom.GaussianSteps[ i ], 0, AmplifyBloom.MaxGaussian );

								GUILayout.Space( -27 );
								bloom.GaussianRadius[ i ] = EditorGUILayout.FloatField( string.Empty, bloom.GaussianRadius[ i ], GUILayout.MinWidth( blurRadiusSize ) );
								bloom.GaussianRadius[ i ] = Mathf.Max( bloom.GaussianRadius[ i ], 0f );

								GUILayout.Space( -27 );
								int id = weightMaxDowsampleCount - 1 - i;
								bloom.UpscaleWeights[ id ] = EditorGUILayout.FloatField( string.Empty, bloom.UpscaleWeights[ id ], GUILayout.MinWidth( weightSize ) );
								bloom.UpscaleWeights[ id ] = Mathf.Max( bloom.UpscaleWeights[ id ], 0f );

								GUILayout.EndHorizontal();
							}
							EditorGUI.indentLevel--;
						}

					}
					GUI.enabled = true;

					bool applyTemporalFilter = bloom.TemporalFilteringActive;
					ToggleFoldout( TemporalFilterFoldoutGC, ref m_temporalFilterFoldout, ref applyTemporalFilter, false, 4 );
					bloom.TemporalFilteringActive = applyTemporalFilter;
					if ( m_temporalFilterFoldout )
					{
						GUI.enabled = bloom.TemporalFilteringActive;
						{
							bloom.TemporalFilteringCurve = EditorGUILayout.CurveField( FilterCurveGC, bloom.TemporalFilteringCurve, TemporalCurveColor, TemporalCurveRanges );
							bloom.TemporalFilteringValue = EditorGUILayout.Slider( FilterValueGC, bloom.TemporalFilteringValue, 0.01f, 1f );
						}
						GUI.enabled = true;
					}

					bool applySeparateFeatures = bloom.SeparateFeaturesThreshold;
					ToggleFoldout( FeaturesThresholdFoldoutGC, ref m_featuresFoldout, ref applySeparateFeatures, false, 4 );
					bloom.SeparateFeaturesThreshold = applySeparateFeatures;

					if ( m_featuresFoldout )
					{
						GUI.enabled = bloom.SeparateFeaturesThreshold;
						{
							bloom.FeaturesThreshold = EditorGUILayout.FloatField( SeparateThresholdGC, bloom.FeaturesThreshold );
						}
						GUI.enabled = true;
					}
					EditorGUI.indentLevel--;

				}

				bool applyLensDirt = bloom.ApplyLensDirt;
				ToggleFoldout( LensDirtFoldoutGC, ref m_lensDirtFoldout, ref applyLensDirt, true );
				bloom.ApplyLensDirt = applyLensDirt;
				if ( m_lensDirtFoldout )
				{
					GUI.enabled = bloom.ApplyLensDirt;
					bloom.LensDirtStrength = EditorGUILayout.FloatField( LensDirtIntensityGC, bloom.LensDirtStrength );


					EditorGUI.indentLevel++;
					m_lensDirtWeightsFoldout = CustomFoldout( m_lensDirtWeightsFoldout, LensWeightsFoldoutGC );
					if ( m_lensDirtWeightsFoldout )
					{
						for ( int i = 0; i < bloom.BloomDownsampleCount; i++ )
						{
							int id = bloom.BloomDownsampleCount - 1 - i;
							bloom.LensDirtWeights[ id ] = EditorGUILayout.FloatField( m_lensWeightGCArr[ i ], bloom.LensDirtWeights[ id ] );
							bloom.LensDirtWeights[ id ] = Mathf.Max( bloom.LensDirtWeights[ id ], 0f );
						}
					}
					EditorGUI.indentLevel--;
					bloom.LensDirtTexture = EditorGUILayout.ObjectField( LensDirtTextureGC, bloom.LensDirtTexture, typeof( Texture ), false ) as Texture;
					GUI.enabled = true;
				}

				bool applyStarburst = bloom.ApplyLensStardurst;
				ToggleFoldout( LensStarburstFoldoutGC, ref m_lensStarburstFoldout, ref applyStarburst, true );
				bloom.ApplyLensStardurst = applyStarburst;
				if ( m_lensStarburstFoldout )
				{
					GUI.enabled = bloom.ApplyLensStardurst;
					{
						bloom.LensStarburstStrength = EditorGUILayout.FloatField( LensStarburstIntensityGC, bloom.LensStarburstStrength );
						EditorGUI.indentLevel++;
						m_lensStarburstWeightsFoldout = CustomFoldout( m_lensStarburstWeightsFoldout, LensWeightsFoldoutGC );
						if ( m_lensStarburstWeightsFoldout )
						{
							for ( int i = 0; i < bloom.BloomDownsampleCount; i++ )
							{
								int id = bloom.BloomDownsampleCount - 1 - i;
								bloom.LensStarburstWeights[ id ] = EditorGUILayout.FloatField( m_lensWeightGCArr[ i ], bloom.LensStarburstWeights[ id ] );
								bloom.LensStarburstWeights[ id ] = Mathf.Max( bloom.LensStarburstWeights[ id ], 0f );
							}

						}
						EditorGUI.indentLevel--;
						bloom.LensStardurstTex = EditorGUILayout.ObjectField( LensStarburstTextureGC, bloom.LensStardurstTex, typeof( Texture ), false ) as Texture;
					}
					GUI.enabled = true;
				}

				bool applyBokeh = bloom.BokehFilter.ApplyBokeh;
				ToggleFoldout( BokehFilterFoldoutGC, ref m_bokehFilterFoldout, ref applyBokeh, true );
				bloom.BokehFilter.ApplyBokeh = applyBokeh;
				if ( m_bokehFilterFoldout )
				{
					GUI.enabled = bloom.BokehFilter.ApplyBokeh;
					{
						bloom.BokehFilter.ApplyOnBloomSource = EditorGUILayout.Toggle( BokehApplyOnBloomSourceGC, bloom.BokehFilter.ApplyOnBloomSource );
						bloom.BokehFilter.ApertureShape = ( ApertureShape ) EditorGUILayout.EnumPopup( BokehApertureShapeGC, bloom.BokehFilter.ApertureShape );
						EditorGUI.indentLevel++;
						m_bokehAdvancedSettingsFoldout = CustomFoldout( m_bokehAdvancedSettingsFoldout, AdvancedSettingsBokehFoldoutGC );
						if ( m_bokehAdvancedSettingsFoldout )
						{
							bloom.BokehFilter.OffsetRotation = EditorGUILayout.Slider( BokehRotationGC, bloom.BokehFilter.OffsetRotation, 0, 360 );
							bloom.BokehFilter.BokehSampleRadius = EditorGUILayout.Slider( BokehSampleRadiusGC, bloom.BokehFilter.BokehSampleRadius, 0.01f, 1f );
							bloom.BokehFilter.Aperture = EditorGUILayout.Slider( BokehApertureGC, bloom.BokehFilter.Aperture, 0.01f, 0.05f );
							bloom.BokehFilter.FocalLength = EditorGUILayout.Slider( BokehFocalLengthGC, bloom.BokehFilter.FocalLength, 0.018f, 0.055f );
							bloom.BokehFilter.FocalDistance = EditorGUILayout.Slider( BokehFocalDistanceGC, bloom.BokehFilter.FocalDistance, 0.055f, 3f );
							bloom.BokehFilter.MaxCoCDiameter = EditorGUILayout.Slider( BokehMaxCoCDiameterGC, bloom.BokehFilter.MaxCoCDiameter, 0f, 2f );
						}
						EditorGUI.indentLevel--;
					}
					GUI.enabled = true;
				}

				bool applyLensFlare = bloom.LensFlare.ApplyLensFlare;
				ToggleFoldout( LensFlareFoldoutGC, ref m_lensFlareFoldout, ref applyLensFlare, true );
				bloom.LensFlare.ApplyLensFlare = applyLensFlare;
				if ( m_lensFlareFoldout )
				{
					GUI.enabled = bloom.LensFlare.ApplyLensFlare;
					{
						bloom.LensFlare.OverallIntensity = EditorGUILayout.FloatField( LensFlareIntensityGC, bloom.LensFlare.OverallIntensity );
						bloom.LensFlare.LensFlareGaussianBlurAmount = EditorGUILayout.IntSlider( LensFlareBlurAmountGC, bloom.LensFlare.LensFlareGaussianBlurAmount, 0, 3 );

						EditorGUI.BeginChangeCheck();
						SerializedProperty gradientField = bloomObj.FindProperty( "m_lensFlare.m_lensGradient" );
						EditorGUILayout.PropertyField( gradientField, LensFlareRadialTintGC );
						if ( EditorGUI.EndChangeCheck() )
						{
							bloomObj.ApplyModifiedProperties();
							bloom.LensFlare.TextureFromGradient();
						}

						EditorGUI.indentLevel++;
						m_ghostsFoldout = CustomFoldout( m_ghostsFoldout, GhostsFoldoutGC );
						if ( m_ghostsFoldout )
						{
							bloom.LensFlare.LensFlareNormalizedGhostsIntensity = EditorGUILayout.FloatField( LensFlareGhostsInstensityGC, bloom.LensFlare.LensFlareNormalizedGhostsIntensity );
							bloom.LensFlare.LensFlareGhostAmount = EditorGUILayout.IntSlider( LensFlareGhostAmountGC, bloom.LensFlare.LensFlareGhostAmount, 0, AmplifyBloom.MaxGhosts );
							bloom.LensFlare.LensFlareGhostsDispersal = EditorGUILayout.Slider( LensFlareGhostDispersalGC, bloom.LensFlare.LensFlareGhostsDispersal, 0.01f, 1.0f );
							bloom.LensFlare.LensFlareGhostChrDistortion = EditorGUILayout.Slider( LensFlareGhostChrmDistortGC, bloom.LensFlare.LensFlareGhostChrDistortion, 0, 10 );
							EditorGUI.indentLevel++;
							m_ghostsAdvancedSettingsFoldout = CustomFoldout( m_ghostsAdvancedSettingsFoldout, AdvancedSettingsGhostsFoldoutGC ,19);
							if ( m_ghostsAdvancedSettingsFoldout )
							{
								bloom.LensFlare.LensFlareGhostsPowerFactor = EditorGUILayout.Slider( LensFlareGhostPowerFactorGC, bloom.LensFlare.LensFlareGhostsPowerFactor, 0, 2 );
								bloom.LensFlare.LensFlareGhostsPowerFalloff = EditorGUILayout.Slider( LensFlareGhostPowerFalloffGC, bloom.LensFlare.LensFlareGhostsPowerFalloff, 1, 128 );
							}
							EditorGUI.indentLevel--;
						}

						m_haloFoldout = CustomFoldout( m_haloFoldout, HalosFoldoutGC );
						if ( m_haloFoldout )
						{
							bloom.LensFlare.LensFlareNormalizedHaloIntensity = EditorGUILayout.FloatField( LensFlareHalosIntensityGC, bloom.LensFlare.LensFlareNormalizedHaloIntensity );
							bloom.LensFlare.LensFlareHaloWidth = EditorGUILayout.Slider( LensFlareHaloWidthGC, bloom.LensFlare.LensFlareHaloWidth, 0, 1 );
							bloom.LensFlare.LensFlareHaloChrDistortion = EditorGUILayout.Slider( LensFlareHaloChrmDistGC, bloom.LensFlare.LensFlareHaloChrDistortion, 0, 10 );
							EditorGUI.indentLevel++;
							m_haloAdvancedSettingsFoldout = CustomFoldout( m_haloAdvancedSettingsFoldout, AdvancedSettingsHalosFoldoutGC ,19);
							if ( m_haloAdvancedSettingsFoldout )
							{
								bloom.LensFlare.LensFlareHaloPowerFactor = EditorGUILayout.Slider( LensFlareHaloPowerFactorGC, bloom.LensFlare.LensFlareHaloPowerFactor, 1, 2 );
								bloom.LensFlare.LensFlareHaloPowerFalloff = EditorGUILayout.Slider( LensFlareHaloPowerFalloffGC, bloom.LensFlare.LensFlareHaloPowerFalloff, 1, 128 );
							}
							EditorGUI.indentLevel--;
						}
						EditorGUI.indentLevel--;
					}
					GUI.enabled = true;
				}

				bool applyGlare = bloom.LensGlare.ApplyLensGlare;
				ToggleFoldout( LensGlareFoldoutGC, ref m_lensGlareFoldout, ref applyGlare, true );
				bloom.LensGlare.ApplyLensGlare = applyGlare;
				if ( m_lensGlareFoldout )
				{
					GUI.enabled = bloom.LensGlare.ApplyLensGlare;
					{
						bloom.LensGlare.Intensity = EditorGUILayout.FloatField( LensGlareIntensityGC, bloom.LensGlare.Intensity );
						bloom.LensGlare.OverallStreakScale = EditorGUILayout.Slider( LensGlareOverallStreakScaleGC, bloom.LensGlare.OverallStreakScale, 0, 2 );
						bloom.LensGlare.OverallTint = EditorGUILayout.ColorField( LensGlareOverallTintGC, bloom.LensGlare.OverallTint );

						EditorGUI.BeginChangeCheck();
						SerializedProperty gradientField = bloomObj.FindProperty( "m_anamorphicGlare.m_cromaticAberrationGrad" );
						EditorGUILayout.PropertyField( gradientField, LensGlareTintAlongGlareGC );
						if ( EditorGUI.EndChangeCheck() )
						{
							bloomObj.ApplyModifiedProperties();
							bloom.LensGlare.SetDirty();
						}

						bloom.LensGlare.CurrentGlare = ( GlareLibType ) EditorGUILayout.EnumPopup( LensGlareTypeGC, bloom.LensGlare.CurrentGlare );
						if ( bloom.LensGlare.CurrentGlare == GlareLibType.Custom )
						{
							EditorGUI.indentLevel++;
							bloom.LensGlare.CustomGlareDefAmount = EditorGUILayout.IntSlider( CustomGlareSizeGC, bloom.LensGlare.CustomGlareDefAmount, 0, AmplifyGlare.MaxCustomGlare );
							if ( bloom.LensGlare.CustomGlareDefAmount > 0 )
							{
								bloom.LensGlare.CustomGlareDefIdx = EditorGUILayout.IntSlider( CustomGlareIdxGC, bloom.LensGlare.CustomGlareDefIdx, 0, bloom.LensGlare.CustomGlareDef.Length - 1 );
								for ( int i = 0; i < bloom.LensGlare.CustomGlareDef.Length; i++ )
								{
									EditorGUI.indentLevel++;
									CustomGlareFoldoutGC.text = "[" + i + "] " + bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.StarName;
									bloom.LensGlare.CustomGlareDef[ i ].FoldoutValue = CustomFoldout( bloom.LensGlare.CustomGlareDef[ i ].FoldoutValue, CustomGlareFoldoutGC );
									if ( bloom.LensGlare.CustomGlareDef[ i ].FoldoutValue )
									{
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.StarName = EditorGUILayout.TextField( CustomGlareNameGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.StarName );
										bloom.LensGlare.CustomGlareDef[ i ].StarInclinationDeg = EditorGUILayout.Slider( CustomGlareStarInclinationGC, bloom.LensGlare.CustomGlareDef[ i ].StarInclinationDeg, 0, 180 );
										bloom.LensGlare.CustomGlareDef[ i ].ChromaticAberration = EditorGUILayout.Slider( CustomGlareChromaticAberrationGC, bloom.LensGlare.CustomGlareDef[ i ].ChromaticAberration, 0, 1 );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.StarlinesCount = EditorGUILayout.IntSlider( CustomGlareStarlinesCountGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.StarlinesCount, 1, AmplifyGlare.MaxStarLines );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.PassCount = EditorGUILayout.IntSlider( CustomGlarePassCountGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.PassCount, 1, AmplifyGlare.MaxPasses );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.SampleLength = EditorGUILayout.Slider( CustomGlareSampleLengthGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.SampleLength, 0, 2 );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.Attenuation = EditorGUILayout.Slider( CustomGlareAttenuationGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.Attenuation, 0, 1 );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.CameraRotInfluence = EditorGUILayout.FloatField( CustomGlareRotationGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.CameraRotInfluence ); ;
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.CustomIncrement = EditorGUILayout.Slider( CustomGlareCustomIncrementGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.CustomIncrement, 0, 180 );
										bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.LongAttenuation = EditorGUILayout.Slider( CustomGlareLongAttenuationGC, bloom.LensGlare.CustomGlareDef[ i ].CustomStarData.LongAttenuation, 0, 1 );
									}
									EditorGUI.indentLevel--;
								}
							}
							EditorGUI.indentLevel--;
						}

						EditorGUI.indentLevel++;
						m_lensGlareAdvancedSettingsFoldout = CustomFoldout( m_lensGlareAdvancedSettingsFoldout, AdvancedSettingsLensGlareFoldoutGC );
						if ( m_lensGlareAdvancedSettingsFoldout )
						{
							bloom.LensGlare.PerPassDisplacement = EditorGUILayout.Slider( LensGlarePerPassDispGC, bloom.LensGlare.PerPassDisplacement, 1, 8 );
							bloom.LensGlare.GlareMaxPassCount = EditorGUILayout.IntSlider( LensGlareMaxPerRayGC, bloom.LensGlare.GlareMaxPassCount, 1, AmplifyGlare.MaxPasses );

						}
						EditorGUI.indentLevel--;
					}
					GUI.enabled = true;
				}

				if ( EditorGUI.EndChangeCheck() )
				{
					EditorUtility.SetDirty( bloom );
					if ( !Application.isPlaying )
					{
						EditorSceneManager.MarkSceneDirty( bloom.gameObject.scene );
					}
				}
			}
			GUILayout.EndVertical();
		}
	}
}
