using System.Linq;
using ProtoBuf.CodeGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace ProtobufTest
{
    class Program
    {
        static Dictionary<string, string> Parameters = new Dictionary<string, string>() 
        {
            {"BDUSS","Voa3U3Zlg0aGVkSVJOWH5OQzEyTTg5ajVQVFd1MktIWHVwNkZSSjJjZ1E4SFpUQVFBQUFBJCQAAAAAAAAAAAEAAADYBQAAYWJjAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABBjT1MQY09TZE|731c8630afee0ca4d401e30c8d2b7bf9"},
            {"_client_id","wappc_1397711637514_858"},
            {"_client_type","2"},
            {"_client_version","6.0.0"},
            {"_phone_imei","860173013275544"},
            {"cuid","81566934889971CB5EAEE4C2A251CB80|445572310371068"},
            {"from","tieba"},
            {"model","MI-ONE Plus"},
            {"sign","21B428AD821EE27F51F06C2345456D61"},
            {"stErrorNums","1"},
            {"stMethod","1"},
            {"stMode","1"},
            {"stSize","0"},
            {"stTime","100"},
            {"stTimesNum","1"},
            {"timestamp","1397711906579"}
        };

        static void Main(string[] args)
        {
            //CreateShortRequest(@"frsreq.bin");
            using (FileStream strem = File.OpenRead("rsp.bin"))
            {
                byte[] buffer = new byte[strem.Length];
                strem.Read(buffer, 0, (int)strem.Length);
                //Console.Write(ProtoTransformer.DecodeRaw(buffer));
                var desc = ProtoTransformer.LoadFilesAsFileDescription(new List<string>() { @"E:\workspaces\LCS\frsPage\frsPageRes.proto" });
                //foreach (var proto in desc.file)
                //{
                //    foreach (var message in proto.message_type)
                //    {
                //        Console.WriteLine(message.name);
                //    }
                //}
                var messages = desc.file.SelectMany(f => f.message_type.Select(p => string.Format("{0}.{1}", f.package, p.name)));
                foreach (var message in messages)
                {
                    Console.WriteLine(message);
                }
                Console.Write(ProtoTransformer.DecodeWithProtoFile(buffer, @"E:\workspaces\LCS\frsPage\frsPageRes.proto", "tbclient.FrsPage.FrsPageResIdl"));
            }
            Console.ReadKey(true);
        }

        static HttpWebRequest CreateShortRequest(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "--\r\n");
            //请求头部信息
            StringBuilder sb = new StringBuilder();
            foreach (var param in Parameters)
            {
                sb.Append("--");
                sb.AppendLine(strBoundary);
                sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", param.Key);
                sb.Append("\r\n");
                sb.AppendLine(param.Value);
            }
            sb.Append("--");
            sb.AppendLine(strBoundary);
            //, Path.GetFileName(filePath)
            sb.AppendFormat("Content-Disposition: form-data; name=\"data\"; filename=\"file\" \r\n\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
            // 根据uri创建HttpWebRequest对象
            var httpReq = (HttpWebRequest)WebRequest.Create(new Uri("http://cq01-testing-platqa2224.vm.baidu.com:8080/c/f/frs/page?cmd=301001"));
            httpReq.Proxy = new WebProxy("127.0.0.1:8888");
            httpReq.Method = "POST";
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            httpReq.Headers.Add("x_bd_data_type", "protobuf");
            httpReq.Headers.Add("Accept-Encoding", "gzip");
            httpReq.Headers.Add("Charset", "UTF-8");
            httpReq.Headers.Add("net", "3");
            httpReq.Headers.Add("client_user_token", "1496");
            httpReq.Headers.Add("Cookie", "ka=open");
            httpReq.Headers.Add("sid", "743e73bcd81c4b6a");
            httpReq.UserAgent = "BaiduTieba for Android 6.0.0";
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                int bufferLength = 409600;
                byte[] buffer = new byte[bufferLength];
                Stream postStream = httpReq.GetRequestStream();
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                int size = r.Read(buffer, 0, bufferLength);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    size = r.Read(buffer, 0, bufferLength);
                }
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();
                WebResponse response = httpReq.GetResponse();
                Stream s = response.GetResponseStream();

                using (BinaryReader bReader = new BinaryReader(s))
                {
                    using (FileStream fWriter = new FileStream("rsp.bin", FileMode.Truncate))
                    {
                        while (true)
                        {
                            size = bReader.Read(buffer, 0, bufferLength);
                            if (size <= 0) break;
                            fWriter.Write(buffer, 0, size);
                        }
                        fWriter.SetLength(fWriter.Position);
                        fWriter.Close();

                    }
                    bReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                fs.Close();
                r.Close();
            }
            return null;
        }

        static void MakeDll()
        {
            List<string> files = new List<string>() { @"E:\workspaces\LCS\FRSPAGE\frsPageReq.proto", @"E:\workspaces\LCS\FRSPAGE\frsPageRes.proto", @"E:\workspaces\LCS\FRSPAGE\client.proto" };
            ProtoTransformer.Transform(files, "frs.dll");
            Console.ReadKey();
        }
    }
}
