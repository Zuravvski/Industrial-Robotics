using System.Text.RegularExpressions;

namespace IDE.Common.Models
{
    public class Command
    {
        public string Name { get; private set; }
        public string Content { get; private set; }
        public string Description { get; private set; }
        public Regex Regex { get; private set; }

        public Command(string name, string content, string description, Regex regex)
        {
            Name = name;
            Content = content;
            Description = description;
            Regex = regex;
        }
    }
}
