using System;
using System.Text.RegularExpressions;

namespace IDE.Common.Models.Value_Objects
{
    public class Command
    {
        public string Name { get; private set; }
        public string Content { get; private set; }
        public string Description { get; private set; }
        public Regex Regex { get; private set; }

        protected Command()
        {
        }

        public static Command CreateCommand(string name, string content, string description, Regex regex)
        {
            if(ValidateData(name, content, description, regex))
            {
                var command = new Command
                {
                    Name = name,
                    Content = content,
                    Description = description,
                    Regex = regex
                };
                return command;
            }
            throw new ArgumentException("Invalid arguments were passed");
        }

        private static bool ValidateData(string name, string content, string description, Regex regex)
        {
            return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(content) &&
                   null != description && null != regex;
        }
    }
}
