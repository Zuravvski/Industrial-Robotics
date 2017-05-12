using System.Collections.ObjectModel;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Models
{
    public class MessageList
    {

        #region Constructor

        public MessageList()
        {
            Messages = new ObservableCollection<Message>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Message list.
        /// </summary>
        public ObservableCollection<Message> Messages { get; }

        #endregion

        #region Actions

        /// <summary>
        /// Adds message to the message list.
        /// </summary>
        /// <param name="message">Message to add.</param>
        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Removes message from the message list.
        /// </summary>
        /// <param name="message">Message to remove.</param>
        public void RemoveMessage(Message message)
        {
            Messages.Remove(message);
        }

        #endregion

    }
}
