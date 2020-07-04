using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Network;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using MATS_Server;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ManualInspectPhase1UI
{
    public class ServerProtocol
    {
        private Client client;
        private Queue<string> LastReceivedDataQueue = new Queue<string>();
        private bool isBenchSocketConnected = false;
        Thread clientReceiveThread;

        public const string USER_ACCESS_ALL_TABLES = "ALL";
        public const string USER_ACCESS_BATCH = "BATCH";
        public const string USER_ACCESS_BENCH = "BENCH";
        public const string USER_ACCESS_COMPONENT = "COMPONENT";
        public const string USER_ACCESS_COMPONENT_TEST = "TEST";
        public const string USER_ACCESS_USER = "USER";
        public const string USER_ACCESS_INVENTORY = "INVENTORY";
        public const string USER_ACCESS_CHANNEL = "CHANNEL";
        public const string USER_ACCESS_VENDOR = "VENDOR";

        private const string STATE_POWER_ON = "0";
        private const string STATE_CONNECTING_TO_SERVER = "1";
        private const string STATE_CONNECTED_TO_SERVER = "2";
        private const string STATE_USER_AUTHENTICATING = "3";
        private const string STATE_USER_AUTHENTICATION_OK = "4";
        private const string STATE_TOP_MENU = "5";
        private const string STATE_MANUAL_SELECTION = "6";
        private const string STATE_BARCODE_ENTRY = "7";
        private const string STATE_BARCODE_SCANNING = "8";
        private const string STATE_GET_COMP_LIST = "9";
        private const string STATE_GET_BATCH_NUM_LIST = "10";
        private const string STATE_GET_TEST_LIST = "11";
        private const string STATE_CONFIG_TEST_PARAMS = "12";
        private const string STATE_CONFIGURED = "13";
        private const string STATE_GET_DEVICE_NUM = "14";
        private const string STATE_READY_FOR_TESTS = "15";
        private const string STATE_RUN_MODE = "16";
        private const string STATE_TEST_MENU = "17";
        private const string STATE_HOME_BUTTON = "18";
        private const string STATE_LOGGED_OUT = "19";
        private const string STATE_POWER_OFF = "20";
        private const string STATE_FAIL_CONNECT = "21";
        private const string STATE_WAIT_INFO_ACK = "ACK";
        private const string STATE_INFO_VERIFIED = "VRF";
        private const string STATE_GET_TEST_ALL = "22";
        private const string STATE_GET_COMP_IMG = "23";
        private const string STATE_GET_TEST_IMG = "24";
        private const string STATE_GET_VENDOR_LIST = "25";
        private const string STATE_SAVE_BATCH = "26";
        private const string STATE_BATCH_STATUS = "27";
        private const string STATE_GET_INVENTORY = "28";
        private const string STATE_GET_COMPONENT_COMPLETE = "29";
        private const string STATE_DELETE_BATCH = "30";
        private const string STATE_GET_BARCODE_GENERATED_CNT = "31";
        private const string STATE_GET_USER_ACCESS_LEVEL = "32";
        private const string STATE_GET_CHANNEL = "33";

        private const string CMD_STATE = "STATE";
        private const string CMD_CONNECT = "CONNECT";
        private const string CMD_UAUTH = "UAUTH";
        private const string CMD_GET_COMP = "GET_COMP";
        private const string CMD_GET_BATCH = "GET_BATCH";
        private const string CMD_BARCODE = "BARCODE";
        private const string CMD_GET_TEST = "GET_TEST";
        private const string CMD_GET_DEVNUM = "GET_DEVNUM";
        private const string CMD_REPORT = "REPORT";
        private const string CMD_HB = "HB";
        public const string CMD_START_TEST = "TEST_START";
        public const string CMD_END_TEST = "TEST_END";
        public const string CMD_ABORT_TEST = "TEST_ABORT";
        private const string CMD_RE_TEST = "RE_TEST";
        private const string CMD_USER = "USER";
        private const string CMD_CHANNEL = "CHANNEL";
        private const string CMD_COMP = "COMPONENT";
        public ServerProtocol()
        {
            client = new Client(GlobalVar.SERVER_IP_ADDRESS, GlobalVar.SERVER_PORT);
            if(client.Connected)
            {
                isBenchSocketConnected = true;
                clientReceiveThread = new Thread(() => ReceivedData(GlobalVar.SERVER_TERMINATOR));
                clientReceiveThread.Start();
            }
        }
        public void Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();
                    isBenchSocketConnected = false;
                    clientReceiveThread.Abort();
                }
            }
            catch(Exception)
            {
            }
        }

        public string ParseReceivedData(bool isResponseCheck,string dataSent,int loopCount)
        {
            string data = string.Empty;
            int count = 0;
            while (data != null || count++ != loopCount)//loopCount 200
            {
                if (count != 0)
                {
                    Thread.Sleep(250);
                }
                if(!isBenchSocketConnected || !client.Connected)
                {
                    return string.Empty;
                }
                data = LastReceivedData();
                if (data != null && data != string.Empty)
                {
                    try
                    {
                        //Console.WriteLine("received Data: " + data);
                        JObject jsonObject = JObject.Parse(data);
                        //Thread.Sleep(100);
                        JObject requestHeader = (JObject)jsonObject["requested_header"];
                        if (isResponseCheck && Utility.CheckForJsonKeyPresence(jsonObject, "response") && requestHeader["requested_data"].ToString() == dataSent)
                        {
                            isResponseCheck = false;
                        }
                        else
                        {
                            JObject request = (JObject)jsonObject["request"];
                            JObject jsonObj = (JObject)request["data"];
                            if (Utility.CheckForJsonKeyPresence(jsonObj, "HB"))
                            {
                                HeartBeatResponse();
                            }
                            continue;
                        }
                        JObject response = (JObject)jsonObject["response"];
                        string responseMsg = response["response_msg"]["text"].ToString();
                        return Convert.ToString(response["data"]);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Error Msg: " + data);
                        continue;
                    }
                }
            }
            return string.Empty;
        }
        public void GetUserAccess()
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_USER_ACCESS_LEVEL;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                client.Send(Utility.CreateJsonObject(CMD_USER, Convert.ToString(jsonObject)));
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
                GlobalVar.gIsSupervisor = false;
                //GlobalVar.gIsAddBatchAllowed = false;
                if (data != string.Empty)
                {
                    JArray jsonArr = JArray.Parse(data);
                    GlobalVar.USER_NAME = jsonArr[0].ToString();
                    for (int i = 1; i < jsonArr.Count; i++)
                    {
                        switch (Convert.ToString(jsonArr[i]))
                        {
                            case USER_ACCESS_ALL_TABLES:
                                //GlobalVar.gIsAddBatchAllowed = true;
                                GlobalVar.gIsSupervisor = true;
                                break;
                            case USER_ACCESS_BATCH:
                                //GlobalVar.gIsAddBatchAllowed = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public List<STRUCT_TEST> GetTestDetails(int componentId)
        {
            try
            {
                string startTime = DateTime.Now.ToString();
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_CONFIG_TEST_PARAMS;

                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                jsonObject["comp_id"] = componentId;
                client.Send(Utility.CreateJsonObject(CMD_GET_TEST,Convert.ToString(jsonObject)));
                //Thread.Sleep(GlobalVar.TEST_DELAY);
                string data = ParseReceivedData(true,Convert.ToString(jsonObject),200);
                Thread.Sleep(10);
                if(data != string.Empty)
                {
                    List<STRUCT_TEST> structTestList = new List<STRUCT_TEST>();
                    JArray jsonArray = JArray.Parse(data);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        Thread.Sleep(5);
                        JObject jsonTestObject = JObject.Parse(Convert.ToString(jsonArray[i]));
                        STRUCT_TEST structTest = new STRUCT_TEST();
                        structTest.TestId = (uint)jsonTestObject["id"];
                        structTest.Test_Num = (uint)jsonTestObject["test_no"];
                        structTest.MinimumVal = (float)jsonTestObject["min_val"];
                        structTest.MaximumVal = (float)jsonTestObject["max_val"];
                        structTest.TypicalVal = (float)jsonTestObject["avg_val"];
                        structTest.UnitName = (string)jsonTestObject["unit"];
                        structTest.Date_Time = (DateTime)jsonTestObject["date"];
                        structTest.Description = (string)jsonTestObject["description"];
                        structTest.Comments = (string)jsonTestObject["comments"];

                        var value = jsonTestObject.GetValue("image");
                        structTest.TestImage = Utility.ByteArrayToImage((byte[])value);
                        JArray channelName = JArray.Parse(jsonTestObject["name"].ToString());
                        structTest.ChannelName = new string[channelName.Count];
                        for (int j = 0; j < channelName.Count; j++)
                        {
                            structTest.ChannelName[j] = (string)channelName[j];
                        }
                        structTestList.Add(structTest);
                    }
                    string endTime = DateTime.Now.ToString();
                    return structTestList;
                }
                else
                {
                }
            }
            catch(Exception) { }
            return new List<STRUCT_TEST>();
        }

        public STRUCT_COMPONENT GetBarcode(string barcodeVal,bool isManualEntry)
        {
            try
            {
                if(isManualEntry)
                {
                    JObject jsonState = new JObject();
                    jsonState["state"] = STATE_BARCODE_ENTRY;
                    client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                }
                else
                {
                    JObject jsonState = new JObject();
                    jsonState["state"] = STATE_BARCODE_SCANNING;
                    client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                }
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                jsonObject["barcode"] = barcodeVal;
                client.Send(Utility.CreateJsonObject(CMD_BARCODE, Convert.ToString(jsonObject)));
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
                if(data != string.Empty)
                {
                    JObject componentData = JObject.Parse(data);
                    STRUCT_COMPONENT structComponent = new STRUCT_COMPONENT();
                    structComponent.ComponentName = componentData["name"].ToString();
                    structComponent.ComponentCode = componentData["code"].ToString();
                    structComponent.ComponentId = (uint)componentData["id"];
                    return structComponent;
                }
                return new STRUCT_COMPONENT();
            }
            catch(Exception)
            {
                return new STRUCT_COMPONENT();
            }
        }

        public List<string> GetBatch(uint componentId)
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_BATCH_NUM_LIST;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                jsonObject["comp_id"] = componentId;
                client.Send(Utility.CreateJsonObject(CMD_GET_BATCH, Convert.ToString(jsonObject)));
                Thread.Sleep(1000);
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
                if (data != string.Empty)
                {
                    JArray jsonBatchArray = JArray.Parse(data);
                    List<string> batchList = new List<string>();
                    for (int i = 0; i < jsonBatchArray.Count; i++)
                    {
                        batchList.Add((string)jsonBatchArray[i]["batch_no"]);
                    }
                    return batchList;
                }
            }
            catch(Exception){ }
            return new List<string>();
        }

        public List<string> GetChannel()
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_CHANNEL;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                client.Send(Utility.CreateJsonObject(CMD_CHANNEL, Convert.ToString(jsonObject)));
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
                
                List<string> channelName = new List<string>();
                if (data != string.Empty)
                {
                    JArray jsonArray = JArray.Parse(data);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JObject componentData = JObject.Parse(jsonArray[i].ToString());
                        channelName.Add(componentData["name"].ToString());
                    }
                    return channelName;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch(Exception)
            {
                return new List<string>();
            }
        }

        public List<STRUCT_COMPONENT> GetComponent()
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_COMP_LIST;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                client.Send(Utility.CreateJsonObject(CMD_GET_COMP, Convert.ToString(jsonObject)));
                //Thread.Sleep(2000);
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),200);
                
                List<STRUCT_COMPONENT> structComponentList = new List<STRUCT_COMPONENT>();
                if (data != string.Empty)
                {
                    JArray jsonArray = JArray.Parse(data);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JObject componentData = JObject.Parse(jsonArray[i].ToString());
                        STRUCT_COMPONENT structComponent = new STRUCT_COMPONENT();
                        structComponent.ComponentName = componentData["name"].ToString();
                        structComponent.ComponentCode = componentData["code"].ToString();
                        structComponent.ComponentId = (uint)componentData["id"];
                        structComponent.ComponentImage = Utility.ByteArrayToImage((byte[])(componentData["image"]));
                        structComponentList.Add(structComponent);
                    }
                    return structComponentList;
                }
            }
            catch(Exception ex)
            {
            }
            return new List<STRUCT_COMPONENT>();
        }
        public int SetDeviceNumber(string batch_no, int device_no, bool isBarcode)
        {
            string batchNumber = batch_no;
            if (isBarcode)
            {
                batchNumber = GetBarcodeBatchNum(batch_no);
                GlobalVar.IS_TYPE_LOT = BarcodeTypeLotCheck(batch_no);
            }
            JObject jsonState = new JObject();
            jsonState["state"] = STATE_GET_DEVICE_NUM;
            client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
            Thread.Sleep(10);
            JObject jsonObject = new JObject();
            jsonObject["batch_no"] = batchNumber;
            jsonObject["device_no"] = device_no;
            client.Send(Utility.CreateJsonObject(CMD_GET_DEVNUM, Convert.ToString(jsonObject)));
            string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
            if (data != string.Empty)
            {
                return int.Parse(data);
            }
            return -1;
        }
        public List<STRUCT_COMPONENT> GetComponent(int start_idx,int display_cnt,bool isSort)
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_COMP_LIST;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                jsonObject["start_idx"] = start_idx;
                jsonObject["display_cnt"] = display_cnt;
                jsonObject["is_asc"] = isSort;
                client.Send(Utility.CreateJsonObject(CMD_GET_COMP, Convert.ToString(jsonObject)));
                //Thread.Sleep(2000);
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),200);

                List<STRUCT_COMPONENT> structComponentList = new List<STRUCT_COMPONENT>();
                if (data != string.Empty)
                {
                    JArray jsonArray = JArray.Parse(data);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JObject componentData = JObject.Parse(jsonArray[i].ToString());
                        STRUCT_COMPONENT structComponent = new STRUCT_COMPONENT();
                        structComponent.ComponentName = componentData["name"].ToString();
                        structComponent.ComponentCode = componentData["code"].ToString();
                        structComponent.ComponentId = (uint)componentData["id"];
                        structComponent.ComponentImage = Utility.ByteArrayToImage((byte[])(componentData["image"]));
                        structComponent.TestLength = (int)componentData["test_len"];
                        if (Utility.CheckForJsonKeyPresence(componentData, "comp_start_count"))
                        {
                            GlobalVar.COMPONENT_START_INDEX = (int)componentData["comp_start_count"];
                        }
                        structComponentList.Add(structComponent);
                    }
                    return structComponentList;
                }
            }
            catch (Exception ex)
            {
            }
            return new List<STRUCT_COMPONENT>();
        }

        //public List<STRUCT_COMPONENT> GetComponent()
        //{
        //    try
        //    {
        //        JObject jsonState = new JObject();
        //        jsonState["state"] = STATE_GET_COMP_LIST;
        //        client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
        //        Thread.Sleep(10);
        //        JObject jsonObject = new JObject();
        //        client.Send(Utility.CreateJsonObject(CMD_GET_COMP, Convert.ToString(jsonObject)));
        //        //Thread.Sleep(2000);
        //        string data = ParseReceivedData(true, Convert.ToString(jsonObject));

        //        List<STRUCT_COMPONENT> structComponentList = new List<STRUCT_COMPONENT>();
        //        if (data != string.Empty)
        //        {
        //            JArray jsonArray = JArray.Parse(data);
        //            for (int i = 0; i < jsonArray.Count; i++)
        //            {
        //                JObject componentData = JObject.Parse(jsonArray[i].ToString());
        //                STRUCT_COMPONENT structComponent = new STRUCT_COMPONENT();
        //                structComponent.ComponentName = componentData["name"].ToString();
        //                structComponent.ComponentCode = componentData["code"].ToString();
        //                structComponent.ComponentId = (uint)componentData["id"];
        //                structComponent.ComponentImage = Utility.ByteArrayToImage((byte[])(componentData["image"]));
        //                structComponentList.Add(structComponent);
        //            }
        //            return structComponentList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return new List<STRUCT_COMPONENT>();
        //}

        public void HeartBeatResponse()
        {
            JObject jsonObj = new JObject();
            string data = Utility.CreateJsonObject(CMD_HB,Convert.ToString(jsonObj));
            client.Send(data);
        }

        public string GetBarcodeBatchNum(string barcode)
        {
            int startId = GlobalVar.BARCODE_BATCH_START_INDEX, endId = GlobalVar.BARCODE_DEVICENUM_START_INDEX;
            string batchNum = "";
            for (int i = startId; i < endId; i++)
            {
                batchNum += barcode[i];
            }
            return (batchNum);
        }

        public bool BarcodeTypeLotCheck(string barcode)
        {
            int startId = GlobalVar.BARCODE_DEVICENUM_START_INDEX, endId = 20;
            string batchNum = "0";
            for (int i = startId; i < endId; i++)
            {
                batchNum += barcode[i];
            }
            return (int.Parse(batchNum )== 0);
        }

        public int GetDeviceNumber(string batch_no,bool isBarcode)
        {
            string batchNumber = batch_no;
            if(isBarcode)
            {
                batchNumber = GetBarcodeBatchNum(batch_no);
                GlobalVar.IS_TYPE_LOT = BarcodeTypeLotCheck(batch_no);
            }
            JObject jsonState = new JObject();
            jsonState["state"] = STATE_GET_DEVICE_NUM;
            client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
            Thread.Sleep(10);
            JObject jsonObject = new JObject();
            jsonObject["batch_no"] = batchNumber;
            client.Send(Utility.CreateJsonObject(CMD_GET_DEVNUM, Convert.ToString(jsonObject)));
            string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
            if (data != string.Empty)
            {
                return int.Parse(data);
            }
            return -1;
        }

        public void SendState(string state)
        {
            switch(state)
            {
                case STATE_RUN_MODE:
                    JObject jsonState = new JObject();
                    jsonState["state"] = STATE_RUN_MODE;
                    client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                    break;
            }
        }

        public void SendStartEndAbortTestStatus(string command)
        {
            JObject jsonObj = new JObject();
            jsonObj["dev_num"] = GlobalVar.COMPONENT_NUMBER;
            string sendData = Utility.CreateJsonObject(command, Convert.ToString(jsonObj));
            client.Send(sendData);
        }

        public void SendReport(List<string> channelName,List<string> measuredVal,List<string> unit,STRUCT_REPORT structReport)
        {
            JObject jsonObject = new JObject();
            JArray jsonChannelArray = new JArray();
            for(int i = 0;i<channelName.Count;i++)
            {
                JObject jChannelDetails = new JObject();
                jChannelDetails["name"] = channelName[i];  
                jChannelDetails["value"] = measuredVal[i];
                jChannelDetails["unit"] = unit[i];
                jChannelDetails["test_id"] = structReport.TestId;
                jChannelDetails["is_manual"] = structReport.IsManualEntry;
                jChannelDetails["ch_result"] = structReport.TestResult;
                jsonChannelArray.Add(jChannelDetails);
            }
            //jsonObject["test_id"] = structReport.TestId;
            jsonObject["dev_num"] = structReport.ComponentNum;
            jsonObject["channel"] = jsonChannelArray;
            jsonObject["test_result"] = structReport.TestResult;
            jsonObject["ch_result"] = structReport.TestResult;
            JObject jsonState = new JObject();
            jsonState["state"] = STATE_RUN_MODE;
            client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
            Thread.Sleep(10);
            client.Send(Utility.CreateJsonObject(CMD_REPORT, Convert.ToString(jsonObject)));
            //string data = ParseReceivedData(true, Convert.ToString(jsonObject));
            //if (data != string.Empty)
            //{

            //}
        }

        public bool AuthenticateUser(string username,string password)
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_USER_AUTHENTICATING;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                JObject jsonObject = new JObject();
                jsonObject["name"] = username;
                jsonObject["password"] = password;
                client.Send(Utility.CreateJsonObject(CMD_UAUTH, Convert.ToString(jsonObject)));
                string data = ParseReceivedData(true, Convert.ToString(jsonObject),50);
                if(data == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool CheckForServerConnectivity()
        {
            return isBenchSocketConnected;
        }

        public bool ConnectToServer()
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_CONNECTING_TO_SERVER;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                client.Send(Utility.CreateJsonObject(CMD_CONNECT, "{}"));
                string data = ParseReceivedData(true, "{}",25);
                if(data == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }
        private string LastReceivedData ()
        {
            if(LastReceivedDataQueue.Count > 0)
            {
                return LastReceivedDataQueue.Dequeue();
            }
            return null;
        }
        private void ReceivedData(string compare)
        {
            while(isBenchSocketConnected)
            {
                string data = client.Receive(compare);
                if (data == null)
                {
                    isBenchSocketConnected = false;
                    MessageBox.Show("Could Not Connect to Server. Please Check the Network Connection and Try Again","Unable to Reach the Server");
                    Environment.Exit(Environment.ExitCode);
                    Application.Exit();
                    return;
                }
                string[] receivedData = Utility.jSonTokeniser(data);
                for (int i = 0; i < receivedData.Length; i++)
                {
                    string dataReceived = receivedData[i];
                    try
                    {
                        JObject jsonObject = JObject.Parse(dataReceived);
                        if (dataReceived.Contains("request") && dataReceived.Contains("HB") && Utility.CheckForJsonKeyPresence(jsonObject,"request"))
                        {
                            JObject request = (JObject)jsonObject["request"];
                            JObject jsonObj = (JObject)request["data"];
                            if (Utility.CheckForJsonKeyPresence(jsonObj, "HB"))
                            {
                                HeartBeatResponse();
                            }
                        }
                        //Console.WriteLine("RECEIVED DATA: " + Environment.NewLine + dataReceived);
                        LastReceivedDataQueue.Enqueue(dataReceived);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
            
        }

        public JArray GetTreeViewOfComponent(string component_name, bool isAssembledView)
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_COMPONENT_COMPLETE;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                //Thread.Sleep(10);
                JObject jsonObj = new JObject();
                jsonObj["search_val"] = component_name;
                jsonObj["is_assembled_view"] = isAssembledView;
                client.Send(Utility.CreateJsonObject(CMD_COMP, Convert.ToString(jsonObj)));
                //Thread.Sleep(10);
                string data = ParseReceivedData(true, Convert.ToString(jsonObj),100);
                //List<STRUCT_COMPONENT> structComponentList = new List<STRUCT_COMPONENT>();
                if (data != string.Empty)
                {
                    JArray jsonArray = JArray.Parse(data);
                    GlobalVar.TOTAL_COMPONENT_COUNT = jsonArray.Count;
                    return jsonArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXception:{0}", ex.Message);
            }
            return new JArray();
        }


        public List<STRUCT_COMPONENT> GetComponent(string searchVal, int start_idx, int display_cnt)
        {
            try
            {
                JObject jsonState = new JObject();
                jsonState["state"] = STATE_GET_COMP_LIST;
                client.Send(Utility.CreateJsonObject(CMD_STATE, Convert.ToString(jsonState)));
                Thread.Sleep(10);
                JObject jsonObject = new JObject();
                jsonObject["start_idx"] = start_idx;
                jsonObject["display_cnt"] = display_cnt;
                jsonObject["search_val"] = searchVal;
                //jsonObject["is_asc"] = isSort;
                client.Send(Utility.CreateJsonObject(CMD_GET_COMP, Convert.ToString(jsonObject)));
                //Thread.Sleep(2000);
                string data = ParseReceivedData(true, Convert.ToString(jsonObject), 200);

                List<STRUCT_COMPONENT> structComponentList = new List<STRUCT_COMPONENT>();
                if (data != string.Empty)
                {
                    JArray jsonArray = JArray.Parse(data);
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        JObject componentData = JObject.Parse(jsonArray[i].ToString());
                        STRUCT_COMPONENT structComponent = new STRUCT_COMPONENT();
                        structComponent.ComponentName = componentData["name"].ToString();
                        structComponent.ComponentCode = componentData["code"].ToString();
                        structComponent.ComponentId = (uint)componentData["id"];
                        structComponent.ComponentImage = Utility.ByteArrayToImage((byte[])(componentData["image"]));
                        structComponent.TestLength = (int)componentData["test_len"];
                        if (Utility.CheckForJsonKeyPresence(componentData, "comp_start_count"))
                        {
                            GlobalVar.COMPONENT_START_INDEX = (int)componentData["comp_start_count"];
                        }
                        structComponentList.Add(structComponent);
                    }
                    return structComponentList;
                }
            }
            catch (Exception ex)
            {

            }
            return new List<STRUCT_COMPONENT>();
        }
    }
}
