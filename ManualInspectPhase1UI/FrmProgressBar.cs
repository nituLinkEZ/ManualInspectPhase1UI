using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualInspectPhase1UI
{
    public partial class FrmProgressBar : Form
    {
        public FrmProgressBar()
        {
            InitializeComponent();
        }

        public void SetTitleName(string title)
        {
            ToolBox.ToolBoxWrite.LabelWrite(lblTitle, title);
        }

        private void FrmProgressBar_Load(object sender, EventArgs e)
        {

        }
    }
}
