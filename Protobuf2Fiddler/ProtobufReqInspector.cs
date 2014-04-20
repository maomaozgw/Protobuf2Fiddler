using Fiddler;

namespace Protobuf2Fiddler
{
    public class ProtobufReqInspector : ProtobufInspector, IRequestInspector2
    {
        public ProtobufReqInspector()
            : base(true)
        {
            if (!ProtobufHelper.TryLoadDefault())
            {
            }
        }

        public HTTPRequestHeaders headers
        {
            get
            {
                return _headers as HTTPRequestHeaders;
            }
            set
            {
                _headers = value;
            }
        }

        protected override void UpdateView(Session oS)
        {
            base.UpdateView(oS);
        }
    }
}