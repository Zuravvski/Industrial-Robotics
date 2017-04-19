using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;

namespace IDE.Common.Models.Syntax_Check
{
    public class SyntaxChecker
    {
        public ISet<Command> Commands { get; }

        public SyntaxChecker()
        {
            Commands = LazyLibraryLoader.Instance.LoadCommands();
        }

        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }

        public void RemoveCommand(Command command)
        {
            Commands.Remove(command);
        }

        public bool Validate(string line)
        {
            return Commands.Any(command => command.Regex.IsMatch(line));
        }

        public async Task<bool> ValidateAsync(string line)
        {
            return await Task.Run(() => Commands.Any(command => command.Regex.IsMatch(line)));
        }
    }
}
