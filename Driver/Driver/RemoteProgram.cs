using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver
{
    public sealed class RemoteProgram
    {
        public string Name { get; }
        public int Size { get; }
        public string Timestamp { get; }

        private RemoteProgram(string name, int size, string timestamp)
        {
            Name = name;
            Size = size;
            Timestamp = timestamp;
        }

        public static RemoteProgram Create(string info)
        {
            var splittedInfo = info.Split(';');
            //if (splittedInfo.Length != 6)
            //    throw new ArgumentException();
            
            int startIndex = splittedInfo[0].IndexOf("QoK") + "QoK".Length;
            int endIndex = splittedInfo[0].IndexOf(".RE2", startIndex);
            var name = splittedInfo[0].Substring(startIndex, endIndex - startIndex);

            var size = Convert.ToInt32(splittedInfo[1]);
            var timestamp = splittedInfo[2];

            return new RemoteProgram(name, size, timestamp);
        }
    }
}
