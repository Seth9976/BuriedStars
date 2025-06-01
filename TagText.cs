using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TagText : Text
{
	private enum TagType
	{
		Unkown,
		Style,
		Color,
		Size,
		Type,
		Delay,
		NewLine,
		Hide
	}

	public delegate void CompleteTypingEffectCallBack();

	public class Syntax
	{
		public enum SyntaxType
		{
			Text,
			Style,
			Color,
			Size,
			TypeSpeed,
			Delay,
			NewLine,
			Hide,
			TagPack
		}

		private static readonly char[] c_BoundChar_TagNameParam = new char[1] { '=' };

		private SyntaxType m_Type;

		private string m_strOrigin;

		private string m_strTrans;

		private object m_param;

		public bool m_isFinishTag;

		public int m_position = -1;

		public int m_lineNumber = -1;

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024map7;

		public SyntaxType type => m_Type;

		public string strOrigin => m_strOrigin;

		public int strOriginLength => (m_strOrigin != null) ? m_strOrigin.Length : 0;

		public string strTrans => m_strTrans;

		public int strTransLength => (m_strTrans != null) ? m_strTrans.Length : 0;

		public object param => m_param;

		private Syntax()
		{
		}

		public Syntax(SyntaxType _type, string _strOrigin, bool _isFinishTag = false, object _param = null)
		{
			m_Type = _type;
			m_strOrigin = _strOrigin;
			m_isFinishTag = _isFinishTag;
			m_param = _param;
			m_strTrans = TransToUnityTag(m_isFinishTag);
			if (m_strTrans == null)
			{
				m_strTrans = string.Empty;
			}
		}

		public Syntax Clone()
		{
			Syntax syntax = new Syntax();
			syntax.m_Type = m_Type;
			syntax.m_strOrigin = m_strOrigin;
			syntax.m_strTrans = m_strTrans;
			if (m_Type != SyntaxType.TagPack)
			{
				syntax.m_param = m_param;
			}
			else if (m_param is List<Syntax> { Count: >0 } list)
			{
				List<Syntax> list2 = new List<Syntax>();
				foreach (Syntax item in list)
				{
					list2.Add(item.Clone());
				}
				syntax.m_param = list2;
			}
			syntax.m_isFinishTag = m_isFinishTag;
			return syntax;
		}

		public void TransToTextType()
		{
			if (IsPairableType() || m_Type == SyntaxType.TagPack)
			{
				if (m_Type == SyntaxType.TagPack)
				{
					m_strTrans = string.Format("{0}{1}{2}{3}", c_TagBegin, (!m_isFinishTag) ? string.Empty : "/", m_strOrigin, c_TagEnd);
				}
				m_Type = SyntaxType.Text;
				m_isFinishTag = false;
				m_param = null;
			}
		}

		public Syntax CreateFinishTagSyntax(string _strOrigin = null)
		{
			Syntax syntax = null;
			switch (m_Type)
			{
			case SyntaxType.TagPack:
			{
				if (m_param == null || !(m_param is List<Syntax>))
				{
					break;
				}
				List<Syntax> list = m_param as List<Syntax>;
				List<Syntax> list2 = new List<Syntax>();
				int num = list.Count;
				while (num > 0)
				{
					num--;
					Syntax syntax2 = list[num].CreateFinishTagSyntax();
					if (syntax2 != null)
					{
						list2.Add(syntax2);
					}
				}
				return new Syntax(m_Type, (_strOrigin == null) ? m_strOrigin : _strOrigin, _isFinishTag: true, list2);
			}
			case SyntaxType.TypeSpeed:
				return new Syntax(m_Type, string.Empty, _isFinishTag: false, 0f);
			}
			if (!IsPairableType())
			{
				return null;
			}
			if (m_isFinishTag)
			{
				return null;
			}
			return new Syntax(m_Type, (_strOrigin == null) ? m_strOrigin : _strOrigin, _isFinishTag: true, m_param);
		}

		public string GetNewColorStr_ByAlpha(float fNormalizedAlpha)
		{
			if (m_Type != SyntaxType.Color)
			{
				return string.Empty;
			}
			if (m_param == null || !(m_param is Color))
			{
				return "<color>";
			}
			Color color = (Color)m_param;
			color.a *= fNormalizedAlpha;
			Color32 color2 = color;
			return string.Format("<color=#{0}{1}{2}{3}>", color2.r.ToString("x2"), color2.g.ToString("x2"), color2.b.ToString("x2"), color2.a.ToString("x2"));
		}

		public Syntax GetFirstSubSyntax()
		{
			if (m_Type != SyntaxType.TagPack)
			{
				return null;
			}
			if (m_param == null || !(m_param is List<Syntax>))
			{
				return null;
			}
			return (!(m_param is List<Syntax> { Count: >0 } list)) ? null : list[0];
		}

		public Syntax GetLastSubSyntax()
		{
			if (m_Type != SyntaxType.TagPack)
			{
				return null;
			}
			if (m_param == null || !(m_param is List<Syntax>))
			{
				return null;
			}
			return (!(m_param is List<Syntax> { Count: >0 } list)) ? null : list[list.Count - 1];
		}

		public Syntax[] GetSubSyntaxes()
		{
			if (m_Type != SyntaxType.TagPack)
			{
				return null;
			}
			if (m_param == null || !(m_param is List<Syntax>))
			{
				return null;
			}
			List<Syntax> list = m_param as List<Syntax>;
			return list.ToArray();
		}

		public bool IsExistHideTag()
		{
			if (m_Type == SyntaxType.Hide)
			{
				return true;
			}
			if (m_Type != SyntaxType.TagPack)
			{
				return false;
			}
			if (!(m_param is List<Syntax> { Count: >0 } list))
			{
				return false;
			}
			foreach (Syntax item in list)
			{
				if (item.IsExistHideTag())
				{
					return true;
				}
			}
			return false;
		}

		public static Syntax ParseTagPack(string tagPackString)
		{
			int num = ((!string.IsNullOrEmpty(tagPackString)) ? tagPackString.Length : 0);
			if (num > 0)
			{
				int num2 = tagPackString.IndexOf(':');
				if (num2 > 0)
				{
					string text = tagPackString.Substring(0, num2);
					string text2 = tagPackString.Substring(num2 + 1);
					List<Syntax> list = new List<Syntax>();
					int num3 = 0;
					while (true)
					{
						if (num3 < text2.Length)
						{
							int num4 = text2.IndexOf(c_TagBegin, num3);
							if (num4 < 0)
							{
								break;
							}
							int num5 = text2.IndexOf(c_TagEnd, num4 + 1);
							if (num5 < 0)
							{
								break;
							}
							string tagString = text2.Substring(num4, num5 - num4 + 1);
							Syntax syntax = ParseTag(tagString);
							if (syntax == null)
							{
								break;
							}
							list.Add(syntax);
							num3 = num5 + 1;
							continue;
						}
						if (list.Count <= 0)
						{
							break;
						}
						return new Syntax(SyntaxType.TagPack, text, _isFinishTag: false, list);
					}
				}
			}
			return null;
		}

		public static Syntax ParseTag(string tagString, Dictionary<string, Syntax> localDefinedTagPacks = null, Dictionary<string, Syntax> globalDefinedTagPacks = null)
		{
			int num = ((!string.IsNullOrEmpty(tagString)) ? tagString.Length : 0);
			if (num > 0 && tagString[0] == c_TagBegin && tagString[num - 1] == c_TagEnd)
			{
				int num2 = num - 2;
				if (num2 > 0)
				{
					try
					{
						bool flag = tagString[1] == '/';
						string text = null;
						string tagParam = null;
						int num3 = ((!flag) ? 1 : 2);
						int num4 = tagString.IndexOfAny(c_BoundChar_TagNameParam, num3);
						if (num4 < 0)
						{
							text = tagString.Substring(num3, flag ? (num2 - 1) : num2);
						}
						else
						{
							text = tagString.Substring(num3, num4 - num3);
							num3 = num4 + 1;
							tagParam = tagString.Substring(num3, num - num3 - 1);
						}
						if (text != null)
						{
							if (_003C_003Ef__switch_0024map7 == null)
							{
								Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
								dictionary.Add("n", 0);
								dictionary.Add("b", 1);
								dictionary.Add("i", 2);
								dictionary.Add("bi", 3);
								dictionary.Add("color", 4);
								dictionary.Add("size", 5);
								dictionary.Add("type", 6);
								dictionary.Add("delay", 7);
								dictionary.Add("nl", 8);
								dictionary.Add("hide", 9);
								_003C_003Ef__switch_0024map7 = dictionary;
							}
							if (_003C_003Ef__switch_0024map7.TryGetValue(text, out var value))
							{
								switch (value)
								{
								case 0:
									return new Syntax(SyntaxType.Style, tagString, flag, FontStyle.Normal);
								case 1:
									return new Syntax(SyntaxType.Style, tagString, flag, FontStyle.Bold);
								case 2:
									return new Syntax(SyntaxType.Style, tagString, flag, FontStyle.Italic);
								case 3:
									return new Syntax(SyntaxType.Style, tagString, flag, FontStyle.BoldAndItalic);
								case 4:
									return ParseTag_Color(tagString, flag, tagParam);
								case 5:
									return ParseTag_Size(tagString, flag, tagParam);
								case 6:
									return ParseTag_TypeSpeed(tagString, flag, tagParam);
								case 7:
									return ParseTag_TypeDelay(tagString, tagParam);
								case 8:
									return new Syntax(SyntaxType.NewLine, tagString);
								case 9:
									return new Syntax(SyntaxType.Hide, tagString);
								}
							}
						}
						Syntax value2 = null;
						localDefinedTagPacks?.TryGetValue(text, out value2);
						if (value2 == null)
						{
							globalDefinedTagPacks?.TryGetValue(text, out value2);
						}
						if (value2 != null)
						{
							value2 = ((!flag) ? value2.Clone() : value2.CreateFinishTagSyntax(text));
						}
						return value2;
					}
					catch (IndexOutOfRangeException)
					{
						return null;
					}
				}
			}
			return null;
		}

		private static Syntax ParseTag_Color(string tagString, bool isFinishTag, string tagParam)
		{
			if (isFinishTag)
			{
				return new Syntax(SyntaxType.Color, tagString, _isFinishTag: true);
			}
			if (!string.IsNullOrEmpty(tagParam))
			{
				Color outColor = default(Color);
				if (TryParseColor(tagParam, ref outColor))
				{
					return new Syntax(SyntaxType.Color, tagString, _isFinishTag: false, outColor);
				}
			}
			return new Syntax(SyntaxType.Color, tagString);
		}

		public static bool TryParseColor(string strColor, ref Color outColor)
		{
			int num = strColor?.Length ?? 0;
			if (num <= 0)
			{
				return false;
			}
			if (strColor[0] == '#')
			{
				string text = strColor.Substring(1);
				int result = 0;
				if (text.Length == 6)
				{
					text += "ff";
				}
				if (text.Length == 8 && int.TryParse(text, NumberStyles.AllowHexSpecifier, null, out result))
				{
					byte r = (byte)((result >> 24) & 0xFF);
					byte g = (byte)((result >> 16) & 0xFF);
					byte b = (byte)((result >> 8) & 0xFF);
					byte a = (byte)(result & 0xFF);
					Color32 color = new Color32(r, g, b, a);
					outColor = color;
					return true;
				}
			}
			else
			{
				Type typeFromHandle = typeof(Color);
				PropertyInfo property = typeFromHandle.GetProperty(strColor, typeFromHandle);
				if (property != null)
				{
					outColor = (Color)property.GetValue(null, null);
					return true;
				}
			}
			return false;
		}

		private static Syntax ParseTag_Size(string tagString, bool isFinishTag, string tagParam)
		{
			if (isFinishTag)
			{
				return new Syntax(SyntaxType.Size, tagString, _isFinishTag: true);
			}
			if (string.IsNullOrEmpty(tagParam))
			{
				return null;
			}
			int result = 0;
			if (!int.TryParse(tagParam, out result))
			{
				return null;
			}
			return new Syntax(SyntaxType.Size, tagString, _isFinishTag: false, result);
		}

		private static Syntax ParseTag_TypeSpeed(string tagString, bool isFinishTag, string tagParam)
		{
			float result = 0f;
			if (!isFinishTag)
			{
				if (string.IsNullOrEmpty(tagParam))
				{
					return null;
				}
				if (!float.TryParse(tagParam, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return null;
				}
			}
			return new Syntax(SyntaxType.TypeSpeed, tagString, _isFinishTag: false, result);
		}

		private static Syntax ParseTag_TypeDelay(string tagString, string tagParam)
		{
			float result = 0f;
			if (!float.TryParse(tagParam, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return null;
			}
			return new Syntax(SyntaxType.Delay, tagString, _isFinishTag: false, result);
		}

		public bool IsPairableType()
		{
			if (m_Type == SyntaxType.TagPack && m_param != null && m_param is List<Syntax>)
			{
				List<Syntax> list = m_param as List<Syntax>;
				foreach (Syntax item in list)
				{
					if (item.IsPairableType())
					{
						return true;
					}
				}
				return false;
			}
			return m_Type == SyntaxType.Color || m_Type == SyntaxType.Size || m_Type == SyntaxType.Style;
		}

		public bool IsPair(Syntax syntax)
		{
			if (!IsPairableType())
			{
				return false;
			}
			if (syntax == null)
			{
				return false;
			}
			if (m_Type != syntax.m_Type)
			{
				return false;
			}
			if ((m_isFinishTag && syntax.m_isFinishTag) || (!m_isFinishTag && !syntax.m_isFinishTag))
			{
				return false;
			}
			if (m_Type == SyntaxType.Style && (FontStyle)m_param != (FontStyle)syntax.m_param)
			{
				return false;
			}
			return true;
		}

		public string TransToUnityTag(bool isFinishTag)
		{
			switch (m_Type)
			{
			case SyntaxType.Text:
				return m_strOrigin;
			case SyntaxType.Style:
				if (m_param is FontStyle)
				{
					switch ((FontStyle)m_param)
					{
					case FontStyle.Normal:
						return (!isFinishTag) ? "<n>" : "</n>";
					case FontStyle.Bold:
						return (!isFinishTag) ? "<b>" : "</b>";
					case FontStyle.Italic:
						return (!isFinishTag) ? "<i>" : "</i>";
					case FontStyle.BoldAndItalic:
						return (!isFinishTag) ? "<b><i>" : "</i></b>";
					}
				}
				return string.Empty;
			case SyntaxType.Color:
			{
				if (isFinishTag)
				{
					return "</color>";
				}
				if (m_param == null || !(m_param is Color))
				{
					return "<color>";
				}
				Color32 color = (Color)m_param;
				return string.Format("<color=#{0}{1}{2}{3}>", color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"));
			}
			case SyntaxType.Size:
				if (isFinishTag)
				{
					return "</size>";
				}
				if (m_param is int)
				{
					return $"<size={(int)m_param}>";
				}
				return string.Empty;
			case SyntaxType.NewLine:
				return "\r\n";
			case SyntaxType.TagPack:
			{
				if (m_param == null || !(m_param is List<Syntax>))
				{
					return string.Empty;
				}
				List<Syntax> list = m_param as List<Syntax>;
				int count = list.Count;
				if (count <= 0)
				{
					return string.Empty;
				}
				string text = string.Empty;
				if (isFinishTag)
				{
					int num = count;
					while (num > 0)
					{
						num--;
						text += list[num].TransToUnityTag(isFinishTag);
					}
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						text += list[i].TransToUnityTag(isFinishTag);
					}
				}
				return text;
			}
			default:
				return string.Empty;
			}
		}

		public Syntax SplitSyntax(int splitBoundIdx)
		{
			if (m_Type != SyntaxType.Text)
			{
				return null;
			}
			int num = splitBoundIdx - m_position;
			if (num <= 0)
			{
				return null;
			}
			if (num >= m_strTrans.Length)
			{
				return null;
			}
			string value = m_strTrans.Substring(num);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			Syntax syntax = new Syntax(m_Type, value);
			syntax.m_position = splitBoundIdx;
			m_strTrans = m_strTrans.Remove(num);
			m_strOrigin = m_strTrans;
			return syntax;
		}
	}

	private static readonly char c_TagDefineBegin = '{';

	private static readonly char c_TagDefineEnd = '}';

	private static readonly char c_TagBegin = '<';

	private static readonly char c_TagEnd = '>';

	[SerializeField]
	protected string m_tagText;

	[SerializeField]
	private string m_TagDefine = string.Empty;

	private bool m_isNeedAnalyzeLocalDefinedTag = true;

	[SerializeField]
	private bool m_IsUseTypingEffect;

	[SerializeField]
	private float m_TypingCountPerSec;

	[SerializeField]
	private bool m_IsUseDialogSpecial;

	[SerializeField]
	private int m_LineCountPerPage;

	private List<Syntax> m_syntaxList = new List<Syntax>();

	private List<Syntax> m_curPageSyntaxList = new List<Syntax>();

	private const int c_bufferCapacity = 1000;

	private TextGenerator m_textGen;

	private static StringBuilder s_strBulider = new StringBuilder(1000);

	private int m_PageCount;

	private int m_CurrentPage;

	private int m_visibleStartLine;

	private int m_visibleBoundLine;

	private int m_LineCount;

	private int m_TotalHeight;

	private Vector2 m_LastPosition = Vector2.zero;

	private bool m_isPlayTypingEffect;

	private int m_curTypedCharCount;

	private int m_curTotalValidCharCount;

	private int m_curTypingSyntaxIdx;

	private float m_typingSecPerChar;

	private float m_typingTimeChecker;

	private float m_typingDelayChecker;

	private bool m_isAcivatedOutLine;

	public CompleteTypingEffectCallBack m_cbCompleteTypingEffect;

	private Dictionary<string, Syntax> m_LocalDefinedTag;

	private static Dictionary<string, Syntax> s_GlobalDefinedTag = null;

	public virtual string tagText
	{
		get
		{
			return m_tagText;
		}
		set
		{
			m_tagText = value;
		}
	}

	public string tagDefine
	{
		get
		{
			return m_TagDefine;
		}
		set
		{
			m_TagDefine = value;
			m_isNeedAnalyzeLocalDefinedTag = true;
		}
	}

	public bool useTypingEffect
	{
		get
		{
			return m_IsUseTypingEffect;
		}
		set
		{
			m_IsUseTypingEffect = value;
		}
	}

	public float typingCPS
	{
		get
		{
			return m_TypingCountPerSec;
		}
		set
		{
			m_TypingCountPerSec = value;
		}
	}

	public bool useDialogSpecial
	{
		get
		{
			return m_IsUseDialogSpecial;
		}
		set
		{
			m_IsUseDialogSpecial = value;
		}
	}

	public int lineCountPerPage
	{
		get
		{
			return m_LineCountPerPage;
		}
		set
		{
			m_LineCountPerPage = value;
		}
	}

	public int pageCount => m_PageCount;

	public int currentPage => m_CurrentPage;

	public int lineCount => m_LineCount;

	public int totalHeight => m_TotalHeight;

	public Vector2 lastPosition => m_LastPosition;

	protected override void Awake()
	{
		base.supportRichText = true;
		base.Awake();
		m_textGen = new TextGenerator(1000);
	}

	protected override void Start()
	{
		base.Start();
		BuildTextData();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ClearTextData();
	}

	private void Update()
	{
		if (!m_IsUseTypingEffect || !m_isPlayTypingEffect)
		{
			return;
		}
		if (m_typingSecPerChar < 0f || GameGlobalUtil.IsAlmostSame(m_typingSecPerChar, 0f))
		{
			SkipPlayTypingEffect();
			return;
		}
		float deltaTime = Time.deltaTime;
		if (m_typingDelayChecker > 0f)
		{
			m_typingDelayChecker -= deltaTime;
			return;
		}
		int num = 0;
		while (m_typingTimeChecker >= m_typingSecPerChar)
		{
			num++;
			m_typingTimeChecker -= m_typingSecPerChar;
		}
		m_typingTimeChecker += deltaTime;
		if (num <= 0)
		{
			return;
		}
		int num2 = m_curTypedCharCount + num;
		int i = 0;
		int count = m_curPageSyntaxList.Count;
		int num3 = Mathf.Min(count, m_curTypingSyntaxIdx);
		m_curTotalValidCharCount = 0;
		for (; i < num3; i++)
		{
			Syntax syntax = m_curPageSyntaxList[i];
			m_curTotalValidCharCount += syntax.strTransLength;
			if (syntax.type == Syntax.SyntaxType.Text)
			{
				num2 -= syntax.strTransLength;
			}
		}
		bool flag = false;
		for (; i < count; i++)
		{
			Syntax syntax2 = m_curPageSyntaxList[i];
			if (syntax2.type == Syntax.SyntaxType.Text)
			{
				if (num2 > 0)
				{
					if (num2 >= syntax2.strTransLength)
					{
						num2 -= syntax2.strTransLength;
						m_curTotalValidCharCount += syntax2.strTransLength;
						if (num2 <= 0)
						{
							m_curTypingSyntaxIdx = i + 1;
							m_curTypedCharCount += num;
						}
					}
					else
					{
						m_curTypedCharCount += num;
						m_curTotalValidCharCount += num2;
						m_curTypingSyntaxIdx = i;
						num2 = 0;
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
			else if (num2 > 0)
			{
				if (ActivateTagEffect(syntax2))
				{
					num2 = 0;
					m_curTypingSyntaxIdx = i + 1;
				}
				m_curTotalValidCharCount += syntax2.strTransLength;
			}
		}
		SetVerticesDirty();
		if (!flag)
		{
			m_isPlayTypingEffect = false;
			if (m_cbCompleteTypingEffect != null)
			{
				m_cbCompleteTypingEffect();
			}
		}
	}

	private bool ActivateTagEffect(Syntax syntax)
	{
		bool result = false;
		if (syntax == null)
		{
			return result;
		}
		switch (syntax.type)
		{
		case Syntax.SyntaxType.TypeSpeed:
			if (syntax.param != null && syntax.param is float)
			{
				float num = (float)syntax.param;
				if (num < 0f || GameGlobalUtil.IsAlmostSame(num, 0f))
				{
					num = m_TypingCountPerSec;
				}
				SetTypingSpeed(num);
				result = true;
			}
			break;
		case Syntax.SyntaxType.Delay:
			if (syntax.param != null && syntax.param is float)
			{
				m_typingDelayChecker = (float)syntax.param;
				result = true;
			}
			break;
		case Syntax.SyntaxType.TagPack:
		{
			if (syntax.param == null || !(syntax.param is List<Syntax>))
			{
				break;
			}
			List<Syntax> list = syntax.param as List<Syntax>;
			foreach (Syntax item in list)
			{
				if (ActivateTagEffect(item))
				{
					result = true;
				}
			}
			break;
		}
		}
		return result;
	}

	protected override void OnPopulateMesh(VertexHelper vertexHelper)
	{
		base.OnPopulateMesh(vertexHelper);
		if (!m_isPlayTypingEffect || !m_IsUseTypingEffect)
		{
			return;
		}
		if (m_curTotalValidCharCount <= 0)
		{
			vertexHelper.Clear();
			return;
		}
		List<UIVertex> list = new List<UIVertex>();
		vertexHelper.GetUIVertexStream(list);
		if (list.Count > 0)
		{
			int num = m_curTotalValidCharCount * 6;
			if (num < vertexHelper.currentIndexCount)
			{
				num = Mathf.Min(num, list.Count);
				list.RemoveRange(num, list.Count - num);
				vertexHelper.Clear();
				vertexHelper.AddUIVertexTriangleStream(list);
			}
		}
	}

	public bool IsPlayingTypingEffect()
	{
		return m_IsUseTypingEffect && m_isPlayTypingEffect;
	}

	public void SkipPlayTypingEffect()
	{
		m_isPlayTypingEffect = false;
		SetVerticesDirty();
		if (m_cbCompleteTypingEffect != null)
		{
			m_cbCompleteTypingEffect();
		}
	}

	private void InitTypingEffectParam()
	{
		m_isPlayTypingEffect = false;
		if (m_curPageSyntaxList == null || m_curPageSyntaxList.Count <= 0)
		{
			return;
		}
		m_isPlayTypingEffect = true;
		m_curTypedCharCount = 0;
		m_curTotalValidCharCount = 0;
		m_curTypingSyntaxIdx = 0;
		m_typingTimeChecker = 0f;
		m_typingDelayChecker = 0f;
		Outline component = base.gameObject.GetComponent<Outline>();
		m_isAcivatedOutLine = component != null && component.enabled;
		if (m_IsUseDialogSpecial && (base.alignment == TextAnchor.LowerLeft || base.alignment == TextAnchor.LowerCenter || base.alignment == TextAnchor.LowerRight))
		{
			Syntax syntax = m_curPageSyntaxList[0];
			Syntax syntax2 = m_curPageSyntaxList[m_curPageSyntaxList.Count - 1];
			if (syntax.m_lineNumber == syntax2.m_lineNumber)
			{
				Syntax syntax3 = new Syntax(Syntax.SyntaxType.NewLine, "<nl>");
				syntax3.m_lineNumber = syntax2.m_lineNumber;
				m_curPageSyntaxList.Add(syntax3);
			}
		}
	}

	private void SetTypingSpeed(float typeSpeed)
	{
		typeSpeed = Mathf.Max(typeSpeed, 0f);
		m_typingSecPerChar = ((!(typeSpeed > 0f)) ? 0f : (1f / typeSpeed));
	}

	private Syntax FindLocalDefinedTag(string tagName)
	{
		if (m_LocalDefinedTag == null)
		{
			return null;
		}
		return (!m_LocalDefinedTag.ContainsKey(tagName)) ? null : m_LocalDefinedTag[tagName];
	}

	private static Syntax FindGlobalDefinedTag(string tagName)
	{
		if (s_GlobalDefinedTag == null)
		{
			return null;
		}
		return (!s_GlobalDefinedTag.ContainsKey(tagName)) ? null : s_GlobalDefinedTag[tagName];
	}

	public static void BuildGlobalDefinedTag()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Xls.ProgramDefineStr data_byKey = Xls.ProgramDefineStr.GetData_byKey("TAG_TEXT_GLOBAL_MACRO");
		if (data_byKey != null)
		{
			if (s_GlobalDefinedTag != null)
			{
				s_GlobalDefinedTag.Clear();
				s_GlobalDefinedTag = null;
			}
			AnalyzeDefinedTag(data_byKey.m_strTxt, ref s_GlobalDefinedTag);
		}
	}

	public void ClearTextData()
	{
		m_tagText = string.Empty;
		m_Text = string.Empty;
		m_syntaxList.Clear();
		m_curPageSyntaxList.Clear();
		if (m_LocalDefinedTag != null)
		{
			m_LocalDefinedTag.Clear();
			m_LocalDefinedTag = null;
		}
		m_LineCount = 0;
		m_PageCount = 0;
		m_CurrentPage = 0;
	}

	public void BuildTextData()
	{
		if (Application.isPlaying)
		{
			if (m_isNeedAnalyzeLocalDefinedTag)
			{
				AnalyzeDefinedTag(m_TagDefine, ref m_LocalDefinedTag);
				m_isNeedAnalyzeLocalDefinedTag = false;
			}
		}
		else
		{
			AnalyzeDefinedTag(m_TagDefine, ref m_LocalDefinedTag);
		}
		SetTypingSpeed(m_TypingCountPerSec);
		AnalyzeText();
		SetCurrentPage(0);
	}

	private static void AnalyzeDefinedTag(string strDefinedTag, ref Dictionary<string, Syntax> definedTagList)
	{
		if (string.IsNullOrEmpty(strDefinedTag))
		{
			return;
		}
		strDefinedTag = strDefinedTag.Trim();
		int length = strDefinedTag.Length;
		if (length <= 0)
		{
			return;
		}
		char[] separator = new char[1] { c_TagDefineBegin };
		string[] array = strDefinedTag.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || array.Length <= 0)
		{
			return;
		}
		Dictionary<string, Syntax> dictionary = new Dictionary<string, Syntax>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = text.IndexOf(c_TagDefineEnd);
			if (num >= 0)
			{
				string tagPackString = text.Remove(num).Trim();
				Syntax syntax = Syntax.ParseTagPack(tagPackString);
				if (syntax != null && !dictionary.ContainsKey(syntax.strOrigin))
				{
					dictionary.Add(syntax.strOrigin, syntax);
				}
			}
		}
		definedTagList = dictionary;
	}

	protected void AnalyzeText()
	{
		TransTagTextToUnityText(m_tagText, ref m_syntaxList, m_LocalDefinedTag);
		m_LineCount = 0;
		m_curPageSyntaxList.Clear();
		if (m_textGen != null)
		{
			m_textGen.Invalidate();
		}
		TextGenerationSettings generationSettings = GetGenerationSettings(base.rectTransform.rect.size);
		generationSettings.verticalOverflow = VerticalWrapMode.Truncate;
		if (m_textGen == null)
		{
			m_textGen = new TextGenerator(Mathf.Max(1000, s_strBulider.Length));
		}
		m_textGen.Populate(s_strBulider.ToString(), generationSettings);
		m_LineCount = m_textGen.lineCount;
		if (m_LineCount > 0)
		{
			int num = 0;
			for (int i = 0; i < m_syntaxList.Count; i++)
			{
				Syntax syntax = m_syntaxList[i];
				int position = syntax.m_position;
				int num2 = position + syntax.strTransLength;
				for (int j = num; j < m_LineCount; j++)
				{
					int startCharIdx = m_textGen.lines[j].startCharIdx;
					if (num2 <= startCharIdx)
					{
						syntax.m_lineNumber = j - 1;
						break;
					}
					int num3 = ((j + 1 >= m_LineCount) ? m_textGen.characterCount : m_textGen.lines[j + 1].startCharIdx);
					if (position >= num3)
					{
						continue;
					}
					syntax.m_lineNumber = j;
					if (base.horizontalOverflow == HorizontalWrapMode.Wrap && syntax.type == Syntax.SyntaxType.Text && position < num3 && num2 > num3)
					{
						Syntax syntax2 = syntax.SplitSyntax(num3);
						if (syntax2 != null)
						{
							if (i + 1 >= m_syntaxList.Count)
							{
								m_syntaxList.Add(syntax2);
							}
							else
							{
								m_syntaxList.Insert(i + 1, syntax2);
							}
						}
					}
					num = j;
					break;
				}
			}
		}
		m_PageCount = ((m_LineCountPerPage <= 0) ? 1 : (m_LineCount / m_LineCountPerPage + ((m_LineCount % m_LineCountPerPage != 0) ? 1 : 0)));
		m_PageCount = Mathf.Max(m_PageCount, 1);
	}

	public bool SetCurrentPage(int pageIndex)
	{
		if (pageIndex < 0 || pageIndex >= m_PageCount)
		{
			return false;
		}
		m_visibleStartLine = 0;
		m_visibleBoundLine = 0;
		base.text = string.Empty;
		m_curPageSyntaxList.Clear();
		if (m_PageCount <= 1)
		{
			pageIndex = 0;
			m_curPageSyntaxList.AddRange(m_syntaxList);
		}
		else
		{
			int a = pageIndex * m_LineCountPerPage;
			a = Mathf.Max(a, 0);
			if (a >= m_LineCount)
			{
				return false;
			}
			int num = m_LineCount - a;
			num = ((m_LineCountPerPage <= 0) ? num : Mathf.Min(m_LineCountPerPage, num));
			if (num <= 0)
			{
				return false;
			}
			m_visibleStartLine = a;
			m_visibleBoundLine = a + num;
			List<Syntax> list = new List<Syntax>();
			bool flag = false;
			bool flag2 = false;
			foreach (Syntax syntax in m_syntaxList)
			{
				if (syntax.m_lineNumber >= m_visibleStartLine && syntax.m_lineNumber < m_visibleBoundLine)
				{
					if (!flag)
					{
						if (list.Count > 0)
						{
							m_curPageSyntaxList.AddRange(list);
						}
						flag = true;
					}
					m_curPageSyntaxList.Add(syntax);
				}
				else
				{
					if (syntax.type == Syntax.SyntaxType.Text || syntax.type == Syntax.SyntaxType.NewLine || syntax.type == Syntax.SyntaxType.Delay)
					{
						continue;
					}
					if (syntax.m_lineNumber < m_visibleStartLine)
					{
						if (syntax.type == Syntax.SyntaxType.TypeSpeed)
						{
							m_curPageSyntaxList.Add(syntax);
						}
					}
					else if (!flag2)
					{
						list.Clear();
						flag2 = true;
					}
				}
				if (!syntax.IsPairableType())
				{
					continue;
				}
				if (syntax.m_isFinishTag)
				{
					int count = list.Count;
					if (count > 0 && syntax.IsPair(list[count - 1]))
					{
						list.RemoveAt(count - 1);
					}
					else
					{
						list.Add(syntax);
					}
				}
				else
				{
					list.Add(syntax);
				}
			}
			if (list.Count > 0)
			{
				m_curPageSyntaxList.AddRange(list);
			}
		}
		m_isPlayTypingEffect = false;
		s_strBulider.Remove(0, s_strBulider.Length);
		foreach (Syntax curPageSyntax in m_curPageSyntaxList)
		{
			s_strBulider.Append(curPageSyntax.strTrans);
		}
		base.text = s_strBulider.ToString();
		if (m_IsUseTypingEffect && Application.isPlaying)
		{
			InitTypingEffectParam();
		}
		if (m_IsUseDialogSpecial && Application.isPlaying)
		{
			TextGenerationSettings generationSettings = GetGenerationSettings(base.rectTransform.rect.size);
			generationSettings.verticalOverflow = VerticalWrapMode.Truncate;
			if (m_textGen == null)
			{
				m_textGen = new TextGenerator(Mathf.Max(1000, s_strBulider.Length));
			}
			m_textGen.Populate(text, generationSettings);
			IList<UIVertex> verts = m_textGen.verts;
			if (verts == null || verts.Count <= 0 || m_textGen.lineCount <= 0)
			{
				m_LastPosition = Vector2.zero;
			}
			else
			{
				UILineInfo uILineInfo = m_textGen.lines[m_textGen.lineCount - 1];
				Vector3 position = verts[verts.Count - 1].position;
				position.y = uILineInfo.topY - (float)uILineInfo.height;
				position /= base.canvas.scaleFactor;
				position.x += base.rectTransform.rect.width * 0.5f;
				position.y -= base.rectTransform.rect.height * 0.5f;
				m_LastPosition = position;
			}
		}
		m_CurrentPage = pageIndex;
		return true;
	}

	public bool ToNextPage()
	{
		return SetCurrentPage(m_CurrentPage + 1);
	}

	public bool IsExistNextPage()
	{
		return m_CurrentPage < m_PageCount - 1;
	}

	public string GetCurrentPageText()
	{
		return base.text;
	}

	public static string TransTagTextToUnityText(string srcText, bool isIgnoreHideTag)
	{
		List<Syntax> syntaxList = null;
		TransTagTextToUnityText(srcText, ref syntaxList, null, isIgnoreHideTag);
		return s_strBulider.ToString();
	}

	private static void TransTagTextToUnityText(string srcText, ref List<Syntax> syntaxList, Dictionary<string, Syntax> localDefinedTag = null, bool isIgnoreHideTag = false)
	{
		if (syntaxList == null)
		{
			syntaxList = new List<Syntax>();
		}
		syntaxList.Clear();
		s_strBulider.Remove(0, s_strBulider.Length);
		int num = srcText?.Length ?? 0;
		if (num <= 0)
		{
			return;
		}
		Stack<Syntax> stack = new Stack<Syntax>();
		Syntax[] array = new Syntax[1];
		string text = srcText.Replace("\r\n", "<nl>").Replace("\n", "<nl>");
		num = text.Length;
		int num2 = 0;
		bool flag = false;
		while (num2 < num)
		{
			int num3 = text.IndexOf(c_TagBegin, num2);
			if (num3 < 0)
			{
				break;
			}
			int num4 = text.IndexOf(c_TagEnd, num3);
			if (num4 < 0)
			{
				break;
			}
			string text2 = text.Substring(num2, num3 - num2);
			if (!string.IsNullOrEmpty(text2))
			{
				Syntax syntax = new Syntax(Syntax.SyntaxType.Text, text2);
				syntax.m_position = s_strBulider.Length;
				syntaxList.Add(syntax);
				s_strBulider.Append(syntax.strTrans);
			}
			string text3 = text.Substring(num3, num4 - num3 + 1);
			Syntax syntax2 = Syntax.ParseTag(text3, localDefinedTag, s_GlobalDefinedTag);
			if (syntax2 != null)
			{
				Syntax[] array2 = null;
				if (syntax2.type == Syntax.SyntaxType.TagPack)
				{
					array2 = syntax2.GetSubSyntaxes();
				}
				else
				{
					array[0] = syntax2;
					array2 = array;
				}
				if (array2 != null)
				{
					for (int i = 0; i < array2.Length; i++)
					{
						syntax2 = array2[i];
						if (syntax2.IsPairableType())
						{
							if (syntax2.m_isFinishTag)
							{
								if (stack.Count > 0 && syntax2.IsPair(stack.Peek()))
								{
									stack.Pop();
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								stack.Push(syntax2);
							}
						}
						syntax2.m_position = s_strBulider.Length;
						syntaxList.Add(syntax2);
						s_strBulider.Append(syntax2.strTrans);
					}
				}
			}
			else
			{
				syntax2 = new Syntax(Syntax.SyntaxType.Text, text3);
				flag = true;
				syntax2.m_position = s_strBulider.Length;
				syntaxList.Add(syntax2);
				s_strBulider.Append(syntax2.strTrans);
			}
			num2 = num4 + 1;
		}
		string text4 = text.Substring(num2, num - num2);
		if (!string.IsNullOrEmpty(text4))
		{
			Syntax syntax3 = new Syntax(Syntax.SyntaxType.Text, text4);
			syntax3.m_position = s_strBulider.Length;
			syntaxList.Add(syntax3);
			s_strBulider.Append(syntax3.strTrans);
		}
		while (stack.Count > 0)
		{
			Syntax syntax4 = stack.Pop();
			Syntax syntax5 = syntax4.CreateFinishTagSyntax();
			if (syntax5 != null)
			{
				syntaxList.Add(syntax5);
				s_strBulider.Append(syntax5.strTrans);
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (Syntax syntax6 in syntaxList)
		{
			syntax6.TransToTextType();
		}
	}

	public void SetAlpha(int alpha)
	{
		alpha = Mathf.Clamp(alpha, 0, 255);
		SetAlpha((float)alpha / 255f);
	}

	public void SetAlpha(float fNormalizedAlpha)
	{
		if (m_curPageSyntaxList == null || m_curPageSyntaxList.Count <= 0)
		{
			return;
		}
		fNormalizedAlpha = Mathf.Clamp01(fNormalizedAlpha);
		if (GameGlobalUtil.IsAlmostSame(base.color.a, fNormalizedAlpha))
		{
			return;
		}
		Color color = base.color;
		color.a = fNormalizedAlpha;
		base.color = color;
		s_strBulider.Remove(0, s_strBulider.Length);
		foreach (Syntax curPageSyntax in m_curPageSyntaxList)
		{
			if (curPageSyntax.type != Syntax.SyntaxType.Color || curPageSyntax.m_isFinishTag)
			{
				s_strBulider.Append(curPageSyntax.strTrans);
			}
			else
			{
				s_strBulider.Append(curPageSyntax.GetNewColorStr_ByAlpha(fNormalizedAlpha));
			}
		}
		base.text = s_strBulider.ToString();
	}
}
