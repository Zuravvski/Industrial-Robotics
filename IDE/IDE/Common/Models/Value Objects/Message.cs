using System;

namespace IDE.Common.Models.Value_Objects
{
    /// <summary>
    /// Message class
    /// </summary>
    public class Message
    {

        #region Enums

        /// <summary>
        /// 
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// The send
            /// </summary>
            Send,
            /// <summary>
            /// The received
            /// </summary>
            Received
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="message">The message.</param>
        /// <param name="myType">My type.</param>
        public Message(DateTime time, string message, Type myType)
        {
            MyTime = time;
            MyMessage = message;
            MyType = myType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets my time.
        /// </summary>
        /// <value>
        /// My time.
        /// </value>
        public DateTime MyTime { get; }
        /// <summary>
        /// Gets my message.
        /// </summary>
        /// <value>
        /// My message.
        /// </value>
        public string MyMessage { get; }
        /// <summary>
        /// Gets my type.
        /// </summary>
        /// <value>
        /// My type.
        /// </value>
        public Type MyType { get; }

        #endregion

        #region Actions

        public string DisplayMessage()
        {
            if (MyMessage != null)
                return $"{MyTime:dd-MM-yyyy HH:mm:ss}" + ": " + MyMessage + Environment.NewLine;
            return null;
        }

        #endregion

    }
}
