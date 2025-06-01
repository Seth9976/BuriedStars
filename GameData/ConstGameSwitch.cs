namespace GameData;

public static class ConstGameSwitch
{
	public enum eSTARTTYPE
	{
		NEW,
		CONTINUE,
		RESTART
	}

	public enum eSELECT
	{
		ERR = -1,
		NONE,
		LEFT,
		RIGHT,
		TIME_OVER,
		CNT
	}

	public enum eSELECT_TYPE
	{
		NORMAL,
		KEYWORD
	}

	public enum eCHAR_PARTY
	{
		CHR_P_OFF,
		CHR_P_ON,
		CHR_P_DIE
	}

	public enum eKeywordCategory
	{
		Personal,
		Fater,
		Event,
		Cnt
	}

	public enum eSWITCH_STATE
	{
		OFF = 0,
		ON = 1,
		READ = 2,
		CLOSE = 3,
		COUNT = 4,
		NONE = -1
	}

	public enum eKeywordUsing
	{
		NONE,
		NO_CHANGE,
		UP,
		DOWN,
		EXIST_KEYWORD_SELECT,
		COUNT
	}

	public enum eMental
	{
		HIGH,
		NORMAL,
		LOW
	}

	public enum eCallState
	{
		DONOTCALL,
		CALLABLE,
		CALLING,
		DIE,
		ALREADY_CALLED
	}

	public enum eVoiceLang
	{
		KOR = 0,
		JPN = 2
	}

	public const int NONE_IDX = -1;

	public const float CHAR_POS_NONE = 999f;

	public const int MAX_CNT_FOLLOW = 999;

	public const byte MIN_VALUE_CHR_RELATION = 0;

	public const byte MAX_VALUE_CHR_RELATION = 100;

	public static int MAX_MENTAL_POINT = 100;

	public static int MIN_MENTAL_POINT = 0;

	public static int MAX_RETWEET_CNT = 9999;

	public static int MIN_RETWEET_CNT = 0;

	public const int COUNT_SWITCH_CHAR = 8;

	public const int STR_LEN_CHAR_MOT = 40;

	public const int ARRLEN_SWITCH_CHAR_ANONYMOUS = 2;

	public const int RELATION_CONTI_SWITCH_CNT = 20;

	public const int COUNT_CHAR_RELATION_CONTI = 160;

	public const int ARRLEN_SWITCH_CHAR_RELATION_CONTI = 21;

	public const int ARRLEN_SWITCH_CHAR_PARTY = 3;

	public const int ARRLEN_STRING_CHAR_PARTY_MOT = 320;

	public static int COUNT_SWITCH_EVENT = 5000;

	public static int ARRLEN_SWITCH_EVENT = COUNT_SWITCH_EVENT / 8 + 1;

	public static int COUNT_SWITCH_SELECT = 1000;

	public static int ARRLEN_SWITCH_SELECT = COUNT_SWITCH_SELECT / 8 + 1;

	public static int COUNT_INVEST_OBJ = 20;

	public static int COUNT_INVEST_OBJ_MAX_STR_LEN = 100;

	public static int COUNT_MESSENGER_MESSAGE = 2000;

	public static int COUNT_MESSENGER_ORDER = 1500;

	public static int COUNT_SWITCH_SNS_POST = 2500;

	public static int ARRLEN_BIT_SWITCH_SNS_POST = COUNT_SWITCH_SNS_POST / 8 + 1;

	public static int ARRLEN_2BIT_SWITCH_SNS_POST = COUNT_SWITCH_SNS_POST / 4 + 1;

	public static int COUNT_SEQUENCE_LIST = 200;

	public static int COUNT_KEYWORD_GROUP_CNT = 250;

	public static int ARRLEN_BIT_SWITCH_KEYWORD_GROUP = COUNT_KEYWORD_GROUP_CNT / 8 + 1;

	public static int COUNT_REPLY_GROUP_CNT = 250;

	public static int ARRLEN_BIT_SWITCH_REPLY_GROUP = COUNT_REPLY_GROUP_CNT / 8 + 1;

	public static int COUNT_KEYWORD_BY_SEQ = 50;

	public static int COUNT_KEYWORD_ALL_COUNT = 1000;

