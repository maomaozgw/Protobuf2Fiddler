using ProtoBuf.CodeGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Protobuf2Fiddler
{
    public class ProtobufHelper
    {
        public static bool LoadDefault()
        {
            LoadMap();
            return LoadDirectory(ProtocolMap.ProtoDirectory);
        }

        public static bool LoadDirectory(string directory)
        {
            if (!Directory.Exists(directory)) return false;
            var fileList = Directory.GetFiles(directory, "*.proto", SearchOption.TopDirectoryOnly);
            foreach (var file in fileList)
            {
                var fileDesc = ProtoTransformer.LoadFilesAsFileDescription(file);
                var types =
                    fileDesc.file.SelectMany(
                        f => f.message_type.Select(p => string.Format("{0}.{1}", f.package, p.name)));
                if (!ProtoItems.ContainsKey(file))
                {
                    ProtoItems.Add(file, types.ToList());
                }
            }
            ProtocolMap.ProtoDirectory = directory;
            _protoTypes = null;
            return true;
        }

        public static Dictionary<string, List<string>> ProtoItems = new Dictionary<string, List<string>>();

        private static List<string> _protoTypes = null;

        public static List<string> ProtoTypes
        {
            get
            {
                if (_protoTypes == null)
                {
                    _protoTypes = ProtoItems.SelectMany(i => i.Value).ToList();
                }
                return _protoTypes;
            }
        }

        public static string Decode(string url, bool isReq, byte[] data)
        {
            var map = ProtocolMap.Maps.FirstOrDefault(item => item.URL.Equals(url));
            var retVal = string.Empty;
            if (map != null)
            {
                var protoItem = isReq ? map.Request : map.Response;
                if (protoItem != null)
                {
                    try
                    {
                        retVal = ProtoTransformer.DecodeWithProtoFile(data, protoItem.ProtoFile, protoItem.MessageType);
                    }
                    catch
                    {
                        retVal = string.Empty;
                    }
                }
            }
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = ProtoTransformer.DecodeRaw(data);
            }
            return retVal;
        }

        private static ProtocolMap _protocolMap = null;

        public static ProtocolMap ProtocolMap
        {
            get
            {
                if (_protocolMap == null)
                {
                    LoadMap();
                }
                return _protocolMap;
            }
        }

        const string MapFile = "protocolmap.xml";
        private static void LoadMap()
        {
            string oldPath = Environment.CurrentDirectory;
            if (!oldPath.Equals(Assembly.GetExecutingAssembly().Location))
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProtocolMap));
            try
            {
                using (XmlReader reader = XmlReader.Create(MapFile))
                {
                    _protocolMap = serializer.Deserialize(reader) as ProtocolMap;
                }
            }
            catch
            {

            }
            finally
            {
                Environment.CurrentDirectory = oldPath;
            }
            if (_protocolMap == null)
            {
                _protocolMap = new ProtocolMap()
                {
                    Maps = new List<MapItem>()
                };
            }

        }

        public static void SaveMap()
        {
            string oldPath = Environment.CurrentDirectory;
            if (!oldPath.Equals(Assembly.GetExecutingAssembly().Location))
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProtocolMap));
                using (XmlWriter writer = XmlWriter.Create(MapFile))
                {
                    serializer.Serialize(writer, ProtocolMap);
                }
            }
            catch
            {

            }
            finally
            {
                Environment.CurrentDirectory = oldPath;
            }
        }
    }
}
