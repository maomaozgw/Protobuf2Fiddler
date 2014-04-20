using Fiddler;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Protobuf2Fiddler
{
    class ProtobufView : UserControl
    {
        private SplitContainer splitContainer1;
        private Button btnSetMap;
        private Button btnReloadAll;
        private TreeView treeView1;

        private Session _session;

        private bool _isReqWindow;

        public void SetSession(Session session)
        {
            _session = session;
            if (_session == null)
            {
                btnSetMap.Enabled = false;
            }
            else
            {
                btnSetMap.Enabled = true;
            }
        }

        public ProtobufView(bool isReq = true)
        {
            _isReqWindow = isReq;
            InitializeComponent();
        }

        public void UpdateView()
        {

        }

        public void CleanView()
        {
            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();
            this.treeView1.EndUpdate();
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnReloadAll = new System.Windows.Forms.Button();
            this.btnSetMap = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSetMap);
            this.splitContainer1.Panel1.Controls.Add(this.btnReloadAll);

            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(995, 709);
            this.splitContainer1.SplitterDistance = 35;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnReloadAll
            // 
            this.btnReloadAll.Location = new System.Drawing.Point(13, 5);
            this.btnReloadAll.Name = "btnReloadAll";
            this.btnReloadAll.Size = new System.Drawing.Size(127, 23);
            this.btnReloadAll.TabIndex = 0;
            this.btnReloadAll.Text = "Reload All Protos";
            this.btnReloadAll.UseVisualStyleBackColor = true;
            this.btnReloadAll.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnSetMap
            // 
            this.btnSetMap.Location = new System.Drawing.Point(155, 5);
            this.btnSetMap.Name = "btnSetMap";
            this.btnSetMap.Size = new System.Drawing.Size(131, 23);
            this.btnSetMap.TabIndex = 0;
            this.btnSetMap.Text = "Set Protos Map";
            this.btnSetMap.UseVisualStyleBackColor = true;
            this.btnSetMap.Click += new System.EventHandler(this.btnSetProto_Click);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(995, 670);
            this.treeView1.TabIndex = 0;
            // 
            // ProtobufView
            // 
            this.Controls.Add(this.splitContainer1);
            this.Name = "ProtobufView";
            this.Size = new System.Drawing.Size(995, 709);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (ProtobufHelper.ReloadAll(dialog.SelectedPath))
                    {
                        MessageBox.Show(@"载入Proto文件成功");
                    }
                    else
                    {
                        MessageBox.Show(@"载入Proto文件失败");
                    }
                }
            }
        }

        private void btnSetProto_Click(object sender, EventArgs e)
        {
            using (frmChooseProto chooser = new frmChooseProto())
            {
                chooser.SetURL(_session.oRequest.headers.RequestPath);
                chooser.SetProtos(ProtobufHelper.ProtoItems.Keys.ToList());
                if (chooser.ShowDialog() == DialogResult.OK)
                {
                    var name = chooser.ProtoName;
                    ProtocolItem protocolItem = new ProtocolItem()
                    {
                        ProtoName = name,
                        ProtoFullName = ProtobufHelper.ProtoItems[name]
                    };

                    var item = ProtobufHelper.ProtocolMap.Maps.FirstOrDefault();
                    if (item == null)
                    {
                        item = new MapItem()
                        {
                            URL = _session.oRequest.headers.RequestPath
                        };
                    }
                    if (_isReqWindow)
                    {
                        item.Request = protocolItem;
                    }
                    else
                    {
                        item.Response = protocolItem;
                    }
                    ProtobufHelper.ProtocolMap.Maps.Add(item);
                    ProtobufHelper.SaveMap();
                }
            }

        }


        internal void UpdateView(ProtobufMsg msgMap)
        {
            this.treeView1.BeginUpdate();
            this.treeView1.Nodes.Clear();
            if (msgMap != null)
            {
                this.treeView1.Nodes.Add(BuildNode(msgMap));
            }
            this.treeView1.EndUpdate();
        }

        private static TreeNode BuildNode(ProtobufMsg item)
        {
            TreeNode node = new TreeNode();
            if (item.SubMessages == null)
            {
                node.Text = string.Format("{0}:{1}", item.Name, item.Value);
            }
            else
            {
                node.Text = item.Name;
                foreach (var subItem in item.SubMessages)
                {
                    node.Nodes.Add(BuildNode(subItem));
                }
            }
            return node;
        }

        internal void ShowDefault()
        {
            this.treeView1.ResetText();
        }
    }
}
