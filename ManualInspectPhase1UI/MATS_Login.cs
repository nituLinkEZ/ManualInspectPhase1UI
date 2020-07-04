using MATS_Server;
using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolBox;

namespace ManualInspectPhase1UI
{
    public partial class MATS_Login : Form
    {
        public ServerProtocol serverProtocol = new ServerProtocol();
        int LoginAttemptsCount = 0;
        public MATS_Login()
        {
            InitializeComponent();
        }

        private void btnLoginSave_Click(object sender, EventArgs e)
        {
            bool isUserValid = false;
            int user_id = 0;

            string userName = ToolBoxWrite.TextBoxRead(tbLoginUserName);
            string userPassword = ToolBoxWrite.TextBoxRead(tbLoginPassword);

            if (serverProtocol.AuthenticateUser(userName, userPassword))
            {
                isUserValid = true;
            }
            else
            {
                LoginAttemptsCount++;
                ToolBoxWrite.TextBoxWrite(tbLoginUserName, string.Empty);
                ToolBoxWrite.TextBoxWrite(tbLoginPassword, string.Empty);
                if (LoginAttemptsCount == GlobalVar.MATS_SERVER_LOGIN_ATTEMPTS)
                {
                    MessageBox.Show("Invalid Login Attempts For " + GlobalVar.MATS_SERVER_LOGIN_ATTEMPTS.ToString() + "Times. Please Restart The Program.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login Details are Incorrect.", "!Warning");
                }
                return;
            }
            if (isUserValid)
            {
                this.Visible = false;
                GlobalVar.USER_NAME = tbLoginUserName.Text;
                ToolBox.ToolBoxWrite.IS_FORM_LOADED = false;
                FrmMechBench frm = new FrmMechBench(serverProtocol);
                DialogResult dr = frm.ShowDialog();
                if(dr == System.Windows.Forms.DialogResult.Abort)
                {
                    this.Close();
                }
                else if (GlobalVar.isUserLogOut)
                {
                    LoginAttemptsCount = 0;
                    GlobalVar.isUserLogOut = false;
                    this.Visible = true;
                    ToolBoxWrite.TextBoxWrite(tbLoginUserName, string.Empty);
                    ToolBoxWrite.TextBoxWrite(tbLoginPassword, string.Empty);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void btnLoginCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose(true);
        }

        private void tbLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnLoginSave_Click(btnLoginSave, new EventArgs());
            }
        }

        private void MATS_Login_Load(object sender, EventArgs e)
        {
            if(!serverProtocol.ConnectToServer())
            {
                MessageBox.Show("Server Unable to Connect to this Bench.\nPlease Check Server Is Running Or Not? OR Contact the Admin","Client Connection");
                this.Close();
            }
        }

        private void MATS_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            serverProtocol.Disconnect();
            Application.Exit();
           // Environment.Exit(0);
        }
    }
}
