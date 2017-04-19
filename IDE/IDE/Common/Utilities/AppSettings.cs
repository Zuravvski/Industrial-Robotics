using System;
using System.Collections.Generic;
using System.Xml;
using Driver;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.Utilities
{
    public class AppSettings : IXMLObject
    {
        private static readonly Lazy<AppSettings> instance = new Lazy<AppSettings>(() => new AppSettings());
        public static AppSettings Instance => instance.Value;
        public DriverSettings DriverSettings { get; set; }
        public ISet<Command> Commands { get; }

        private readonly XmlDocument document;

        private AppSettings()
        {
            document = new XmlDocument();
            DriverSettings = DriverSettings.CreateFromSettingFile();
            Commands = LazyLibraryLoader.Instance.LoadCommands();
        }

        // TODO: Create xml
        public XmlElement ToXML()
        {
            throw new NotImplementedException();
        }
    }
}
