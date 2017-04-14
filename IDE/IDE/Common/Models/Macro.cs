using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class Macro
    {
        /// <summary>
        /// Initializes new Macro.
        /// </summary>
        /// <param name="name">How you call macro. (eg. GOC)</param>
        /// <param name="content">What will your macro do. (eg. new List)</param>
        public Macro(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; private set; }
        public string Content { get; private set; }

    }
}
