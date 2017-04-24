using System;

namespace IDE.Common.Models.Value_Objects
{
    public class Message
    {
        public Message(DateTime time, string message)
        {
            MyTime = time;
            MyMessage = message;
        }
        
        public DateTime MyTime { private set; get; }
        public string MyMessage { private set; get; }

        public string DisplayMessage()
        {
            if (MyTime != null && MyMessage != null)
                return MyTime + " " + MyMessage + "\n";
            else
                return null;
        }
    }
}
