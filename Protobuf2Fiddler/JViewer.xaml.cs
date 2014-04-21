using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace Protobuf2Fiddler
{
    /// <summary>
    /// JViewer.xaml 的交互逻辑
    /// </summary>
    public partial class JViewer : UserControl
    {
        public JViewer()
        {
            InitializeComponent();
        }

        public void SetProtocOutput(string data)
        {
            treeView.ItemsSource = ProtocOutput.Parse(data);
        }

        public void Clean()
        {
            treeView.ItemsSource = null;
        }

        internal void Search(string p)
        {
            //var findItem = treeView.ItemsSource.Cast<ProtocOutput>().FirstOrDefault(item => item.ShowValue.Contains(p));
            //if (findItem != null)
            //{
            //    findItem.IsExpanded = true;
            //}
        }
    }

    public class ProtoDirectoryChangeArgs : EventArgs
    {
        public string Directory { get; private set; }

        public ProtoDirectoryChangeArgs(string directory)
        {
            this.Directory = directory;
        }
    }

    public class ProtoMapChangeArgs : EventArgs
    {
        public string URL { get; set; }

        public string MessageTyp { get; set; }

        public bool IsReq { get; set; }

        public ProtoMapChangeArgs(string url, string messagetype, bool isReq)
        {
            URL = url;
            MessageTyp = messagetype;
            IsReq = isReq;
        }
    }
}
