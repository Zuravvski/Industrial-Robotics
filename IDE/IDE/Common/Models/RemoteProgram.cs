namespace IDE.Common.Models
{
    public class RemoteProgram
    {
        public RemoteProgram(string name)
        {
            Name = name;
        }

        #region Properties

        public string Name { get; }

        #endregion
    }
}