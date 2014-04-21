using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Fiddler;
using System.Windows.Forms;


namespace Protobuf2Fiddler
{
    public class ProtobufInspector : Inspector2
    {
        readonly ProtobufView _view;

        private static List<ProtobufView> Views = new List<ProtobufView>();

        public ProtobufInspector()
            : this(true)
        {
        }

        public ProtobufInspector(bool isReq = true)
        {
            _view = new ProtobufView(isReq);
            Views.Add(_view);
            ProtobufHelper.LoadDefault();
            _view.UpdateProtoDirectory(ProtobufHelper.ProtocolMap.ProtoDirectory);
            _view.UpdateMessageTypes(ProtobufHelper.ProtoTypes);
            _view.ProtoDirectoryChanged += ProtoDirectoryChanged;
            _view.ProtoMapChanged += ProtoMapChanged;
        }

        private void ProtoMapChanged(object sender, ProtoMapChangeArgs protoMapChangeArgs)
        {
            var findItem =
                ProtobufHelper.ProtoItems.FirstOrDefault(protoItem => protoItem.Value.Contains(protoMapChangeArgs.MessageTyp));
            if (findItem.Value == null)
            {
                return;
            }
            ProtocolItem protocolItem = new ProtocolItem()
            {
                ProtoFile = findItem.Key,
                MessageType = protoMapChangeArgs.MessageTyp
            };

            var item = ProtobufHelper.ProtocolMap.Maps.FirstOrDefault(i => i.URL.Equals(protoMapChangeArgs.URL));
            if (item == null)
            {
                item = new MapItem()
                {
                    URL = protoMapChangeArgs.URL
                };
            }
            if (protoMapChangeArgs.IsReq)
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

        private void ProtoDirectoryChanged(object sender, ProtoDirectoryChangeArgs protoDirectoryChangeArgs)
        {
            if (ProtobufHelper.LoadDirectory(protoDirectoryChangeArgs.Directory))
            {
                foreach (var view in Views)
                {
                    view.UpdateProtoDirectory(protoDirectoryChangeArgs.Directory);
                    view.UpdateMessageTypes(ProtobufHelper.ProtoTypes);
                }
            }
        }

        public override void AddToTab(TabPage o)
        {
            o.Text = @"Protobuf";
            o.Controls.Add(_view);
            o.Controls[0].Dock = DockStyle.Fill;
            _view.Dock = DockStyle.Fill;
        }

        protected Session Session;

        public override void AssignSession(Session oS)
        {
            base.AssignSession(oS);
            Session = oS;
            _view.SetSession(oS);
            try
            {
                if (IsBaiduPacket())
                {
                    UpdateView(oS);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        protected virtual void UpdateView(Session oS)
        {
        }

        public override int GetOrder()
        {
            return 0;
        }

        public void Clear()
        {
            _view.CleanView();
        }

        public bool bDirty
        {
            get { return false; }
            set { }
        }

        public bool bReadOnly
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        protected HTTPHeaders _headers;

        private byte[] _body;

        public byte[] body
        {
            get { return _body; }
            set
            {
                _body = value;
            }
        }

        private bool IsBaiduPacket()
        {
            return _headers != null && _headers.Exists("x_bd_data_type");
        }

        internal void UpdateView(string jsonData)
        {
            _view.UpdateView(jsonData);
        }
    }
}
