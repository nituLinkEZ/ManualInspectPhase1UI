using ManualInspectPhase1UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MATS_Server
{
    class Utility
    {
        private string localKey = "o4d7$a3fwLeir8Iyltas3yNut48234Ku";
        private byte[] localIV = { 
                                0x16, 0x02, 0x33, 0x67, 0x11, 0x32, 0x88, 0x14, 
                                0x16, 0x21, 0x36, 0x87, 0x31, 0x32, 0x18, 0xf2
                            };

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PeekMessage(out NativeMessage message,
            IntPtr handle, uint filterMin, uint filterMax, uint flags);
        private const UInt32 WM_MOUSEFIRST = 0x0200;
        private const UInt32 WM_MOUSELAST = 0x020D;
        public const int PM_REMOVE = 0x0001;

        public static IPAddress GetIPAddress()
        {
            IPAddress IPv4Address = null;

            foreach (IPAddress ipaddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPv4Address = ipaddress;
                    break;
                }
            }
            return IPv4Address;
        }
        // Flush all pending mouse events.
        public static void FlushMouseMessages()
        {
            NativeMessage msg;
            // Repeat until PeekMessage returns false.
            while (PeekMessage(out msg, IntPtr.Zero,
                WM_MOUSEFIRST, WM_MOUSELAST, PM_REMOVE))
                ;
        }
        public static void RemoveTabControlTopLayer(TabControl tabC)
        {
            tabC.Invoke((Action)delegate
            {
                tabC.ItemSize = new Size(0, 1);
                tabC.SizeMode = TabSizeMode.Fixed;
                for (int i = 0; i < tabC.TabCount; i++)
                {
                    TabPage tabP = tabC.TabPages[i];
                    tabP.Text = string.Empty;
                }
            });
        }
        public static void WriteToLogFile(string fxnName,string data)
        {
            if(data != null && data != string.Empty)
            {
                System.IO.File.AppendAllLines(GlobalVar.LOG_NAME, new[] { fxnName + " MESSAGE: " + data + "\tDATE: " + DateTime.Now.ToString() });
            }
        }

        public static List<string> GetComportName()
        {
            List<string> comportList = new List<string>();
            // Add the Comport Name to the Combo Box
            foreach (string str in SerialPort.GetPortNames())
            {
                comportList.Add(str);
            }
            return comportList;
        }
        public static string[] jSonTokeniser(string jsonData)
        {
            List<string> tokedStrings = new List<string>();
            string[] Val = Regex.Split(jsonData, @"}{");
            if(Val.Length > 1)
            {
                tokedStrings.Add(Val[0] + '}');
                for (int k = 1; k < Val.Length - 1; k++)
                {
                    tokedStrings.Add('{' + Val[k] + '}');
                }
                tokedStrings.Add('{' + Val[Val.Length - 1]);
            }
            else
            {
                tokedStrings.Add(Val[0]);
            }
            return tokedStrings.ToArray();
        }

        public static bool CheckForJsonKeyPresence(JObject jObj,string keyName)
        {
            var msgProperty = jObj.Property(keyName);

            //check if property exists
            if (msgProperty != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string CreateJsonObject(string requestType,string sendData)
        {
            JObject jsonSendData = new JObject();
            jsonSendData["Bench_Name"] = GlobalVar.BENCH_NAME;
            jsonSendData["Slave_ID"] = GlobalVar.SLAVE_ID;
            jsonSendData["request_type"] = requestType;
            jsonSendData["sequential_key"] = "1";
            jsonSendData["data"] = JObject.Parse(sendData);
            return Convert.ToString(jsonSendData);
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null || byteArrayIn.Length == 0)
            {
                return null;
            }
            Stream ms = new MemoryStream(byteArrayIn);
            Image img = Image.FromStream(ms); 
            return img;
        }

        public static float DoScalingOfUnit(string unit, string unit_measured, float measuredValue)
        {
            if (unit == unit_measured)
            {
                return measuredValue;
            }
            else
            {
                switch (unit_measured[0])
                {
                    case 'm':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, -3));
                        break;
                    case 'u':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, -6));
                        break;
                    case 'p':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, -12));
                        break;
                    case 'n':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, -9));
                        break;
                    case 'H':
                        break;
                    case 'O':
                        break;
                    case 'F':
                        break;
                    case 'k':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, 3));
                        break;
                    case 'M':
                        measuredValue = measuredValue * Convert.ToSingle(Math.Pow(10, 6));
                        break;
                }
            }

            return measuredValue;
        }

        public string Encrypt(string plainText)
        {
            string key = localKey;
            byte[] IV = localIV;
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string encrypted = string.Empty;
            try
            {
                // Create an encryptor object
                AesManaged myAes = new AesManaged();

                // Override the cipher mode, key and IV
                myAes.Mode = CipherMode.CBC;
                myAes.BlockSize = 128;
                myAes.IV = IV;
                myAes.Key = System.Text.Encoding.UTF8.GetBytes(key);  // Byte array representing the key
                myAes.Padding = PaddingMode.PKCS7;

                // Create a encryption object to perform the stream transform.
                ICryptoTransform encryptor = myAes.CreateEncryptor();

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        Console.WriteLine("Encrypted Binarysize: " + msEncrypt.ToArray().Length);
                        encrypted = ASCIIEncoding.ASCII.GetString(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public string Decrypt(string encryptedText)
        {
            string key = localKey;
            byte[] IV = localIV;
            // Check arguments.
            if (encryptedText == null || encryptedText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string decrypted = string.Empty;
            try
            {
                byte[] encrypted_data = ASCIIEncoding.ASCII.GetBytes(encryptedText.Trim());

                // Create an encryptor object
                AesManaged myAes = new AesManaged();

                // Override the cipher mode, key and IV
                myAes.Mode = CipherMode.CBC;
                myAes.IV = IV;
                myAes.Key = System.Text.Encoding.UTF8.GetBytes(key);  // Byte array representing the key
                myAes.Padding = PaddingMode.PKCS7;

                // Create a encryption object to perform the stream transform.
                ICryptoTransform decryptor = myAes.CreateDecryptor();

                // Create the streams used for encryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted_data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {

                            //Write all data to the stream.
                            decrypted = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
            // Return the encrypted bytes from the memory stream.
            return decrypted;
        }

        public static byte[] ImageToByteArray(Image img)
        {
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    img.Save(mStream, img.RawFormat);
                    return mStream.ToArray();
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static void SearchValueInDataTable(DataGridView table,TextBox tb, int rowValue)
        {
            if (tb.Text == "")
            {
                table.ClearSelection();
                table.Rows[0].Selected = true;
                return;
            }
            //Searches for the component name entered in the search box. If exists selects that row.
            foreach (DataGridViewRow row in table.Rows)
            {
                if (row.Cells[rowValue].Value != null)
                {
                    string comp_name = row.Cells[rowValue].Value.ToString();
                    comp_name = comp_name.ToLower();
                    string compare = tb.Text.ToLower();
                    bool[] b = new bool[tb.Text.Length];
                    for (int i = 0; i < tb.Text.Length; i++)
                    {
                        try
                        {
                            if (compare[i].Equals(comp_name[i]))
                            {
                                table.Rows[row.Index].Selected = true;
                                b[i] = true;
                            }
                            else
                            {
                                b[i] = false;
                                table.Rows[row.Index].Selected = false;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            b[i] = false;
                            table.Rows[row.Index].Selected = false;
                            break;
                        }
                    }
                }
            }
        }

        public static void EmptyImage(PictureBox picBox)
        {
            picBox.Invoke((Action)delegate
            {
                if (picBox.Image != null)
                {
                    picBox.Image.Dispose();
                    picBox.Image = null;
                }
            });
        }

        public static Image ImageUpdateFromFile(PictureBox picbox)
        {
            try
            {
                var FD = new System.Windows.Forms.OpenFileDialog();
                FD.Filter = "Image Files (*.bmp, *.jpg)|*.bmp;*.jpg";
                FD.FilterIndex = 0;
                if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileToOpen = FD.FileName;
                    picbox.Invoke((Action)delegate
                    {
                        picbox.Image = Image.FromFile(fileToOpen);
                    });
                    return Image.FromFile(fileToOpen);
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nPlease try agian", "Message", MessageBoxButtons.OKCancel);
                return null;
            }
        }

        public static void MoveOneListToOther(ListBox lboxSelect, ListBox lboxUnselect)
        {
            int selectedCount = lboxSelect.SelectedItems.Count - 1;
            if (lboxSelect.Items.Count > 0)
            {
                for (int i = 0; (i <= lboxSelect.Items.Count - 1) && i <= selectedCount; i++)
                {
                    string s_Value = lboxSelect.SelectedItems[i].ToString();
                    lboxUnselect.Items.Add(s_Value);
                }
                for (int i = 0; (i != -1) && i <= selectedCount; i++)
                {
                    string s_Value = lboxSelect.SelectedItems[0].ToString();
                    lboxSelect.Items.Remove(s_Value);
                }
            }
        }

        /*
         * pingIp will ping to the socket of the given ipAdress for the number of counts given
         * The pingReply status "Success" indicates socket connection is present
         * The pingReply status "not Success" indicates There may be socket connection problem or power connection problem
         * 
         * According to the reply function returns true or false
         */
        public bool PingIpAddress(string ipAddress, int count)
        {
            bool replyStatus = true;
            using (Ping pingClass = new Ping())
            {
                try
                {
                    while (count-- > 0)
                    {
                        // ping to the specified ipAddress with timeout
                        PingReply reply = pingClass.Send(ipAddress, 1000);
                        // If ping reply is not success return false
                        if (reply.Status != IPStatus.Success)
                        {
                            replyStatus = false;
                            //return false;
                        }
                        else
                        {
                            replyStatus = true;
                        }
                    }
                    // If ping reply is success
                    if (replyStatus == true)
                    {
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            }
        }
        public static bool ValidateNumber(string value,bool isFloat,bool isSigned)
        {
            if(value == string.Empty)
            {
                return false;
            }
            bool isValue = true;

            if(!isFloat && !isSigned )
            {
                if (Regex.IsMatch(value, "[^0-9]"))
                {
                    isValue = false;
                }
            }
            else if(isFloat && !isSigned)
            {
                if (Regex.IsMatch(value, "[^0-9.]"))
                {
                    isValue = false;
                }
            }
            else if(isFloat && isSigned)
            {
                if (Regex.IsMatch(value, "[^+0-9.]") && Regex.IsMatch(value, "[^-0-9.]"))
                {
                    isValue = false;
                }
            }
            else
            {
                if (Regex.IsMatch(value, "[^-0-9]") || Regex.IsMatch(value, "[^+0-9]"))
                {
                    isValue = false;
                }
            }
            return isValue;
        }
    }
}
