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
        private const string AssemblyPathTemplate = "Protos_{0}.dll";

        private static string AssemblyPath;

        public static bool TryLoadDefault()
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Protos_*.dll", SearchOption.TopDirectoryOnly);
            if (files != null && files.Length > 0)
            {
                AssemblyPath = files[0];
                return true;
            }
            files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.tmp", SearchOption.TopDirectoryOnly);
            try
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch
            {

            }
            return false;
        }

        public static bool ReloadAll(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }
            string oldPath = Environment.CurrentDirectory;
            if (!oldPath.Equals(Assembly.GetExecutingAssembly().Location))
            {
                Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            _protoAssembly = null;
            _protoItems = null;
            try
            {
                if (AssemblyPath != null)
                {
                    if (File.Exists(AssemblyPath))
                    {
                        File.Move(AssemblyPath, Path.ChangeExtension(AssemblyPath, "tmp"));
                    }
                }
                AssemblyPath = Path.GetFullPath(string.Format(AssemblyPathTemplate, DateTime.Now.Ticks));
                var files = Directory.GetFiles(directory, "*.proto", SearchOption.AllDirectories);
                return ProtoTransformer.Transform(files.ToList(), AssemblyPath);
            }
            catch
            {
                throw;
            }
            finally
            {
                Environment.CurrentDirectory = oldPath;
            }
        }

        private static Assembly _protoAssembly = null;

        private static Assembly ProtoAssembly
        {
            get
            {
                if (_protoAssembly == null)
                {
                    if (File.Exists(AssemblyPath))
                    {
                        _protoAssembly = Assembly.LoadFile(AssemblyPath);
                    }
                }
                return _protoAssembly;
            }
        }

        public static Dictionary<string, string> _protoItems = null;

        public static Dictionary<string, string> ProtoItems
        {
            get
            {
                if (_protoItems == null)
                {
                    lock (SyncRoot)
                    {
                        if (_protoItems == null)
                        {
                            if (ProtoAssembly != null)
                            {
                                var alltypes = ProtoAssembly.GetTypes();
                                _protoItems = alltypes.Where(t => t.IsDefined(typeof(ProtoBuf.ProtoContractAttribute), false)).ToDictionary<Type, string, string>(t => t.Name, t => t.FullName);
                            }
                        }
                    }
                }
                return _protoItems;
            }
        }

        public static ProtobufMsg ConvertToMsgMap(string url, bool isReq, byte[] data)
        {
            var map = ProtocolMap.Maps.FirstOrDefault(item => item.URL.Equals(url));
            if (map == null) return null;
            var protoItem = isReq ? map.Request : map.Response;
            if (protoItem == null) return null;
            var type = ProtoAssembly.GetType(protoItem.ProtoFullName, false, true);
            if (type == null) return null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                var item = ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
                return DumpFromObject(item);
            }
        }

        public static ProtobufMsg DumpFromObject(object obj)
        {
            ProtobufMsg retVal = new ProtobufMsg()
            {
                SubMessages = new List<ProtobufMsg>()
            };
            var type = obj.GetType();
            var properties = type.GetProperties();
            retVal.Name = type.Name;
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(ProtoBuf.ProtoMemberAttribute), false))
                {
                    if (property.PropertyType.IsDefined(typeof(ProtoBuf.ProtoContractAttribute), false))
                    {
                        var val = property.GetValue(obj, null);
                        if (val == null)
                        {
                            var item = new ProtobufMsg()
                            {
                                Name = property.Name,
                                Value = "NULL"
                            };
                            retVal.SubMessages.Add(item);
                        }
                        else
                        {
                            var item = DumpFromObject(property.GetValue(obj, null));
                            item.Name = property.Name;
                            retVal.SubMessages.Add(item);
                        }

                    }
                    else
                    {
                        var item = new ProtobufMsg()
                        {
                            Name = property.Name,
                            Value = property.GetValue(obj, null).ToString()
                        };
                        retVal.SubMessages.Add(item);
                    }
                }
            }
            return retVal;
        }

        private static object SyncRoot = new object();

        private static ProtocolMap _protocolMap = null;

        public static ProtocolMap ProtocolMap
        {
            get
            {
                if (_protocolMap == null)
                {
                    lock (SyncRoot)
                    {
                        if (_protocolMap == null)
                        {
                            LoadMap();
                        }
                    }
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

    [DataContract]
    public class ProtobufMsg
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public string Value { get; set; }

        [DataMember(IsRequired = false)]
        public List<ProtobufMsg> SubMessages { get; set; }

        public override string ToString()
        {
            if (SubMessages == null)
            {
                return string.Format("{{ {0}:{1} }}", Name, Value);
            }
            StringBuilder builder = new StringBuilder();
            foreach (var msg in SubMessages)
            {
                builder.AppendLine(msg.ToString());
            }
            return string.Format("{{ {0}{1}    {2} }}", Environment.NewLine, Name, builder.ToString());
        }
    }
}
