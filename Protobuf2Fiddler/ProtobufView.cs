﻿using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Protobuf2Fiddler
{
    class ProtobufView : UserControl
    {
        public EventHandler<ProtoDirectoryChangeArgs> ProtoDirectoryChanged;

        public EventHandler<ProtoMapChangeArgs> ProtoMapChanged;

        private SplitContainer splitContainer1;

        private Session _session;
        private Label label2;
        private ComboBox cmbMsgType;
        private TextBox txtDirectory;
        private Label label1;
        private Button btnBrowse;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private JViewer jViewer1;
        private Button btnReload;
        private TextBox txtSearchText;

        private readonly bool _isReqWindow;

        public void SetSession(Session session)
        {
            _session = session;
            UpdateSelectMsgType();
        }

        public void UpdateSelectMsgType()
        {
            if (_session == null) return;
            var item = ProtobufHelper.FindItem(_session.oRequest.headers.RequestPath, _isReqWindow);
            if (item == null) return;
            if (cmbMsgType.Items.Contains(item.MessageType))
            {
                cmbMsgType.SelectedItem = item.MessageType;
            }
        }

        public ProtobufView(bool isReq = true)
        {
            _isReqWindow = isReq;
            InitializeComponent();
        }

        public void CleanView()
        {
            jViewer1.Clean();
            this.cmbMsgType.SelectedItem = null;
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMsgType = new System.Windows.Forms.ComboBox();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.jViewer1 = new Protobuf2Fiddler.JViewer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtSearchText);
            this.splitContainer1.Panel1.Controls.Add(this.btnReload);
            this.splitContainer1.Panel1.Controls.Add(this.btnBrowse);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.cmbMsgType);
            this.splitContainer1.Panel1.Controls.Add(this.txtDirectory);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1MinSize = 55;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.elementHost1);
            this.splitContainer1.Size = new System.Drawing.Size(995, 709);
            this.splitContainer1.SplitterDistance = 56;
            this.splitContainer1.TabIndex = 0;
            // 
            // txtSearchText
            // 
            this.txtSearchText.Location = new System.Drawing.Point(370, 33);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(158, 21);
            this.txtSearchText.TabIndex = 7;
            this.txtSearchText.Visible = false;
            this.txtSearchText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchText_KeyDown);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(534, 6);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(75, 23);
            this.btnReload.TabIndex = 6;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(453, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "MessageType:";
            // 
            // cmbMsgType
            // 
            this.cmbMsgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMsgType.FormattingEnabled = true;
            this.cmbMsgType.Location = new System.Drawing.Point(116, 32);
            this.cmbMsgType.Name = "cmbMsgType";
            this.cmbMsgType.Size = new System.Drawing.Size(233, 20);
            this.cmbMsgType.TabIndex = 2;
            this.cmbMsgType.SelectedIndexChanged += new System.EventHandler(this.cmbMsgType_SelectedIndexChanged);
            // 
            // txtDirectory
            // 
            this.txtDirectory.Location = new System.Drawing.Point(116, 7);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.ReadOnly = true;
            this.txtDirectory.Size = new System.Drawing.Size(330, 21);
            this.txtDirectory.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Protos Directory:";
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(995, 649);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.jViewer1;
            // 
            // ProtobufView
            // 
            this.Controls.Add(this.splitContainer1);
            this.Name = "ProtobufView";
            this.Size = new System.Drawing.Size(995, 709);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public void UpdateMessageTypes(List<string> messageTypes)
        {
            cmbMsgType.BeginUpdate();
            cmbMsgType.Items.Clear();
            cmbMsgType.Items.AddRange(messageTypes.ToArray());
            cmbMsgType.EndUpdate();

        }

        public void UpdateProtoDirectory(string folder)
        {
            this.txtDirectory.Text = folder;
        }

        internal void UpdateView(string jsonData)
        {
            jViewer1.SetProtocOutput(jsonData);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (ProtoDirectoryChanged != null)
                    {
                        ProtoDirectoryChanged(this, new ProtoDirectoryChangeArgs(dialog.SelectedPath));
                    }
                }
            }
        }

        private void cmbMsgType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ProtoMapChanged != null)
            {
                ProtoMapChanged(this, new ProtoMapChangeArgs(_session.oRequest.headers.RequestPath, cmbMsgType.Text, _isReqWindow));
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (ProtoDirectoryChanged != null)
            {
                ProtoDirectoryChanged(this, new ProtoDirectoryChangeArgs(txtDirectory.Text));
            }
        }

        private void txtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                jViewer1.Search(txtSearchText.Text);
            }
        }
    }
}
