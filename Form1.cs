using System;
using System.Windows.Forms;

namespace CodeFileTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnStartCreate_Click(object sender, EventArgs e)
        {
            Utils.StartCreateFile(TxtDirPath.Text);
        }
    }
}
