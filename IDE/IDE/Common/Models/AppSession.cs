using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Driver;

namespace IDE.Common.Models
{
    public class AppSession
    {
        private const string DEFAULT_FILE_PATH = "Session.xml";
        private static readonly Lazy<AppSession> instance = new Lazy<AppSession>(() => new AppSession());
        private readonly XmlDocument document;
        public static AppSession Instance => instance.Value;

        private AppSession()
        {
            document = new XmlDocument();
        }

        // TODO: Consider this method to be async - probably there's no need as it runs only once
        public ObservableCollection<Program> LoadLastSession()
        {
            var list = new ObservableCollection<Program>();
            if (!File.Exists(DEFAULT_FILE_PATH))
            {
                document.AppendChild(document.CreateElement("Session"));
                document.Save(DEFAULT_FILE_PATH);
                return list;
            }

            try
            {
                document.Load(DEFAULT_FILE_PATH);
                var root = document.SelectSingleNode("/Session");

                if (root == null) return list;

                foreach (var child in root.ChildNodes)
                {
                    var path = ((XmlNode)child).InnerText;
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
                        Debug.WriteLine(ex.Message);
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public async void SaveSession(IEnumerable<Program> programsList)
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
                document.Save(DEFAULT_FILE_PATH);
            });
        }
    }
}
