using System.Collections.ObjectModel;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Models
{
    public class MessageList
    {
        public MessageList()
        {
            Messages = new ObservableCollection<Message>();
        }

        public ObservableCollection<Message> Messages { get; }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public void RemoveMessage(Message message)
        {
            Messages.Remove(message);
        }
    }
}
