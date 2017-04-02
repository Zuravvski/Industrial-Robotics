using System;
using System.IO;
using System.Text;

namespace IDE.Common.Models
{
    public class Program : IProgram
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
            if (!string.IsNullOrEmpty(Name))
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
            if (!string.IsNullOrEmpty(saveAs))
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
