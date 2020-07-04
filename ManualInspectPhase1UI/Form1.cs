using MATS_Server;
using ManualInspectPhase1UI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolBox;

namespace ManualInspectPhase1UI
{
    public partial class FrmMechBench : Form
    {
        //public PCComm.Serial bluetoothSerial;
        //public PCComm.Serial benchSerial;
        public PCComm.Serial barcodeSerial;
        //public PCComm.Serial bluetoothTapeSerial;


        //ClientProtocol bluetoothClient;
        //ClientProtocol bluetoothTapeClient;
        //ClientProtocol benchClient;

        public ServerProtocol serverProtocol;

        //bool bluetoothVernierConnected = false;
        //bool bluetoothTapeConnected = false;
        bool benchConnected = false;

        public bool isManualTestSelected = false;

        // This parameter store the latest component test result
        private bool[] componentTestValues;
        private List<bool> componentValues = new List<bool>();

        // Below declared variable will check whether the test is started or not.
        // When button start is pressed and any single data is received over UART then it is considered as test started.
        public bool testStartClicked = false;

        public bool isTestsCompleted = false;

        private bool isFormClosePressed = false;
        private bool isStopButtonPressed = false;

        public GlobalVar.TEST_MENU_STATUS testMenuStatus = GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT;

        //Panel pnlComponent = null;
        //Button closeButton = null;

        List<string> ChannelName = new List<string>();

        TabPage prevTabPageSelected;
        TabPage selectedTabPage;

        public FrmMechBench(ServerProtocol protocol)
        {
            serverProtocol = protocol;
            InitializeComponent();
            prevTabPageSelected = tabPHome;
            selectedTabPage = tabPHome;
            List<string> comportName = Utility.GetComportName();

            if (comportName.Count == 0)
            {
                MessageBox.Show("Please Connect the COMPORT and Click Refresh button", "!Warning");
            }
            else
            {
                foreach (string str in comportName)
                {
                    cbComportName.Items.Add(str);
                    cbBluetoothVernierPort.Items.Add(str);
                    cbBarCodePort.Items.Add(str);
                    cbBluetoothTapePort.Items.Add(str);
                }
                //cbComportName.Text = cbComportName.Items[0].ToString();
            }
            dgvComponent.RowTemplate.MinimumHeight = 32;// dgvTest.RowTemplate.MinimumHeight = 32;
        }

        private void FrmMechBench_Load(object sender, EventArgs e)
        {
            Utility.RemoveTabControlTopLayer(tabCMainPanel);
            serverProtocol.GetUserAccess();
            if(GlobalVar.gIsSupervisor)
            {
                tsmiSupervisor.Checked = true;
                tsmiSupervisor.ForeColor = Color.Green;
            }

            //serverProtocol.GetTreeViewOfComponent("", false);
            tslUserName.Text = GlobalVar.USER_NAME;
            tslUserName.ForeColor = Color.DarkOrange;
            tslUserName.ToolTipText = GlobalVar.USER_NAME;
            tslUserName.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Bold);

