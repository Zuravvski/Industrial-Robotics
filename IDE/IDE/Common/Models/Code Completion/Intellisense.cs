using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Models.Code_Completion
{
    public class Intellisense
    {
        private static readonly string DEFAULT_COMMANDS_PATH = "Commands.xml";
        public ISet<Command> Commands { get; }

        public Intellisense()
        {
            Commands = new HashSet<Command>();

            try
            {
                var document = new XmlDocument();
                document.Load(DEFAULT_COMMANDS_PATH);

                var root = document.SelectSingleNode("/Commands");
                var commandNodes = root.ChildNodes;

                foreach (XmlNode commandNode in commandNodes)
                {
                    var name = commandNode.Attributes[0].Value;
                    var content = commandNode.Attributes[1].Value;
                    var regex = new Regex(commandNode.Attributes[2].Value);
                    var description = commandNode.FirstChild.InnerText;

                    Commands.Add(Command.CreateCommand(name, content, description, regex));
                }
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
            }
        }

        public async Task<IEnumerable<Command>> GetCompletionAsync(string context)
        {
            return await Task.Run(() => Commands.Where(command => command.Content.StartsWith(context)));
        }
    }
}
