using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Protobuf2Fiddler
{
    class ProtobufConverter
    {
        public static Dictionary<string, ExpandoObject> GetObjFromProto(string filePath)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, string> AnalisisProto(string filePath)
        {
            throw new NotImplementedException();
        }
    }

    public class Message
    {
        public string Name { get; set; }

        public ItemType ItemType { get; set; }

        public string ValueType { get; set; }

        public int ItemOrder { get; set; }

        public string DefaultValue { get; set; }

        public static Dictionary<string, Message> Collection = new Dictionary<string, Message>();

        private static readonly List<string> DefaultValues = new List<string> { "double", "float", "int32", "int64", "uint32", "uint64", "sint32", "sint64", "fixed32", "fixed64", "sfixed32", "sfixed64", "bool", "string", "bytes" };

        public static List<Message> LoadFromProto(string filePath)
        {
            var retVal = new List<Message>();
            using (TextReader reader = File.OpenText(filePath))
            {
                Regex regex = new Regex("message (?<messageName>[^\\{]+)\\{+\\s+\\n+(?<messageBody>[^\\}]+)\\}+");
                var contents = reader.ReadToEnd();
                var matches = regex.Matches(contents);
                foreach (Match match in matches)
                {
                    Console.WriteLine(match.Groups["messageName"]);
                    retVal.AddRange(MatchItems(match.Groups["messageBody"].Value));
                }
            }
            dynamic obj = new ExpandoObject();
            obj.Param = 1;
            

            return retVal;
        }

        private static List<Message> MatchItems(string messageBody)
        {
            var retVal = new List<Message>();
            Regex regex = new Regex("(?<itemType>[^\\s]+)\\s+(?<valueType>[^\\s]+)\\s+(?<itemName>[^=\\s]+)\\s*=(?<itemOrder>[^\\[;\\s]+).*;");
            var matches = regex.Matches(messageBody);
            //foreach (Match match in matches)
            Parallel.ForEach<Match>(matches.Cast<Match>(), (match) =>
            {
                Message msg = new Message()
                {
                    ItemOrder = int.Parse(match.Groups["itemOrder"].Value.Trim()),
                    Name = match.Groups["itemName"].Value.Trim(),
                    ValueType = match.Groups["valueType"].Value.Trim(),
                    ItemType = (ItemType)Enum.Parse(typeof(ItemType), match.Groups["itemType"].Value.Trim(), true)
                };
                var line = match.Groups[0].Value;
                if (line.ToLower().Contains("[default]"))
                {
                    int index = line.LastIndexOf('=');
                    msg.DefaultValue = line.Substring(index + 1, line.Length - index - 2).Trim();
                }
                lock (retVal)
                {
                    if (!DefaultValues.Contains(msg.ValueType, StringComparer.CurrentCultureIgnoreCase))
                    {
                        if (!Collection.ContainsKey(msg.ValueType))
                        {
                            Collection.Add(msg.ValueType, msg);
                        }
                    }
                    retVal.Add(msg);
                }
            });
            return retVal;
        }
    }

    public enum ItemType
    {
        Required,
        Optional,
        Repeated
    }
}
