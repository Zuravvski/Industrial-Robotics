using System;
using System.Collections.Generic;

namespace IDE.Common.Models
{
    public class Program
    {
        #region Constructor

        public Program(string name)
        {
            Name = name;
            Content = "";
        }

        #endregion

        #region Properties

        public string Name { private set; get; }
        public string Content { set; get; }

        public IEnumerable<string> GetLines()
        {
<<<<<<< Updated upstream
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
=======
            var lines = Content.Split(new[] { "\r\n"}, StringSplitOptions.None);
            return lines;
>>>>>>> Stashed changes
        }

        #endregion
    }
}
