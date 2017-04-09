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
