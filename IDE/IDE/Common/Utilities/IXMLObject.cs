using System.Xml;

namespace IDE.Common.Utilities
{
    public interface IXMLObject
    {
        XmlElement ToXML();
    }
}
