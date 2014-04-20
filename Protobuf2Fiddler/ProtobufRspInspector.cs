using Fiddler;

namespace Protobuf2Fiddler
{
    public class ProtobufRspInspector : ProtobufInspector, IResponseInspector2
    {
        public ProtobufRspInspector()
            : base(false)
        {

        }

        public HTTPResponseHeaders headers
        {
            get
            {
                return _headers as HTTPResponseHeaders;
            }
            set
            {
                _headers = value;
            }
        }

        protected override void UpdateView(Session oS)
        {
            var msgMap = ProtobufHelper.ConvertToMsgMap(Session.oRequest.headers.RequestPath, false, body);
            UpdateView(msgMap);
        }

    }
}