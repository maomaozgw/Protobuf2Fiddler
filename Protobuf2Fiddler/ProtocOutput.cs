using System.Collections.Generic;
using System.Windows.Controls;

namespace Protobuf2Fiddler
{
    class ProtocOutput : TreeViewItem
    {
        public string ShowValue { get; private set; }

        public static IEnumerable<ProtocOutput> Parse(string data)
        {
            var datas = data.Split('\n');
            int index = 0;
            return ProcessChildren(datas, 0, null, ref index);
        }

        private static IEnumerable<ProtocOutput> ProcessChildren(string[] data, int level, ProtocOutput parent, ref int index)
        {
            string prefix = new string(' ', level * 2);
            string nextPrefix = new string(' ', (level + 1) * 2);
            List<ProtocOutput> retVal = new List<ProtocOutput>();
            ProtocOutput preItem = null;

            while (index < data.Length)
            {
                if (data[index].StartsWith(nextPrefix))
                {
                    if (preItem != null) ProcessChildren(data, level + 1, preItem, ref index);
                }
                else if (data[index].StartsWith(prefix))
                {
                    ProtocOutput item = new ProtocOutput { Header = data[index] };
                    if (parent == null)
                    {
                        retVal.Add(item);
                    }
                    else
                    {
                        parent.Items.Add(item);
                    }

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