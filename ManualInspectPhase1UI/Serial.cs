using MATS_Server;
using ManualInspectPhase1UI;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public delegate void read(string data);
namespace PCComm
{
    public class Serial
    {
        read readCallback;

        private string errorMsg = string.Empty;

        private int baudRate;
        private int dataBits;
        private string comPort;
        private Parity parityBit;
        private StopBits stopBits;
        private SerialPort serialComPort;

        private string readbytes;
        private bool instrumentBuffer1 = false;
        private bool instrumentBuffer2 = false;
        private string instrumentMsg1 = null;
        private string instrumentMsg2 = null;

        bool isAsyncRead = false;
        string lastReceivedData = string.Empty;
        //Constructors to set the SerialPort and comport
        public Serial(SerialPort port, string comport)
        {
            serialComPort = port;
            baudRate = 9600;
            dataBits = 8;
            comPort = comport;
            parityBit = Parity.None;
            stopBits = StopBits.One;
            serialComPort.DataReceived += new SerialDataReceivedEventHandler(serialComPort_DataReceived);
        }
        //Constructors to set the SerialPort, comport and baud rate
        public Serial(SerialPort port, string comport, int baud)
        {
            if(serialComPort != null)
            {
                serialComPort.DataReceived -= new SerialDataReceivedEventHandler(serialComPort_DataReceived);//Added
            }
            serialComPort = port;
            serialComPort.PortName = comport;
            baudRate = baud;
            dataBits = 8;
            comPort = comport;
            parityBit = Parity.None;
            stopBits = StopBits.One;
            serialComPort.DataReceived += new SerialDataReceivedEventHandler(serialComPort_DataReceived);
        }
        //Constructors to set the SerialPort, comport, baud rate and parity 
        public Serial(SerialPort port, string comport, int baud, Parity parity)
        {
            serialComPort = port;
            serialComPort.PortName = comport;
            baudRate = baud;
            comPort = comport;
            parityBit = parity;
            stopBits = StopBits.One;
            dataBits = 8;
            serialComPort.DataReceived += new SerialDataReceivedEventHandler(serialComPort_DataReceived);
        }
        //Constructors to set the SerialPort, comport, baud rate,parity and stopbits
        public Serial(SerialPort port, string comport, int baud, Parity parity, int data, StopBits stopbit)
        {
            serialComPort = port;
            serialComPort.PortName = comport;
            baudRate = baud;
            dataBits = data;
            comPort = comport;
            parityBit = parity;
            stopBits = stopbit;
            serialComPort.DataReceived += new SerialDataReceivedEventHandler(serialComPort_DataReceived);
        }
        //Port open
        public bool Open()
        {
            try
            {
                if(serialComPort == null)
                {
                    return false;
                }
                if (serialComPort.IsOpen == true) serialComPort.Close();
                serialComPort.BaudRate = baudRate;
                serialComPort.DataBits = dataBits;
                serialComPort.StopBits = stopBits;
                serialComPort.PortName = comPort;
                serialComPort.Parity = parityBit;
                serialComPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                errorMsg = ex.Message;
                return false;
            }
        }
        public string GetErrorMsg()
        {
            return errorMsg;
        }

        //Write to the serial port
        public void Write(string data)
        {
            try
            {
                if (!(serialComPort.IsOpen == true)) serialComPort.Open();

                // serialComPort.DiscardOutBuffer();
                //send the message to the port
                serialComPort.Write(data);
            }
            catch(Exception ex)
            {
                errorMsg = ex.Message;
                return;
            }
        }
        //Write to the serial port
        public void Write(byte[] data, int length)
        {
            if (!(serialComPort.IsOpen == true)) serialComPort.Open();

            serialComPort.DiscardOutBuffer();
            //send the message to the port
            serialComPort.Write(data, 0, length);
        }

        //Read data through port asynchronously
        public void AsyncRead(read s,bool start)
        {
            readCallback = s;
            isAsyncRead = start;
            //serialComPort.Close();
            ////if(start)
            ////{
            
            ////}
            ////else
            ////{
            ////    serialComPort.DataReceived -= new SerialDataReceivedEventHandler(serialComPort_DataReceived);
            ////}
            //Thread.Sleep(20);
            //Open();
        }
        //Close the port
        public void Close()
        {
            if(serialComPort != null && serialComPort.IsOpen)
            {
                serialComPort.Close();
            }
        }

