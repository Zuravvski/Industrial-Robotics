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
        public string[] Lines
        {
            get
            {
                string[] lines = Content.Split(new string[] { "\r\n"}, StringSplitOptions.None);
                return lines;
            }
        }

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

        public void SaveProgram(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    File.WriteAllText(@"Programs\" + name + ".txt", Content, Encoding.ASCII);
                }
                catch (Exception) { };
            }
        }

        public void RemoveProgram(Program program)
        {
            if (!string.IsNullOrEmpty(program.Name))
            {
                try
                {
                    File.Delete(@"Programs\" + program.Name + ".txt");
                }
                catch (Exception) { };
            }
        }

        #endregion

    }
}
