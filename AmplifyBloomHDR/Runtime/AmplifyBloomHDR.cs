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
			internal static readonly int InputTexture = Shader.PropertyToID( "_MainTex" );
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
			int nameId = Shader.PropertyToID( "_AB_SourceArrayToTexture" );
			int nameId2 = Shader.PropertyToID( "_AB_BloomAuxBuffer" );

			cmd.GetTemporaryRT( nameId, destRT.rt.width, destRT.rt.height, 0, FilterMode.Point, srcRT.rt.graphicsFormat );
			cmd.GetTemporaryRT( nameId2, destRT.rt.width, destRT.rt.height, 0, FilterMode.Point, srcRT.rt.graphicsFormat );

			//Source render texture is a texture array which gives access to VR frame buffers into each slice
			//We need to copy each one individually and apply bloom to it
			cmd.CopyTexture( srcRT, 0, 0, nameId, 0, 0 );

			//
			cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			_material.SetColor( ShaderIDs.Color, color.value );
			cmd.Blit( nameId, nameId2, _material, 0 );

			//Bloom Code goes here

			//Copying first result to first slice
			cmd.CopyTexture( nameId2, 0, 0, destRT, 0, 0 );

			cmd.ReleaseTemporaryRT( nameId );
			cmd.ReleaseTemporaryRT( nameId2 );

		}

		public override void Cleanup()
		{
			CoreUtils.Destroy( _material );
		}

	}
}
