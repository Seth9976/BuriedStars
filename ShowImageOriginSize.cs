using GameData;
using UnityEngine;
using UnityEngine.UI;

public class ShowImageOriginSize : MonoBehaviour
{
	public GameObject m_goSpreadGroup;

	public Image m_imgSpread;

	public RectTransform m_rtfImage;

	private Sprite m_sprImage;

	private bool m_isMake;

	private GameDefine.EventProc m_fpClosedCB;

	private RectTransform m_rtParentCanvas;

	private float m_imageScaling = 1f;

	private void OnDestroy()
	{
		m_sprImage = null;
		m_fpClosedCB = null;
	}

	private void InitImg()
	{
		if (m_isMake)
		{
			Object.Destroy(m_sprImage);
			m_sprImage = null;
			m_isMake = false;
		}
		if (m_imgSpread != null)
		{
			m_imgSpread.raycastTarget = false;
		}
	}

	public void OnClickScreen()
	{
		ActiveImage(isShow: false, null, m_isMake);
		AudioManager audioManager = GameGlobalUtil.GetAudioManager();
		if (audioManager != null)
		{
			audioManager.PlayUISound("Menu_Cancel");
		}
	}

	public void ShowImage(bool isShow, Sprite sprImg = null, GameDefine.EventProc fpClosedCB = null, Xls.CollImages xlsColImageData = null)
	{
		m_fpClosedCB = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		ActiveImage(isShow, sprImg, isMake: false);
		if (isShow && xlsColImageData != null)
		{
			AwardCollectionImage(xlsColImageData);
		}
	}

	public void ShowImage(bool isShow, string strPath, GameDefine.EventProc fpClosedCB = null, Xls.CollImages xlsColImageData = null)
	{
		InitImg();
		if (isShow)
		{
			m_isMake = true;
			m_sprImage = Resources.Load<Sprite>(strPath);
		}
		m_fpClosedCB = ((fpClosedCB == null) ? null : new GameDefine.EventProc(fpClosedCB.Invoke));
		ActiveImage(isShow, m_sprImage, isMake: true);
		if (isShow && xlsColImageData != null)
		{
			AwardCollectionImage(xlsColImageData);
		}
	}

	private void ActiveImage(bool isShow, Sprite sprImg, bool isMake)
	{
		if (!isShow || (isShow && !isMake))
		{
			InitImg();
		}
		m_goSpreadGroup.SetActive(isShow);
		if (isShow)
		{
			float width = sprImg.rect.width;
			float height = sprImg.rect.height;
			m_rtfImage.sizeDelta = new Vector2(width, height);
			m_imgSpread.sprite = sprImg;
			m_imgSpread.sprite = sprImg;
		}
		else if (m_fpClosedCB != null)
		{
			m_fpClosedCB(this, null);
		}
	}

	private void AwardCollectionImage(Xls.CollImages xlsColImageData)
	{
		GameSwitch.GetInstance().SetCollImage(xlsColImageData.m_iIdx, 1, isPop: true);
	}

	private void Update()
	{
		if (GamePadInput.IsButtonState_Down(PadInput.GameInput.CircleButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.CrossButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.SquareButton) || GamePadInput.IsButtonState_Down(PadInput.GameInput.TriangleButton))
		{
			OnClickScreen();
		}
		else if (Input.anyKeyDown)
		{
			OnClickScreen();
		}
	}

	public void TouchScreen()
	{
		OnClickScreen();
	}
}
