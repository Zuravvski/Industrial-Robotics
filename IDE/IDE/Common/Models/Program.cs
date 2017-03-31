using System;
using System.IO;
using System.Text;

namespace IDE.Common.Model
{
    public class Program
    {

        #region Constructor

        public Program(string name)
        {
            Name = name;
            LoadProgram();
        }

        #endregion

        #region Properties

        public string Name { private set; get; }
        public string Content { set; get; }

        #endregion

        #region Actions

        public void LoadProgram()
        {
            if (Name != null && Name != "")
            {
                try
                {
                    Content = File.ReadAllText(@"Programs\" + Name + ".txt", Encoding.ASCII);
                }
                catch (Exception) { };
            }
        }

        public void SaveProgram(string saveAs)
        {
            if (saveAs != null && saveAs != "")
            {
                try
                {
                    File.WriteAllText(@"Programs\" + saveAs + ".txt", Content, Encoding.ASCII);
                }
                catch (Exception) { };
            }
        }

        #endregion

    }
}
