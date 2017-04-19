using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Utilities
{
    public class LazyLibraryLoader
    {
        private const string DEFAULT_COMMANDS_PATH = "Commands.xml";

        private static readonly Lazy<LazyLibraryLoader> instance = new Lazy<LazyLibraryLoader>(() => new LazyLibraryLoader());
        private ISet<Command> Commands;

        public static LazyLibraryLoader Instance => instance.Value;

        public ISet<Command> LoadCommands(string path = DEFAULT_COMMANDS_PATH)
        {
            if (null == Commands)
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
            return Commands;
        }
    }
}
