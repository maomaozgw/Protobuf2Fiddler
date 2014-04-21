using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Protobuf2Fiddler
{
    [Serializable]
    public class ProtocolMap
    {
        [XmlElement]
        public string ProtoDirectory { get; set; }

        [XmlElement]
        public List<MapItem> Maps { get; set; }
    }

    [Serializable]
    public class MapItem
    {
        [XmlElement]
        public string URL { get; set; }

        [XmlElement]
        public ProtocolItem Request { get; set; }

        [XmlElement]
        public ProtocolItem Response { get; set; }
    }

    [Serializable]
    public class ProtocolItem
    {
        [XmlElement]
        public string ProtoFile { get; set; }

        [XmlElement]
        public string MessageType { get; set; }
    }
}
