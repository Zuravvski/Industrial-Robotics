using System;

namespace IDE.Common.Models.Value_Objects
{
    public class Message
    {

        #region Enums

        public enum Type
        {
            Send,
            Received
        }

        #endregion

        #region Constructor

        public Message(DateTime time, string message, Type myType)
        {
            MyTime = time;
            MyMessage = message;
            MyType = myType;
        }

        #endregion

        #region Properties

        public DateTime MyTime { get; }
        public string MyMessage { get; }
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
