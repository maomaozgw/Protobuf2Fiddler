using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using Fiddler;

namespace Protobuf2Fiddler
{
    public class ProtobufReqInspector : ProtobufInspector, IRequestInspector2, IBaseInspector2
    {
        public ProtobufReqInspector()
            : base(true)
        {
            if (!ProtobufHelper.LoadDefault())
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

        private Encoding _encoding;

        protected override void UpdateView(Session oS)
        {
            if (this._headers != null)
            {
                this._encoding = Utilities.getEntityBodyEncoding(_headers, oS.RequestBody);
            }
            else
            {
                _encoding = CONFIG.oHeaderEncoding;
            }
            if (!Utilities.IsNullOrEmpty(oS.RequestBody))
            {
                string boundary = Utilities.GetCommaTokenValue(_headers["Content-Type"], "boundary");
                string bodyString = _encoding.GetString(oS.RequestBody).Trim();
                if (!string.IsNullOrWhiteSpace(boundary) && !string.IsNullOrWhiteSpace(bodyString))
                {
                    var binData = GetRequestData(bodyString, boundary);
                    var protocData = ProtobufHelper.Decode(oS.oRequest.headers.RequestPath, true, binData);
                    UpdateView(protocData);
                }
            }
        }

        private byte[] GetRequestData(string bodyString, string boundary)
        {
            var datas = bodyString.Split(new string[] { boundary }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string data in datas)
            {
                string txtData = data.Trim();
                if (!txtData.Equals("--"))
                {
                    int num = txtData.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                    if (num > 0)
                    {
                        string header = txtData.Substring(0, num);
                        if (header.Contains("name=\"data\";"))
                        {
                            string strData = txtData.Substring(num + 4, txtData.Length - num - 8);
                            byte[] bytes = _encoding.GetBytes(strData);
                            return bytes;
                        }
                    }
                }
            }
            return new byte[] { };
        }
    }
}