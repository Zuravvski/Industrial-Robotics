using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Driver;

namespace IDE.Common.Utilities
{
    // TODO: This should be singleton
    public static class Session
    {
        #region Constants
        private const string DEFAULT_SESSION_PATH = "Session.xml";
        private const string DEFAULT_COMMANDS_PATH = "Commands.xml";
        private const string DEFAULT_HIGHLIGHTING_PATH = "CustomHighlighting.xshd";
        #endregion

        private static readonly XmlDocument document = new XmlDocument();

        
        public static void LoadSession()
        {
            // TODO: Load previous configuration paths and files
        }

        public static async void SaveSession(IEnumerable<Program> programsList)
        {
            await Task.Run(() =>
            {
                document.RemoveAll();
                var node = document.AppendChild(document.CreateElement("Session"));
                foreach (var program in programsList)
                {
                    var element = document.CreateElement("Program");
                    element.InnerText = program.Path;
                    node.AppendChild(element);
                }
                document.Save(DEFAULT_SESSION_PATH);
            });
        }

        public static ObservableCollection<Program> LoadPrograms()
        {
            var list = new ObservableCollection<Program>();

            // Create session file if it does not exist
            if (!File.Exists(DEFAULT_SESSION_PATH))
            {
                document.AppendChild(document.CreateElement("Session"));
                document.Save(DEFAULT_SESSION_PATH);
                return list;
            }

            // Load session if file containing it exists
            try
            {
                document.Load(DEFAULT_SESSION_PATH);
                var root = document.SelectSingleNode("/Session");

                if (root == null)
                    return list;

                foreach (var child in root.ChildNodes)
                {
                    var path = ((XmlNode) child).InnerText;
                    try
                    {
                        if (!string.IsNullOrEmpty(path))
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
    }
}
