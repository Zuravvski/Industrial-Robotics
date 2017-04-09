using System;
using System.Collections.Generic;

namespace IDE.Common.Models
{
    // TODO: Apply prototype design pattern
    public class Program
    {
        public Program(string name)
        {
            Name = name;
            Content = "";
        }

        #region Properties

        public string Name { private set; get; }
        public string Path { set; get; }
        public string Content { set; get; }

        #endregion

        public IEnumerable<string> GetLines()
        {
            var lines = Content.Split(new[] { "\r\n" }, StringSplitOptions.None);
            return lines;
        }
    }
}
