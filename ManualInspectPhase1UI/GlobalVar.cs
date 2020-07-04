using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ManualInspectPhase1UI
{
    #region TestStructureDetails
    public struct STRUCT_TEST
    {
        public uint TestId;
        public uint Test_Num;
        public float MinimumVal;
        public float MaximumVal;
        public float TypicalVal;
        public string UnitName;
        public string[] ChannelName;
        public uint ComponentId;
        public string Description;
        public string Comments;
        public Image TestImage;
        public DateTime Date_Time;
    } 
    #endregion
    public struct STRUCT_COMPONENT
    {
        public uint ComponentId;
        public string ComponentName;
        public string ComponentCode;
        public Image ComponentImage;
        public int TestLength;
    }
    public struct STRUCT_REPORT
    {
        public uint TestId;
        public string ChannelName;
        public string MeasuredVal;
        public string UnitName;
        public int ComponentNum;
        public bool TestResult;
        public bool IsManualEntry;
    }
    
    public class GlobalVar
    {
        public static int BAUDRATE_BLUETOOTH = 9600;
        public static int BAUDRATE_BARCODE = 9600;
        public static int BAUDRATE_EMBEDDED_BENCH = 38400; 

        public enum BARCODE_MODE
        {
            BARCODE_MANUAL = 0,
            BARCODE_SCAN = 1,
            BARCODE_DISABLE = 2
        }

        public static List<STRUCT_COMPONENT> STRUCT_COMPONENT_LIST = new List<STRUCT_COMPONENT>();
        public static List<STRUCT_TEST> STRUCT_TEST_LIST = new List<STRUCT_TEST>();
        public static List<string> BATCH_LIST = new List<string>();

        public static int BENCH_BAUDRATE = 115200;
        public static int BLUETOOTH_BAUDRATE = 9600;

        public static STRUCT_COMPONENT gSelectedComponent = new STRUCT_COMPONENT();

        public static bool isUserLogOut = false;
        public static string USER_NAME = string.Empty;
        public static int MATS_SERVER_LOGIN_ATTEMPTS = 3;

        public static bool IS_FORM_LOADED = false;

        public static Color COLOR_TSMI_SELECT = Color.DarkOrange;

        public static STRUCT_REPORT STRUCT_REPORT = new STRUCT_REPORT();

        public static STRUCT_TEST SELECTED_STRUCT_TEST = new STRUCT_TEST();
        public static uint COMPONENT_ID = 0;
        public static int TABLE_ROW_INDEX = 0;
        public static bool TEST_STARTED_FLAG = false;
        public static int COMPONENT_NUMBER = 0;
        public static bool IS_BTN_START_CLICKED = false;


        public static bool PLEASE_WAIT_ENABLED = false;

        public const string Btn_Save = "Set";//"Save";
        public const string Btn_Update = "Update";
        public const string Btn_Start = "Start";
        public const string Btn_Stop = "Stop";
        public const string Btn_Next = "Next";
        public const string Btn_Change = "Change";
        public const string Btn_Modify = "Modify";
        public const string Btn_Select = "Select";

        //public static string SELECTED_TAB_PAGE_NAME = "tabPHome";
        public static BARCODE_MODE BARCODE_STATUS = BARCODE_MODE.BARCODE_MANUAL;

        public static string SERVER_PORT = "9876";
        public static string SERVER_IP_ADDRESS = "192.168.43.202";
        public const int BENCH_BAUD_RATE = 115200;
        public const int BLUETOOTH_BAUD_RATE = 115200;

        public static int BARCODE_BATCH_START_INDEX = 11;
        public static int BARCODE_DEVICENUM_START_INDEX = 15;
        public static int BARCODE_YEAR_STATRT_INDEX = 0;
        public static int BARCODE_CODE_STATRT_INDEX = 2;

        public static int TOTAL_COMPONENT_COUNT = 0;

        public static int DeviceNumber = 0;
        public static string SERVER_TERMINATOR = "\r\n    }\r\n  }\r\n}";

        public static string BENCH_NAME = "MNL_INSPECT";
        public static string SLAVE_ID = "0";
        public static string UserId = "0";
        //Log File Name
        public static string LOG_NAME = "Log Error.log";

        public float MINIMUM_VALUE = 0;
        public float MAXIMUM_VALUE = 0;
        public float TYPICAL_VALUE = 0;
        public string UNIT_VALUE = "0";
        public string[] CHANNEL_NAME =null; 
            
        //public const string STATE_POWER_ON = "0";
        //public const string STATE_CONNECTING_TO_SERVER = "1";
        //public const string STATE_CONNECTED_TO_SERVER = "2";
        //public const string STATE_USER_AUTHENTICATING = "3";
        //public const string STATE_USER_AUTHENTICATION_OK = "4";
        //public const string STATE_TOP_MENU = "5";
        //public const string STATE_MANUAL_SELECTION = "6";
        //public const string STATE_BARCODE_ENTRY = "7";
        //public const string STATE_BARCODE_SCANNING = "8";
        //public const string STATE_GET_COMP_LIST = "9";
        //public const string STATE_GET_BATCH_NUM_LIST = "10";
        //public const string STATE_GET_TEST_LIST = "11";
        //public const string STATE_CONFIG_TEST_PARAMS = "12";
        //public const string STATE_CONFIGURED = "13";
        //public const string STATE_GET_DEVICE_NUM = "14";
        //public const string STATE_READY_FOR_TESTS = "15";
        //public const string STATE_RUN_MODE = "16";
        //public const string STATE_TEST_MENU = "17";
        //public const string STATE_HOME_BUTTON = "18";
        //public const string STATE_LOGGED_OUT = "19";
        //public const string STATE_POWER_OFF = "20";
        //public const string STATE_FAIL_CONNECT = "21";
        //public const string STATE_WAIT_INFO_ACK = "ACK";
        //public const string STATE_INFO_VERIFIED = "VRF";

        public static bool gIsSupervisor = false;

        //public const string CMD_STATE = "STATE";
        //public const string CMD_CONNECT = "CONNECT";
        //public const string CMD_UAUTH = "UAUTH";
        //public const string CMD_GET_COMP = "GET_COMP";
        //public const string CMD_GET_BATCH = "GET_BATCH";
        //public const string CMD_BARCODE = "BARCODE";
        //public const string CMD_GET_TEST = "GET_TEST";
        //public const string CMD_GET_DEVNUM = "GET_DEVNUM";
        //public const string CMD_REPORT = "REPORT";
        //public const string CMD_HB = "HB";
        //public const string CMD_START_TEST = "TEST_START";
        //public const string CMD_END_TEST = "TEST_END";
        //public const string CMD_ABORT_TEST = "TEST_ABORT";
        //public const string CMD_RE_TEST = "RE_TEST";
        //public const string CMD_USER = "USER";

        public static int COMPONENT_START_INDEX = 0;
        public static int COMPONENT_DISPLAY_COUNT = 15;
        public static int COMPONENT_TOTAL_COUNT = 15;
        public static bool COMPONENT_SORT = false;

        public static bool IS_TYPE_LOT = false;

        public static string LAST_SCANNED_BARCODE_VAL = string.Empty;

        public static int TEST_DELAY = 0;

        public enum OBJECT_DATA_TYPE
        {
            STRING_VAL = 0,
            JSON_OBJECT = 1,
            BOOLEAN_VAL = 2,
            INT_VAL = 3,
            FLOAT_VAL = 4,
            JSON_ARRAY = 5
        }
      
        public enum TEST_MENU_STATUS
        {
            SELECT_COMPONENT = 0,
            BARCODE_SCAN =1,
            BARCODE_MANUAL = 2
        }
    }
}
