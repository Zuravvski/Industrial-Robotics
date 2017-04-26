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
        private string filePath;
        public ISet<Command> CommandsMap { get; }

        public string FilePath
        {
            get { return filePath; }
            private set
            {
                filePath = value;
                Session.Instance.SubmitCommands(filePath);
            }
        }

        // Utils
        private readonly XmlDocument document;

        public Commands(string path = MissingFileManager.DEFAULT_COMMANDS_PATH)
        {
            document = new XmlDocument();
            CommandsMap = new HashSet<Command>();
            FilePath = Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute) ? path : MissingFileManager.DEFAULT_COMMANDS_PATH;
            Load(FilePath);
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
                FilePath = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
                MissingFileManager.CreateCommandsFile();
                Load(MissingFileManager.DEFAULT_COMMANDS_PATH);
            }
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
                FilePath = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not save commands.");
            }
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
