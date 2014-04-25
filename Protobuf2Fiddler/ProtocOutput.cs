using System.Collections.Generic;

namespace Protobuf2Fiddler
{
    class ProtocOutput  
    {
        public string ShowValue { get; private set; }

        public IEnumerable<ProtocOutput> Children { get; private set; }

        public static IEnumerable<ProtocOutput> Parse(string data)
        {
            var datas = data.Split('\n');
            int index = 0;
            return ProcessChildren(datas, 0, ref index);
        }

        private static IEnumerable<ProtocOutput> ProcessChildren(string[] data, int level, ref int index)
        {
            string prefix = new string(' ', level * 2);
            string nextPrefix = new string(' ', (level + 1) * 2);
            List<ProtocOutput> retVal = new List<ProtocOutput>();
            ProtocOutput preItem = null;

            while (index < data.Length)
            {
                if (data[index].StartsWith(nextPrefix))
                {
                    if (preItem != null) preItem.Children = ProcessChildren(data, level + 1, ref index);
                }
                else if (data[index].StartsWith(prefix))
                {
                    ProtocOutput item = new ProtocOutput { ShowValue = data[index] };
                    retVal.Add(item);
                    preItem = item;
                    index++;
                }
                else
                {
                    return retVal;
                }

            }
            return retVal;
        }
    }
}