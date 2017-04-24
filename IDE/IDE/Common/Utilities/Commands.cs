using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities.Extensions;

namespace IDE.Common.Utilities
{
    public class Commands
    {
        private const string DEFAULT_COMMANDS_PATH = "Commands.xml";

        // Configuration related objects
        public ISet<Command> CommandsMap { get; }

        private string path;

        // Utils
        private readonly XmlDocument document;

        public Commands(string path = DEFAULT_COMMANDS_PATH)
        {
            document = new XmlDocument();
            CommandsMap = new HashSet<Command>();
            this.path = Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute) ? path : DEFAULT_COMMANDS_PATH;
            Load(path);
        }

        public void Load(string path)
        {
            try
            {
                CommandsMap.Clear();
                document.RemoveAll();
                document.Load(path);

                var root = document.SelectSingleNode("/Commands");
                var commandNodes = root.ChildNodes;

                foreach (XmlNode commandNode in commandNodes)
                {
                    var name = commandNode.Attributes["name"].Value;
                    var content = commandNode.Attributes["content"].Value;
                    var regex = new Regex(commandNode.Attributes[2].Value);
                    var type = EnumExtensions
                        .GetValueFromDescription<Command.TypeE>(commandNode.Attributes["type"].Value);
                    var description = commandNode.FirstChild.InnerText;

                    CommandsMap.Add(Command.CreateCommand(name, content, description, regex, type));
                }
                this.path = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
                this.path = DEFAULT_COMMANDS_PATH;
            }
            Session.Instance.SubmitCommands(this.path);
        }

        public void Save(string path)
        {
            if (null == CommandsMap)
            {
                throw new InvalidOperationException("Commands were never loaded to memory.");
            }
            try
            {
                document.RemoveAll();
                foreach (var command in CommandsMap)
                {
                    document.AppendChild(command.ToXML());
                }
                document.Save(path);
                this.path = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not save commands.");
            }
            Session.Instance.SubmitCommands(this.path);
        }

        public void AddCommand(Command command)
        {
            CommandsMap.Add(command);
        }

        public void RemoveCommand(Command command)
        {
            CommandsMap.Remove(command);
        }
    }
}
