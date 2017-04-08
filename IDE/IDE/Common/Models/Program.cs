using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace IDE.Common.Models
{
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
        public string[] Lines
        {
            get
            {
                var lines = Content.Split(new [] { "\r\n"}, StringSplitOptions.None);
                return lines;
            }
        }
        
        public IEnumerable<string> GetLines()
        {
            var lines = Content.Split(new[] { "\r\n" }, StringSplitOptions.None);
            return lines;
        }
        #endregion
    }
}
