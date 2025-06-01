using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/HBAO")]
[RequireComponent(typeof(Camera))]
public class HBAO : HBAO_Core
{
	private RenderTexture[] _mrtTexDepth = new RenderTexture[16];

	private RenderTexture[] _mrtTexNrm = new RenderTexture[16];

	private RenderTexture[] _mrtTexAO = new RenderTexture[16];

	private RenderBuffer[][] _mrtRB = new RenderBuffer[4][]
	{
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4]
	};

	private RenderBuffer[][] _mrtRBNrm = new RenderBuffer[4][]
	{
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4]
	};

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (hbaoShader == null || _hbaoCamera == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		_hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
		if (base.aoSettings.perPixelNormals == PerPixelNormals.Camera)
		{
			_hbaoCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		CheckParameters();
		UpdateShaderProperties();
		UpdateShaderKeywords();
		if (base.generalSettings.deinterleaving == Deinterleaving._2x)
		{
			RenderHBAODeinterleaved2x(source, destination);
		}
		else if (base.generalSettings.deinterleaving == Deinterleaving._4x)
		{
			RenderHBAODeinterleaved4x(source, destination);
		}
		else
		{
			RenderHBAO(source, destination);
		}
	}

	private void RenderHBAO(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.downsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: false, clearColor: true, Color.white);
		RenderTexture.active = active;
		Graphics.Blit(source, temporary, _hbaoMaterial, GetAoPass());
		if (base.blurSettings.amount != Blur.None)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor);
			Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurXPass());
			temporary.DiscardContents();
			Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurYPass());
			RenderTexture.ReleaseTemporary(temporary2);
		}
		_hbaoMaterial.SetTexture(ShaderProperties.hbaoTex, temporary);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void RenderHBAODeinterleaved2x(RenderTexture source, RenderTexture destination)
	{
		RenderTexture active = RenderTexture.active;
		for (int i = 0; i < 4; i++)
		{
			_mrtTexDepth[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.RFloat);
			_mrtTexNrm[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.ARGB2101010);
			_mrtTexAO[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight);
			ref RenderBuffer reference = ref _mrtRB[0][i];
			reference = _mrtTexDepth[i].colorBuffer;
			ref RenderBuffer reference2 = ref _mrtRBNrm[0][i];
			reference2 = _mrtTexNrm[i].colorBuffer;
			RenderTexture.active = _mrtTexAO[i];
			GL.Clear(clearDepth: false, clearColor: true, Color.white);
		}
		_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[0], new Vector2(0f, 0f));
		_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[1], new Vector2(1f, 0f));
		_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[2], new Vector2(0f, 1f));
		_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[3], new Vector2(1f, 1f));
		Graphics.SetRenderTarget(_mrtRB[0], _mrtTexDepth[0].depthBuffer);
		_hbaoMaterial.SetPass(10);
		Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		Graphics.SetRenderTarget(_mrtRBNrm[0], _mrtTexNrm[0].depthBuffer);
		_hbaoMaterial.SetPass(12);
		Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		RenderTexture.active = active;
		for (int j = 0; j < 4; j++)
		{
			_hbaoMaterial.SetTexture(ShaderProperties.depthTex, _mrtTexDepth[j]);
			_hbaoMaterial.SetTexture(ShaderProperties.normalsTex, _mrtTexNrm[j]);
			_hbaoMaterial.SetVector(ShaderProperties.jitter, _jitter[j]);
			Graphics.Blit(source, _mrtTexAO[j], _hbaoMaterial, GetAoDeinterleavedPass());
			_mrtTexDepth[j].DiscardContents();
			_mrtTexNrm[j].DiscardContents();
		}
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int k = 0; k < 4; k++)
		{
			_hbaoMaterial.SetVector(ShaderProperties.layerOffset, new Vector2((k & 1) * _renderTarget.layerWidth, (k >> 1) * _renderTarget.layerHeight));
			Graphics.Blit(_mrtTexAO[k], temporary, _hbaoMaterial, 14);
			RenderTexture.ReleaseTemporary(_mrtTexAO[k]);
			RenderTexture.ReleaseTemporary(_mrtTexNrm[k]);
			RenderTexture.ReleaseTemporary(_mrtTexDepth[k]);
		}
		RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		Graphics.Blit(temporary, temporary2, _hbaoMaterial, 15);
		temporary.DiscardContents();
		if (base.blurSettings.amount != Blur.None)
		{
			if (base.blurSettings.downsample)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
				Graphics.Blit(temporary2, temporary3, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary3, temporary2, _hbaoMaterial, GetBlurYPass());
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurYPass());
			}
		}
		RenderTexture.ReleaseTemporary(temporary);
		_hbaoMaterial.SetTexture(ShaderProperties.hbaoTex, temporary2);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary2);
	}

	private void RenderHBAODeinterleaved4x(RenderTexture source, RenderTexture destination)
	{
		RenderTexture active = RenderTexture.active;
		for (int i = 0; i < 16; i++)
		{
			_mrtTexDepth[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.RFloat);
			_mrtTexNrm[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.ARGB2101010);
			_mrtTexAO[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight);
			RenderTexture.active = _mrtTexAO[i];
			GL.Clear(clearDepth: false, clearColor: true, Color.white);
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				ref RenderBuffer reference = ref _mrtRB[j][k];
				reference = _mrtTexDepth[k + 4 * j].colorBuffer;
				ref RenderBuffer reference2 = ref _mrtRBNrm[j][k];
				reference2 = _mrtTexNrm[k + 4 * j].colorBuffer;
			}
		}
		for (int l = 0; l < 4; l++)
		{
			int num = (l & 1) << 1;
			int num2 = l >> 1 << 1;
			_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[0], new Vector2(num, num2));
			_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[1], new Vector2(num + 1, num2));
			_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[2], new Vector2(num, num2 + 1));
			_hbaoMaterial.SetVector(ShaderProperties.deinterleavingOffset[3], new Vector2(num + 1, num2 + 1));
			Graphics.SetRenderTarget(_mrtRB[l], _mrtTexDepth[4 * l].depthBuffer);
			_hbaoMaterial.SetPass(11);
			Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
			Graphics.SetRenderTarget(_mrtRBNrm[l], _mrtTexNrm[4 * l].depthBuffer);
			_hbaoMaterial.SetPass(13);
			Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		}
		RenderTexture.active = active;
		for (int m = 0; m < 16; m++)
		{
			_hbaoMaterial.SetTexture(ShaderProperties.depthTex, _mrtTexDepth[m]);
			_hbaoMaterial.SetTexture(ShaderProperties.normalsTex, _mrtTexNrm[m]);
			_hbaoMaterial.SetVector(ShaderProperties.jitter, _jitter[m]);
			Graphics.Blit(source, _mrtTexAO[m], _hbaoMaterial, GetAoDeinterleavedPass());
			_mrtTexDepth[m].DiscardContents();
			_mrtTexNrm[m].DiscardContents();
		}
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int n = 0; n < 16; n++)
		{
			_hbaoMaterial.SetVector(ShaderProperties.layerOffset, new Vector2(((n & 1) + ((n & 7) >> 2 << 1)) * _renderTarget.layerWidth, (((n & 3) >> 1) + (n >> 3 << 1)) * _renderTarget.layerHeight));
			Graphics.Blit(_mrtTexAO[n], temporary, _hbaoMaterial, 14);
			RenderTexture.ReleaseTemporary(_mrtTexAO[n]);
			RenderTexture.ReleaseTemporary(_mrtTexNrm[n]);
			RenderTexture.ReleaseTemporary(_mrtTexDepth[n]);
		}
		RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		Graphics.Blit(temporary, temporary2, _hbaoMaterial, 16);
		temporary.DiscardContents();
		if (base.blurSettings.amount != Blur.None)
		{
			if (base.blurSettings.downsample)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
				Graphics.Blit(temporary2, temporary3, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary3, temporary2, _hbaoMaterial, GetBlurYPass());
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurYPass());
			}
		}
		RenderTexture.ReleaseTemporary(temporary);
		_hbaoMaterial.SetTexture(ShaderProperties.hbaoTex, temporary2);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary2);
	}
}
