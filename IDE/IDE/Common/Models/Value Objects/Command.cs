using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IDE.Common.Models.Value_Objects
{
    public class Command
    {
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Content { get; private set; }
        public string Description { get; private set; }
        public Regex Regex { get; private set; }

        protected Command()
        {
        }

        public static Command CreateCommand(string type, string name, string content, string description, Regex regex)
        {
            if (ValidateData(type, name, content, description, regex))
            {
                var command = new Command
                {
                    Type = type,
                    Name = name,
                    Content = content,
                    Description = description,
                    Regex = regex
                };
                return command;
            }
            throw new ArgumentException("Invalid arguments were passed");
        }

        private static bool ValidateData(string type, string name, string content, string description, Regex regex)
        {
            return !string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(content) &&
                   null != description && null != regex;
        }
    }
}
