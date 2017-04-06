using System.Text.RegularExpressions;

namespace Driver
{
    public class Command
    {
        public readonly string name;
        public readonly string content;
        public readonly Regex regex;
        public readonly string description;

        public Command(string name, string content, string pattern, string description)
        {
            this.name = name;
            this.content = content;
            this.description = description;
            regex = new Regex(pattern);
        }

        public virtual bool Match(string text)
        {
            return regex.Match(text).Success;
        }
    }
}
