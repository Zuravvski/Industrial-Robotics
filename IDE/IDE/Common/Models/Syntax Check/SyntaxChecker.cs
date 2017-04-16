using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IDE.Common.Models
{
    public class SyntaxChecker
    {
        private static readonly string DEFAULT_COMMANDS_PATH = "Commands.xml";

        public ISet<Command> Commands { get; }

        public SyntaxChecker()
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

                    Commands.Add(new Command(name, content, description, regex));
                }
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
            }
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
