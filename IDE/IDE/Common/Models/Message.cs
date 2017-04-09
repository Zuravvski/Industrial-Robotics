using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class Message
    {
        public Message(string time, string message)
        {
            MyTime = time;
            MyMessage = message;
        }
        
        public string MyTime { private set; get; }
        public string MyMessage { private set; get; }
    }
}
