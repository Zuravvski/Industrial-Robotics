using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities.Extensions;

namespace IDE.Common.Utilities
{
    // TODO: Useless class - dispose
    public class ConfigurationService
    {
        private const string DEFAULT_COMMANDS_PATH = "Commands.xml";

        private static readonly Lazy<ConfigurationService> instance = new Lazy<ConfigurationService>(() => new ConfigurationService());
        public static ConfigurationService Instance => instance.Value;

        // Configuration related objects
        private ISet<Command> commands;

        // Utils
        private readonly XmlDocument document;

        private ConfigurationService()
        {
            document = new XmlDocument();
        }

        // TODO: Move to session
        public ISet<Command> LoadCommands(string path = DEFAULT_COMMANDS_PATH)
        {
            if (null == commands)
            {
                commands = new HashSet<Command>();
            }
            try
            {
                commands.Clear();
                document.RemoveAll();
                document.Load(DEFAULT_COMMANDS_PATH);

                var root = document.SelectSingleNode("/Commands");
                var commandNodes = root.ChildNodes;

                foreach (XmlNode commandNode in commandNodes)
                {
                    var name = commandNode.Attributes[0].Value;
                    var content = commandNode.Attributes[1].Value;
                    var regex = new Regex(commandNode.Attributes[2].Value);
                    var type = EnumExtensions
                        .GetValueFromDescription<Command.TypeE>(commandNode.Attributes[3].Value);
                    var description = commandNode.FirstChild.InnerText;

                    commands.Add(Command.CreateCommand(name, content, description, regex, type));
                }
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
            }
            return commands;
        }

        // TODO: Move to command manager
        public void SaveCommands(string path = DEFAULT_COMMANDS_PATH)
        {
            if (null == commands)
            {
                throw new InvalidOperationException("Commands were never loaded to memory.");
            }

            document.RemoveAll();
            foreach (var command in commands)
            {
                document.AppendChild(command.ToXML());
            }
            document.Save(path);
        }
    }
}
