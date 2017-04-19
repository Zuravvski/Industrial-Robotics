using System;
using System.Text.RegularExpressions;
using System.Xml;
using Driver;

namespace IDE.Common.Models.Value_Objects
{
    public class Command : IXMLObject
    {
        public string Name { get; private set; }
        public string Content { get; private set; }
        public string Description { get; private set; }
        public Regex Regex { get; private set; }

        protected Command()
        {
        }

        /// <summary>
        /// Creates new command with specified parameters
        /// </summary>
        /// <param name="name">Defnies the name of command</param>
        /// <param name="content">Defines command keyword in movemaster language</param>
        /// <param name="description">Describes command [not mandatory]</param>
        /// <param name="regex">Defines valid syntax of command [not mandatory]</param>
        /// <returns></returns>
        public static Command CreateCommand(string name, string content, string description, Regex regex)
        {
            if(ValidateData(name, content, description, regex))
            {
                var command = new Command
                {
                    Name = name,
                    Content = content,
                    Description = description,
                    Regex = regex
                };
                return command;
            }
            throw new ArgumentException("Invalid arguments were passed");
        }

        private static bool ValidateData(string name, string content, string description, Regex regex)
        {
            return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(content) &&
                   null != description && null != regex;
        }

        public XmlElement ToXML()
        {
            var document = new XmlDocument();
            var root = document.CreateElement("Command");
            root.SetAttribute("name", Name);
            root.SetAttribute("content", Content);
            root.SetAttribute("regex", Convert.ToString(Regex));
            var descriptioNode = root.AppendChild(document.CreateElement("Description"));
            descriptioNode.InnerText = Description;

            return root;
        }
    }
}
