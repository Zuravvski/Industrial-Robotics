namespace IDE.Common.Models
{
    public class RemoteProgram
    {

        #region Constructor

        public RemoteProgram(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; }

        #endregion

    }
}