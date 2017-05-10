using System;

namespace IDE.Common.Models.Value_Objects
{
    public class Message
    {

        #region Constructor

        public Message(DateTime time, string message)
        {
            MyTime = time;
            MyMessage = message;
        }

        #endregion

        #region Properties

        public DateTime MyTime { get; }
        public string MyMessage { get; }

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
