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
			//int passID = 0;
			//_material.SetColor( ShaderIDs.Color, color.value );
			//_material.SetTexture( ShaderIDs.InputTexture,srcRT );

			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, srcRT );
			//cmd.SetGlobalColor( ShaderIDs.Color, color.value );

			int nameId = Shader.PropertyToID( "_NomeAleatorio" );
			int nameId2 = Shader.PropertyToID( "_NomeAleatorio2" );
			cmd.GetTemporaryRT( nameId, camera.actualWidth, camera.actualHeight, 0, FilterMode.Point, srcRT.rt.graphicsFormat );
			cmd.GetTemporaryRT( nameId2, camera.actualWidth, camera.actualHeight, 0, FilterMode.Point, srcRT.rt.graphicsFormat );

			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, srcRT );
			//cmd.Blit( srcRT, nameId);

			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			//cmd.Blit( srcRT, destRT );

			cmd.CopyTexture( srcRT, 0, 0, nameId, 0, 0 );

			cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			cmd.Blit( nameId, nameId2, _material, 0 );

			//Bloom Code goes here

			cmd.CopyTexture( nameId2, 0, 0, destRT, 0, 0 );

			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			//RTHandles.Alloc(
			//HDUtils.DrawFullScreen( cmd, _material, nameId, null, 0 );
			//
			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			//HDUtils.DrawFullScreen( cmd, _material, destRT, null, 1 );


			cmd.ReleaseTemporaryRT( nameId );
			cmd.ReleaseTemporaryRT( nameId2 );

			//cmd.SetGlobalTexture( ShaderIDs.InputTexture, nameId );
			//HDUtils.DrawFullScreen( cmd, _material, destRT, null, 1 );
			//HDUtils.DrawFullScreen( cmd, _material, destRT, null, 0 );

		}

		public override void Cleanup()
		{
			CoreUtils.Destroy( _material );
		}

	}
}
