// Amplify Bloom - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmplifyBloom
{
	public class DemoMainUI : MonoBehaviour
	{
		private const string VERTICAL_GAMEPAD = "Vertical";
		private const string HORIZONTAL_GAMEPAD = "Horizontal";
		private const string SUBMIT_BUTTON = "Submit";

		public Toggle BloomToggle;
		public Toggle HighPrecision;
		public Toggle UpscaleType;
		public Toggle TemporalFilter;
		public Toggle BokehToggle;
		public Toggle LensFlareToggle;
		public Toggle LensGlareToggle;
		public Toggle LensDirtToggle;
		public Toggle LensStarburstToggle;
		public Slider ThresholdSlider;
		public Slider DownscaleAmountSlider;
		public Slider IntensitySlider;
		public Slider ThresholdSizeSlider;

		private AmplifyBloom m_amplifyBloom;
		private Camera m_camera;

		private DemoUIElement[] m_uiElements;

		private bool m_gamePadMode = false;

		private int m_currentOption = 0;
		private int m_lastOption = 0;

		private int m_lastAxisValue = 0;

		void Awake()
		{
			m_camera = Camera.main;
		#if UNITY_EDITOR
			if ( PlayerSettings.colorSpace == ColorSpace.Gamma )
			{
				Debug.LogWarning("Detected Gamma Color Space. For better visual results please switch to Linear Color Space by going to Player Settings > Other Settings > Rendering Path > Color Space > Linear.");
			}

			if ( !m_camera.allowHDR )
			{
				Debug.LogWarning( "Detected LDR on camera. For better visual results please switch to HDR by hitting the HDR toggle on the Camera component." );
			}
		#endif
			m_amplifyBloom = m_camera.GetComponent<AmplifyBloom>();

			BloomToggle.isOn = m_amplifyBloom.enabled;
			HighPrecision.isOn = m_amplifyBloom.HighPrecision;
			UpscaleType.isOn = (m_amplifyBloom.UpscaleQuality == UpscaleQualityEnum.Realistic);
			TemporalFilter.isOn = m_amplifyBloom.TemporalFilteringActive;
			BokehToggle.isOn = m_amplifyBloom.BokehFilter.ApplyBokeh;
			LensFlareToggle.isOn = m_amplifyBloom.LensFlare.ApplyLensFlare;
			LensGlareToggle.isOn = m_amplifyBloom.LensGlare.ApplyLensGlare;
			LensDirtToggle.isOn = m_amplifyBloom.ApplyLensDirt;
			LensStarburstToggle.isOn = m_amplifyBloom.ApplyLensStardurst;

			BloomToggle.onValueChanged.AddListener( OnBloomToggle );
			HighPrecision.onValueChanged.AddListener( OnHighPrecisionToggle );
			UpscaleType.onValueChanged.AddListener( OnUpscaleTypeToogle );
			TemporalFilter.onValueChanged.AddListener( OnTemporalFilterToggle );
			BokehToggle.onValueChanged.AddListener( OnBokehFilterToggle );
			LensFlareToggle.onValueChanged.AddListener( OnLensFlareToggle );
			LensGlareToggle.onValueChanged.AddListener( OnLensGlareToggle );
			LensDirtToggle.onValueChanged.AddListener( OnLensDirtToggle );
			LensStarburstToggle.onValueChanged.AddListener( OnLensStarburstToggle );

			ThresholdSlider.value = m_amplifyBloom.OverallThreshold;
			ThresholdSlider.onValueChanged.AddListener( OnThresholdSlider );

			DownscaleAmountSlider.value = m_amplifyBloom.BloomDownsampleCount;
			DownscaleAmountSlider.onValueChanged.AddListener( OnDownscaleAmount );

			IntensitySlider.value = m_amplifyBloom.OverallIntensity;
			IntensitySlider.onValueChanged.AddListener( OnIntensitySlider );

			ThresholdSizeSlider.value = ( float ) m_amplifyBloom.MainThresholdSize;
			ThresholdSizeSlider.onValueChanged.AddListener( OnThresholdSize );

			if ( Input.GetJoystickNames().Length > 0 )
			{
				m_gamePadMode = true;
				m_uiElements = new DemoUIElement[ 13 ];
				m_uiElements[ 0 ] = BloomToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 1 ] = HighPrecision.GetComponent<DemoUIElement>();
				m_uiElements[ 2 ] = UpscaleType.GetComponent<DemoUIElement>();
				m_uiElements[ 3 ] = TemporalFilter.GetComponent<DemoUIElement>();
				m_uiElements[ 4 ] = BokehToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 5 ] = LensFlareToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 6 ] = LensGlareToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 7 ] = LensDirtToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 8 ] = LensStarburstToggle.GetComponent<DemoUIElement>();
				m_uiElements[ 9 ] = ThresholdSlider.GetComponent<DemoUIElement>();
				m_uiElements[ 10 ] = DownscaleAmountSlider.GetComponent<DemoUIElement>();
				m_uiElements[ 11 ] = IntensitySlider.GetComponent<DemoUIElement>();
				m_uiElements[ 12 ] = ThresholdSizeSlider.GetComponent<DemoUIElement>();

				for ( int i = 0; i < m_uiElements.Length; i++ )
				{
					m_uiElements[ i ].Init();
				}

				m_uiElements[ m_currentOption ].Select = true;
			}

		}

		public void OnThresholdSize( float selection )
		{
			m_amplifyBloom.MainThresholdSize = ( MainThresholdSizeEnum ) selection;
		}

		public void OnThresholdSlider( float value )
		{
			m_amplifyBloom.OverallThreshold = value;
		}

		public void OnDownscaleAmount( float value )
		{
			m_amplifyBloom.BloomDownsampleCount = ( int ) value;
		}

		public void OnIntensitySlider( float value )
		{
			m_amplifyBloom.OverallIntensity = value;
		}

		public void OnBloomToggle( bool value )
		{
			m_amplifyBloom.enabled = BloomToggle.isOn;
		}

		public void OnHighPrecisionToggle( bool value )
		{
			m_amplifyBloom.HighPrecision = value;
		}

		public void OnUpscaleTypeToogle( bool value )
		{
			m_amplifyBloom.UpscaleQuality = (value)? UpscaleQualityEnum.Realistic:UpscaleQualityEnum.Natural;
		}

		public void OnTemporalFilterToggle( bool value )
		{
			m_amplifyBloom.TemporalFilteringActive = value;
		}

		public void OnBokehFilterToggle( bool value )
		{
			m_amplifyBloom.BokehFilter.ApplyBokeh = BokehToggle.isOn;
		}
		public void OnLensFlareToggle( bool value )
		{
			m_amplifyBloom.LensFlare.ApplyLensFlare = LensFlareToggle.isOn;
		}
		public void OnLensGlareToggle( bool value )
		{
			m_amplifyBloom.LensGlare.ApplyLensGlare = LensGlareToggle.isOn;
		}
		public void OnLensDirtToggle( bool value )
		{
			m_amplifyBloom.ApplyLensDirt = LensDirtToggle.isOn;
		}
		public void OnLensStarburstToggle( bool value )
		{
			m_amplifyBloom.ApplyLensStardurst = LensStarburstToggle.isOn;
		}

		public void OnQuitButton()
		{
			Application.Quit();
		}

		void Update()
		{
			if ( m_gamePadMode )
			{
				int axisValue = ( int ) Input.GetAxis( VERTICAL_GAMEPAD );
				if ( axisValue != m_lastAxisValue )
				{
					m_lastAxisValue = axisValue;

					if ( axisValue == 1 )
					{
						m_currentOption = ( m_currentOption + 1 ) % m_uiElements.Length;
					}
					else if ( axisValue == -1 )
					{
						m_currentOption = ( m_currentOption == 0 ) ? ( m_uiElements.Length - 1 ) : ( m_currentOption - 1 );
					}
					m_uiElements[ m_lastOption ].Select = false;
					m_uiElements[ m_currentOption ].Select = true;
					m_lastOption = m_currentOption;
				}

				if ( Input.GetButtonDown( SUBMIT_BUTTON ) )
				{
					m_uiElements[ m_currentOption ].DoAction( DemoUIElementAction.Press );
				}

				float slideValue = Input.GetAxis( HORIZONTAL_GAMEPAD );
				if ( Mathf.Abs( slideValue ) > 0 )
				{
					m_uiElements[ m_currentOption ].DoAction( DemoUIElementAction.Slide, slideValue * Time.deltaTime );
				}
				else
				{
					m_uiElements[ m_currentOption ].Idle();
				}
			}
			if ( Input.GetKey( KeyCode.LeftAlt ) && Input.GetKey( KeyCode.Q ) )
			{
				OnQuitButton();
			}

			if ( Input.GetKeyDown( KeyCode.Alpha0 ) )
			{
				m_camera.orthographic = !m_camera.orthographic;
			}

			if ( Input.GetKeyDown( KeyCode.Alpha1 ) )
			{
				m_amplifyBloom.enabled = BloomToggle.isOn = !BloomToggle.isOn;
			}


			TemporalFilter.interactable =
			UpscaleType.interactable =
			BokehToggle.interactable =
			LensFlareToggle.interactable =
			LensGlareToggle.interactable =
			LensDirtToggle.interactable =
			LensStarburstToggle.interactable =
			ThresholdSlider.interactable =
			DownscaleAmountSlider.interactable =
			HighPrecision.interactable =
			ThresholdSizeSlider.interactable =
			IntensitySlider.interactable = BloomToggle.isOn;

			if ( BloomToggle.isOn )
			{
				if ( Input.GetKeyDown( KeyCode.Alpha2 ) )
				{
					m_amplifyBloom.BokehFilter.ApplyBokeh = BokehToggle.isOn = !BokehToggle.isOn;
				}

				if ( Input.GetKeyDown( KeyCode.Alpha3 ) )
				{
					m_amplifyBloom.LensFlare.ApplyLensFlare = LensFlareToggle.isOn = !LensFlareToggle.isOn;
				}

				if ( Input.GetKeyDown( KeyCode.Alpha4 ) )
				{
					m_amplifyBloom.LensGlare.ApplyLensGlare = LensGlareToggle.isOn = !LensGlareToggle.isOn;
				}

				if ( Input.GetKeyDown( KeyCode.Alpha5 ) )
				{
					m_amplifyBloom.ApplyLensDirt = LensDirtToggle.isOn = !LensDirtToggle.isOn;
				}

				if ( Input.GetKeyDown( KeyCode.Alpha6 ) )
				{
					m_amplifyBloom.ApplyLensStardurst = LensStarburstToggle.isOn = !LensStarburstToggle.isOn;
				}
			}
		}
	}
}
