using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Driver;
using IDE.Common.Models;

namespace IDE.Common.Utilities
{
    public class Session
    {
        #region Constants
        private const string SESSION_NODE = "/Session";
        private const string COMMANDS_PARAM = "CommandsMap";
        private const string HIGHLIGHTING_PARAM = "HighlightingMap";
        #endregion

        #region Settings
        public Commands Commands { get; private set; }
        public Highlighting Highlighting { get; private set; }
        #endregion

        private static readonly Lazy<Session> instance = new Lazy<Session>(() => new Session());
        public static Session Instance => instance.Value;
        
        private readonly XmlDocument document = new XmlDocument();

        private Session()
        {
            document.Load(MissingFileManager.SESSION_PATH);
            if (document.SelectSingleNode("/Session") == null)
            {
                MissingFileManager.CreateSessionFile();
            }
        }

        public void Initialize()
        {
            document.Load(MissingFileManager.SESSION_PATH);
            var root = document.SelectSingleNode("/Session");

            // Loading commands path
            var commandsMapParam = root.Attributes["CommandsMap"];
            Commands = commandsMapParam != null ? new Commands(commandsMapParam.Value) : new Commands();

            // Loading highlighting
            var highlightingMapParam = root.Attributes["HighlightingMap"];
            Highlighting = highlightingMapParam != null ? new Highlighting(highlightingMapParam.Value) : new Highlighting();
        }

        public ObservableCollection<Program> LoadPrograms()
        {
            var list = new ObservableCollection<Program>();

            // Create session file if it does not exist
            MissingFileManager.CheckForRequiredFiles();
            
            try
            {
                document.Load(MissingFileManager.SESSION_PATH);
                var root = document.SelectSingleNode(SESSION_NODE);

                if (root == null)
                    return list;

                foreach (XmlNode child in root.ChildNodes)
                {
                    var path = child.InnerText;
                    try
                    {
                        if (!string.IsNullOrEmpty(path) && list.All(p => p.Path != path))
                        {
                            var program = new Program(Path.GetFileNameWithoutExtension(path))
                            {
                                Content = File.ReadAllText(path, Encoding.ASCII),
                                Path = path
                            };
                            list.Add(program);   
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public void SavePrograms(IEnumerable<TabItem> tabItems)
        {
            // Remove program nodes to override them
            document.Load(MissingFileManager.SESSION_PATH);
            var root = document.SelectSingleNode("Session");
            var programNodes = root.SelectNodes("Program");
            if (programNodes != null)
            {
                foreach (XmlNode programNode in programNodes)
                {
                    root.RemoveChild(programNode);
                }
            }
            
            // Add refreshed programs
            foreach (var tabItem in tabItems)
            {
                var program = tabItem.Program;
                if(program == null)
                    continue;

                var element = document.CreateElement("Program");
                element.InnerText = program.Path;
                root.AppendChild(element);
            }
            document.Save(MissingFileManager.SESSION_PATH);
        }

        public void SubmitHighlighting(string path)
        {
            var root = document.SelectSingleNode("Session");
            var commandsMapParam = root.Attributes[HIGHLIGHTING_PARAM];
            if (commandsMapParam != null)
            {
                root.Attributes.Remove(commandsMapParam);
            }
            commandsMapParam = document.CreateAttribute(HIGHLIGHTING_PARAM);
            commandsMapParam.Value = path;
            root.Attributes.Append(commandsMapParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }

        public void SubmitCommands(string path)
        {
            var root = document.SelectSingleNode("Session");
            var commandsMapParam = root.Attributes[COMMANDS_PARAM];
            if (commandsMapParam != null)
            {
                root.Attributes.Remove(commandsMapParam);
            }
            commandsMapParam = document.CreateAttribute(COMMANDS_PARAM);
            commandsMapParam.Value = path;
            root.Attributes.Append(commandsMapParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }
    }
}
