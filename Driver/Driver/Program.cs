using System;
using System.Collections.Generic;
using System.Linq;

namespace Driver
{
    public class Program
    {
        public Program(string name)
        {
            Name = name;
            Content = string.Empty;
        }

        #region Properties

        public string Name { private set; get; }
        public string Path { set; get; }
        public string Content { set; get; }
        //public List<Position> Positions;

        #endregion

        public List<string> GetLines()
        {
            var lines = Content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return lines;
        }

        /// <summary>
        /// Provides a shallow copy of program
        /// </summary>
        /// <returns>Shallow copy of program</returns>
        public Program Clone()
        {
            return (Program)MemberwiseClone();
        }
    }
}
