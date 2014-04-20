using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Protobuf2Fiddler
{
    public partial class frmChooseProto : Form
    {
        public frmChooseProto()
        {
            InitializeComponent();
        }

        public void SetURL(string url)
        {
            this.lblURL.Text = url;
        }

        public void SetProtos(List<string> protos)
        {
            this.cmbProtos.Items.AddRange(protos.ToArray());
        }

        public string ProtoName
        {
            get;
            private set;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ProtoName = cmbProtos.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
