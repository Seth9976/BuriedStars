using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/HBAO Integrated")]
[RequireComponent(typeof(Camera))]
public class HBAO_Integrated : HBAO_Core
{
	private CommandBuffer _hbaoCommandBuffer;

	private IntegrationStage _integrationStage;

	private Resolution _resolution;

	private DisplayMode _displayMode;

	private RenderingPath _renderingPath;

	private bool _hdr;

	private int _width;

	private int _height;

	private Quality _aoQuality;

	private Deinterleaving _deinterleaving;

	private bool _colorBleedingEnabled;

	private Blur _blurAmount;

	private bool _prepareInitialCommandBuffer;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (_hbaoCommandBuffer == null)
		{
			_hbaoCommandBuffer = new CommandBuffer();
			_hbaoCommandBuffer.name = "HBAO";
		}
		_prepareInitialCommandBuffer = true;
	}

	protected override void OnDisable()
	{
		ClearCommandBuffer();
		base.OnDisable();
	}

	protected override void CheckParameters()
	{
		base.CheckParameters();
		CameraEvent cameraEvent = GetCameraEvent();
		if (cameraEvent != CameraEvent.BeforeImageEffectsOpaque && !IsDeferredShading())
		{
			GeneralSettings generalSettings = base.generalSettings;
			generalSettings.integrationStage = IntegrationStage.BeforeImageEffectsOpaque;
			base.generalSettings = generalSettings;
		}
		if (cameraEvent == CameraEvent.BeforeImageEffectsOpaque && base.aoSettings.perPixelNormals == PerPixelNormals.GBuffer)
		{
			AOSettings aOSettings = base.aoSettings;
			aOSettings.perPixelNormals = PerPixelNormals.Camera;
			base.aoSettings = aOSettings;
		}
		else if (cameraEvent != CameraEvent.BeforeImageEffectsOpaque && base.aoSettings.perPixelNormals == PerPixelNormals.Camera)
		{
			AOSettings aOSettings2 = base.aoSettings;
			aOSettings2.perPixelNormals = PerPixelNormals.GBuffer;
			base.aoSettings = aOSettings2;
		}
	}

	private void OnPreRender()
	{
		if (hbaoShader == null || _hbaoCamera == null)
		{
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
		bool flag = false;
		if (_integrationStage != base.generalSettings.integrationStage || _resolution != base.generalSettings.resolution || _displayMode != base.generalSettings.displayMode || _renderingPath != _renderTarget.renderingPath || _hdr != _renderTarget.hdr || _width != _renderTarget.fullWidth || _height != _renderTarget.fullHeight || _aoQuality != base.generalSettings.quality || _deinterleaving != base.generalSettings.deinterleaving || _colorBleedingEnabled != base.colorBleedingSettings.enabled || _blurAmount != base.blurSettings.amount)
		{
			_integrationStage = base.generalSettings.integrationStage;
			_resolution = base.generalSettings.resolution;
			_displayMode = base.generalSettings.displayMode;
			_renderingPath = _renderTarget.renderingPath;
			_hdr = _renderTarget.hdr;
			_width = _renderTarget.fullWidth;
			_height = _renderTarget.fullHeight;
			_aoQuality = base.generalSettings.quality;
			_deinterleaving = base.generalSettings.deinterleaving;
			_colorBleedingEnabled = base.colorBleedingSettings.enabled;
			_blurAmount = base.blurSettings.amount;
			flag = true;
		}
		if (flag || _prepareInitialCommandBuffer)
		{
			ClearCommandBuffer();
			CameraEvent cameraEvent = GetCameraEvent();
			if (base.generalSettings.deinterleaving == Deinterleaving._2x)
			{
				PrepareCommandBufferHBAODeinterleaved2x(cameraEvent);
			}
			else if (base.generalSettings.deinterleaving == Deinterleaving._4x)
			{
				PrepareCommandBufferHBAODeinterleaved4x(cameraEvent);
			}
			else
			{
				PrepareCommandBufferHBAO(cameraEvent);
			}
			_hbaoCamera.AddCommandBuffer(cameraEvent, _hbaoCommandBuffer);
			_prepareInitialCommandBuffer = false;
		}
	}

	private void PrepareCommandBufferHBAO(CameraEvent cameraEvent)
	{
		RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ShaderProperties.mainTex);
		RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(ShaderProperties.hbaoTex);
		_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.hbaoTex, _renderTarget.fullWidth / _renderTarget.downsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor);
		_hbaoCommandBuffer.SetRenderTarget(renderTargetIdentifier2);
		_hbaoCommandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.white);
		_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier2, _hbaoMaterial, GetAoPass());
		if (base.blurSettings.amount != Blur.None)
		{
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.fullWidth / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor);
			_hbaoCommandBuffer.Blit(renderTargetIdentifier2, renderTargetIdentifier, _hbaoMaterial, GetBlurXPass());
			_hbaoCommandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, _hbaoMaterial, GetBlurYPass());
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
		}
		_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.hbaoTex, renderTargetIdentifier2);
		RenderHBAO(cameraEvent);
		_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.hbaoTex);
	}

	private void PrepareCommandBufferHBAODeinterleaved2x(CameraEvent cameraEvent)
	{
		RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ShaderProperties.mainTex);
		RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(ShaderProperties.hbaoTex);
		RenderTargetIdentifier[] array = new RenderTargetIdentifier[4]
		{
			ShaderProperties.mrtDepthTex[0],
			ShaderProperties.mrtDepthTex[1],
			ShaderProperties.mrtDepthTex[2],
			ShaderProperties.mrtDepthTex[3]
		};
		RenderTargetIdentifier[] array2 = new RenderTargetIdentifier[4]
		{
			ShaderProperties.mrtNrmTex[0],
			ShaderProperties.mrtNrmTex[1],
			ShaderProperties.mrtNrmTex[2],
			ShaderProperties.mrtNrmTex[3]
		};
		RenderTargetIdentifier[] array3 = new RenderTargetIdentifier[4]
		{
			ShaderProperties.mrtHBAOTex[0],
			ShaderProperties.mrtHBAOTex[1],
			ShaderProperties.mrtHBAOTex[2],
			ShaderProperties.mrtHBAOTex[3]
		};
		for (int i = 0; i < 4; i++)
		{
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtDepthTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.RFloat);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtNrmTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtHBAOTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
		}
		_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[0], new Vector2(0f, 0f));
		_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[1], new Vector2(1f, 0f));
		_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[2], new Vector2(0f, 1f));
		_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[3], new Vector2(1f, 1f));
		_hbaoCommandBuffer.SetRenderTarget(array, array[0]);
		_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 10);
		_hbaoCommandBuffer.SetRenderTarget(array2, array2[0]);
		_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 12);
		for (int j = 0; j < 4; j++)
		{
			_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.depthTex, array[j]);
			_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.normalsTex, array2[j]);
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.jitter, _jitter[j]);
			_hbaoCommandBuffer.SetRenderTarget(array3[j]);
			_hbaoCommandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.white);
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, array3[j], _hbaoMaterial, GetAoDeinterleavedPass());
		}
		_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int k = 0; k < 4; k++)
		{
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.layerOffset, new Vector2((k & 1) * _renderTarget.layerWidth, (k >> 1) * _renderTarget.layerHeight));
			_hbaoCommandBuffer.Blit(array3[k], renderTargetIdentifier, _hbaoMaterial, 14);
		}
		_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.hbaoTex, _renderTarget.fullWidth, _renderTarget.fullHeight);
		_hbaoCommandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, _hbaoMaterial, 15);
		if (base.blurSettings.amount != Blur.None)
		{
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
			_hbaoCommandBuffer.Blit(renderTargetIdentifier2, renderTargetIdentifier, _hbaoMaterial, GetBlurXPass());
			_hbaoCommandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, _hbaoMaterial, GetBlurYPass());
		}
		_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
		for (int l = 0; l < 4; l++)
		{
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtHBAOTex[l]);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtNrmTex[l]);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtDepthTex[l]);
		}
		_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.hbaoTex, renderTargetIdentifier2);
		RenderHBAO(cameraEvent);
		_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.hbaoTex);
	}

	private void PrepareCommandBufferHBAODeinterleaved4x(CameraEvent cameraEvent)
	{
		RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ShaderProperties.mainTex);
		RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(ShaderProperties.hbaoTex);
		RenderTargetIdentifier[] array = new RenderTargetIdentifier[16];
		RenderTargetIdentifier[] array2 = new RenderTargetIdentifier[16];
		RenderTargetIdentifier[] array3 = new RenderTargetIdentifier[16];
		for (int i = 0; i < 16; i++)
		{
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtDepthTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.RFloat);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtNrmTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mrtHBAOTex[i], _renderTarget.layerWidth, _renderTarget.layerHeight, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
			ref RenderTargetIdentifier reference = ref array[i];
			reference = ShaderProperties.mrtDepthTex[i];
			ref RenderTargetIdentifier reference2 = ref array2[i];
			reference2 = ShaderProperties.mrtNrmTex[i];
			ref RenderTargetIdentifier reference3 = ref array3[i];
			reference3 = ShaderProperties.mrtHBAOTex[i];
		}
		for (int j = 0; j < 4; j++)
		{
			int num = (j & 1) << 1;
			int num2 = j >> 1 << 1;
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[0], new Vector2(num, num2));
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[1], new Vector2(num + 1, num2));
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[2], new Vector2(num, num2 + 1));
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.deinterleavingOffset[3], new Vector2(num + 1, num2 + 1));
			RenderTargetIdentifier[] array4 = new RenderTargetIdentifier[4]
			{
				array[j << 2],
				array[(j << 2) + 1],
				array[(j << 2) + 2],
				array[(j << 2) + 3]
			};
			RenderTargetIdentifier[] array5 = new RenderTargetIdentifier[4]
			{
				array2[j << 2],
				array2[(j << 2) + 1],
				array2[(j << 2) + 2],
				array2[(j << 2) + 3]
			};
			_hbaoCommandBuffer.SetRenderTarget(array4, array4[0]);
			_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 11);
			_hbaoCommandBuffer.SetRenderTarget(array5, array5[0]);
			_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 13);
		}
		for (int k = 0; k < 16; k++)
		{
			_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.depthTex, array[k]);
			_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.normalsTex, array2[k]);
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.jitter, _jitter[k]);
			_hbaoCommandBuffer.SetRenderTarget(array3[k]);
			_hbaoCommandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.white);
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, array3[k], _hbaoMaterial, GetAoDeinterleavedPass());
		}
		_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int l = 0; l < 16; l++)
		{
			_hbaoCommandBuffer.SetGlobalVector(ShaderProperties.layerOffset, new Vector2(((l & 1) + ((l & 7) >> 2 << 1)) * _renderTarget.layerWidth, (((l & 3) >> 1) + (l >> 3 << 1)) * _renderTarget.layerHeight));
			_hbaoCommandBuffer.Blit(array3[l], renderTargetIdentifier, _hbaoMaterial, 14);
		}
		_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.hbaoTex, _renderTarget.fullWidth, _renderTarget.fullHeight);
		_hbaoCommandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, _hbaoMaterial, 16);
		if (base.blurSettings.amount != Blur.None)
		{
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
			_hbaoCommandBuffer.Blit(renderTargetIdentifier2, renderTargetIdentifier, _hbaoMaterial, GetBlurXPass());
			_hbaoCommandBuffer.Blit(renderTargetIdentifier, renderTargetIdentifier2, _hbaoMaterial, GetBlurYPass());
		}
		_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
		for (int m = 0; m < 16; m++)
		{
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtHBAOTex[m]);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtNrmTex[m]);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mrtDepthTex[m]);
		}
		_hbaoCommandBuffer.SetGlobalTexture(ShaderProperties.hbaoTex, renderTargetIdentifier2);
		RenderHBAO(cameraEvent);
		_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.hbaoTex);
	}

	private void RenderHBAO(CameraEvent cameraEvent)
	{
		if (base.generalSettings.displayMode == DisplayMode.Normal)
		{
			if (cameraEvent == CameraEvent.BeforeReflections)
			{
				RenderTargetIdentifier[] colors = new RenderTargetIdentifier[2]
				{
					BuiltinRenderTextureType.GBuffer0,
					(!_renderTarget.hdr) ? BuiltinRenderTextureType.GBuffer3 : BuiltinRenderTextureType.CameraTarget
				};
				if (_renderTarget.hdr)
				{
					_hbaoCommandBuffer.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
					_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 32);
					if (base.colorBleedingSettings.enabled)
					{
						_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 35);
					}
				}
				else
				{
					RenderTargetIdentifier dest = new RenderTargetIdentifier(ShaderProperties.rt0Tex);
					RenderTargetIdentifier dest2 = new RenderTargetIdentifier(ShaderProperties.rt3Tex);
					_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.rt0Tex, _renderTarget.fullWidth, _renderTarget.fullHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
					_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.rt3Tex, _renderTarget.fullWidth, _renderTarget.fullHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB2101010);
					_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.GBuffer0, dest);
					_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.GBuffer3, dest2);
					_hbaoCommandBuffer.SetRenderTarget(colors, BuiltinRenderTextureType.GBuffer3);
					_hbaoCommandBuffer.DrawMesh(quadMesh, Matrix4x4.identity, _hbaoMaterial, 0, 31);
					_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.rt3Tex);
					_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.rt0Tex);
				}
			}
			switch (cameraEvent)
			{
			case CameraEvent.AfterLighting:
				if (_renderTarget.hdr)
				{
					_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 34);
					if (base.colorBleedingSettings.enabled)
					{
						_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 35);
					}
				}
				else
				{
					RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(ShaderProperties.rt3Tex);
					_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.rt3Tex, _renderTarget.fullWidth, _renderTarget.fullHeight, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
					_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.GBuffer3, renderTargetIdentifier);
					_hbaoCommandBuffer.Blit(renderTargetIdentifier, BuiltinRenderTextureType.GBuffer3, _hbaoMaterial, 33);
					_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.rt3Tex);
				}
				break;
			case CameraEvent.BeforeImageEffectsOpaque:
				_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 34);
				if (base.colorBleedingSettings.enabled)
				{
					_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 35);
				}
				break;
			}
		}
		else if (base.generalSettings.displayMode == DisplayMode.AOOnly)
		{
			RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(ShaderProperties.mainTex);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.width, _renderTarget.height);
			_hbaoCommandBuffer.SetRenderTarget(renderTargetIdentifier2);
			_hbaoCommandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.white);
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier2, _hbaoMaterial, 34);
			_hbaoCommandBuffer.Blit(renderTargetIdentifier2, BuiltinRenderTextureType.CameraTarget);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
		}
		else if (base.generalSettings.displayMode == DisplayMode.ColorBleedingOnly)
		{
			RenderTargetIdentifier renderTargetIdentifier3 = new RenderTargetIdentifier(ShaderProperties.mainTex);
			_hbaoCommandBuffer.GetTemporaryRT(ShaderProperties.mainTex, _renderTarget.width, _renderTarget.height);
			_hbaoCommandBuffer.SetRenderTarget(renderTargetIdentifier3);
			_hbaoCommandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, Color.black);
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier3, _hbaoMaterial, 35);
			_hbaoCommandBuffer.Blit(renderTargetIdentifier3, BuiltinRenderTextureType.CameraTarget);
			_hbaoCommandBuffer.ReleaseTemporaryRT(ShaderProperties.mainTex);
		}
		else if (base.generalSettings.displayMode == DisplayMode.SplitWithAOAndAOOnly)
		{
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 34);
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 36);
		}
		else if (base.generalSettings.displayMode == DisplayMode.SplitWithoutAOAndAOOnly)
		{
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 36);
		}
		else if (base.generalSettings.displayMode == DisplayMode.SplitWithoutAOAndWithAO)
		{
			_hbaoCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, _hbaoMaterial, 37);
		}
	}

	private void ClearCommandBuffer()
	{
		if (_hbaoCommandBuffer != null)
		{
			if (_hbaoCamera != null)
			{
				_hbaoCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, _hbaoCommandBuffer);
				_hbaoCamera.RemoveCommandBuffer(CameraEvent.AfterLighting, _hbaoCommandBuffer);
				_hbaoCamera.RemoveCommandBuffer(CameraEvent.BeforeReflections, _hbaoCommandBuffer);
			}
			_hbaoCommandBuffer.Clear();
		}
	}

	private CameraEvent GetCameraEvent()
	{
		if (base.generalSettings.displayMode != DisplayMode.Normal)
		{
			return CameraEvent.BeforeImageEffectsOpaque;
		}
		return base.generalSettings.integrationStage switch
		{
			IntegrationStage.BeforeReflections => CameraEvent.BeforeReflections, 
			IntegrationStage.AfterLighting => CameraEvent.AfterLighting, 
			_ => CameraEvent.BeforeImageEffectsOpaque, 
		};
	}
}
