using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace AmplifyBloom
{
	[Serializable, VolumeComponentMenu( "Post-processing/Amplify Creations/Bloom" )]
	public sealed class AmplifyBloomHDR : CustomPostProcessVolumeComponent, IPostProcessComponent
	{
		public ColorParameter color = new ColorParameter( Color.red, false, false, true );
		
		static class ShaderIDs
		{
			internal static readonly int Color = Shader.PropertyToID( "_Color" );
			internal static readonly int InputTexture = Shader.PropertyToID( "_InputTexture" );
		}

		Material _material;
		
		public bool IsActive() => _material != null ;

		public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

		public override void Setup()
		{
			_material = CoreUtils.CreateEngineMaterial( "Hidden/AmplifyBloomHDR" );
		}

		public override void Render( CommandBuffer cmd, HDCamera camera, RTHandle srcRT, RTHandle destRT )
		{
			int passID = 0;
			_material.SetColor( ShaderIDs.Color, color.value );
			_material.SetTexture( ShaderIDs.InputTexture, srcRT );
			HDUtils.DrawFullScreen( cmd, _material, destRT, null, passID );
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy( _material );
		}

	}
}
