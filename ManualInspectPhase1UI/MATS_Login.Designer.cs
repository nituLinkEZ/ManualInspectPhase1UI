namespace ManualInspectPhase1UI
{
    partial class MATS_Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.tbLoginUserName = new System.Windows.Forms.TextBox();
            this.tbLoginPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoginSave = new System.Windows.Forms.Button();
            this.btnLoginCancel = new System.Windows.Forms.Button();
            this.lblLoginWarning = new System.Windows.Forms.Label();
            this.pnlLoginHeader = new System.Windows.Forms.Panel();
            this.lblProductName = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.pnlLoginHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Name:";
            // 
            // tbLoginUserName
            // 
            this.tbLoginUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLoginUserName.Location = new System.Drawing.Point(102, 71);
            this.tbLoginUserName.Name = "tbLoginUserName";
            this.tbLoginUserName.Size = new System.Drawing.Size(182, 21);
            this.tbLoginUserName.TabIndex = 2;
            // 
            // tbLoginPassword
            // 
            this.tbLoginPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLoginPassword.Location = new System.Drawing.Point(102, 114);
            this.tbLoginPassword.Name = "tbLoginPassword";
            this.tbLoginPassword.PasswordChar = '*';
            this.tbLoginPassword.Size = new System.Drawing.Size(182, 21);
            this.tbLoginPassword.TabIndex = 4;
            this.tbLoginPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbLoginPassword_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(23, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // btnLoginSave
            // 
            this.btnLoginSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoginSave.Location = new System.Drawing.Point(102, 167);
            this.btnLoginSave.Name = "btnLoginSave";
            this.btnLoginSave.Size = new System.Drawing.Size(86, 30);
            this.btnLoginSave.TabIndex = 5;
            this.btnLoginSave.Text = "Login";
            this.btnLoginSave.UseVisualStyleBackColor = true;
            this.btnLoginSave.Click += new System.EventHandler(this.btnLoginSave_Click);
            // 
            // btnLoginCancel
            // 
            this.btnLoginCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoginCancel.Location = new System.Drawing.Point(194, 167);
            this.btnLoginCancel.Name = "btnLoginCancel";
            this.btnLoginCancel.Size = new System.Drawing.Size(90, 30);
            this.btnLoginCancel.TabIndex = 6;
            this.btnLoginCancel.Text = "Cancel";
            this.btnLoginCancel.UseVisualStyleBackColor = true;
            this.btnLoginCancel.Click += new System.EventHandler(this.btnLoginCancel_Click);
            // 
            // lblLoginWarning
            // 
            this.lblLoginWarning.AutoSize = true;
            this.lblLoginWarning.ForeColor = System.Drawing.Color.Red;
            this.lblLoginWarning.Location = new System.Drawing.Point(99, 137);
            this.lblLoginWarning.Name = "lblLoginWarning";
            this.lblLoginWarning.Size = new System.Drawing.Size(0, 13);
            this.lblLoginWarning.TabIndex = 7;
            // 
            // pnlLoginHeader
            // 
            this.pnlLoginHeader.Controls.Add(this.lblProductName);
            this.pnlLoginHeader.Controls.Add(this.label23);
            this.pnlLoginHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLoginHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLoginHeader.Name = "pnlLoginHeader";
            this.pnlLoginHeader.Size = new System.Drawing.Size(301, 55);
            this.pnlLoginHeader.TabIndex = 8;
            // 
            // lblProductName
            // 
            this.lblProductName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblProductName.Font = new System.Drawing.Font("Calibri", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.ForeColor = System.Drawing.Color.Gray;
            this.lblProductName.Location = new System.Drawing.Point(0, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(99, 55);
            this.lblProductName.TabIndex = 4;
            this.lblProductName.Text = "MATS";
            this.lblProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.Transparent;
            this.label23.Font = new System.Drawing.Font("Calibri", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.Black;
            this.label23.Location = new System.Drawing.Point(93, 8);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(201, 40);
            this.label23.TabIndex = 5;
            this.label23.Text = "VISUALBENCH LOGIN";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MATS_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 222);
            this.Controls.Add(this.pnlLoginHeader);
            this.Controls.Add(this.lblLoginWarning);
            this.Controls.Add(this.btnLoginCancel);
            this.Controls.Add(this.btnLoginSave);
            this.Controls.Add(this.tbLoginPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbLoginUserName);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MATS_Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MATS_Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MATS_Login_FormClosing);
            this.Load += new System.EventHandler(this.MATS_Login_Load);
            this.pnlLoginHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbLoginUserName;
        private System.Windows.Forms.TextBox tbLoginPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLoginSave;
        private System.Windows.Forms.Button btnLoginCancel;
        private System.Windows.Forms.Label lblLoginWarning;
        private System.Windows.Forms.Panel pnlLoginHeader;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label label23;
    }
}