	public static int ARRLEN_BIT_SWITCH_KEYWORD_ALL = COUNT_KEYWORD_ALL_COUNT / 8 + 1;

	public static int ARRLEN_2BIT_SWITCH_KEYWORD_ALL = COUNT_KEYWORD_ALL_COUNT / 4 + 1;

	public static int COUNT_KEYWORD_USE_CHAR_CNT = 5;

	public static int COUNT_KEYWORD_USE_ALL_CNT = COUNT_KEYWORD_ALL_COUNT * COUNT_KEYWORD_USE_CHAR_CNT;

	public static int ARRLEN_2BIT_SWITCH_KEYWORD_USE = COUNT_KEYWORD_USE_ALL_CNT / 4 + 1;

	public static int ARRLEN_BIT_SWITCH_KEYWORD_USE = COUNT_KEYWORD_USE_ALL_CNT / 8 + 1;

	public static int COUNT_TUTORIAL_CNT = 50;

	public static int ARRLEN_BIT_SWITCH_TUTORIAL = COUNT_TUTORIAL_CNT / 8 + 1;

	public static int COUNT_KEYWORD_SEQ_STATE = COUNT_SEQUENCE_LIST * COUNT_KEYWORD_BY_SEQ + COUNT_KEYWORD_ALL_COUNT;

	public static int ARRLEN_BIT_SWITCH_KEYWORD_SEQ = COUNT_KEYWORD_SEQ_STATE / 8 + 1;

	public static int ARRLEN_2BIT_SWITCH_KEYWORD_SEQ = COUNT_KEYWORD_SEQ_STATE / 4 + 1;

	public static int COUNT_KEYWORD_SEQ_BY_CHAR = COUNT_KEYWORD_SEQ_STATE * 8;

	public static int ARRLEN_BIT_SWITCH_KEYWORD_SEQ_BY_CHAR = COUNT_KEYWORD_SEQ_BY_CHAR / 8 + 1;

	public static int COUNT_CUT_MAX = 600;

	public static int COUNT_MAX_MUST_GET_CUT_KEYWORD = 30;

	public static int ARRLEN_BIT_SWITCH_MUST_GET_CUT_KEYWORD = COUNT_MAX_MUST_GET_CUT_KEYWORD / 8 + 1;

	public static int COUNT_CUT_CHAR_CALL_MAX = COUNT_CUT_MAX * 8;

	public static int COUNT_CHAR_PROFILE = 800;

	public static int ARRLEN_2BIT_SWITCH_CHAR_PROFILE = COUNT_CHAR_PROFILE / 4 + 1;

	public static int COUNT_COLL_IMAGE = 300;

	public static int ARRLEN_2BIT_SWITCH_COLL_IMAGE = COUNT_COLL_IMAGE / 4 + 1;

	public static int COUNT_COLL_SOUND = 300;

	public static int ARRLEN_2BIT_SWITCH_COLL_SOUND = COUNT_COLL_SOUND / 4 + 1;

	public static int COUNT_MAX_VOTES_CNT = 99999999;

	public static int COUNT_MIN_VOTES_CNT = 0;

	public static int COUNT_MAX_TROPHY_CNT = 100;

	public static int ARRLEN_2BIT_SWITCH_TROPHY = COUNT_MAX_TROPHY_CNT / 4 + 1;

	public static int COUNT_MAX_CHAR_POS_CNT = 7;

	public static int COUNT_MAX_CHAR_SIZE_CNT = 5;

	public static int COUNT_MAX_1_CHAR_POS_XLS_CNT = COUNT_MAX_CHAR_POS_CNT * COUNT_MAX_CHAR_SIZE_CNT;

	public static int COUNT_MAX_CHAR_POS_X_ALL_CNT = 8 * COUNT_MAX_CHAR_POS_CNT * COUNT_MAX_CHAR_SIZE_CNT;

	public static int COUNT_MAX_CHAR_POS_Y_ALL_CNT = 8 * COUNT_MAX_CHAR_SIZE_CNT;

	public static string XLS_KEY_CATE_PRE_NAME = "KEYWORD_CATEGORY_";

	public const int MAX_FREQUENCY_NUM = 100;
}
