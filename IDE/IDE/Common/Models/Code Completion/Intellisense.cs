using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;

namespace IDE.Common.Models.Code_Completion
{
    public class Intellisense
    {
        public ISet<Command> Commands { get; }

        public Intellisense()
        {
            Commands = Session.Instance.Commands.CommandsMap;
        }

        public IEnumerable<Command> GetCompletion(string context)
        {
            return Commands.Where(command => command.Content.StartsWith(context));
        }
        
        public async Task<IEnumerable<Command>> GetCompletionAsync(string context)
        {
            return await Task.Run(() => Commands.Where(command => command.Content.StartsWith(context)));
        }
    }
}
