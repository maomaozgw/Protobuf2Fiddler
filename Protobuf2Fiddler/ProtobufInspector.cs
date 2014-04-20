using System;
using Fiddler;
using System.Windows.Forms;


namespace Protobuf2Fiddler
{
    public class ProtobufInspector : Inspector2
    {
        readonly ProtobufView _view;

        public ProtobufInspector()
        {
            _view = new ProtobufView(true);
        }

        public ProtobufInspector(bool isReq = true)
        {
            _view = new ProtobufView(isReq);
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
                else
                {

                    _view.UpdateView(null);
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

        internal void UpdateView(ProtobufMsg msgMap)
        {
            _view.UpdateView(msgMap);
        }
    }
}