        private string pingPongBuffer(string data)
        {
            // If the buffer1 flag is not set Indicate data is present in the msg1 string 
            // It Indicates data verification of the instrumentMsg1 data is not completed.
            if (instrumentBuffer1 == false)
            {
                instrumentMsg1 = data;
                instrumentBuffer1 = true;
            }
            /* This will be called when primary data is still in process
             * If the buffer2 flag is not set Indicate data is present in the msg2 string 
             * It Indicates data verification of the instrumentMsg2 data is not completed.
             */
            else if (instrumentBuffer2 == false)
            {
                instrumentMsg2 = data;
                instrumentBuffer2 = true;
            }
            // If any of the flag is set then The received buffer will be sent to parse data function
            // In parseData function data is parsed accordingly and further verification, calculation of data is done
            if (instrumentBuffer1 || instrumentBuffer2)
            {
                if (instrumentBuffer1)
                {
                    instrumentBuffer1 = false;

                    return instrumentMsg1;

                }
                else
                {
                    instrumentBuffer2 = false;
                    return instrumentMsg2;
                }
            }
            return null;
        }

        //Read data through synchrounsly
        public byte[] Read()
        {
            try
            {
                serialComPort.ReadTimeout = 4500;
                byte[] data = new byte[2000];
                int count = 0;
                data[count++] = Convert.ToByte(serialComPort.ReadByte());
                while (data[count - 1] != 13)
                {
                    data[count++] = Convert.ToByte(serialComPort.ReadByte());
                }
                Array.Resize(ref data, count);

                return (data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string ReadLine(int loopCount)
        {
            int count = 0;
            //while(count++ < loopCount)
            //{
            //    try
            //    {
            //        //serialComPort.NewLine = "\n";
            //        serialComPort.ReadTimeout = 6000;
            //        Thread.Sleep(100);
            //        string data = serialComPort.ReadExisting();
            //        //    serialComPort.ReadExisting();
            //        //serialComPort.DiscardInBuffer();
            //        //string str = (pingPongBuffer(data));
            //        return data;
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}
            string str = lastReceivedData;
            ////
            while (lastReceivedData == string.Empty && count++ != 10)
            {
                Thread.Sleep(GlobalVar.TEST_DELAY);
                str += lastReceivedData;
            }
           // Utility.WriteToLogFile("Data SerialRead: fxn: " , "DATA : " + str + ",DateTime" +DateTime.Now.ToLongTimeString()  + ", Last Received Data" +lastReceivedData);
            //serialComPort.DiscardInBuffer();
            if(lastReceivedData != string.Empty)
                lastReceivedData = string.Empty;
            return str;
        }

        //Call back the function when data is received
        private void serialComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialComPort.IsOpen)
            {
                try
                {
                    string str = serialComPort.ReadExisting();
                    if (str == string.Empty)
                    {
                        return;
                    }
                    while ((str.Length <= 0 || str[str.Length - 1] != '\n') && (serialComPort.IsOpen))// Added && (serialComPort.IsOpen)
                    {
                        Console.WriteLine("Data: " + str);
                        str += serialComPort.ReadExisting();
                    }
                    serialComPort.DiscardInBuffer();
                    if (isAsyncRead)
                    {
                       // Console.WriteLine("Data SerialAsyncRead: " + str);
                        //Utility.WriteToLogFile("Data SerialAsyncRead:", "DATA : " + str + ",DateTime" + DateTime.Now.ToLongTimeString() + ", Last Received Data" + lastReceivedData);
            
                        readCallback(pingPongBuffer(str));
                    }
                    else
                    {
                       // Utility.WriteToLogFile("Data SerialRead:", "DATA : " + str + ",DateTime" + DateTime.Now.ToLongTimeString() + ", Last Received Data" + lastReceivedData);
                      //  Console.WriteLine("Data SerialRead:{0} , Date = {1} ", str, DateTime.Now.ToLongTimeString());
                        lastReceivedData = str;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}