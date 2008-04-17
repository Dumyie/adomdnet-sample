using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ADOMD_Csharp_example
{
    public partial class frmOptions : Form
    {

        private Font moFont;

        public frmOptions()
        {
            InitializeComponent();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            txtProvider.Text = Settings1.Default.Provider;
            txtFont.Text = Settings1.Default.Font.ToString();
            fontDialog1.Font = Settings1.Default.Font;
            moFont = Settings1.Default.Font;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings1.Default.Provider = txtProvider.Text;
            Settings1.Default.Font = moFont;

            Settings1.Default.Save();

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFont.Text = fontDialog1.Font.ToString();
                moFont = fontDialog1.Font;
            }
        }
    }
}