            tsmiUserPermission.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular);
            tsmiLogOut.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular);
            tsmiExit.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 8.5f, FontStyle.Regular);

            tabCMainPanel.SelectedTab = tabPHome;
            ToolBox.ToolBoxWrite.IS_FORM_LOADED = true;

            ChannelName = serverProtocol.GetChannel();
            cbChannel1.Items.Clear();
            cbChannel2.Items.Clear();
            cbChannel3.Items.Clear();
            cbChannel4.Items.Clear();
            for (int i = 0; i < ChannelName.Count;i++ )
            {
                cbChannel1.Items.Add(ChannelName[i]);
                cbChannel2.Items.Add(ChannelName[i]);
                cbChannel3.Items.Add(ChannelName[i]);
                cbChannel4.Items.Add(ChannelName[i]);
            }
            tbBenchName.Enabled = false;
            //tbBenchName.Enabled = cbChannel1.Enabled = cbChannel2.Enabled = cbChannel3.Enabled = cbChannel4.Enabled = false;
            //cbBluetoothVernierChannel.Enabled = cbBluetoothTapeChannel.Enabled = false;
        }

        public void GetBarcodeData(string data)
        {
            string val = data.Replace("\r\n", "");
            GlobalVar.LAST_SCANNED_BARCODE_VAL = data;
            if(!tbBatchNum.Enabled )
            {
                ToolBoxWrite.TextBoxWrite(tbBatchNum, data);
            }
        }

        public bool connectBarCodePort()
        {
            if(cbBarCodePort.Text == string.Empty)
            {
                return false;
            }
            barcodeSerial = new PCComm.Serial(serialPBarcode,cbBarCodePort.Text,GlobalVar.BAUDRATE_BARCODE);
            if (!barcodeSerial.Open())
            {
                return false;
            }
            barcodeSerial.AsyncRead(GetBarcodeData,true);
            return true;
        }

        private void btnBluetoothVernierSelect_Click(object sender, EventArgs e)
        {
            //if(cbBluetoothVernierPort.Items.Count == 0)
            //{
            //    ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect the COMPORT and Continue",Color.Red);
            //    return;
            //}
            //if(serialPBluetooth.IsOpen)
            //{
            //    return;
            //}
            //serialPBluetooth.Close();
            //btnBluetoothSelect.Enabled = false;

            //bluetoothSerial = new PCComm.Serial(serialPBluetooth, cbBluetoothVernierPort.Text, GlobalVar.BLUETOOTH_BAUDRATE);
            //cbBluetoothVernierPort.Enabled = false;
            //int retryCount = 0;
            //while (!bluetoothSerial.Open() && retryCount != 3)
            //{
            //    System.Windows.Forms.DialogResult dialogOutput = MessageBox.Show(bluetoothSerial.GetErrorMsg(), "ERROR", MessageBoxButtons.RetryCancel);
            //    if (dialogOutput == System.Windows.Forms.DialogResult.Retry)
            //    {
            //        bluetoothSerial.Close();
            //        retryCount++;
            //        //bluetoothSerial.Open();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Problem With Bluetooth Connection. Please Check the Bluetooth COMPORT Connection And Restart the Program");
            //        this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //        this.Close();
            //        return;
            //    }
            //}
            //if (retryCount == 3)
            //{
            //    MessageBox.Show("Please Check the Bench COMPORT Connection And Restart the Program");
            //    //Environment.Exit(0);
            //    this.Close();
            //    this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //    return;
            //}
            ////bluetoothClient = new ClientProtocol(this, bluetoothSerial);
            ////if (!bluetoothClient.GetBenchDetails())
            ////{
            ////    MessageBox.Show("Couldn't Receive Data From Bench");
            ////    btnBluetoothSelect.Enabled = true;
            ////    bluetoothVernierConnected = false;
            ////}
            ////else
            //{
            //    cbBluetoothVernierChannel.Text = "VERNIER";
            //    btnBluetoothSelect.Enabled = false;
            //    bluetoothVernierConnected = true;
            //}
        }

        private void btnBluetoothTapeSelect_Click(object sender, EventArgs e)
        {
            //if (cbBluetoothTapePort.Items.Count == 0)
            //{
            //    ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect the COMPORT and Continue", Color.Red);
            //    return;
            //}
            //if (serialPBluetoothTape.IsOpen)
            //{
            //    return;
            //}
            //serialPBluetoothTape.Close();
            //btnBluetoothTapeSelect.Enabled = false;

            //bluetoothTapeSerial = new PCComm.Serial(serialPBluetoothTape, cbBluetoothTapePort.Text, GlobalVar.BLUETOOTH_BAUDRATE);
            //cbBluetoothTapePort.Enabled = false;
            //int retryCount = 0;
            //while (!bluetoothTapeSerial.Open() && retryCount != 3)
            //{
            //    System.Windows.Forms.DialogResult dialogOutput = MessageBox.Show(bluetoothTapeSerial.GetErrorMsg(), "ERROR", MessageBoxButtons.RetryCancel);
            //    if (dialogOutput == System.Windows.Forms.DialogResult.Retry)
            //    {
            //        bluetoothTapeSerial.Close();
            //        retryCount++;
            //        //bluetoothSerial.Open();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Problem With Bluetooth Connection. Please Check the Bluetooth COMPORT Connection And Restart the Program");
            //        this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //        this.Close();
            //        return;
            //        //Environment.Exit(0);
            //    }
            //}
            //if (retryCount == 3)
            //{
            //    MessageBox.Show("Please Check the Bench COMPORT Connection And Restart the Program");
            //    //Environment.Exit(0);
            //    this.Close();
            //    this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //    return;
            //}
            ////bluetoothTapeClient = new ClientProtocol(this, bluetoothTapeSerial);
            ////if (!bluetoothTapeClient.GetBenchDetails())
            ////{
            ////    MessageBox.Show("Couldn't Receive Data From Bench");
            ////    btnBluetoothTapeSelect.Enabled = true;
            ////    bluetoothTapeConnected = false;
            ////}
            ////else
            //{
            //    cbBluetoothTapeChannel.Text = "DIGITAPE";
            //    btnBluetoothTapeSelect.Enabled = false;
            //    bluetoothTapeConnected = true;
            //}
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //if(cbComportName.Items.Count == 0)
            //{
            //    ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect Comport And Continue.",Color.Red);
            //    return;
            //}

            //this.Cursor = Cursors.WaitCursor;

            //btnUpdate.Enabled = false;
            //if(btnUpdate.Text == GlobalVar.Btn_Select)
            //{
            //    serialPBench.Close();
            //    serialPBench.Dispose();
            //    benchSerial = new PCComm.Serial(serialPBench, cbComportName.Text, GlobalVar.BENCH_BAUDRATE);
            //    cbComportName.Enabled = false;
            //    int retryCount = 0;
            //    while (!benchSerial.Open() && retryCount != 3)
            //    {
            //        System.Windows.Forms.DialogResult dialogOutput = MessageBox.Show(benchSerial.GetErrorMsg(), "ERROR", MessageBoxButtons.RetryCancel);
            //        if (dialogOutput == System.Windows.Forms.DialogResult.Retry)
            //        {
            //            benchSerial.Close();
            //            retryCount++;
            //            //benchSerial.Open();
            //        }
            //        else
            //        {
            //            MessageBox.Show("Please Check the Bench COMPORT Connection And Restart the Program");
            //            //Environment.Exit(0);
            //            this.Close();
            //            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //            return;
            //        }
            //    }
            //    if(retryCount == 3)
            //    {
            //        MessageBox.Show("Please Check the Bench COMPORT Connection And Restart the Program");
            //        //Environment.Exit(0);
            //        this.Close();
            //        this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            //        return;
            //    }
            //    //benchClient = new ClientProtocol(this, benchSerial);
            //    //if (!benchClient.GetBenchDetails())
            //    //{
            //    //    MessageBox.Show("Couldn't Receive Data From Bench. Please Check the Bench PORT And Restart the Program");
            //    //    benchConnected = false;
            //    //    serialPBench.Close();
            //    //    serialPBench.Dispose();
            //    //}
            //    //else
            //    //{
            //    //    btnUpdate.Text = GlobalVar.Btn_Update;
            //    //    benchConnected = true;
            //    //}
            //}
            //else
            //{
            //    ToolBoxWrite.LabelWrite(lblStatus, string.Empty);
            //    if (cbChannel1.Text == cbChannel2.Text)
            //    {
            //        if (MessageBox.Show("Both the Channel Name are same.. Do you want to continue?", "!Warning", MessageBoxButtons.YesNo) == DialogResult.No)
            //        {
            //            ToolBoxWrite.LabelWrite(lblStatus, "Change the Channel name and Press Update");
            //            btnUpdate.Enabled = true;

            //            btnSaveContinue.Text = GlobalVar.Btn_Update;
            //            return;
            //        }
            //    }
            //    //btnSaveContinue.Text = GlobalVar.Btn_Save;
            //    //if (!benchClient.updateData(tbBenchName.Text, cbChannel1.Text, cbChannel2.Text, cbChannel3.Text, cbChannel4.Text))
            //    //{
            //    //    btnSaveContinue.Text = GlobalVar.Btn_Update;
            //    //    MessageBox.Show("Couldn't Receive Data From Bench. Please Try Again");
            //    //}
            //    //else
            //    //{
            //    //    ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, "Updated Channel Details.", Color.OrangeRed);
            //    //}
            //}
            //btnUpdate.Enabled = true;

            //Utility.FlushMouseMessages();
            //this.Cursor = Cursors.Default;
        }

        private void btnBarcodePortSelect_Click(object sender, EventArgs e)
        {
            serialPBarcode.Close();
            if(!connectBarCodePort())
            {
                btnBarcodePortSelect.Enabled = false;
                GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_MANUAL;
                testMenuStatus = GlobalVar.TEST_MENU_STATUS.BARCODE_MANUAL;
            }
            else
            {
                btnBarcodePortSelect.Enabled = true;
                GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_SCAN;
                testMenuStatus = GlobalVar.TEST_MENU_STATUS.BARCODE_SCAN;
            }
        }

        private void cBoxBarcodeMode_CheckedChanged(object sender, EventArgs e)
        {
            if(cBoxBarcodeMode.Checked)
            {
                GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_MANUAL;
                testMenuStatus = GlobalVar.TEST_MENU_STATUS.BARCODE_MANUAL;
            }
            else
            {
                GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_DISABLE;
                testMenuStatus = GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT;
            }
            tbComponent.Enabled = cbComponent.Visible = btnComponentChange.Enabled = !cBoxBarcodeMode.Checked;
        }

        private void UpdateTestTable(bool update)
        {
            if(update)
            {
                dgvMechBenchTest.Invoke((Action)delegate
                {
                    dgvMechBenchTest.Rows.Clear();
                    for (int i = 0; i < GlobalVar.STRUCT_TEST_LIST.Count; i++)
                    {
                        STRUCT_TEST structTest = GlobalVar.STRUCT_TEST_LIST[i];
                        dgvMechBenchTest.Rows.Add(structTest.TestId, structTest.Test_Num, structTest.Date_Time.ToString(), "", structTest.TypicalVal);
                        dgvMechBenchTest.Refresh();
                    }
                });
            }
        }

        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            //if(cbComportName.Text == string.Empty && cbBarCodePort.Text == string.Empty && cbBluetoothTapePort.Text == string.Empty && cbBluetoothVernierPort.Text == string.Empty)
            //{
            //    ToolBoxWrite.LabelWrite(lblHomeStatus,"Please Connect Mechbench Or Bluetooth PORT",Color.Red);
            //    return;
            //}
            this.Cursor = Cursors.WaitCursor;

            tbBenchName.Enabled = cbChannel1.Enabled = cbChannel2.Enabled = cbChannel3.Enabled = cbChannel4.Enabled = true;
            cbBluetoothTapeChannel.Enabled = cbBluetoothVernierChannel.Enabled = true;
            if(btnSaveContinue.Text == GlobalVar.Btn_Save)
            {
                //if(cbComportName.Text != string.Empty && btnUpdate.Text == GlobalVar.Btn_Select)
                //{
                //    btnUpdate.Text = GlobalVar.Btn_Select;
                //    btnUpdate_Click(btnUpdate, new EventArgs());
                //}
                //if(cbBluetoothVernierPort.Text != string.Empty)
                //{
                //    btnBluetoothVernierSelect_Click(btnBluetoothSelect, new EventArgs());
                //}
                //if(cbBluetoothTapePort.Text != string.Empty)
                //{
                //    btnBluetoothTapeSelect_Click(btnBluetoothTapeSelect, new EventArgs());
                //}
                //if(cbBarCodePort.Text != string.Empty)
                //{
                //    btnBarcodePortSelect_Click(btnBarcodePortSelect, new EventArgs());
                //}
                //if (bluetoothVernierConnected || benchConnected || bluetoothTapeConnected) //To be Added
                //{
                //    if (benchConnected)
                //    {
                        benchConnected = true;
                        btnSaveContinue.Text = GlobalVar.Btn_Update;
                    //}
                    //else
                    //{
                    //    ToolBoxWrite.LabelWrite(lblHomeStatus, "Bluetooth Devices Are Connected. Mechbench Is Not Connected.", Color.Orange);
                    //}
                    btnHomeComponent.Enabled = true;
                    //if (!cBoxBarcodeMode.Checked)
                    //{
                    //    GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_DISABLE;
                    //    tsmiComponent_Click(tsmiComponent, new EventArgs());
                    //}
                    //else
                    //{
                    //    tabCMainPanel.SelectedTab = tabPReport;
                    //}
                    //timerServerConnect.Start();
                //}
                //else //To be Added
                //{
                //    ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect to the Mechbench and Continue..",Color.Red);
                //}
            }
            else
            {
                //btnUpdate.Text = GlobalVar.Btn_Update;
                //btnUpdate_Click(btnUpdate, new EventArgs());
                //if (cbBluetoothVernierPort.Text != string.Empty)
                //{
                //    btnBluetoothVernierSelect_Click(btnBluetoothSelect, new EventArgs());
                //}
                //if (cbBluetoothTapePort.Text != string.Empty)
                //{
                //    btnBluetoothTapeSelect_Click(btnBluetoothTapeSelect, new EventArgs());
                //}
                //if (cbBarCodePort.Text != string.Empty)
                {
                    btnBarcodePortSelect_Click(btnBarcodePortSelect, new EventArgs());
                }
            }

            Utility.FlushMouseMessages();
            this.Cursor = Cursors.Default;
        }
        private void ShowPleaseWait(string title)
        {
            try
            {
                Console.WriteLine("Thread Started ==============>");
                FrmProgressBar frm = new FrmProgressBar();
                frm.Show();
                frm.SetTitleName(title);
                frm.Refresh();
                ThreadState state = Thread.CurrentThread.ThreadState;
                while (GlobalVar.PLEASE_WAIT_ENABLED && state != ThreadState.AbortRequested && state != ThreadState.Aborted)
                {
                    Console.WriteLine("Thread State =" + state.ToString());
                    state = Thread.CurrentThread.ThreadState;
                    Thread.Sleep(100);
                }
                Console.WriteLine("Thread Ended ==============>" + state);
                frm.Close();
                frm.Dispose();
            }
            catch(ThreadAbortException ex)
            {
                Console.WriteLine("Thread Exception Is Handled..");
            }
        }

        private void tabCMainPanel_Selected(object sender, TabControlEventArgs e)
        {
            for (int i = 0; i < msLeftMain.Items.Count; i++)
            {
                msLeftMain.Items[i].ForeColor = Color.Black;
            }
            //ToolStripMenuItem item = (ToolStripMenuItem)sender;
            //item.ForeColor = GlobalVar.COLOR_TSMI_SELECT;
            
            ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, string.Empty);
            ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, string.Empty);
            ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, string.Empty);
            ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, string.Empty);
            Console.WriteLine("Prev Name: " + prevTabPageSelected.Name + " , Present Selected TabName: " + selectedTabPage.Name);
            prevTabPageSelected = selectedTabPage;//GlobalVar.SELECTED_TAB_PAGE_NAME;
            //GlobalVar.SELECTED_TAB_PAGE_NAME = e.TabPage.Name;
            selectedTabPage = e.TabPage;
            string tabName = e.TabPage.Name;
            switch (tabName)
            {
                case "tabPReport":
                    ToolBox.ToolBoxWrite.ButtonWrite(btnStart, GlobalVar.Btn_Start, Color.Green);
                    btnNext.Visible = btnStart.Visible = false;
                    if (pbTestImg.Image != null)
                    {
                        pbTestImg.Image.Dispose();
                        pbTestImg.Image = null;
                    }
                    miNextTest.Visible = false;
                    lblStatusTest.Text = "Status";
                    lblTestResultnStatus.Text = lblTestDescription.Text = lblComponentNumCount.Text = cbManualInstrumentName.Text = lblTestPassFailCount.Text = string.Empty;
                    lblServerResponse.Text = tbComponent.Text = tbBatchNum.Text = string.Empty;
                
                    dgvMechBenchTest.Rows.Clear();
                    cBoxIsManualEntry.Visible = false;
                
                    cbManualInstrumentName.Items.Clear();

                    tsmiReport.ForeColor = GlobalVar.COLOR_TSMI_SELECT;

                    miRefresh.Enabled = false;
                    miClosePort.Enabled = false;
                    //tsmiView.Visible = true;
                    if(GlobalVar.BARCODE_STATUS == GlobalVar.BARCODE_MODE.BARCODE_DISABLE)
                    {
                        ToolBox.ToolBoxWrite.ComboBox(new ComboBox[] { cbBatchNum, cbComponent }, false, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent}, true, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, GlobalVar.gSelectedComponent.ComponentName, false, true);
                        ToolBox.ToolBoxWrite.LabelWrite(lblBatchNum, "Batch Number");
                        btnComponentChange.Enabled = true;
                        gBoxParameter.Visible = true;
                        tbBatchNum.Visible = false;
                        //cbComponent.Visible = false;
                        cbBatchNum.Visible = true;
                        cbBatchNum.Enabled = true;
                        //tbComponent.Enabled = false;
                        //tbComponent.Visible = true;
                        //tbComponent.Text = GlobalVar.gSelectedComponent.ComponentName;

                        btnComponentChange.Text = GlobalVar.Btn_Select;
                        btnComponentChange_Click(btnComponentChange, new EventArgs());
                        btnComponentChange.Text = GlobalVar.Btn_Change;
                        btnBatchSave.Text = GlobalVar.Btn_Select;
                    }
                    else if (GlobalVar.BARCODE_STATUS == GlobalVar.BARCODE_MODE.BARCODE_SCAN)
                    {
                        ToolBox.ToolBoxWrite.ComboBox(new ComboBox[] { cbBatchNum, cbComponent }, false, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent, tbBatchNum }, false, ToolBoxWrite.Property.Enable);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent, tbBatchNum }, true, ToolBoxWrite.Property.Visible);
                        btnComponentChange.Enabled = false;
                        btnBatchSave.Text = GlobalVar.Btn_Save;
                        //cbComponent.Visible = false;
                        //tbComponent.Enabled = false;
                        //cbBatchNum.Visible = false;
                        //tbBatchNum.Enabled = false;
                        ToolBoxWrite.LabelWrite(lblBatchNum, "Scan Barcode:", Color.Black);
                    }
                    else
                    {
                        btnBatchSave.Text = GlobalVar.Btn_Save;
                        ToolBox.ToolBoxWrite.ComboBox(new ComboBox[] { cbBatchNum, cbComponent }, false, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent, tbBatchNum }, true, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbBatchNum }, true, ToolBoxWrite.Property.Enable);
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent }, true, ToolBoxWrite.Property.Enable);
                        btnComponentChange.Enabled = false;
                        //cbComponent.Visible = false;
                        //tbComponent.Enabled = false;
                        //cbBatchNum.Visible = false;
                        //tbBatchNum.Enabled = true;
                        ToolBoxWrite.LabelWrite(lblBatchNum, "Enter Barcode:", Color.Black);
                    }
                    break;//if(e.TabPage == tabPTest)
                case "tabPHome":
                    tsmiHome.ForeColor = GlobalVar.COLOR_TSMI_SELECT;
                    ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, "");
                    miClosePort.Enabled = true;
                    miRefresh.Enabled = true;

                    ToolBox.ToolBoxWrite.ComboBox(new ComboBox[] { cbBatchNum, cbComponent }, false, ToolBoxWrite.Property.Visible);
                    ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent, tbBatchNum }, false, ToolBoxWrite.Property.Visible);
                    ToolBox.ToolBoxWrite.ComboBox(new ComboBox[] { cbBatchNum, cbComponent }, false, ToolBoxWrite.Property.Enable);
                    ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbComponent, tbBatchNum }, false, ToolBoxWrite.Property.Enable);
                        
                    //tbComponent.Visible = tbBatchNum.Visible = cbBatchNum.Visible = cbComponent.Visible = true;
                    //tbComponent.Enabled = tbBatchNum.Enabled = cbBatchNum.Enabled = cbComponent.Enabled = true;
                    miNextTest.Enabled = false;
                    miStop.Enabled = false;
                    GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
                    break;
                case "tabPComponent":
                    tsmiComponent.ForeColor = GlobalVar.COLOR_TSMI_SELECT;
                    GlobalVar.COMPONENT_START_INDEX = 0;
                    GetComponent();
                   // UpdateComponentTable();
                    break;
            }
            
        }

        private void btnRefreshComports_Click(object sender, EventArgs e)
        {
            serialPBarcode.Close();
            serialPBench.Close();
            serialPBluetooth.Close();
            btnUpdate.Text = GlobalVar.Btn_Select;
            cbComportName.Enabled = cbBarCodePort.Enabled = cbBluetoothVernierPort.Enabled = true;
            cbComportName.Items.Clear();
            cbBarCodePort.Items.Clear();
            cbBluetoothVernierPort.Items.Clear();
            List<string> comportName = Utility.GetComportName();

            if (comportName.Count == 0)
            {
                MessageBox.Show("Please Connect the COMPORT and Try Again", "!Warning");
                return;
            }
            foreach (string str in comportName)
            {
                cbComportName.Items.Add(str);
                cbBluetoothVernierPort.Items.Add(str);
                cbBarCodePort.Items.Add(str);
                cbBluetoothTapePort.Items.Add(str);
            }
            cbComportName.Text = cbComportName.Items[0].ToString();
            btnBarcodePortSelect.Enabled = btnBluetoothSelect.Enabled = btnUpdate.Enabled = true;
        }

        private void btnComponentChange_Click(object sender, EventArgs e)
        {
            ToolBoxWrite.LabelWrite(lblWarnBatchNum, string.Empty);
            switch(btnComponentChange.Text)
            {
                case GlobalVar.Btn_Select:

                    cbBatchNum.Items.Clear();
                    cbBatchNum.Text = string.Empty;
                    GlobalVar.COMPONENT_ID = GlobalVar.gSelectedComponent.ComponentId;
                    GlobalVar.BATCH_LIST = serverProtocol.GetBatch(GlobalVar.COMPONENT_ID);
                    if(GlobalVar.BATCH_LIST.Count == 0)
                    {
                        MessageBox.Show("Selected Component Doesn't Have Batches. Please Contact Admin");
                        tabCMainPanel.SelectedTab = tabPComponent;
                    }
                    for (int i = 0; i < GlobalVar.BATCH_LIST.Count; i++)
                    {
                        cbBatchNum.Items.Add(GlobalVar.BATCH_LIST[i]);
                    }
                    cbBatchNum.Enabled = true;
                    btnBatchSave.Text = GlobalVar.Btn_Select;
                    btnComponentChange.Text = GlobalVar.Btn_Change;
                    cbComportName.Enabled = false;
                    break;
                case GlobalVar.Btn_Change:
 
                    btnNext_Click(btnNext, new EventArgs());
                    if (isTestsCompleted)
                    {
                        btnStart.Visible = false;
                        ToolBoxWrite.ButtonWrite(btnStart, "Start", Color.Green);
                        tabCMainPanel.SelectedTab = tabPComponent;
                        btnComponentChange.Text = GlobalVar.Btn_Select;
                        cbComportName.Enabled = true;
                        cbBatchNum.Text = string.Empty;
                        cbBatchNum.Items.Clear();
                        btnBatchSave.Text = GlobalVar.Btn_Select;
                        GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
                        cBoxRetest.Enabled = false;
                    }
                    break;
            }
        }

        private void btnChangeBatch_Click()
        {
            if (testMenuStatus == GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT)
            {
                cbBatchNum.Enabled = true;
                btnBatchSave.Text = GlobalVar.Btn_Select;
            }
            else if (testMenuStatus == GlobalVar.TEST_MENU_STATUS.BARCODE_MANUAL)
            {
                tbBatchNum.Enabled = true;
                btnBatchSave.Text = GlobalVar.Btn_Save;
            }
            else
            {
                tbBatchNum.Enabled = false;
                btnBatchSave.Text = GlobalVar.Btn_Save;
            }
            ToolBoxWrite.ButtonWrite(btnStart, "Start", Color.Green);
            btnStart.Visible = false;
            btnNext.Visible = false;
            cBoxIsManualEntry.Visible = false;
            cbManualInstrumentName.Visible = false;
            dgvMechBenchTest.Invoke((Action)delegate
            {
                dgvMechBenchTest.Rows.Clear();
            });
            if (pbTestImg.Image != null)
            {
                pbTestImg.Image.Dispose();
                pbTestImg.Image = null;
            }
        }

        private void btnBatchSave_Click(object sender, EventArgs e)
        {
            ToolBoxWrite.LabelWrite(lblWarnBatchNum, "", Color.Red);
            switch(btnBatchSave.Text)
            {
                case GlobalVar.Btn_Select:
                    if(cbBatchNum.Text == string.Empty)
                    {
                        ToolBoxWrite.LabelWrite(lblWarnBatchNum, "Please Select Batch", Color.Red); return;
                    }
                    FrmProgressBar frm = new FrmProgressBar();
                    frm.Show();
                    frm.SetTitleName("Retriving Test Details For " + GlobalVar.gSelectedComponent.ComponentName);
                    frm.Refresh();
                    GlobalVar.STRUCT_TEST_LIST = serverProtocol.GetTestDetails((int)GlobalVar.COMPONENT_ID);
                    Thread.Sleep(10);
                    //changeSelectionPosition();
                    UpdateTestTable(true);
                    changeSelectionPosition();
                    GlobalVar.COMPONENT_NUMBER = serverProtocol.GetDeviceNumber(cbBatchNum.Text, false);
                    lblComponentNumCount.Text = GlobalVar.COMPONENT_NUMBER.ToString();
                    frm.Close();
                    frm.Dispose();
                    if(GlobalVar.COMPONENT_NUMBER == 0)
                    {
                        ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                        ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "No Component Is Present To Test");
                        ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "No Component Is Present");
                        break;
                    }
                    btnBatchSave.Text = GlobalVar.Btn_Change;
                    cbBatchNum.Enabled = false;
                    btnStart.Visible = true;
                    cBoxRetest.Enabled = true;
                    break;

                case GlobalVar.Btn_Change:
                    if (prevTabPageSelected == tabPReport)
                    {
                        Console.WriteLine(prevTabPageSelected.Name + " " + selectedTabPage.Name);
                        btnNext_Click(btnNext, new EventArgs());
                        if (!isTestsCompleted)
                        {
                            ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "Please Complete The Test Or Press Stop To Change the Batch");
                            return;
                        }
                        cBoxRetest.Enabled = false;
                    }
                    btnChangeBatch_Click();
                    break;

                case GlobalVar.Btn_Save:
                    if(tbBatchNum.Text != string.Empty || testMenuStatus == GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT)
                    {
                        tbBatchNum.Text = tbBatchNum.Text.Replace("\r\n", "");
                        if(!Utility.ValidateNumber(tbBatchNum.Text,false,false))
                        {
                            tbBatchNum.Text = string.Empty;
                            lblServerResponse.Text = "INVALID BARCODE";
                            return;
                        }
                        GlobalVar.gSelectedComponent= serverProtocol.GetBarcode(tbBatchNum.Text.Replace("\r\n",""),true);
                        if (GlobalVar.gSelectedComponent.ComponentId == 0)
                        {
                            tbBatchNum.Text = string.Empty;
                            lblServerResponse.Text = "INVALID BARCODE";
                            return;
                        }
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, GlobalVar.gSelectedComponent.ComponentName, Color.Black);
                        GlobalVar.COMPONENT_ID = GlobalVar.gSelectedComponent.ComponentId;
                        FrmProgressBar frmP = new FrmProgressBar();
                        frmP.Show();
                        frmP.SetTitleName("Retriving Test Details For " + GlobalVar.gSelectedComponent.ComponentName);
                        frmP.Refresh();
                        GlobalVar.STRUCT_TEST_LIST = serverProtocol.GetTestDetails((int)GlobalVar.COMPONENT_ID);

                        UpdateTestTable(true);
                        GlobalVar.COMPONENT_NUMBER = serverProtocol.GetDeviceNumber(tbBatchNum.Text, true);
                        lblComponentNumCount.Text = GlobalVar.COMPONENT_NUMBER.ToString();
                        frmP.Close();
                        frmP.Dispose();
                        if (GlobalVar.COMPONENT_NUMBER == 0)
                        {
                            ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                            ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "No Component Is Present To Test");
                            break;
                        }
                        if (GlobalVar.IS_TYPE_LOT)
                        {
                            cBoxRetest.Enabled = true;
                        }
                        tbBatchNum.Enabled = false;
                        btnBatchSave.Text = GlobalVar.Btn_Change;
                        btnStart.Visible = true;

                    }
                    else
                    {
                        lblServerResponse.Text = "Please Provide Barcode";
                    }
                    break;
            }
        }
        public void updateTableToEnterManualData(bool readOnly)
        {
            isManualTestSelected = !readOnly;
            cBoxIsManualEntry.Invoke((Action)delegate
            {
                cBoxIsManualEntry.Visible = !readOnly;
            });
            cbManualInstrumentName.Invoke((Action)delegate
            {
                cbManualInstrumentName.Visible = !readOnly;
            });

            if (readOnly)
            {
                dgvMechBenchTest.Invoke((Action)delegate
                {
                    dgvMechBenchTest.ReadOnly = readOnly;
                    for (int i = 0; i < dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells.Count; i++)
                        dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[i].ReadOnly = readOnly;
                    dgvMechBenchTest.Refresh();
                });
            }
            else
            {
                dgvMechBenchTest.Invoke((Action)delegate
                {
                    dgvMechBenchTest.ReadOnly = false;
                    for (int i = 0; i < dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells.Count; i++)
                        dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[i].ReadOnly = true;
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[5].ReadOnly = false;
                });
                cBoxIsManualEntry.Invoke((Action)delegate
                {
                    cBoxIsManualEntry.CheckState = CheckState.Checked;
                });
            }
        }

        public void UpdateServerResponse(string msg)
        {
            if (selectedTabPage== tabPHome)
                //if (GlobalVar.SELECTED_TAB_PAGE_NAME == "tabPHome")
            {
                ToolBoxWrite.LabelWrite(lblHomeStatus, msg, Color.OrangeRed);
            }
            else
            {
                ToolBoxWrite.LabelWrite(lblServerResponse, msg, Color.OrangeRed);
            }
        }

        public void UpdateReportDetails(STRUCT_REPORT structReport)
        {
            dgvMechBenchTest.Invoke((Action)delegate
            {
                if(structReport.TestResult)
                {
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[3].Value = "PASS";
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].DefaultCellStyle.ForeColor = Color.Green;
                }
                else
                {
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[3].Value = "FAIL";
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].DefaultCellStyle.ForeColor = Color.Red;
                }
                dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[5].Value = structReport.MeasuredVal;
                dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[6].Value = structReport.UnitName;
                dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[7].Value = structReport.ChannelName;
                componentTestValues[GlobalVar.TABLE_ROW_INDEX] = structReport.TestResult;
                GlobalVar.TABLE_ROW_INDEX++;
                if (GlobalVar.TABLE_ROW_INDEX == dgvMechBenchTest.RowCount)
                {
                    GlobalVar.TABLE_ROW_INDEX = 0;
                }
            });
            changeSelectionPosition();
            UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);
        }

        public void UpdateAllValues(int rowNumber)
        {
            if(dgvMechBenchTest.RowCount <= rowNumber)
            {
                return;
            }
            uint testId = (uint)dgvMechBenchTest.Rows[rowNumber].Cells[0].Value;
            for (int i = 0; i < GlobalVar.STRUCT_TEST_LIST.Count; i++)
            {
                //STRUCT_TEST structTest = GlobalVar.STRUCT_TEST_LIST[i];
                if (GlobalVar.STRUCT_TEST_LIST[i].TestId == testId)
                {
                    GlobalVar.SELECTED_STRUCT_TEST = GlobalVar.STRUCT_TEST_LIST[i];
                    ToolBoxWrite.LabelWrite(lblMinValue, GlobalVar.SELECTED_STRUCT_TEST.MinimumVal.ToString() + " " + GlobalVar.SELECTED_STRUCT_TEST.UnitName);
                    ToolBoxWrite.LabelWrite(lblMaxValue, GlobalVar.SELECTED_STRUCT_TEST.MaximumVal.ToString() + " " + GlobalVar.SELECTED_STRUCT_TEST.UnitName);
                    ToolBoxWrite.LabelWrite(lblMeasuredValue, GlobalVar.SELECTED_STRUCT_TEST.TypicalVal.ToString() + " " + GlobalVar.SELECTED_STRUCT_TEST.UnitName);
                    ToolBoxWrite.LabelWrite(lblTestDescription, GlobalVar.SELECTED_STRUCT_TEST.Description);
                    pbTestImg.Image = GlobalVar.SELECTED_STRUCT_TEST.TestImage;

                    //All the Data will be entered manually for manual inspection.
                    isManualTestSelected = true;

                    cbManualInstrumentName.Invoke((Action)delegate
                    {
                        cbManualInstrumentName.Visible = true;
                        cbManualInstrumentName.Items.Clear();
                        for (int j = 0; j < GlobalVar.SELECTED_STRUCT_TEST.ChannelName.Length; j++)
                        {
                            //if (GlobalVar.SELECTED_STRUCT_TEST.ChannelName[j] != "Manual" )
                            {
                                cbManualInstrumentName.Items.Add(GlobalVar.SELECTED_STRUCT_TEST.ChannelName[j]);
                            }
                            ToolBoxWrite.LabelWrite(lblTestDescription, lblTestDescription.Text + "\nInstrument name: " + GlobalVar.SELECTED_STRUCT_TEST.ChannelName[j]);
                        }
                        cbManualInstrumentName.SelectedIndex = 0;
                    });
                    //updateTableToEnterManualData(!(lblTestDescription.Text.Contains("Manual")));

                    ToolBox.ToolBoxWrite.TextBoxWrite(tbMeasuredValue, string.Empty, Color.Black);
                    ToolBox.ToolBoxWrite.TextBoxWrite(tbUnit, string.Empty, Color.Black);
                    if (GlobalVar.SELECTED_STRUCT_TEST.MinimumVal == 0 && GlobalVar.SELECTED_STRUCT_TEST.MaximumVal == 0)
                    {
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbMeasuredValue, tbUnit }, false, ToolBoxWrite.Property.Enable);
                    }
                    else
                    {
                        ToolBox.ToolBoxWrite.TextBox(new TextBox[] { tbMeasuredValue, tbUnit }, true, ToolBoxWrite.Property.Enable);
                    }
                    if (btnStart.Text == "Stop")
                        GlobalVar.TEST_STARTED_FLAG = !isManualTestSelected;
                    else
                        GlobalVar.TEST_STARTED_FLAG = false;
                }
            }
        }

        private void dgvMechBenchTest_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                return;
            }
            GlobalVar.TABLE_ROW_INDEX = e.RowIndex;
            UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);
        }

        private void FrmMechBench_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Invoke((Action)delegate
            {
                if (tabCMainPanel.SelectedTab == tabPReport && GlobalVar.TEST_STARTED_FLAG)
                {
                    isFormClosePressed = true;
                    btnNext_Click(btnNext, new EventArgs());
                    if (!isTestsCompleted)
                    {
                        if (MessageBox.Show("Tests for the selected component is not completed.Do you want to Abort the Test", "Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            serverProtocol.SendStartEndAbortTestStatus(ServerProtocol.CMD_ABORT_TEST);
                            e.Cancel = false;
                            serverProtocol.Disconnect();
                            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                        }
                        else
                        {
                            e.Cancel = true;
                            isFormClosePressed = false;
                        }
                    }
                    else
                    {
                        //Environment.Exit(Environment.ExitCode);
                        serverProtocol.Disconnect();
                        this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                        return;
                    }
                }
                else
                {
                    //Environment.Exit(Environment.ExitCode);
                    //Application.Exit();
                    if (!GlobalVar.isUserLogOut)
                    {
                        serverProtocol.Disconnect();
                        this.DialogResult = System.Windows.Forms.DialogResult.Abort;
                    }
                    return;
                }
            });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            switch(btnStart.Text)
            {
                case GlobalVar.Btn_Start:
                    //if(benchClient == null && bluetoothClient == null && bluetoothTapeClient == null)
                    //{
                    //    ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "Please Connect the Embedded Bench To Test The Component");
                    //    Utility.FlushMouseMessages();
                    //    this.Cursor = Cursors.Default;
                    //    return;
                    //}
                    if(testMenuStatus != GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT && (tbBatchNum.Text == string.Empty || btnBatchSave.Text == GlobalVar.Btn_Save))
                    {
                        ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "Please Provide Barcode Value");
                        Utility.FlushMouseMessages();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    else if (testMenuStatus == GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT && (cbBatchNum.Text == string.Empty || btnBatchSave.Text == GlobalVar.Btn_Save))
                    {
                        ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "Please Select Batch Number");
                        Utility.FlushMouseMessages();
                        this.Cursor = Cursors.Default;
                        return;
                    }
                    else if(lblComponentNumCount.Text == "0")
                    {
                        Utility.FlushMouseMessages();
                        this.Cursor = Cursors.Default;
                        return;                       
                    }
                    //btnComponentChange.Enabled = false;
                    //btnBatchSave.Enabled = false;
                    //tsmiComponent.Enabled = false;
                    //tsmiHome.Enabled = false;
                    //tsmiTopComponent.Enabled = false;
                    //tsmiTopHome.Enabled = false;
                    ToolBox.ToolBoxWrite.Button(new Button[] { btnComponentChange, btnBatchSave }, false, ToolBoxWrite.Property.Enable);
                    ToolBox.ToolBoxWrite.ToolStripMenuItem(new ToolStripMenuItem[] { tsmiComponent, tsmiHome, tsmiTopComponent, tsmiTopHome }, false, ToolBoxWrite.Property.Enable);

                    GlobalVar.IS_BTN_START_CLICKED = true;
                    btnNext.Visible = true;
                    btnNext.Enabled = true;
                    testStartClicked = true;
                   // miNextTest.Enabled = true;
                    {
                        GlobalVar.TABLE_ROW_INDEX = 0;
                        UpdateTestTable(true);
                        changeSelectionPosition();
                        UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);
                        ToolBoxWrite.ButtonWrite(btnStart, "Stop", Color.Red);
                        ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "");
                        //if (benchClient != null)
                        //{
                        //    benchClient.AsyncReadReport(true);
                        //}
                        //if (bluetoothTapeClient != null)
                        //{
                        //    bluetoothTapeClient.AsyncReadReport(true);
                        //}
                        //if (bluetoothClient != null)
                        //{
                        //    bluetoothClient.AsyncReadReport(true);
                        //}
                        // The flag is used to retrieve and access the mechbench report
                        if (!isManualTestSelected)
                        {
                            GlobalVar.TEST_STARTED_FLAG = true;
                        }

                        //Update the length of componentTestValue list variable to add the test result
                        componentTestValues = new bool[dgvMechBenchTest.RowCount];

                        serverProtocol.SendStartEndAbortTestStatus(ServerProtocol.CMD_START_TEST);
                    }
                    break;
                case GlobalVar.Btn_Stop:
                    isStopButtonPressed = true;
                    btnNext_Click(btnNext, new EventArgs());
                    if (!isTestsCompleted || !testStartClicked)
                    {
                        bool val = UpdateAbortValues();
                        //GlobalVar.TABLE_ROW_INDEX = 0;
                        //UpdateTestTable(true);
                        //changeSelectionPosition();
                        //UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);
                        if (!isTestsCompleted)
                        {
                            ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "Test Aborted.");
                            serverProtocol.SendStartEndAbortTestStatus(ServerProtocol.CMD_ABORT_TEST);
                        }
                    }
                    //else
                    {
                        GlobalVar.TABLE_ROW_INDEX = 0;
                        UpdateTestTable(true);
                        changeSelectionPosition();
                        UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);
                        //serverProtocol.SendStartEndAbortTestStatus(GlobalVar.CMD_END_TEST);
                    }
                    ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "");
                    
                    ToolBoxWrite.ButtonWrite(btnStart, "Start", Color.Green);

                    GlobalVar.TEST_STARTED_FLAG = false;
                    //btnNext.Visible = false;
                    //btnNext.Enabled = false;
                    GlobalVar.IS_BTN_START_CLICKED = false;

                    ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
                    //btnComponentChange.Enabled = true;
                    //btnBatchSave.Enabled = true;
                    //tsmiComponent.Enabled = true;
                    //tsmiHome.Enabled = true;
                    //tsmiTopComponent.Enabled = true;
                    //tsmiTopHome.Enabled = true;
                    
                    if(testMenuStatus == GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT)
                    {
                        btnComponentChange.Enabled = true;
                    }
                    else
                    {
                        btnComponentChange.Enabled = false;
                        btnBatchSave.Text = GlobalVar.Btn_Save;
                        if(testMenuStatus == GlobalVar.TEST_MENU_STATUS.BARCODE_MANUAL)
                        {
                            tbBatchNum.Enabled = true;
                        }
                    }
                    ToolBox.ToolBoxWrite.Button(new Button[] {  btnBatchSave, btnNext }, true, ToolBoxWrite.Property.Enable);
                    ToolBox.ToolBoxWrite.ToolStripMenuItem(new ToolStripMenuItem[] { tsmiComponent, tsmiHome, tsmiTopComponent, tsmiTopHome }, true, ToolBoxWrite.Property.Enable);
                    btnNext.Enabled = btnNext.Visible = false;
                    isStopButtonPressed = false;
                    break;
            }
            Utility.FlushMouseMessages();
            this.Cursor = Cursors.Default;
        }
        private bool UpdateAbortValues()
        {
            bool testResultValue = false;
            //Below declaration will show the result of the all the tests and wait for few seconds.
            ToolBoxWrite.LabelWrite(lblStatusTest, "Result");

            //this.Invoke((Action)delegate
            //{
            //    lblPassOrFailText.Visible = lblTestPassFailCount.Visible = true;
            //});

            ToolBox.ToolBoxWrite.Label(new Label[] { lblPassOrFailText, lblTestPassFailCount }, false, ToolBoxWrite.Property.Visible);
            // Show the Final component result in the Tests UI.
            if (componentTestValues != null && componentTestValues.Length > 0 && componentTestValues.Contains(false))
            {
                int count = 0;
                foreach (bool b in componentTestValues)
                {
                    if (b == false)
                        count++;
                }
                ToolBoxWrite.LabelWrite(lblPassOrFailText, "Pass/Fail:");

                ToolBoxWrite.LabelWrite(lblTestPassFailCount, "P-" + (componentTestValues.Length - count).ToString() + " / F-" + count.ToString(), Color.Red);
                ToolBoxWrite.LabelWrite(lblTestResultnStatus, "FAIL", Color.Red);

                componentValues.Add(false);
            }
            else if (componentTestValues != null)
            {
                ToolBoxWrite.LabelWrite(lblPassOrFailText, "Pass/Fail:");
                ToolBoxWrite.LabelWrite(lblTestPassFailCount, "P-" + (componentTestValues.Length).ToString() + " / F-0", Color.Green);
                ToolBoxWrite.LabelWrite(lblTestResultnStatus, "PASS", Color.Green);
                componentValues.Add(true);
                testResultValue = true;
            }

            Thread.Sleep(4000);
            //this.Invoke((Action)delegate
            //{
            //    //lblPassOrFailText.Refresh();
            //    //lblTestPassFailCount.Refresh();
            //    //lblTestPassFailCount.Refresh();
            //    lblTestPassFailCount.Visible = lblPassOrFailText.Visible = false;
            //});

            ToolBox.ToolBoxWrite.Label(new Label[] { lblTestPassFailCount, lblPassOrFailText }, false, ToolBoxWrite.Property.Visible);
            
            ToolBoxWrite.LabelWrite(lblStatusTest, "Status");
            ToolBoxWrite.LabelWrite(lblTestResultnStatus, "");

            componentTestValues = new bool[dgvMechBenchTest.RowCount];
            //this.Invoke((Action)delegate
            //{
            //    dgvMechBenchTest.Rows.Clear();

            //    table.displayCompTestTable(this, componentId, "", 0);
            //});

            this.Invoke((Action)delegate
            {
                lblMinValue.Text = lblMaxValue.Text = lblMeasuredValue.Text = lblTestDescription.Text = string.Empty;
                pbTestImg.Image = null;
            });
            //ToolBoxWrite.LabelWrite(lblComponentNumCount, component_number.ToString());
            return testResultValue;
        }


        private void dgvMechBenchTest_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (cBoxIsManualEntry.Visible == true && GlobalVar.TEST_STARTED_FLAG == false && btnStart.Text == "Stop")
            {
                if(cbManualInstrumentName.Text == string.Empty)
                {
                    return;
                }
                dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[5].ReadOnly = true;
                ToolBoxWrite.LabelWrite(lblTestResultnStatus, string.Empty);
                try
                {
                    string str = tbBenchName.Text + " REPORT: " + cbManualInstrumentName.Text + " " + dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[5].Value.ToString() + " " + GlobalVar.SELECTED_STRUCT_TEST.UnitName + "\n";

                    cBoxIsManualEntry.Visible = false;
                    cbManualInstrumentName.Invoke((Action)delegate
                    {
                        cbManualInstrumentName.Visible = false;
                        cbManualInstrumentName.Text = string.Empty;
                    });
                    //if(benchClient != null)
                    //{
                    //    benchClient.SendReport(str, true);
                    //}
                    //else if(bluetoothClient != null)
                    //{
                    //    bluetoothClient.SendReport(str, true);
                    //}
                    //else if(bluetoothTapeClient != null)
                    //{
                    //    bluetoothTapeClient.SendReport(str, true);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Bench Is Not Connected.");
                    //}
                    isManualTestSelected = false;
                    GlobalVar.TEST_STARTED_FLAG = true;
                }
                catch (Exception ex)
                {
                    dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[5].ReadOnly = false;
                    MessageBox.Show(ex.Message);
                }
            }
            else if (cBoxIsManualEntry.Visible == true && GlobalVar.TEST_STARTED_FLAG == false)
            {
                ToolBoxWrite.LabelWrite(lblServerResponse, "Please Start the test.", Color.Red);
            }
        }

        public bool updateDefaultTestData()
        {
            bool testResultValue = false;
            int countTestDone = 0;
            isTestsCompleted = true;

            for (int i = 0; i < dgvMechBenchTest.RowCount; i++)
            {
                // Checks whether all the tests for the component is conducted or not
                if (dgvMechBenchTest.Rows[i].Cells[3].Value.ToString() == "")
                {
                    countTestDone++;
                }
            }
            if (countTestDone != 0 && countTestDone != dgvMechBenchTest.RowCount)
            {
                // If test is not completed
                isTestsCompleted = false;
                return false;
            }
            else if (countTestDone == dgvMechBenchTest.RowCount)
            {
                //btnStart is clicked but no single test is conducted
                testStartClicked = false;
                return true;
            }
            //componentId = getComponentId(tbComponent.Text);
            //Below declaration will show the result of the all the tests and wait for few seconds.
            ToolBoxWrite.LabelWrite(lblStatusTest, "Result");

            //this.Invoke((Action)delegate
            //{
            //    lblPassOrFailText.Visible = lblTestPassFailCount.Visible = true;
            //});

            ToolBox.ToolBoxWrite.Label(new Label[] { lblPassOrFailText, lblTestPassFailCount }, true, ToolBoxWrite.Property.Visible);

            // Show the Final component result in the Tests UI.
            if (componentTestValues != null && componentTestValues.Length > 0 && componentTestValues.Contains(false))
            {
                int count = 0;
                foreach (bool b in componentTestValues)
                {
                    if (b == false)
                        count++;
                }
                ToolBoxWrite.LabelWrite(lblPassOrFailText, "Pass/Fail:");

                ToolBoxWrite.LabelWrite(lblTestPassFailCount, "P-" + (componentTestValues.Length - count).ToString() + " / F-" + count.ToString(), Color.Red);
                ToolBoxWrite.LabelWrite(lblTestResultnStatus, "FAIL", Color.Red);

                componentValues.Add(false);
            }
            else if (componentTestValues != null)
            {
                ToolBoxWrite.LabelWrite(lblPassOrFailText, "Pass/Fail:");
                ToolBoxWrite.LabelWrite(lblTestPassFailCount, "P-" + (componentTestValues.Length).ToString() + " / F-0", Color.Green);
                ToolBoxWrite.LabelWrite(lblTestResultnStatus, "PASS", Color.Green);
                componentValues.Add(true);
                testResultValue = true;
            }
            Thread.Sleep(4000);
            //this.Invoke((Action)delegate
            //{
            //    lblPassOrFailText.Refresh();
            //    lblTestPassFailCount.Refresh();
            //    lblTestPassFailCount.Refresh();
            //    lblTestPassFailCount.Visible = lblPassOrFailText.Visible = false;
            //});

            ToolBox.ToolBoxWrite.Label(new Label[] { lblTestPassFailCount, lblPassOrFailText }, false, ToolBoxWrite.Property.Visible);

            ToolBoxWrite.LabelWrite(lblStatusTest, "Status");
            ToolBoxWrite.LabelWrite(lblTestResultnStatus, "");

            componentTestValues = new bool[dgvMechBenchTest.RowCount];
            //this.Invoke((Action)delegate
            //{
            //    dgvMechBenchTest.Rows.Clear();

            //    table.displayCompTestTable(this, componentId, "", 0);
            //});

            this.Invoke((Action)delegate
            {
                lblMinValue.Text = lblMaxValue.Text = lblMeasuredValue.Text = lblTestDescription.Text = string.Empty;
                pbTestImg.Image = null;
            });

            //write.LabelWrite(lblComponentNumCount, component_number.ToString());
            return testResultValue;
        }

        /*******************************************************************************
         * @brief  : Changes the row selection When test is started
         * 	         This function Changes the selected row of the datagridview table
         *           And Changes the test selected to the next test
         * @param  : None.
         * @retval : None.
         **=============================================================================*/
        public void changeSelectionPosition()
        {
            dgvMechBenchTest.Invoke((Action)delegate
            {
                if(dgvMechBenchTest.RowCount > GlobalVar.TABLE_ROW_INDEX)
                {
                    int scrollRowIndex = 0;
                    if(GlobalVar.TABLE_ROW_INDEX > 0)
                    {
                        scrollRowIndex = GlobalVar.TABLE_ROW_INDEX - 1;
                    }
                    dgvMechBenchTest.FirstDisplayedScrollingRowIndex = scrollRowIndex;

                    dgvMechBenchTest.Refresh();

                    dgvMechBenchTest.CurrentCell = dgvMechBenchTest.Rows[GlobalVar.TABLE_ROW_INDEX].Cells[1];
                }
            });
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.IS_BTN_START_CLICKED)
            {
                isTestsCompleted = true;
                ToolBox.ToolBoxWrite.CheckBox(cBoxIsManualEntry, false, ToolBoxWrite.Property.Visible);
                ToolBox.ToolBoxWrite.ComboBox(cbManualInstrumentName, false, ToolBoxWrite.Property.Visible);
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            bool checkManualCheckBoxVisibility = false;
            if (cBoxIsManualEntry.Visible)
            {
                checkManualCheckBoxVisibility = true;
                ToolBox.ToolBoxWrite.CheckBox(cBoxIsManualEntry, false, ToolBoxWrite.Property.Visible);
                ToolBox.ToolBoxWrite.ComboBox(cbManualInstrumentName, false, ToolBoxWrite.Property.Visible);
            }
            ToolBox.ToolBoxWrite.Button(btnNext, false, ToolBoxWrite.Property.Visible);
            bool val = updateDefaultTestData();
            if (!isTestsCompleted)
            {
                if (checkManualCheckBoxVisibility)
                {
                    ToolBox.ToolBoxWrite.CheckBox(cBoxIsManualEntry, true, ToolBoxWrite.Property.Visible);
                    ToolBox.ToolBoxWrite.ComboBox(cbManualInstrumentName, true, ToolBoxWrite.Property.Visible);
                }
                ToolBox.ToolBoxWrite.Button(btnNext, true, ToolBoxWrite.Property.Visible);
                //ToolBox.ToolBoxWrite.ToolStripMenuItem(new ToolStripMenuItem[] { miNextTest }, true, ToolBoxWrite.Property.Visible);
                ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "Complete The Test To Proceed");
                Utility.FlushMouseMessages();
                this.Cursor = Cursors.Default;
                return;
            }
            //if(isFormClosePressed && isTestsCompleted)
            if(!testStartClicked)
            {
                //Send Test End Msg to Server
                serverProtocol.SendStartEndAbortTestStatus(ServerProtocol.CMD_ABORT_TEST);
            }
            else
            {
                //Send Test End Msg to Server
                serverProtocol.SendStartEndAbortTestStatus(ServerProtocol.CMD_END_TEST);
            }
            if(!isFormClosePressed)
            {
                dgvMechBenchTest.Invoke((Action)delegate
                {
                    dgvMechBenchTest.Rows.Clear();
                });
                if (GlobalVar.BARCODE_STATUS == GlobalVar.BARCODE_MODE.BARCODE_DISABLE)
                {

                        UpdateTestTable(!isStopButtonPressed);
                        GlobalVar.COMPONENT_NUMBER = serverProtocol.GetDeviceNumber(cbBatchNum.Text, false);
                        ToolBoxWrite.LabelWrite(lblComponentNumCount, GlobalVar.COMPONENT_NUMBER.ToString());
                        if (GlobalVar.COMPONENT_NUMBER == 0)
                        {
                            ToolBox.ToolBoxWrite.ButtonWrite(btnBatchSave, GlobalVar.Btn_Select,Color.Black);
                            ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                            ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "No Component Is Present To Test");
                            ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "No Component Is Present");
                        }
                        else
                        {
                            this.Invoke((Action)delegate
                            {
                                btnBatchSave.Text = GlobalVar.Btn_Change;
                                cbBatchNum.Enabled = false;
                                btnStart.Visible = true;
                            });
                        }
                }
                else if (GlobalVar.BARCODE_STATUS == GlobalVar.BARCODE_MODE.BARCODE_MANUAL)
                {

                    ToolBoxWrite.LabelWrite(lblComponentNumCount, "");
                    if (GlobalVar.IS_TYPE_LOT)
                    {
                        UpdateTestTable(!isStopButtonPressed);
                        GlobalVar.COMPONENT_NUMBER = serverProtocol.GetDeviceNumber(tbBatchNum.Text, true);
                        ToolBoxWrite.LabelWrite(lblComponentNumCount, GlobalVar.COMPONENT_NUMBER.ToString());
                        if(GlobalVar.COMPONENT_NUMBER == 0)
                        {
                            this.Invoke((Action)delegate
                            {
                                btnBatchSave.Text = GlobalVar.Btn_Save;
                                tbBatchNum.Enabled = true;
                            });
                            ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                            ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
                            ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, string.Empty);
                        }
                    }
                    else
                    {
                        this.Invoke((Action)delegate
                        {
                            btnBatchSave.Text = GlobalVar.Btn_Save;
                            tbBatchNum.Enabled = true;
                        });
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, string.Empty);
                    }
                }
                else
                {
                    ToolBoxWrite.LabelWrite(lblComponentNumCount, "");
                    if (GlobalVar.IS_TYPE_LOT)
                    {
                        UpdateTestTable(!isStopButtonPressed);
                        GlobalVar.COMPONENT_NUMBER = serverProtocol.GetDeviceNumber(tbBatchNum.Text, true);
                        ToolBoxWrite.LabelWrite(lblComponentNumCount, GlobalVar.COMPONENT_NUMBER.ToString());
                        if (GlobalVar.COMPONENT_NUMBER == 0)
                        {
                            this.Invoke((Action)delegate
                            {
                                btnBatchSave.Text = GlobalVar.Btn_Save;
                                tbBatchNum.Enabled = false;
                            });
                            ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                            ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
                            ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, string.Empty);
                        }
                    }
                    else
                    {
                        this.Invoke((Action)delegate
                        {
                            btnBatchSave.Text = GlobalVar.Btn_Save;
                            tbBatchNum.Enabled = false;
                        });
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
                        ToolBox.ToolBoxWrite.TextBoxWrite(tbComponent, string.Empty);
                    }
                }
            }
            //ToolBox.ToolBoxWrite.TextBoxWrite(tbBatchNum, string.Empty);
            if(GlobalVar.COMPONENT_NUMBER == 0)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "No Component Is Present To Test");
                ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "No Component Is Present");
            }

            if (!isStopButtonPressed)
            {
                if (testMenuStatus == GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT)
                {
                    ToolBox.ToolBoxWrite.Button(new Button[] { btnComponentChange }, true, ToolBoxWrite.Property.Enable);
                }
                else
                {
                    ToolBox.ToolBoxWrite.Button(new Button[] { btnComponentChange }, false, ToolBoxWrite.Property.Enable);
                }
                ToolBox.ToolBoxWrite.Button(new Button[] {  btnBatchSave }, true, ToolBoxWrite.Property.Enable);
                ToolBox.ToolBoxWrite.ToolStripMenuItem(new ToolStripMenuItem[] { tsmiComponent, tsmiHome, tsmiTopComponent, tsmiTopHome }, true, ToolBoxWrite.Property.Enable);
                //btnComponentChange.Enabled = btnBatchSave.Enabled = tsmiComponent.Enabled = tsmiHome.Enabled = tsmiTopComponent.Enabled = tsmiTopHome.Enabled = true;

                // If all the test are completed then move to next component
                GlobalVar.TABLE_ROW_INDEX = 0;
                GlobalVar.TEST_STARTED_FLAG = false;
                Thread.Sleep(500);


                UpdateTestTable(true);
                //This function changes the selection of the test in the datagrid view table.
                changeSelectionPosition();

                // This function updates all the required components
                UpdateAllValues(GlobalVar.TABLE_ROW_INDEX);

                if (checkManualCheckBoxVisibility)
                {
                    //cBoxIsManualEntry.Invoke((Action)delegate
                    //{
                    //    cBoxIsManualEntry.Visible = true;
                    //});
                    //cbManualInstrumentName.Invoke((Action)delegate
                    //{
                    //    cbManualInstrumentName.Visible = true;
                    //});
                    ToolBox.ToolBoxWrite.CheckBox(cBoxIsManualEntry, true, ToolBoxWrite.Property.Visible);
                    ToolBox.ToolBoxWrite.ComboBox(cbManualInstrumentName, true, ToolBoxWrite.Property.Visible);
                }
                ToolBoxWrite.ButtonWrite(btnStart, "Start", Color.Green);
            }
            Utility.FlushMouseMessages();
            this.Cursor = Cursors.Default;
        }

        private void tsmiHome_Click(object sender, EventArgs e)
        {
            GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
            tabCMainPanel.SelectedTab = tabPHome;
        }

        private void cmsMechBench_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void miNextTest_Click(object sender, EventArgs e)
        {
            if (selectedTabPage == tabPReport && GlobalVar.IS_BTN_START_CLICKED)
            {
                btnNext_Click(btnNext, new EventArgs());
            }
        }

        private void miRefreshMechbench_Click(object sender, EventArgs e)
        {
            if(!serialPBench.IsOpen)
            {
                cbComportName.Enabled = true;
                serialPBench.Close();
                serialPBench.Dispose();
                btnUpdate.Text = GlobalVar.Btn_Select;
            }
            if(cbComportName.Items.Count == 0)
            {
                List<string> comportName = Utility.GetComportName();

                if (comportName.Count == 0)
                {
                    MessageBox.Show("Please Connect the COMPORT and Try Again", "!Warning");
                    return;
                }
                foreach (string str in comportName)
                {
                    cbComportName.Items.Add(str);
                }
                cbComportName.Text = cbComportName.Items[0].ToString();
            }
        }

        private void tsmiComponent_Click(object sender, EventArgs e)
        {
            //if (benchClient == null && bluetoothTapeClient == null && bluetoothClient == null)
            //{
            //    ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect the Embedded Bench And Proceed",Color.Red);
            //    return;
            //}
            GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
            
            tabCMainPanel.SelectedTab = tabPComponent;
        }

        private void miRefreshIndividual_Click(object sender,EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            Button btn = null;
            ComboBox cb = null;
            SerialPort serialPort = null;
            string text = string.Empty;
            switch(mi.Name)
            {
                case "miRefreshMechbench":
                    btn = btnUpdate;
                    cb = cbComportName;
                    serialPort = serialPBench;
                    text = GlobalVar.Btn_Select;
                    break;
                case "miRefreshBarcode":
                    btn = btnBarcodePortSelect;
                    cb = cbBarCodePort;
                    serialPort = serialPBarcode;
                    break;
                case "miRefreshBVernier":
                    btn = btnBluetoothSelect;
                    cb = cbBluetoothVernierPort;
                    serialPort = serialPBluetooth;
                    break;
                case "miRefreshBTape":
                    btn = btnBluetoothTapeSelect;
                    cb = cbBluetoothTapePort;
                    serialPort = serialPBluetoothTape;
                    break;
            }
            if(!serialPort.IsOpen)
            {
                cb.Enabled = true;
                serialPort.Close();
                serialPort.Dispose();
                if (text != string.Empty)
                    btn.Text = text;
            }
            cb.Items.Clear();
            List<string> comportName = Utility.GetComportName();

            if (comportName.Count == 0)
            {
                MessageBox.Show("Please Connect the COMPORT and Try Again", "!Warning");
                return;
            }
            foreach (string str in comportName)
            {
                cb.Items.Add(str);
            }
        }

        private void miStop_Click(object sender, EventArgs e)
        {
            if (selectedTabPage == tabPReport && GlobalVar.TEST_STARTED_FLAG)
            {
                btnStart_Click(btnStart, new EventArgs());
            }
        }

        private void miCloseMechBench_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            switch(mi.Name)
            {
                case "miCloseBarcode":
                    serialPBarcode.Close();
                    serialPBarcode.Dispose();
                    cbBarCodePort.Enabled = true;
                    break;
                case "miCloseBVernier":
                    serialPBluetooth.Close();
                    serialPBluetooth.Dispose();
                    cbBluetoothVernierPort.Enabled = true;
                    break;
                case "miCloseMechBench":
                    serialPBench.Close();
                    serialPBench.Dispose();
                    cbComportName.Enabled = true;
                    break;
                case "miCloseBTape":
                    serialPBluetoothTape.Close();
                    serialPBluetoothTape.Dispose();
                    cbBluetoothTapePort.Enabled = true;
                    break;
            }
            btnSaveContinue.Text = GlobalVar.Btn_Save;
        }

        private void tabPTest_Leave(object sender, EventArgs e)
        {
            if(GlobalVar.TEST_STARTED_FLAG || btnStart.Visible )
            {
                btnNext_Click(btnNext, new EventArgs());
            }
        }

        private void UpdateComponentTable()
        {
            dgvComponent.Invoke((Action)delegate
            {
                dgvComponent.Rows.Clear();
                int j = GlobalVar.COMPONENT_START_INDEX;
                for (int i = 0;(i < GlobalVar.STRUCT_COMPONENT_LIST.Count); i++)
                {
                    dgvComponent.Rows.Add(++j, GlobalVar.STRUCT_COMPONENT_LIST[i].ComponentId, GlobalVar.STRUCT_COMPONENT_LIST[i].ComponentName, GlobalVar.STRUCT_COMPONENT_LIST[i].ComponentCode,GlobalVar.STRUCT_COMPONENT_LIST[i].ComponentImage);
                }
            });
        }

        private void btnComponentBack_Click(object sender, EventArgs e)
        {
            btnComponentBack.Enabled = false;
            if (GlobalVar.COMPONENT_START_INDEX - GlobalVar.COMPONENT_DISPLAY_COUNT >= 0)
            {
                GlobalVar.COMPONENT_START_INDEX -= GlobalVar.COMPONENT_DISPLAY_COUNT;
                GetComponent();
            }
            btnComponentBack.Enabled = true;
        }

        private void btnComponentNext_Click(object sender, EventArgs e)
        {
            btnComponentNext.Enabled = false;

            GlobalVar.COMPONENT_START_INDEX += GlobalVar.COMPONENT_DISPLAY_COUNT;
            GetComponent();
            btnComponentNext.Enabled = true;
        }

        private void nupCompDisplay_ValueChanged(object sender, EventArgs e)
        {
            GlobalVar.COMPONENT_DISPLAY_COUNT = (int) nupCompDisplay.Value;
            GlobalVar.COMPONENT_START_INDEX = 0;
            GetComponent();
        }

        //private void GetComponent()
        //{
        //    this.Cursor = Cursors.WaitCursor;
        //    FrmProgressBar frm = new FrmProgressBar();
        //    frm.Show();
        //    frm.SetTitleName("Retriving Component Details From Server");
        //    frm.Refresh();
        //    GlobalVar.STRUCT_COMPONENT_LIST = serverProtocol.GetComponent(GlobalVar.COMPONENT_START_INDEX, GlobalVar.COMPONENT_DISPLAY_COUNT, GlobalVar.COMPONENT_SORT);
        //    if (GlobalVar.STRUCT_COMPONENT_LIST.Count == 0 && GlobalVar.COMPONENT_START_INDEX + GlobalVar.COMPONENT_DISPLAY_COUNT <= GlobalVar.TOTAL_COMPONENT_COUNT)
        //    {
        //        GlobalVar.COMPONENT_START_INDEX -= GlobalVar.COMPONENT_DISPLAY_COUNT;
        //        GlobalVar.STRUCT_COMPONENT_LIST = serverProtocol.GetComponent(GlobalVar.COMPONENT_START_INDEX, GlobalVar.COMPONENT_DISPLAY_COUNT, GlobalVar.COMPONENT_SORT);
        //    }
        //    UpdateComponentTable();

        //    frm.Close();
        //    frm.Dispose();
        //    Utility.FlushMouseMessages();
        //    this.Cursor = Cursors.Default;
        //}

        private void GetComponent()
        {
            this.Cursor = Cursors.WaitCursor;
            FrmProgressBar frm = new FrmProgressBar();
            frm.Show();
            frm.SetTitleName("Retriving Component Details From Server");
            frm.Refresh();
            if (tbSearchComponent.Text == string.Empty)
            {
                GlobalVar.STRUCT_COMPONENT_LIST = serverProtocol.GetComponent(GlobalVar.COMPONENT_START_INDEX, GlobalVar.COMPONENT_DISPLAY_COUNT, GlobalVar.COMPONENT_SORT);
            }
            else
            {
                GlobalVar.STRUCT_COMPONENT_LIST = serverProtocol.GetComponent(tbSearchComponent.Text, GlobalVar.COMPONENT_START_INDEX, GlobalVar.COMPONENT_DISPLAY_COUNT);
            }
            if (GlobalVar.STRUCT_COMPONENT_LIST.Count == 0)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, "No More Components to Retrieve.");
                GlobalVar.COMPONENT_START_INDEX -= GlobalVar.COMPONENT_DISPLAY_COUNT;
                GlobalVar.STRUCT_COMPONENT_LIST = serverProtocol.GetComponent(GlobalVar.COMPONENT_START_INDEX, GlobalVar.COMPONENT_DISPLAY_COUNT, GlobalVar.COMPONENT_SORT);
            }
            UpdateComponentTable();

            frm.Close();
            frm.Dispose();
            Utility.FlushMouseMessages();
            this.Cursor = Cursors.Default;
        }

        private void cBoxIsManualEntry_CheckedChanged(object sender, EventArgs e)
        {
            if (cBoxIsManualEntry.CheckState == CheckState.Checked)
            {
                isManualTestSelected = true;
                cBoxIsManualEntry.Visible = true;
                cbManualInstrumentName.Visible = true;
                GlobalVar.TEST_STARTED_FLAG = false;
                updateTableToEnterManualData(false);
            }
            else
            {
                isManualTestSelected = false;
                cBoxIsManualEntry.Visible = false;
                cbManualInstrumentName.Visible = false;
                GlobalVar.TEST_STARTED_FLAG = true;
                updateTableToEnterManualData(true);
            }
        }

        private void tsmiReport_Click(object sender, EventArgs e)
        {
            //if(benchClient == null && bluetoothTapeClient == null && bluetoothClient == null)
            //{
            //    ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect the Embedded Bench And Proceed",Color.Red);
            //    return;
            //}
            if (GlobalVar.gSelectedComponent.ComponentId == 0)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, "Please Select The Component From Component List", Color.Red);
                ToolBox.ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Select The Component From Component List", Color.Red);
            }
            else
            {
                tabCMainPanel.SelectedTab = tabPReport;
            }
        }

        private void dgvComponent_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, "");
            if(e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, "Invalid Selection.");
                GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
                return;
            }
            uint ComponentId = (uint)dgvComponent.Rows[e.RowIndex].Cells[1].Value;

            GlobalVar.gSelectedComponent = GlobalVar.STRUCT_COMPONENT_LIST.Find(item => (item.ComponentId == ComponentId));
            if(GlobalVar.gSelectedComponent.ComponentId == 0)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblComponentStatus, "Invalid Component Selected.", Color.Orange);
                GlobalVar.gSelectedComponent = new STRUCT_COMPONENT();
                return;
            }
            else
            {
                componentValues = new List<bool>();
                btnChangeBatch_Click();
                tabCMainPanel.SelectedTab = tabPReport;
            }
        }

        private void tsmiLogOut_Click(object sender, EventArgs e)
        {
            //serverProtocol.Disconnect();
            GlobalVar.isUserLogOut = true;
            GlobalVar.USER_NAME = string.Empty;
            serialPBench.Close();
            serialPBluetooth.Close();
            serialPBluetoothTape.Close();
            serialPBarcode.Close();
            this.Close();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            serverProtocol.Disconnect();
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
        }

        private void cBoxComponentNameSort_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVar.COMPONENT_START_INDEX = 0;
            GlobalVar.COMPONENT_SORT = cBoxComponentNameSort.Checked;
            GetComponent();
        }

        private void tbSearchComponent_TextChanged(object sender, EventArgs e)
        {
            Utility.SearchValueInDataTable(dgvComponent, tbSearchComponent, 2);
        }

        private void tbBatchNum_TextChanged(object sender, EventArgs e)
        {
            if(tbBatchNum.Text != string.Empty)
            {
                ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, string.Empty);
            }
        }

        private void btnHomeComponent_Click(object sender, EventArgs e)
        {
            if (benchConnected) //(bluetoothVernierConnected || benchConnected || bluetoothTapeConnected) //To be Added
            {
                btnBarcodePortSelect_Click(btnBarcodePortSelect, new EventArgs());
                if (!cBoxBarcodeMode.Checked)
                {
                    GlobalVar.BARCODE_STATUS = GlobalVar.BARCODE_MODE.BARCODE_DISABLE;
                    testMenuStatus = GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT;
                    tsmiComponent_Click(tsmiComponent, new EventArgs());
                }
                else
                {
                    tabCMainPanel.SelectedTab = tabPReport;
                }
            }
            else //To be Added
            {
                ToolBoxWrite.LabelWrite(lblHomeStatus, "Please Connect to the Mechbench and Continue..", Color.Red);
            }
        }

        private void miRefreshAll_Click(object sender, EventArgs e)
        {

        }

        //private void cbBatchNum_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "");
        //}

        private void cbBatchNum_TextChanged(object sender, EventArgs e)
        {
            ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "");
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            ClientProtocol cl = new ClientProtocol(this);
            string measuredVal = "{" + tbMeasuredValue.Text + "}," + rtbTestDescription.Text;
            measuredVal = measuredVal.Replace(" ", "$#");
            string report = tbBenchName.Text + " REPORT: " + cbManualInstrumentName.Text + " " + measuredVal + " " + tbUnit.Text + "\n";

            cl.SendReport(report, true,cBoxTestPass.Checked);
        }

        private void cBoxRetest_CheckedChanged(object sender, EventArgs e)
        {
            {
                tbDeviceNum.Enabled = btnDeviceNumSave.Enabled = cBoxRetest.Checked;
            }
            if (cBoxRetest.Checked)
            {
                tbDeviceNum.Text = "0";
            }
        }

        private void btnDeviceNumSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblWarnBatchNum.Text = string.Empty;
                if (!Utility.ValidateNumber(tbDeviceNum.Text, false, false))
                {
                    lblWarnBatchNum.Text = "Invalid Device Num";
                    return;
                }
                if (tbBatchNum.Visible && testMenuStatus != GlobalVar.TEST_MENU_STATUS.SELECT_COMPONENT)
                {
                    GlobalVar.COMPONENT_NUMBER = serverProtocol.SetDeviceNumber(tbBatchNum.Text, int.Parse(tbDeviceNum.Text), true);
                }
                else
                {
                    GlobalVar.COMPONENT_NUMBER = serverProtocol.SetDeviceNumber(cbBatchNum.Text, int.Parse(tbDeviceNum.Text), false);
                }
                lblComponentNumCount.Text = GlobalVar.COMPONENT_NUMBER.ToString();
                if (GlobalVar.COMPONENT_NUMBER == 0)
                {
                    ToolBox.ToolBoxWrite.Button(new Button[] { btnStart, btnNext }, false, ToolBoxWrite.Property.Visible);
                    ToolBox.ToolBoxWrite.LabelWrite(lblServerResponse, "No Component Is Present To Test");
                    ToolBox.ToolBoxWrite.LabelWrite(lblWarnBatchNum, "No Component Is Present");
                    return;
                }
                tbDeviceNum.Text = GlobalVar.COMPONENT_NUMBER.ToString();
                tbDeviceNum.Enabled = btnDeviceNumSave.Enabled = false;

            }
            catch (Exception ex)
            {

            }
        }

        private void tbSearchComponent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    GlobalVar.COMPONENT_START_INDEX = 0;
                    GetComponent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
