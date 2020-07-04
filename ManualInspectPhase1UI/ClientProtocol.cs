using MATS_Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolBox;

namespace ManualInspectPhase1UI
{
    class ClientProtocol
    {
        private string configStart = "setcfg start";
        private string configEnd = "setcfg end";
        private string configBenchName = "setcfg prod.name ";
        private string configChan1 = "setcfg chname 1 ";
        private string configChan2 = "setcfg chname 2 ";
        private string configChan3 = "setcfg chname 3 ";
        private string configChan4 = "setcfg chname 4 ";
        private string configIdentify = "identify";
        public string receiveTerminator = "\n";
        public string transmitTerminator = "\r";

        PCComm.Serial serial;
        FrmMechBench mainForm;
        BenchDetails bench = new BenchDetails();
        ServerProtocol serverProtocol;

        public ClientProtocol(FrmMechBench frm,PCComm.Serial serialCom)
        {
            mainForm = frm;
            serial = serialCom;
            serverProtocol = frm.serverProtocol;
            serial.AsyncRead(BenchReceivedData, false);
        }
        public ClientProtocol(FrmMechBench frm)
        {
            mainForm = frm;
            serverProtocol = frm.serverProtocol;
        }
        public void Disconnect()
        {
            serial.Close();
        }
        public void AsyncReadReport(bool readVal)
        {
            serial.AsyncRead(BenchReceivedData, readVal);
        }
        public static bool TestCheck(string measuredVal)
        {
            float val = Convert.ToSingle(measuredVal);
            if(GlobalVar.SELECTED_STRUCT_TEST.MinimumVal <= val && GlobalVar.SELECTED_STRUCT_TEST.MaximumVal >= val)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SendReport(string data,bool isManual,bool result)
        {
            Console.WriteLine("Report Data: " + data);
            string[] receivedData = data.Split(' ');
            string benchName = receivedData[0];
            string msgType = receivedData[1];
            string channelName = receivedData[2];
            string measuredVal = receivedData[3];
            if (GlobalVar.SELECTED_STRUCT_TEST.ChannelName.Contains(channelName))
            {
                //if (!Utility.ValidateNumber(measuredVal, true, true))
                //{
                //    string errorMsg = "Loose Connection(Data Corrupt). Please Try Again";
                //    ToolBoxWrite.LabelWrite(mainForm.lblServerResponse, errorMsg, Color.Red);
                //    return false;
                //}
                //else
                {
                    string unit = receivedData[4];
                    if(unit == "\n")
                    {
                        unit = string.Empty;
                    }
                    GlobalVar.STRUCT_REPORT = new STRUCT_REPORT();
                    GlobalVar.STRUCT_REPORT.ChannelName = channelName;
                    GlobalVar.STRUCT_REPORT.ComponentNum = GlobalVar.COMPONENT_NUMBER;
                    GlobalVar.STRUCT_REPORT.MeasuredVal = measuredVal.Replace("$#"," ");
                    GlobalVar.STRUCT_REPORT.TestId = GlobalVar.SELECTED_STRUCT_TEST.TestId;
                    GlobalVar.STRUCT_REPORT.TestResult = result;//TestCheck(GlobalVar.STRUCT_REPORT.MeasuredVal);
                    GlobalVar.STRUCT_REPORT.UnitName = unit;
                    GlobalVar.STRUCT_REPORT.IsManualEntry = isManual;
                    List<string> ChannelNameList = new List<string>();
                    ChannelNameList.Add(GlobalVar.STRUCT_REPORT.ChannelName);
                    List<string> UnitNameList = new List<string>();
                    UnitNameList.Add(GlobalVar.STRUCT_REPORT.UnitName);
                    List<string> MeasuredValueList = new List<string>();
                    MeasuredValueList.Add(GlobalVar.STRUCT_REPORT.MeasuredVal);
                    serverProtocol.SendReport(ChannelNameList, MeasuredValueList, UnitNameList, GlobalVar.STRUCT_REPORT);
                    mainForm.UpdateReportDetails(GlobalVar.STRUCT_REPORT);
                    bool retVal = GlobalVar.STRUCT_REPORT.TestResult;
                    string sendData = string.Empty;
                    //if (retVal)
                    //{
                    //    sendData = "pwm 1 50 30 1" + transmitTerminator;
                    //    serial.Write(sendData);
                    //    Thread.Sleep(200);
                    //    sendData = "pwm 16 50 20 1" + transmitTerminator;
                    //    serial.Write(sendData);
                    //}
                    //else
                    //{
                    //    sendData = "pwm 2 50 30 3" + transmitTerminator;
                    //    serial.Write(sendData);
                    //    Thread.Sleep(200);
                    //    sendData = "pwm 16 50 20 3" + transmitTerminator;
                    //    serial.Write(sendData);
                    //}
                }
            }
            else
            {
                string errorMsg = "The used instrument/channel is incorrect." + " Channel:" + channelName;
                ToolBoxWrite.LabelWrite(mainForm.lblServerResponse, errorMsg, Color.Red);
                return false;
            }
            return true;
        }

        public void BenchReceivedData(string data)
        {
            ToolBox.ToolBoxWrite.LabelWrite(mainForm.lblServerResponse, "");
            if(data == null || data == string.Empty || data.Length < 4 || mainForm.isManualTestSelected || !GlobalVar.TEST_STARTED_FLAG)
            {
                if (!GlobalVar.TEST_STARTED_FLAG && !mainForm.isManualTestSelected)
                {
                    ToolBox.ToolBoxWrite.LabelWrite(mainForm.lblServerResponse, "Please Start The Test And Proceed.");
                }
                return;
            }
            string[] receivedLines = data.Split('\n');
            for(int i =0;i<receivedLines.Length;i++)
            {
                string reportData = receivedLines[i];
                if (reportData.Contains("REPORT:"))
                {
                    if (!SendReport(reportData,false,true))
                    {
                        break;
                    }
                }
            }
        }

        public bool GetBenchDetails()
        {
            serial.AsyncRead(BenchReceivedData, false);
            serial.Write(configEnd + transmitTerminator);
            string data = serial.ReadLine(4);
            serial.Write(configStart + transmitTerminator);
            data = serial.ReadLine(4);
            if(data == null || !data.Contains("Started"))
            {
                return false;
            }
            serial.Write(configIdentify + transmitTerminator);

            //while (!data.Contains("prod.ch2:"))
            {
                data = serial.ReadLine(4);
                if(data == null)
                {
                    serial.Write(configEnd + transmitTerminator);
                    return false;
                }
                data = data.Replace("\n",string.Empty);
                Console.WriteLine("Received Client Data: " + data);
                string[] instrumentMsg = data.Split(';');
                for (int i = 0; i < instrumentMsg.Length; i++)
                {
                    if (instrumentMsg[i].Length > 2)
                    {
                        string[] instrumentMsgfields = instrumentMsg[i].Split(' ');
                        switch (instrumentMsgfields[0])
                        {
                            case "prod.name:":
                                ToolBoxWrite.TextBoxWrite(mainForm.tbBenchName, instrumentMsgfields[1]);
                                // frm.tbBenchName.Text = instrumentMsgfields[1];
                                bench.setBenchName(instrumentMsgfields[1]);
                                break;
                            case "prod.swver:":
                                break;
                            case "prod.hwver:":
                                break;
                            case "prod.type:":
                                bench.setType(instrumentMsgfields[1]);
                                break;
                            case "prod.class:":
                                bench.setClass(instrumentMsgfields[1]);
                                break;
                            case "prod.num:":
                                instrumentMsgfields[1] = instrumentMsgfields[1].Replace("\0\n", "");
                                bench.setNum(instrumentMsgfields[1]);
                                break;
                            case "prod.interface:":
                                bench.setInterface(instrumentMsgfields[1]);
                                break;
                            case "prod.channelcount:":
                                bench.setChCount(instrumentMsgfields[1]);
                                break;
                            case "prod.ch1:":
                                ToolBoxWrite.ComboBoxWrite(mainForm.cbChannel1, instrumentMsgfields[1]);
                                bench.setChan1(instrumentMsgfields[1]);
                                break;
                            case "prod.ch2:":
                                ToolBoxWrite.ComboBoxWrite(mainForm.cbChannel2, instrumentMsgfields[1]);
                                bench.setChan2(instrumentMsgfields[1]);
                                break;
                            case "prod.ch3:":
                                ToolBoxWrite.ComboBoxWrite(mainForm.cbChannel3, instrumentMsgfields[1]);
                                bench.setChan3(instrumentMsgfields[1]);
                                break;
                            //case "prod.ch4:":
                            //    ToolBoxWrite.ComboBoxWrite(mainForm.cbChannel4, instrumentMsgfields[1]);
                            //    bench.setChan4(instrumentMsgfields[1]);
                            //    break;
                        } // switch (instrumentMsgfields[0]) if(sentCommand[0] == "identify\r")
                    }
                }
            }
            serial.Write(configEnd + transmitTerminator);
            data = serial.ReadLine(4);
            if (!data.Contains("Ended"))
            {
                return false;
            }
            return true;
        }

        //Update the bench name, channel1 name,channel2 name
        public bool updateData(string benchName, string chName1, string chName2,string chName3,string chName4)
        {
            try
            {
                AsyncReadReport(false);
                serial.Write(configStart + transmitTerminator);
                string data = serial.ReadLine(4);
                if (!data.Contains("Started"))
                {
                    return false;
                }

                serial.Write(configBenchName + benchName + transmitTerminator);
                data = serial.ReadLine(4);
                if (!data.Contains("prod.name") && !data.Contains("SET"))
                {
                    return false;
                }

                serial.Write(configChan1 + chName1 + transmitTerminator);
                data = serial.ReadLine(4);
                if (!data.Contains("chname1") && !data.Contains("SET"))
                {
                    return false;
                }

                serial.Write(configChan2 + chName2 + transmitTerminator);
                data = serial.ReadLine(4);
                if (!data.Contains("chname2") && !data.Contains("SET"))
                {
                    return false;
                }
                serial.Write(configChan3 + chName3 + transmitTerminator);
                data = serial.ReadLine(4);
                if (!data.Contains("chname3") && !data.Contains("SET"))
                {
                    return false;
                }
                //serial.Write(configChan4 + chName4 + transmitTerminator);
                //data = serial.ReadLine();
                //if (!data.Contains("chname4") && !data.Contains("SET"))
                //{
                //    return false;
                //}
                serial.Write(configEnd + transmitTerminator);
                data = serial.ReadLine(4);
                if (!data.Contains("Ended"))
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
    class BenchDetails
    {
        private string benchName = "";
        private string channel1 = "";
        private string channel2 = "";
        private string channel3 = "";
        private string channel4 = "";
        private string channelCount = "";
        private string benchInterface = "";
        private string benchClass = "";
        private string benchType = "";
        private int benchNum = 0;
        public void setBenchName(string name)
        {
            this.benchName = name;
        }
        public void setInterface(string ver)
        {
            this.benchInterface = ver;
        }
        public void setChCount(string ver)
        {
            this.channelCount = ver;
        }
        public void setClass(string ver)
        {
            this.benchClass = ver;
        }
        public void setNum(string ver)
        {
            this.benchNum = Convert.ToInt16(ver);
        }
        public void setChan1(string ch)
        {
            this.channel1 = ch;
        }
        public void setChan2(string ch)
        {
            this.channel2 = ch;
        }
        public void setChan3(string ch)
        {
            this.channel3 = ch;
        }
        public void setChan4(string ch)
        {
            this.channel4 = ch;
        }
        public void setType(string ch)
        {
            this.benchType = ch;
        }
        /*  public string getBenchName()
          {
              return this.benchName;
          }
          public string getSWVer()
          {
              return this.softwareVersion;
          }
          public string getHWVer()
          {
              return this.hardwareVersion;
          }
          public string getInterface()
          {
              return this.benchInterface;
          }
          public string getChCount()
          {
              return this.channelCount;
          }
          public string getClass()
          {
              return this.benchClass;
          }
          public int getNum()
          {
              return this.benchNum;
          }
          public string getChan1()
          {
              return this.channel1;
          }
          public string getChan2()
          {
              return this.channel2;
          }
          public string getChan3()
          {
              return this.channel3;
          }
          public string getChan4()
          {
             return this.channel4 ;
          }*/
    }
}
