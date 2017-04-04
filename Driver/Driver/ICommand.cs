using System.Text.RegularExpressions;

namespace Driver
{
    public abstract class Command
    {
        protected string content;
        protected Regex regex;

        public abstract void Execute();
        public abstract void Undo();

        public virtual bool Match(string text)
        {
            return regex.Match(text).Success;
        }
    }
}
