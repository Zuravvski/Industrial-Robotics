namespace IDE.Common.Models
{
    public class Macro
    {

        #region Constructor

        /// <summary>
        /// Initializes new Macro.
        /// </summary>
        /// <param name="name">How you call macro. (eg. GOC)</param>
        /// <param name="content">What will your macro do. (eg. new List)</param>
        public Macro(string name, string content)
        {
            Name = name;
            Content = content;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }
        public string Content { get; set; }

        #endregion

    }
}
