using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities.Extensions;

namespace IDE.Common.Utilities
{
    public class Highlighting
    {
        private string filePath;

        public Dictionary<Command.TypeE, Color> Colors { get; private set; }
        
        public delegate void HighlightingChangedDelegate();
        public event HighlightingChangedDelegate HighlightingChanged;

        #region Properties

        public string FilePath
        {
            get { return filePath; }
            private set
            {
                filePath = value;
                Session.Instance.SubmitHighlighting(filePath);
            }
        }

        #endregion

        public Highlighting(string path = MissingFileManager.DEFAULT_HIGHLIGHTING_PATH)
        {
            FilePath = Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute) ? path : MissingFileManager.DEFAULT_HIGHLIGHTING_PATH;
            Colors = new Dictionary<Command.TypeE, Color>();
            Import(path);
        }

        #region Actions

        public void Apply(Dictionary<Command.TypeE, Color> colors)
        {
            Colors = colors;
            Export(FilePath);
            HighlightingChanged?.Invoke();
        }

        public void Import(string path)
        {
            try
            {
                var document = new XmlDocument();
                document.Load(path);
                var colorNodes = document.GetElementsByTagName("Color");

                foreach (XmlNode node in colorNodes)
                {
                    var name = node.Attributes["name"];
                    var foreground = node.Attributes["foreground"];

                    var type = EnumExtensions.GetValueFromDescription<Command.TypeE>(name.Value);
                    var color = (Color)ColorConverter.ConvertFromString(foreground.Value);

                    Colors[type] = color;
                }
                FilePath = path;
                HighlightingChanged?.Invoke();
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Could not import highlighting definition. Loading defauls.");
                MissingFileManager.CreateHighlightingDefinitionFile();
                Import(MissingFileManager.DEFAULT_HIGHLIGHTING_PATH);
            }
        }

        public void Export(string path)
        {
            try
            {
                var document = new XmlDocument();
                document.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("IDE.Others.Resources.Highlighting.xshd"));
                var copy = document.InnerXml;
                document.LoadXml(copy);
                var colorNodes = document.GetElementsByTagName("Color");

                foreach (XmlNode node in colorNodes)
                {
                    var type = EnumExtensions.GetValueFromDescription<Command.TypeE>(node.Attributes["name"].Value);
                    node.Attributes["foreground"].Value = Colors[type].ToString();
                }
                document.Save(path);
                FilePath = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save highlighting definition",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
