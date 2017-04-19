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

        public string Name { get; }
        public string Path { get; set; }
        public string Content { get; set; }
        //public Timestamp Timestamp { get; private set; }
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
